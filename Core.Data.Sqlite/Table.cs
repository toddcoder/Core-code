using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;

namespace Core.Data.Sqlite;

public abstract class Table<TKey, TValue> : IEnumerable<TValue>, IHash<TKey, TValue> where TValue : IMappable, new()
{
   protected Database database;
   protected Lazy<string> fieldNames;

   public event EventHandler<DatabaseEventArgs> DatabaseEvent;
   public event EventHandler<DatabaseFailureArgs> DatabaseFailure;
   public event EventHandler<DatabaseExceptionArgs> DatabaseException;

   public Table(Database database)
   {
      this.database = database;

      this.database.DatabaseEvent += (_, e) => DatabaseEvent?.Invoke(this, e);
      this.database.DatabaseFailure += (_, e) => DatabaseFailure?.Invoke(this, e);
      this.database.DatabaseException += (_, e) => DatabaseException?.Invoke(this, e);

      this.database.CreateDatabaseIfNonExistent();

      fieldNames = new Lazy<string>(() => FieldNames.ToString(", "));
   }

   public abstract IEnumerable<string> FieldNames { get; }

   public abstract string TableName { get; }

   public abstract string KeyName { get; }

   public abstract TKey KeyFromValue(TValue value);

   public virtual Optional<TValue> Retrieve(TKey key)
   {
      var whereClause = $"{KeyName} = $key";
      using var connection = database.GetConnection();
      try
      {
         connection.Open();
         using var command = connection.CreateCommand();
         var _result = database.ExecuteFirstRow(command, $"SELECT {fieldNames.Value} FROM {TableName} WHERE {whereClause}", ("$key", key));
         if (_result)
         {
            var value = new TValue();
            value.ToObject(_result);

            return value;
         }
         else
         {
            return _result.Exception;
         }
      }
      finally
      {
         connection?.Close();
      }
   }

   public virtual Optional<int> Save(TValue value)
   {
      var fromObject = value.FromObject();
      var fields = fromObject.Keys.ToString(", ");
      var parameterNames = fromObject.Keys.Select(key => $"${key}").ToString(", ");
      var commandText = $"REPLACE INTO {TableName} ({fields}) VALUES ({parameterNames})";

      return database.ExecuteNonQuery(commandText, fromObject.Tuples().ToArray());
   }

   public IEnumerator<TValue> GetEnumerator()
   {
      using var connection = database.GetConnection();
      connection.Open();
      using var command = connection.CreateCommand();
      using var reader = database.ExecuteDataReader(command, $"SELECT {fieldNames.Value} FROM {TableName}");

      while (reader.Read())
      {
         var value = new TValue();
         value.ToObject(reader);
         yield return value;
      }

      connection.Close();
   }

   public IEnumerable<TValue> EnumerableWhere(string whereClause, params (string, object)[] parameters)
   {
      using var connection = database.GetConnection();
      connection.Open();
      using var command = connection.CreateCommand();
      using var reader = database.ExecuteDataReader(command, $"SELECT {fieldNames.Value} FROM {TableName} WHERE {whereClause}", parameters);

      while (reader.Read())
      {
         var value = new TValue();
         value.ToObject(reader);
         yield return value;
      }

      connection.Close();
   }

   public Optional<T> ExecuteScalar<T>(string fieldName, string whereClause, params (string, object)[] parameters)
   {
      return database.ExecuteScalar<T>($"SELECT {fieldName} FROM {TableName} WHERE {whereClause}", parameters);
   }

   public long Count()
   {
      var _count = database.ExecuteScalar<long>($"SELECT COUNT({KeyName}) FROM {TableName}");
      return _count ? _count : -1;
   }

   public Optional<int> DeleteAll() => database.ExecuteNonQuery($"DELETE FROM {TableName}");

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public TValue this[TKey key] => Retrieve(key);

   public bool ContainsKey(TKey key)
   {
      var _scalar = database.ExecuteScalar<string>($"SELECT {KeyName} FROM {TableName} WHERE {KeyName} = $key", ("$key", key));
      return _scalar.Map(r => r.Equals(key)).Recover(_ => false);
   }

   public Optional<Hash<TKey, TValue>> AnyHash() => this.ToHash(KeyFromValue);
}