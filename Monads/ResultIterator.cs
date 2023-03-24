using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

internal class ResultIterator<T>
{
   protected IEnumerable<Optional<T>> enumerable;
   protected Optional<Action<T>> _success;
   protected Optional<Action<Exception>> _failure;

   public ResultIterator(IEnumerable<Optional<T>> enumerable, Action<T> ifSuccess = null, Action<Exception> ifFailure = null)
   {
      this.enumerable = enumerable;
      _success = ifSuccess.Some();
      _failure = ifFailure.Some();
   }

   protected void handle(Optional<T> result)
   {
      if (result is (true, var resultValue))
      {
         if (_success is (true, var success))
         {
            success(resultValue);
         }
      }
      else if (_failure is (true, var failure))
      {
         failure(result.Exception);
      }
   }

   public IEnumerable<Optional<T>> All()
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

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (_result is (true, var result))
         {
            list.Add(result);
         }
      }

      return list;
   }

   public IEnumerable<Exception> FailuresOnly()
   {
      var list = new List<Exception>();

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (!_result)
         {
            list.Add(_result.Exception);
         }
      }

      return list;
   }

   public (IEnumerable<T> enumerable, Optional<Exception> exception) SuccessesThenFailure()
   {
      var list = new List<T>();

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (_result is (true, var result))
         {
            list.Add(result);
         }
         else
         {
            return (list, _result.Exception);
         }
      }

      return (list, nil);
   }

   public Optional<IEnumerable<T>> IfAllSuccesses()
   {
      var list = new List<T>();

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (_result)
         {
            list.Add(_result);
         }
         else
         {
            return _result.Exception;
         }
      }

      return list;
   }
}