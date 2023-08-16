using System;
using System.Collections.Generic;

namespace Core.Monads;

public class OptionalIterator<T>
{
   protected IEnumerable<Optional<T>> enumerable;
   protected Maybe<Action<T>> _just;
   protected Maybe<Action> _empty;
   protected Maybe<Action<Exception>> _failed;

   public OptionalIterator(IEnumerable<Optional<T>> enumerable, Action<T> response = null, Action noResponse = null,
      Action<Exception> failure = null)
   {
      this.enumerable = enumerable;
      _just = response.Some();
      _empty = noResponse.Some();
      _failed = failure.Some();
   }

   protected void handle(Optional<T> optional)
   {
      if (optional is (true, var optionalValue) && _just is (true, var action))
      {
         action(optionalValue);
      }
      else if (optional.Exception is (true, var exception) && _failed is (true, var failed))
      {
         failed(exception);
      }
      else if (_empty is (true, var empty))
      {
         empty();
      }
   }

   public IEnumerable<Optional<T>> All()
   {
      foreach (var optional in enumerable)
      {
         handle(optional);
      }

      return enumerable;
   }

   public IEnumerable<T> JustOnly()
   {
      foreach (var optional in enumerable)
      {
         handle(optional);
         if (optional)
         {
            yield return optional;
         }
      }
   }

   public IEnumerable<Exception> FailedOnly()
   {
      foreach (var optional in enumerable)
      {
         handle(optional);
         if (!optional && optional.Exception is (true, var exception))
         {
            yield return exception;
         }
      }
   }
}