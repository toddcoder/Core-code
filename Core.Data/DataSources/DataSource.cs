using System;
using System.Data;
using System.Data.SqlClient;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.DataSources;

public abstract class DataSource
{
   protected static DbType typeToDBType(Type parameterType) => Type.GetTypeCode(parameterType) switch
   {
      TypeCode.Boolean => DbType.Boolean,
      TypeCode.Byte => DbType.Byte,
      TypeCode.DateTime => DbType.DateTime,
      TypeCode.Decimal => DbType.Decimal,
      TypeCode.Double => DbType.Double,
      TypeCode.Int16 => DbType.Int16,
      TypeCode.Int32 => DbType.Int32,
      TypeCode.Int64 => DbType.Int64,
      TypeCode.String => DbType.String,
      TypeCode.Char => DbType.String,
      _ => DbType.Object
   };

   protected Optional<IDbConnection> _connection;
   protected TimeSpan commandTimeout;
   protected Fields.Fields fields;
   protected Optional<IActive> _activeObject;

   public event EventHandler<CancelEventArgs> NextRow;

   public DataSource(string connectionString, TimeSpan commandTimeout)
   {
      ConnectionString = connectionString;
      this.commandTimeout = commandTimeout;
      ResultIndex = 0;
      Command = nil;
      _connection = nil;
      Reader = nil;
   }

   public bool Deallocated { get; set; }

   public int ResultIndex { get; set; }

   public Optional<IDbCommand> Command { get; set; }

   public string ConnectionString { get; set; }

   public bool HasRows { get; set; }

   public abstract IDbConnection GetConnection();

   public abstract IDbCommand GetCommand();

   public abstract void AddParameters(object entity, Parameters.Parameters parameters);

   protected static string modifyCommand(object entity, string commandText)
   {
      var evaluator = PropertyInterface.GetEvaluator(entity);
      var text = commandText;
      foreach (var signature in evaluator.Signatures)
      {
         var name = signature.Name;
         var pattern = $"'{{{name}}}'; f";
         var value = evaluator[signature].ToNonNullString().Replace("'", "''");
         var _result = text.Matches(pattern);
         if (_result is (true, var result))
         {
            foreach (var match in result)
            {
               match.Text = value;
            }

            text = result.Text;
         }
      }

      return text;
   }

