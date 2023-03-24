using System;
using System.Data.SQLite;
using Core.Applications;
using Core.Collections;
using Core.Computers;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Sqlite;

public abstract class Database
{
   protected FileName databaseFile;

   public event EventHandler<DatabaseEventArgs> DatabaseEvent;
   public event EventHandler<DatabaseFailureArgs> DatabaseFailure;
   public event EventHandler<DatabaseExceptionArgs> DatabaseException;

   public Database(FolderName databaseFolder, string databaseName)
   {
      databaseFile = databaseFolder + $"{databaseName}.db";
   }

   public SQLiteConnection GetConnection()
   {
      var connectionString = $"Data Source={databaseFile.FullPath}; Version=3";
      return new SQLiteConnection(connectionString, true);
   }

   public void RaiseDatabaseEvent(string message) => DatabaseEvent?.Invoke(this, new DatabaseEventArgs(message));

   public void RaiseDatabaseFailure(string message) => DatabaseFailure?.Invoke(this, new DatabaseFailureArgs(message));

   public void RaiseDatabaseException(Exception exception) => DatabaseException?.Invoke(this, new DatabaseExceptionArgs(exception));

   protected static void addParametersToCommand(SQLiteCommand command, (string, object)[] parameters)
   {
      foreach (var (name, value) in parameters)
      {
         var newName = name.StartsWith("$") ? name : $"${name}";
         command.Parameters.AddWithValue(newName, value);
      }
   }

   public abstract Optional<Unit> CreateTables();

   public void CreateDatabaseIfNonExistent()
   {
      try
      {
         var fullPath = databaseFile.FullPath;
         if (!databaseFile)
         {
            SQLiteConnection.CreateFile(fullPath);
            RaiseDatabaseEvent($"Created database file {fullPath}");

            var _result = CreateTables();
            if (_result)
            {
               RaiseDatabaseEvent("Tables created");
            }
            else
            {
               RaiseDatabaseException(_result.Exception);
            }
         }
      }
      catch (Exception exception)
      {
         RaiseDatabaseException(exception);
      }
   }

   public Optional<Unit> CreateTable(string fileName)
   {
      try
      {
         var resources = new Resources<Database>();
         var commandText = resources.String(fileName);

         return ExecuteNonQuery(commandText).Unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<int> ExecuteNonQuery(string commandText, params (string, object)[] parameters)
   {
      using var connection = GetConnection();
      try
      {
         connection.Open();
         using var command = connection.CreateCommand();

         addParametersToCommand(command, parameters);

         command.CommandText = commandText;
         var result = command.ExecuteNonQuery();

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
      finally
      {
         connection?.Close();
      }
   }

   public SQLiteDataReader ExecuteDataReader(SQLiteCommand command, string commandText, params (string, object)[] parameters)
   {
      command.CommandText = commandText;

      addParametersToCommand(command, parameters);

      return command.ExecuteReader();
   }

   public Optional<T> ExecuteScalar<T>(string commandText, params (string, object)[] parameters)
   {
      var connection = GetConnection();
      try
      {
         connection.Open();
         connection.BeginTransaction();
         using var command = connection.CreateCommand();

         command.CommandText = commandText;

         addParametersToCommand(command, parameters);

         var obj = command.ExecuteScalar();

         return ConversionFunctions.Result.Cast<T>(obj);
      }
      catch (Exception exception)
      {
         return exception;
      }
      finally
      {
         connection?.Close();
      }
   }

   public Optional<StringHash> ExecuteFirstRow(SQLiteCommand command, string commandText, params (string, object)[] parameters)
   {
      try
      {
         using var reader = ExecuteDataReader(command, commandText, parameters);
         if (reader.Read())
         {
            var result = new StringHash(true);
            var value = reader.GetValues();
            for (var i = 0; i < value.Count; i++)
            {
               result[value.GetKey(i)] = value.Get(i);
            }

            return result;
         }
         else
         {
            return fail("Result set is empty");
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}