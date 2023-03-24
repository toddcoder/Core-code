using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data;

public class AdapterTrying<T> where T : class
{
   protected Adapter<T> adapter;

   public AdapterTrying(Adapter<T> adapter) => this.adapter = adapter;

   public Optional<T> Execute()
   {
      try
      {
         var result = adapter.Execute();
         return adapter.HasRows ? result : fail("No rows");
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<IBulkCopyTarget> BulkCopy<TSource>(Adapter<TSource> sourceAdapter) where TSource : class
   {
      return tryTo(() => adapter.BulkCopy(sourceAdapter));
   }

   public Optional<IBulkCopyTarget> BulkCopy(IDataReader reader, TimeSpan timeout)
   {
      return tryTo(() => adapter.BulkCopy(reader, timeout));
   }

   public Optional<IEnumerable<T>> Enumerable() => tryTo(() => success<IEnumerable<T>>(adapter));

   public Optional<Adapter<T>> WithNewCommand(string newCommand) => tryTo(() => adapter.WithNewCommand(newCommand));

   public Optional<T[]> ToArray() => Enumerable().Map(e => e.ToArray());
}