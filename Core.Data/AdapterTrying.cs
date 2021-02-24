using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data
{
   public class AdapterTrying<T>
      where T : class
   {
      protected Adapter<T> adapter;

      public AdapterTrying(Adapter<T> adapter) => this.adapter = adapter;

      public IResult<T> Execute() => tryTo(() =>
      {
         var result = adapter.Execute();
         return adapter.HasRows ? result.Success() : "No rows".Failure<T>();
      });

      public IResult<IBulkCopyTarget> BulkCopy<TSource>(Adapter<TSource> sourceAdapter)
         where TSource : class
      {
         return tryTo(() => adapter.BulkCopy(sourceAdapter));
      }

      public IResult<IBulkCopyTarget> BulkCopy(IDataReader reader, TimeSpan timeout)
      {
         return tryTo(() => adapter.BulkCopy(reader, timeout));
      }

      public IResult<IEnumerable<T>> Enumerable() => tryTo(() => success<IEnumerable<T>>(adapter));

      public IResult<Adapter<T>> WithNewCommand(string newCommand) => tryTo(() => adapter.WithNewCommand(newCommand));

      public IResult<T[]> ToArray() => Enumerable().Map(e => e.ToArray());
   }
}