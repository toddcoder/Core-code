using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Core.Assertions;
using Core.Data.DataSources;
using Core.Data.Setups;
using Core.Exceptions;
using Core.Monads;
using Core.Objects;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;

namespace Core.Data
{
   public class Adapter<T> : IEnumerable<T>
      where T : class
   {
      public static IResult<Adapter<T>> FromSetup(ISetup setup, T entity) => tryTo(() => new Adapter<T>(entity, setup));

      protected T entity;
      protected Func<T> newFunc;

      public Adapter(T entity, ISetup setup)
      {
         DataSource = setup.DataSource;
         if (setup is ISetupWithInfo setupWithInfo && setupWithInfo.Handler.If(out var handler))
         {
            DataSource.SetMessageHandler(handler);
         }

         Command = setup.CommandText;
         Parameters = new Parameters.Parameters(setup.Parameters);
         Fields = new Fields.Fields(setup.Fields);

         this.entity = assert(() => (object)entity).Must().Not.BeNull().Force<T>();
         setEntityType();

         newFunc = () => entity;
      }

      internal Adapter(Adapter<T> other, string command)
      {
         DataSource = other.DataSource;
         Command = command;
         Parameters = other.Parameters;
         Fields = other.Fields;
         entity = other.entity;
         setEntityType();

         newFunc = () => entity;
      }

      public T Entity
      {
         get => entity;
         set
         {
            entity = assert(() => value).Must().Not.BeNull().Force<T>();
            setEntityType();
         }
      }

      public Func<T> NewFunc
      {
         get => newFunc;
         set => newFunc = value;
      }

      public DataSource DataSource { get; set; }

      public string Command { get; set; }

      public Parameters.Parameters Parameters { get; set; }

      public Fields.Fields Fields { get; set; }

      public int RecordsAffected { get; set; }

      public string ConnectionString
      {
         get => DataSource.ConnectionString;
         set => DataSource.ConnectionString = value;
      }

      protected void setEntityType()
      {
         Parameters.DeterminePropertyTypes(entity);
         Fields.DeterminePropertyTypes(entity);
      }

      public T Execute()
      {
         RecordsAffected = DataSource.Execute(entity, Command, Parameters, Fields);
         return entity;
      }

      public IDataReader ExecuteReader() => DataSource.ExecuteReader(entity, Command, Parameters);

      public IBulkCopyTarget BulkCopy<TSource>(Adapter<TSource> sourceAdapter)
         where TSource : class
      {
         if (DataSource is IBulkCopyTarget bulkCopy)
         {
            bulkCopy.TableName = Command;
            bulkCopy.Copy(sourceAdapter);

            return bulkCopy;
         }
         else
         {
            throw "This data source doesn't support a bulk copy".Throws();
         }
      }

      public IBulkCopyTarget BulkCopy(IDataReader reader, TimeSpan timeout)
      {
         if (DataSource is IBulkCopyTarget bulkCopy)
         {
            bulkCopy.TableName = Command;
            bulkCopy.Copy(reader, timeout);

            return bulkCopy;
         }
         else
         {
            throw "This data source doesn't support a bulk copy".Throws();
         }
      }

      public IDbConnection NativeConnection() => DataSource.GetConnection();

      public Reader<T> Reader() => new Reader<T>(DataSource, Entity, newFunc, Command, Parameters, Fields);

      public DataSet DataSet()
      {
         if (DataSource is SQLDataSource ds)
         {
            return ds.DataSet(entity, Command, Parameters);
         }
         else
         {
            throw new ApplicationException("You may only use a SQLDataSource for this function.");
         }
      }

      public IEnumerator<T> GetEnumerator() => Reader().GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public AdapterTrying<T> TryTo => new AdapterTrying<T>(this);

      public Adapter<T> WithNewCommand(string newCommand) => new Adapter<T>(this, newCommand);
   }

   public class Adapter : Adapter<DataContainer>
   {
      public Adapter(ISetup setup) : base(new DataContainer(), setup) { }

      internal Adapter(Adapter other, string command) : base(other, command) { }
   }
}