using System;
using System.Collections.Generic;

namespace Core.Monads
{
   public class ResponseIterator<T>
   {
      protected IEnumerable<Responding<T>> enumerable;
      protected Maybe<Action<T>> response;
      protected Maybe<Action> noResponse;
      protected Maybe<Action<Exception>> failure;

      public ResponseIterator(IEnumerable<Responding<T>> enumerable, Action<T> response = null, Action noResponse = null,
         Action<Exception> failure = null)
      {
         this.enumerable = enumerable;
         this.response = response.Some();
         this.noResponse = noResponse.Some();
         this.failure = failure.Some();
      }

      protected void handle(Responding<T> responding)
      {
         if (responding && response)
         {
            (~response)(~responding);
         }
         else if (responding.AnyException && failure)
         {
            (~failure)(responding.Exception);
         }
         else if (noResponse)
         {
            (~noResponse)();
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