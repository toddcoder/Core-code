using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

internal class ResultIterator<T>
{
   protected IEnumerable<Result<T>> enumerable;
   protected Maybe<Action<T>> _success;
   protected Maybe<Action<Exception>> _failure;

   public ResultIterator(IEnumerable<Result<T>> enumerable, Action<T> ifSuccess = null, Action<Exception> ifFailure = null)
   {
      this.enumerable = enumerable;
      _success = ifSuccess.Some();
      _failure = ifFailure.Some();
   }

   protected void handle(Result<T> result)
   {
      if (result)
      {
         if (_success)
         {
            _success.Value(result.Value);
         }
      }
      else if (_failure)
      {
         _failure.Value(result.Exception);
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

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (_result)
         {
            list.Add(_result.Value);
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

   public (IEnumerable<T> enumerable, Maybe<Exception> exception) SuccessesThenFailure()
   {
      var list = new List<T>();

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (_result)
         {
            list.Add(_result.Value);
         }
         else
         {
            return (list, _result.Exception);
         }
      }

      return (list, nil);
   }

   public Result<IEnumerable<T>> IfAllSuccesses()
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