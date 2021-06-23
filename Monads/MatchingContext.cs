using System;

namespace Core.Monads
{
   public class MatchingContext<T>
   {
      protected readonly Matched<T> matched;

      public MatchingContext(Matched<T> matched) => this.matched = matched;

      public Matching<T, TResult> IfMatched<TResult>(Func<T, TResult> ifMatched)
      {
         return new Matching<T, TResult>(matched).IfMatched(ifMatched);
      }

      public Matching<T, TResult> IfNotMatched<TResult>(Func<TResult> ifNotMatched)
      {
         return new Matching<T, TResult>(matched).IfNotMatched(ifNotMatched);
      }

      public Matching<T, TResult> IfFailedMatch<TResult>(Func<Exception, TResult> ifFailedMatch)
      {
         return new Matching<T, TResult>(matched).IfFailedMatch(ifFailedMatch);
      }

      public Matched<TResult> Map<TResult>(Func<T, TResult> mapping)
      {
         return new Matching<T, TResult>(matched).Map(mapping);
      }

      public Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> mapping)
      {
         return new Matching<T, TResult>(matched).Map(mapping);
      }

      public Matching<T, TResult> Default<TResult>(Func<TResult> ifDefault)
      {
         return new Matching<T, TResult>(matched).Default(ifDefault);
      }
   }
}