using System;
using System.Collections.Generic;

namespace Core.Monads
{
   public class ResponseIterator<T>
   {
      protected IEnumerable<Responding<T>> enumerable;
      protected Maybe<Action<T>> _response;
      protected Maybe<Action> _noResponse;
      protected Maybe<Action<Exception>> _failure;

      public ResponseIterator(IEnumerable<Responding<T>> enumerable, Action<T> response = null, Action noResponse = null,
         Action<Exception> failure = null)
      {
         this.enumerable = enumerable;
         _response = response.Some();
         _noResponse = noResponse.Some();
         _failure = failure.Some();
      }

      protected void handle(Responding<T> responding)
      {
         if (responding && _response)
         {
            var action = _response.Value;
            action(responding.Value);
         }
         else if (responding.AnyException && _failure)
         {
            _failure.Value(responding.Exception);
         }
         else if (_noResponse)
         {
            _noResponse.Value();
         }
      }

      public IEnumerable<Responding<T>> All()
      {
         foreach (var match in enumerable)
         {
            handle(match);
         }

         return enumerable;
      }

      public IEnumerable<T> ResponsesOnly()
      {
         foreach (var responding in enumerable)
         {
            handle(responding);
            if (responding)
            {
               yield return responding;
            }
         }
      }

      public IEnumerable<Exception> FailuresOnly()
      {
         foreach (var responding in enumerable)
         {
            handle(responding);
            if (!responding && responding.AnyException)
            {
               yield return responding.Exception;
            }
         }
      }
   }
}