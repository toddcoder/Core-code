using System;
using static Core.Lambdas.LambdaFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
	public class Matching<T, TResult>
	{
		protected Matched<T> matched;
		protected Maybe<Func<TResult>> action;

		public Matching(Matched<T> matched)
		{
			this.matched = matched;
			action = nil;
		}

		public Matching<T, TResult> IfMatched(Func<T, TResult> ifMatched)
		{
			if (matched.Map(out var value))
         {
            action = func(() => ifMatched(value));
         }

         return this;
		}

		public Matching<T, TResult> IfNotMatched(Func<TResult> ifNotMatched)
		{
			if (matched.IsNotMatched)
         {
            action = ifNotMatched;
         }

         return this;
		}

		public Matching<T, TResult> IfFailedMatch(Func<Exception, TResult> ifFailedMatch)
		{
			if (matched.UnMap(out var anyException) && anyException.Map(out var exception))
         {
            action = func(() => ifFailedMatch(exception));
         }

         return this;
		}

		public Matched<TResult> Map(Func<T, TResult> mapping)
		{
			if (matched.Map(out var value))
         {
            return mapping(value);
         }
         else
         {
            return matched.Unmatched<TResult>();
         }
      }

		public Matched<TResult> Map(Func<T, Matched<TResult>> mapping)
		{
			if (matched.Map(out var value))
         {
            return mapping(value);
         }
         else
         {
            return matched.Unmatched<TResult>();
         }
      }

		public Matching<T, TResult> Default(Func<TResult> ifDefault)
		{
			if (action.IsNone)
         {
            action = ifDefault;
         }

         return this;
		}

		public TResult Get() => action.Required("Action not set")();

		public Result<TResult> Result() => action.Result("Action not set").Map(a => a());
	}
}