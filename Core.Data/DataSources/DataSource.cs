using System;
using System.Data;
using System.Data.SqlClient;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Data.Fields;
using Core.Exceptions;
using Core.Monads;
using Core.Objects;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.DataSources
{
   public abstract class DataSource
   {
      protected static DbType typeToDBType(Type parameterType)
      {
         switch (Type.GetTypeCode(parameterType))
         {
            case TypeCode.Boolean:
               return DbType.Boolean;
            case TypeCode.Byte:
               return DbType.Byte;
            case TypeCode.DateTime:
               return DbType.DateTime;
            case TypeCode.Decimal:
               return DbType.Decimal;
            case TypeCode.Double:
               return DbType.Double;
            case TypeCode.Int16:
               return DbType.Int16;
            case TypeCode.Int32:
               return DbType.Int32;
            case TypeCode.Int64:
               return DbType.Int64;
            case TypeCode.String:
            case TypeCode.Char:
               return DbType.String;
            default:
               return DbType.Object;
         }
      }

      protected IMaybe<IDbConnection> connection;
      protected TimeSpan commandTimeout;
      protected Fields.Fields fields;
      protected IMaybe<IActive> activeObject;

      public event EventHandler<CancelEventArgs> NextRow;

      public DataSource(string connectionString, TimeSpan commandTimeout)
      {
         ConnectionString = connectionString;
         this.commandTimeout = commandTimeout;
         ResultIndex = 0;
         Command = none<IDbCommand>();
         connection = none<IDbConnection>();
         Reader = none<IDataReader>();
      }

      public bool Deallocated { get; set; }

      public int ResultIndex { get; set; }

      public IMaybe<IDbCommand> Command { get; set; }

      public string ConnectionString { get; set; }

      public abstract IDbConnection GetConnection();

      public abstract IDbCommand GetCommand();

      public abstract void AddParameters(object entity, Parameters.Parameters parameters);

      protected static string modifyCommand(object entity, string commandText)
      {
         var evaluator = PropertyInterface.GetEvaluator(entity);
         var matcher = new Matcher();
         var text = commandText;
         foreach (var signature in evaluator.Signatures)
         {
            var name = signature.Name;
            var pattern = "'{" + name + "}'";
            var value = evaluator[signature].ToNonNullString().Replace("'", "''");
            if (matcher.IsMatch(text, pattern))
            {
               for (var i = 0; i < matcher.MatchCount; i++)
               {
                  matcher[i] = value;
               }
            }

            text = matcher.ToString();
         }

         return text;
      }

      internal int Execute(object entity, string command, Parameters.Parameters parameters, Fields.Fields inFields)
      {
         activeObject = entity.IfCast<IActive>();

         fields = inFields;

         allocateConnection();
         allocateCommand();

         AddParameters(entity, parameters);

         setCommand(entity, command);
         IDataReader reader = null;

         try
         {
            if (Command.If(out var com) & connection.If(out var con))
            {
               com.Connection = con;
            }
            else
            {
               throw "Command and connection not properly initialized".Throws();
            }

            int recordsAffected;
            if (inFields.Count > 0)
            {
               reader = com.ExecuteReader();
               for (var i = 1; i <= ResultIndex; i++)
               {
                  reader.NextResult();
               }

               recordsAffected = reader.RecordsAffected;
               FillOutput(entity, com.Parameters, parameters);

               var cancel = false;

               FillOrdinals(reader, inFields);

               if (activeObject.If(out var ao))
               {
                  ao.BeforeExecute();
               }

               while (!cancel && reader.Read())
               {
                  fill(entity, reader);
                  NextRow?.Invoke(this, new CancelEventArgs());
                  cancel = new CancelEventArgs().Cancel;
               }

               if (activeObject.If(out ao))
               {
                  ao.AfterExecute();
               }
            }
            else
            {
               if (activeObject.If(out var ao))
               {
                  ao.BeforeExecute();
               }

               recordsAffected = com.ExecuteNonQuery();
               FillOutput(entity, com.Parameters, parameters);

               if (activeObject.If(out ao))
               {
                  ao.AfterExecute();
               }
            }

            return recordsAffected;
         }
         finally
         {
            reader?.Dispose();
            deallocateObjects();
         }
      }

      public IMaybe<IDataReader> Reader { get; set; }

      internal void BeginReading(object entity, string command, Parameters.Parameters parameters, Fields.Fields inFields)
      {
         inFields.Count.Must().BePositive().Assert("You must have at least one field defined");

         activeObject = entity.IfCast<IActive>();
         fields = inFields;

         IDataReader reader = null;

         try
         {
            allocateConnection();
            allocateCommand();

            AddParameters(entity, parameters);

            setCommand(entity, command);
            if (Command.If(out var com) & connection.If(out var con))
            {
               com.Connection = con;
            }
            else
            {
               throw "Command and connection not properly initialized".Throws();
            }

            reader = com.ExecuteReader();
            for (var i = 1; i <= ResultIndex; i++)
            {
               reader.NextResult();
            }

            FillOutput(entity, com.Parameters, parameters);
            FillOrdinals(reader, inFields);

            if (activeObject.If(out var ao))
            {
               ao.BeforeExecute();
            }

            Reader = reader.Some();
         }
         catch
         {
            reader?.Dispose();
            deallocateObjects();
            throw;
         }
      }

      void setCommand(object entity, string command)
      {
         var commandText = modifyCommand(entity, command);
         if (Command.If(out var dbCommand))
         {
            dbCommand.CommandText = commandText;
            changeCommandType(dbCommand, commandText);
         }
         else
         {
            throw "Command hasn't been created".Throws();
         }
      }

      internal IMaybe<object> NextReading(object entity)
      {
         if (Reader.If(out var reader) && reader.Read())
         {
            fill(entity, reader);
            return entity.Some();
         }
         else
         {
            return none<object>();
         }
      }

      internal void EndReading()
      {
         if (Reader.If(out var reader))
         {
            if (activeObject.If(out var ao))
            {
               ao.AfterExecute();
            }

            if (!reader.IsClosed)
            {
               try
               {
                  reader.Close();
               }
               catch { }
            }

            deallocateObjects();
         }
      }

      protected void deallocateObjects()
      {
         try
         {
            disposeCommand();
            disposeConnection();
         }
         finally
         {
            flagAsDeallocated();
         }
      }

      public static void FillFields(object entity, IDataReader reader, Fields.Fields fields)
      {
         var current = new Field();
         var value = new object();

         foreach (var field in fields)
         {
            current = field;

            if (current.Ordinal > -1)
            {
               value = reader.GetValue(current.Ordinal);
               if (value == DBNull.Value)
               {
                  value = null;
               }

               current.SetValue(entity, value);
            }
         }
      }

      void fill(object entity, IDataReader dataReader) => FillFields(entity, dataReader, fields);

      public static void FillOrdinals(IDataReader reader, Fields.Fields fields)
      {
         var matcher = new Matcher();

         foreach (var field in fields)
         {
            if (matcher.IsMatch(field.Name, "^ 'field' /(/d+) $"))
            {
               field.Ordinal = matcher[0, 1].ToInt(-1);
            }
            else
            {
               try
               {
                  field.Ordinal = reader.GetOrdinal(field.Name);
               }
               catch (IndexOutOfRangeException)
               {
                  field.Ordinal = -1;
               }
            }

            if (!field.Optional)
            {
               field.Ordinal.Must().Not.BeNegative().Assert($"Couldn't find {field.Name} field");
            }
         }
      }

      public static void FillOutput(object entity, IDataParameterCollection dataParameters,
         Parameters.Parameters parameters)
      {
         foreach (var dataParameter in dataParameters)
         {
            var p = (IDataParameter)dataParameter;
            if (p.Direction == ParameterDirection.Output)
            {
               var parameter = parameters.Value(p.ParameterName);
               parameter.SetValue(entity, p.Value);
            }
         }
      }

      protected static void changeCommandType(IDbCommand command, string commandText)
      {
         command.CommandType = commandText.IsMatch("/s+") ? CommandType.Text : CommandType.StoredProcedure;
      }

      void allocateCommand()
      {
         if (Command.IsNone)
         {
            var command = GetCommand();
            if (connection.If(out var c))
            {
               command.Connection = c;
            }
            else
            {
               throw "Connection not properly initialized".Throws();
            }

            command.CommandTimeout = (int)commandTimeout.TotalSeconds;

            Command = command.Some();
         }
      }

      void allocateConnection()
      {
         if (connection.If(out var c))
         {
            if (c.State == ConnectionState.Closed)
            {
               c.Open();
            }
         }
         else
         {
            connection = GetConnection().Some();
         }
      }

      protected void flagAsDeallocated() => Deallocated = true;

      protected void disposeCommand()
      {
         if (Command.If(out var command))
         {
            command.Dispose();
            Command = none<IDbCommand>();
         }
      }

      protected void disposeConnection()
      {
         if (connection.If(out var conn))
         {
            conn.Dispose();
            connection = none<IDbConnection>();
         }
      }

      protected string getFileConnectionString(IMaybe<FileName> associatedFile)
      {
         if (associatedFile.If(out var file))
         {
            var formatter = Formatter.WithStandard(true);
            formatter["file"] = file.ToNonNullString();
            return formatter.Format(ConnectionString);
         }
         else
         {
            return ConnectionString;
         }
      }

      public IDataReader ExecuteReader(object entity, string command, Parameters.Parameters parameters)
      {
         allocateConnection();
         allocateCommand();

         if (entity != null)
         {
            AddParameters(entity, parameters);
         }

         setCommand(entity, command);

         if (Command.If(out var c))
         {
            return c.ExecuteReader(CommandBehavior.CloseConnection);
         }
         else
         {
            throw "Command not initialized".Throws();
         }
      }

      public abstract void ClearAllPools();

      public virtual void SetMessageHandler(SqlInfoMessageEventHandler handler) { }
   }
}