   internal int Execute(object entity, string command, Parameters.Parameters parameters, Fields.Fields inFields)
   {
      _activeObject = entity.IfCast<IActive>();

      fields = inFields;

      allocateConnection();
      allocateCommand();

      AddParameters(entity, parameters);

      setCommand(entity, command);
      IDataReader reader = null;
      HasRows = false;

      try
      {
         if (Command is (true, var dbCommand) && _connection is (true, var connection))
         {
            dbCommand.Connection = connection;
         }
         else
         {
            throw fail("Command and connection not properly initialized");
         }

         int recordsAffected;
         if (inFields.Count > 0)
         {
            reader = dbCommand.ExecuteReader();
            for (var i = 1; i <= ResultIndex; i++)
            {
               reader.NextResult();
            }

            recordsAffected = reader.RecordsAffected;
            FillOutput(entity, dbCommand.Parameters, parameters);

            var cancel = false;

            FillOrdinals(reader, inFields);

            if (_activeObject is (true, var activeObject))
            {
               activeObject.BeforeExecute();
            }

            while (!cancel && reader.Read())
            {
               HasRows = true;
               fill(entity, reader);
               NextRow?.Invoke(this, new CancelEventArgs());
               cancel = new CancelEventArgs().Cancel;
            }

            if (_activeObject is (true, var activeObject2))
            {
               activeObject2.AfterExecute();
            }
         }
         else
         {
            if (_activeObject is (true, var activeObject))
            {
               activeObject.BeforeExecute();
            }

            recordsAffected = dbCommand.ExecuteNonQuery();
            FillOutput(entity, dbCommand.Parameters, parameters);

            if (_activeObject is (true, var activeObject2))
            {
               activeObject2.AfterExecute();
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

   public Optional<IDataReader> Reader { get; set; }

   internal void BeginReading(object entity, string command, Parameters.Parameters parameters, Fields.Fields inFields)
   {
      inFields.Count.Must().BePositive().OrThrow("You must have at least one field defined");

      _activeObject = entity.IfCast<IActive>();
      fields = inFields;

      IDataReader reader = null;

      try
      {
         allocateConnection();
         allocateCommand();

         AddParameters(entity, parameters);

         setCommand(entity, command);
         if (Command is (true, var dbCommand) && _connection is (true, var connection))
         {
            dbCommand.Connection = connection;
         }
         else
         {
            throw fail("Command and connection not properly initialized");
         }

         reader = dbCommand.ExecuteReader();
         for (var i = 1; i <= ResultIndex; i++)
         {
            reader.NextResult();
         }

         FillOutput(entity, dbCommand.Parameters, parameters);
         FillOrdinals(reader, inFields);

         if (_activeObject is (true, var activeObject))
         {
            activeObject.BeforeExecute();
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

   protected void setCommand(object entity, string command)
   {
      var commandText = modifyCommand(entity, command);
      if (Command is (true, var dbCommand))
      {
         dbCommand.CommandText = commandText;
         changeCommandType(dbCommand, commandText);
      }
      else
      {
         throw fail("Command hasn't been created");
      }
   }

   internal Optional<object> NextReading(object entity)
   {
      if (Reader is (true, var reader) && reader.Read())
      {
         HasRows = true;
         fill(entity, reader);
         return entity;
      }
      else
      {
         return nil;
      }
   }

   internal void EndReading()
   {
      if (Reader is (true, var reader))
      {
         if (_activeObject is (true, var activeObject))
         {
            activeObject.AfterExecute();
         }

         if (!reader.IsClosed)
         {
            try
            {
               reader.Close();
            }
            catch
            {
            }
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
      foreach (var field in fields)
      {
         if (field.Ordinal > -1)
         {
            var value = reader.GetValue(field.Ordinal);
            if (value == DBNull.Value)
            {
               value = null;
            }

            field.SetValue(entity, value);
         }
      }
   }

   protected void fill(object entity, IDataReader dataReader) => FillFields(entity, dataReader, fields);

   public static void FillOrdinals(IDataReader reader, Fields.Fields fields)
   {
      foreach (var field in fields)
      {
         try
         {
            field.Ordinal = reader.GetOrdinal(field.Name);
         }
         catch (IndexOutOfRangeException)
         {
            field.Ordinal = -1;
         }

         if (!field.Optional)
         {
            field.Ordinal.Must().Not.BeNegative().OrThrow($"Couldn't find {field.Name} field");
         }
      }
   }

   public static void FillOutput(object entity, IDataParameterCollection dataParameters, Parameters.Parameters parameters)
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
      command.CommandType = commandText.IsMatch("/s+; f") ? CommandType.Text : CommandType.StoredProcedure;
   }

   protected void allocateCommand()
   {
      if (!Command)
      {
         var command = GetCommand();
         command.Connection = _connection.Required("Connection not properly initialized");
         command.CommandTimeout = (int)commandTimeout.TotalSeconds;

         Command = command.Some();
      }
   }

   protected void allocateConnection()
   {
      if (_connection is (true, var connection))
      {
         if (connection.State == ConnectionState.Closed)
         {
            connection.Open();
         }
      }
      else
      {
         _connection = GetConnection().Some();
      }
   }

   protected void flagAsDeallocated() => Deallocated = true;

   public void Close()
   {
      disposeCommand();
      disposeConnection();
   }

   protected void disposeCommand()
   {
      if (Command is (true, var command))
      {
         command.Dispose();
         Command = nil;
      }
   }

   protected void disposeConnection()
   {
      if (_connection is (true, var connection))
      {
         connection.Dispose();
         _connection = nil;
      }
   }

   protected string getFileConnectionString(Optional<FileName> associatedFile)
   {
      if (associatedFile is (true, var associatedFileValue))
      {
         var formatter = Formatter.WithStandard(true);
         formatter["file"] = associatedFileValue.ToNonNullString();
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

      if (Command is (true, var dbCommand))
      {
         return dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
      }
      else
      {
         throw fail("Command not initialized");
      }
   }

   public abstract void ClearAllPools();

   public virtual void SetMessageHandler(SqlInfoMessageEventHandler handler)
   {
   }

   public abstract DataSource WithNewConnectionString(string newConnectionString);
}