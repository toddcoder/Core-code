using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   internal class ResultIterator<T>
   {
      protected IEnumerable<Result<T>> enumerable;
      protected Maybe<Action<T>> success;
      protected Maybe<Action<Exception>> failure;

      public ResultIterator(IEnumerable<Result<T>> enumerable, Action<T> ifSuccess = null, Action<Exception> ifFailure = null)
      {
         this.enumerable = enumerable;
         success = ifSuccess.Some();
         failure = ifFailure.Some();
      }

      protected void handle(Result<T> result)
      {
         if (result.If(out var value, out var exception))
         {
            if (success.If(out var action))
            {
               action(value);
            }
         }
         else if (failure.If(out var faction))
         {
            faction(exception);
         }
      }

      public IEnumerable<Result<T>> All()
      {
         foreach (var result in enumerable)
         {
            handle(result);
         }

         return enumerable;
      }

      public IEnumerable<T> SuccessesOnly()
      {
         var list = new List<T>();

         foreach (var result in enumerable)
         {
            handle(result);
            if (result.If(out var t))
            {
               list.Add(t);
            }
         }

         return list;
      }

      public IEnumerable<Exception> FailuresOnly()
      {
         var list = new List<Exception>();

         foreach (var result in enumerable)
         {
            handle(result);
            if (result.IfNot(out var e))
            {
               list.Add(e);
            }
         }

         return list;
      }

      public (IEnumerable<T> enumerable, Maybe<Exception> exception) SuccessesThenFailure()
      {
         var list = new List<T>();

         foreach (var result in enumerable)
         {
            handle(result);
            if (result.If(out var value, out var exception))
            {
               list.Add(value);
            }
            else
            {
               return (list, exception);
            }
         }

         return (list, nil);
      }

      public Result<IEnumerable<T>> IfAllSuccesses()
      {
         var list = new List<T>();

         foreach (var result in enumerable)
         {
            handle(result);
            if (result.If(out var value, out var exception))
            {
               list.Add(value);
            }
            else
            {
               return exception;
            }
         }

         return list;
      }
   }
}