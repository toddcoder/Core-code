using System;
using System.Collections.Generic;

namespace Core.Monads
{
   public class MatchedIterator<T>
   {
      IEnumerable<Matched<T>> enumerable;
      Maybe<Action<T>> matched;
      Maybe<Action> notMatched;
      Maybe<Action<Exception>> failure;

      public MatchedIterator(IEnumerable<Matched<T>> enumerable, Action<T> matched = null, Action notMatched = null,
         Action<Exception> failure = null)
      {
         this.enumerable = enumerable;
         this.matched = matched.Some();
         this.notMatched = notMatched.Some();
         this.failure = failure.Some();
      }

      void handle(Matched<T> match)
      {
         if (match.Map(out var value, out var exception) && matched.Map(out var action))
         {
            action(value);
         }
         else if (exception.Map(out var e) && failure.Map(out var exAction))
         {
            exAction(e);
         }
         else if (notMatched.Map(out var nAction))
         {
            nAction();
         }
      }

      public IEnumerable<Matched<T>> All()
      {
         foreach (var match in enumerable)
         {
            handle(match);
         }

         return enumerable;
      }

      public IEnumerable<T> MatchesOnly()
      {
         foreach (var match in enumerable)
         {
            handle(match);
            if (match.Map(out var value))
            {
               yield return value;
            }
         }
      }

      public IEnumerable<Exception> FailuresOnly()
      {
         foreach (var match in enumerable)
         {
            handle(match);
            if (!match.Map(out _, out var exception) && exception.Map(out var e))
            {
               yield return e;
            }
         }
      }
   }
}