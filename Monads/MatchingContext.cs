using System;

namespace Core.Monads
{
   public class MatchingContext<T>
   {
      protected readonly IMatched<T> matched;

      public MatchingContext(IMatched<T> matched) => this.matched = matched;

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

      public IMatched<TResult> Map<TResult>(Func<T, TResult> mapping)
      {
         return new Matching<T, TResult>(matched).Map(mapping);
      }

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> mapping)
      {
         return new Matching<T, TResult>(matched).Map(mapping);
      }

      public Matching<T, TResult> Default<TResult>(Func<TResult> ifDefault)
      {
         return new Matching<T, TResult>(matched).Default(ifDefault);
      }
   }
}