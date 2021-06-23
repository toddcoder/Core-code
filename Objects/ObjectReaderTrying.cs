using System;
using System.Linq.Expressions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects
{
   public class ObjectReaderTrying
   {
      protected ObjectReader reader;

      public ObjectReaderTrying(ObjectReader reader) => this.reader = reader;

      public Result<TResult> Invoke<T, TResult>(Expression<Func<T, TResult>> expression) => tryTo(() => reader.Invoke(expression));

      public Result<TResult> Invoke<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression) => tryTo(() => reader.Invoke(expression));

      public Result<TResult> Invoke<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> expression) => tryTo(() => reader.Invoke(expression));

      public Result<TResult> Invoke<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression)
      {
         return tryTo(() => reader.Invoke(expression));
      }

      public Result<TResult> Invoke<T1, T2, T3, T4, T5, TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> expression)
      {
         return tryTo(() => reader.Invoke(expression));
      }

      public Result<TResult> Invoke<T1, T2, T3, T4, T5, T6, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6,
         TResult>> expression)
      {
         return tryTo(() => reader.Invoke(expression));
      }

      public Result<Unit> Do<T>(Expression<Action<T>> expression) => tryTo(() => reader.Do(expression));

      public Result<Unit> Do<T1, T2>(Expression<Action<T1, T2>> expression) => tryTo(() => reader.Do(expression));

      public Result<Unit> Do<T1, T2, T3>(Expression<Action<T1, T2, T3>> expression) => tryTo(() => reader.Do(expression));

      public Result<Unit> Do<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> expression) => tryTo(() => reader.Do(expression));

      public Result<Unit> Do<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> expression) => tryTo(() => reader.Do(expression));

      public Result<Unit> Do<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> expression) => tryTo(() => reader.Do(expression));

      public Result<T> Assign<T>(string name) => tryTo(() => reader.Assign<T>(name));
   }
}