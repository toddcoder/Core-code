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
using Core.Strings;
using static System.Convert;
using static Core.Monads.MonadFunctions;

namespace Core.Data.DataSources;

public class SqlDataSource : DataSource, IBulkCopyTarget, IHash<string, string>
{
   protected static string getConnectionString(string server, string database, string user, string password)
   {
      return $"Data Source={server};Initial Catalog={database};User ID={user}; Password={password}";
   }

   protected static string getConnectionString(string server, string database)
   {
      return $"Data Source={server};Initial Catalog={database};Integrated Security=SSPI;";
   }

   public static void AddParametersToCommand(IDbCommand command, object entity, Parameters.Parameters parameters)
   {
      command.Parameters.Clear();

      foreach (var parameter in parameters)
      {
         Type parameterType;
         if (parameter.Type)
         {
            parameterType = parameter.Type;
         }
         else
         {
            parameter.DeterminePropertyType(entity);
            parameterType = parameter.PropertyType;
            parameter.Type = parameterType;
         }

         var sqlParameter = parameter.Size
               .Map(size => new SqlParameter(parameter.Name, typeToSqlType(parameterType), size))
            | (() => new SqlParameter(parameter.Name, typeToSqlType(parameterType)));

         if (parameter.Output)
         {
            sqlParameter.Direction = ParameterDirection.Output;
         }
         else if (parameter.Value is (true, var parameterValue))
         {
            if (parameterType == typeof(string))
            {
               sqlParameter.Value = parameterValue;
            }
            else
            {
               var obj = parameterValue.ToObject().Required($"Couldn't convert {parameterValue}");
               sqlParameter.Value = ChangeType(obj, parameterType);
            }
         }
         else
         {
            var value = parameter.GetValue(entity).Required($"Parameter {parameter.Name}'s value couldn't be determined");
            if (value is null && parameter.Default is (true, var defaultValue))
            {
               value = parameter.Type.Map(t => ChangeType(defaultValue, t)) | (object)defaultValue;
            }

            var type = value?.GetType();
            var underlyingType = type?.UnderlyingTypeOf() ?? nil;
            if (underlyingType)
            {
               value = type.InvokeMember("Value", BindingFlags.GetProperty, null, value, Array.Empty<object>());
            }

            sqlParameter.Value = value;
         }

         command.Parameters.Add(sqlParameter);
      }
   }

   protected static SqlDbType typeToSqlType(Type type) => type.Name switch
   {
      "Int64" => SqlDbType.BigInt,
      "Byte[]" => SqlDbType.Binary,
      "Boolean" => SqlDbType.Bit,
      "String" => SqlDbType.VarChar,
      "DateTime" => SqlDbType.DateTime,
      "Decimal" => SqlDbType.Decimal,
      "Double" => SqlDbType.Float,
      "Int32" => SqlDbType.Int,
      "Single" => SqlDbType.Real,
      "Int16" => SqlDbType.SmallInt,
      "Byte" => SqlDbType.TinyInt,
      "Guid" => SqlDbType.UniqueIdentifier,
      _ => SqlDbType.Variant
   };

   protected StringHash<string> attributes;
   protected long recordCount;

   public event SqlInfoMessageEventHandler Message;

   public SqlDataSource(string connectionString, TimeSpan timeout) : base(connectionString, timeout)
   {
      CommandTimeout = timeout;
      attributes = new StringHash<string>(true);
      recordCount = 0;
   }

   public SqlDataSource(string connectionString) : this(connectionString, 30.Seconds())
   {
   }

   public SqlDataSource(string server, string database, string user, string password) :
      this(getConnectionString(server, database, user, password))
   {
   }

   public SqlDataSource(string server, string database, string user, string password, TimeSpan timeout)
      : this(getConnectionString(server, database, user, password), timeout)
   {
   }

   public SqlDataSource(string server, string database) : this(getConnectionString(server, database))
   {
   }

   public SqlDataSource(string server, string database, TimeSpan timeout) : this(getConnectionString(server, database), timeout)
   {
   }

   public long RecordCount => recordCount;

   public TimeSpan CommandTimeout { get; set; }

   public string this[string keyWord]
   {
      get => attributes[keyWord];
      set => attributes[keyWord] = value;
   }

   public bool ContainsKey(string key) => attributes.ContainsKey(key);

   public Result<Hash<string, string>> AnyHash() => attributes;

   public override IDbConnection GetConnection()
   {
      var connectionString = ConnectionString;
      if (attributes.Count > 0)
      {
         connectionString += attributes.Select(i => $"{i.Key} = {i.Value}").ToString("; ");
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
      using var sqlConnection = new SqlConnection(ConnectionString);
      using var sqlCommand = new SqlCommand(command, sqlConnection);

      sqlConnection.Open();

      var adapter = new SqlDataAdapter { SelectCommand = sqlCommand };
      Command = sqlCommand;

      changeCommandType(adapter.SelectCommand, command);
      addCommandParameters(entity, parameters);
      adapter.Fill(dataSet);

      return dataSet;
   }

   protected void addCommandParameters(object entity, Parameters.Parameters parameters)
   {
      AddParametersToCommand(Command.Required("Command not initialized"), entity, parameters);
   }

   public string TableName { get; set; }

   public void Copy<T>(Adapter<T> sourceAdapter) where T : class
   {
      TableName.Must().Not.BeEmpty().OrThrow();

      recordCount = 0;

      using var dataReader = sourceAdapter.ExecuteReader();
      using var sqlConnection = (SqlConnection)GetConnection();
      using var bulkCopy = new SqlBulkCopy(sqlConnection) { DestinationTableName = TableName, NotifyAfter = 5000 };
      bulkCopy.SqlRowsCopied += (_, e) => recordCount += e.RowsCopied;
      bulkCopy.BulkCopyTimeout = (int)CommandTimeout.TotalSeconds;
      bulkCopy.WriteToServer(dataReader);
   }

   public void Copy(IDataReader reader, TimeSpan timeout)
   {
      TableName.Must().Not.BeEmpty().OrThrow();

      recordCount = 0;

      using var sqlConnection = (SqlConnection)GetConnection();
      using var bulkCopy = new SqlBulkCopy(sqlConnection) { NotifyAfter = 5000 };
      bulkCopy.SqlRowsCopied += (_, e) => recordCount += e.RowsCopied;
      bulkCopy.DestinationTableName = TableName;
      bulkCopy.BulkCopyTimeout = (int)timeout.TotalSeconds;
      bulkCopy.WriteToServer(reader);
   }

   public override void ClearAllPools() => SqlConnection.ClearAllPools();

   public override void SetMessageHandler(SqlInfoMessageEventHandler handler)
   {
      if (handler != null)
      {
         Message += handler;
      }
   }

   public override DataSource WithNewConnectionString(string newConnectionString) => new SqlDataSource(newConnectionString, CommandTimeout);
}