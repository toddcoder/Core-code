using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Dates.DateIncrements;
using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static System.Convert;
using static Core.Monads.MonadFunctions;

namespace Core.Data.DataSources
{
   public class SQLDataSource : DataSource, IBulkCopyTarget, IHash<string, string>
   {
      static string getConnectionString(string server, string database, string user, string password)
      {
         return $"Data Source={server};Initial Catalog={database};User ID={user}; Password={password}";
      }

      static string getConnectionString(string server, string database)
      {
         return $"Data Source={server};Initial Catalog={database};Integrated Security=SSPI;";
      }

      public static void AddParametersToCommand(IDbCommand command, object entity, Parameters.Parameters parameters)
      {
         command.Parameters.Clear();

         foreach (var parameter in parameters)
         {
            if (parameter.Type.If(out var parameterType)) { }
            else
            {
               parameter.DeterminePropertyType(entity);
               parameterType = parameter.PropertyType;
               parameter.Type = parameterType.Some();
            }

            SqlParameter sqlParameter;
            if (parameter.Size.If(out var size))
            {
               sqlParameter = new SqlParameter(parameter.Name, typeToSQLType(parameterType), size);
            }
            else
            {
               sqlParameter = new SqlParameter(parameter.Name, typeToSQLType(parameterType));
            }

            if (parameter.Output)
            {
               sqlParameter.Direction = ParameterDirection.Output;
            }
            else if (parameter.Value.If(out var str))
            {
               if (parameterType == typeof(string))
               {
                  sqlParameter.Value = str;
               }
               else
               {
                  var obj = str.ToObject().Required($"Couldn't convert {str}");
                  sqlParameter.Value = ChangeType(obj, parameterType);
               }
            }
            else
            {
               var value = parameter.GetValue(entity).Required($"Parameter {parameter.Name}'s value couldn't be determined");
               if (value.IsNull() && parameter.Default.If(out var defaultValue))
               {
                  value = parameter.Type.FlatMap(t => ChangeType(defaultValue, t), () => defaultValue);
               }

               var type = value?.GetType();
               var underlyingType = type?.UnderlyingTypeOf() ?? none<Type>();
               if (underlyingType.IsSome)
               {
                  value = type.InvokeMember("Value", BindingFlags.GetProperty, null, value, new object[0]);
               }

               sqlParameter.Value = value;
            }

            command.Parameters.Add(sqlParameter);
         }
      }

      static SqlDbType typeToSQLType(Type type)
      {
         switch (type.Name)
         {
            case "Int64":
               return SqlDbType.BigInt;
            case "Byte[]":
               return SqlDbType.Binary;
            case "Boolean":
               return SqlDbType.Bit;
            case "String":
               return SqlDbType.VarChar;
            case "DateTime":
               return SqlDbType.DateTime;
            case "Decimal":
               return SqlDbType.Decimal;
            case "Double":
               return SqlDbType.Float;
            case "Int32":
               return SqlDbType.Int;
            case "Single":
               return SqlDbType.Real;
            case "Int16":
               return SqlDbType.SmallInt;
            case "Byte":
               return SqlDbType.TinyInt;
            case "Guid":
               return SqlDbType.UniqueIdentifier;
            default:
               return SqlDbType.Variant;
         }
      }

      Hash<string, string> attributes;
      long recordCount;

      public event SqlInfoMessageEventHandler Message;

      public SQLDataSource(string connectionString, TimeSpan timeout)
         : base(connectionString, timeout)
      {
         CommandTimeout = timeout;
         attributes = new Hash<string, string>();
         recordCount = 0;
      }

      public SQLDataSource(string connectionString) : this(connectionString, 30.Seconds()) { }

      public SQLDataSource(string server, string database, string user, string password) :
         this(getConnectionString(server, database, user, password)) { }

      public SQLDataSource(string server, string database, string user, string password, TimeSpan timeout)
         : this(getConnectionString(server, database, user, password), timeout) { }

      public SQLDataSource(string server, string database) : this(getConnectionString(server, database)) { }

      public SQLDataSource(string server, string database, TimeSpan timeout) : this(getConnectionString(server, database), timeout) { }

      public long RecordCount => recordCount;

      public TimeSpan CommandTimeout { get; set; }

      public string this[string keyWord]
      {
         get => attributes[keyWord];
         set => attributes[keyWord] = value;
      }

      public bool ContainsKey(string key) => attributes.ContainsKey(key);

      public override IDbConnection GetConnection()
      {
         var connectionString = ConnectionString;
         if (attributes.Count > 0)
         {
            connectionString += attributes.Select(i => $"{i.Key} = {i.Value}").Stringify("; ");
         }

         var sqlConnection = new SqlConnection(connectionString);
         if (Message != null)
         {
            sqlConnection.InfoMessage += Message;
         }

         sqlConnection.Open();

         return sqlConnection;
      }

      public override IDbCommand GetCommand() => new SqlCommand { CommandTimeout = (int)CommandTimeout.TotalSeconds };

      public override void AddParameters(object entity, Parameters.Parameters parameters)
      {
         AddParametersToCommand(Command.Required("Command not initialized"), entity, parameters);
      }

      public DataSet DataSet(object entity, string command, Parameters.Parameters parameters)
      {
         var dataSet = new DataSet();

         using (var sqlConnection = new SqlConnection(ConnectionString))
         using (var sqlCommand = new SqlCommand(command, sqlConnection))
         {
            sqlConnection.Open();
            var adapter = new SqlDataAdapter { SelectCommand = sqlCommand };
            Command = sqlCommand.Some<IDbCommand>();
            changeCommandType(adapter.SelectCommand, command);
            addCommandParameters(entity, parameters);
            adapter.Fill(dataSet);
         }

         return dataSet;
      }

      void addCommandParameters(object entity, Parameters.Parameters parameters)
      {
         AddParametersToCommand(Command.Required("Command not initialized"), entity, parameters);
      }

      public string TableName { get; set; }

      public void Copy<T>(Adapter<T> sourceAdapter)
         where T : class
      {
         TableName.Must().Not.BeEmpty().Assert("Table name is not set");

         recordCount = 0;

         using (var dataReader = sourceAdapter.ExecuteReader())
         using (var sqlConnection = (SqlConnection)GetConnection())
         using (var bulkCopy = new SqlBulkCopy(sqlConnection))
         {
            bulkCopy.DestinationTableName = TableName;
            bulkCopy.NotifyAfter = 5000;
            bulkCopy.SqlRowsCopied += (sender, e) => recordCount += e.RowsCopied;
            bulkCopy.BulkCopyTimeout = (int)CommandTimeout.TotalSeconds;
            bulkCopy.WriteToServer(dataReader);
         }
      }

      public void Copy(IDataReader reader, TimeSpan timeout)
      {
         TableName.Must().Not.BeEmpty().Assert("Table name is not set");

         recordCount = 0;

         using (var sqlConnection = (SqlConnection)GetConnection())
         using (var bulkCopy = new SqlBulkCopy(sqlConnection))
         {
            bulkCopy.NotifyAfter = 5000;
            bulkCopy.SqlRowsCopied += (sender, e) => recordCount += e.RowsCopied;
            bulkCopy.DestinationTableName = TableName;
            bulkCopy.BulkCopyTimeout = (int)timeout.TotalSeconds;
            bulkCopy.WriteToServer(reader);
         }
      }

      public override void ClearAllPools() => SqlConnection.ClearAllPools();

      public override void SetMessageHandler(SqlInfoMessageEventHandler handler)
      {
         if (handler != null)
         {
            Message += handler;
         }
      }
   }
}