using System;
using static Core.Lambdas.LambdaFunctions;

namespace Core.Monads
{
	public class Matching<T, TResult>
	{
		IMatched<T> matched;
		Maybe<Func<TResult>> action;

		public Matching(IMatched<T> matched)
		{
			this.matched = matched;
			action = MonadFunctions.none<Func<TResult>>();
		}

		public Matching<T, TResult> IfMatched(Func<T, TResult> ifMatched)
		{
			if (matched.If(out var value))
         {
            action = func(() => ifMatched(value)).Some();
         }

         return this;
		}

		public Matching<T, TResult> IfNotMatched(Func<TResult> ifNotMatched)
		{
			if (matched.IsNotMatched)
         {
            action = ifNotMatched.Some();
         }

         return this;
		}

		public Matching<T, TResult> IfFailedMatch(Func<Exception, TResult> ifFailedMatch)
		{
			if (matched.IfNot(out var anyException) && anyException.If(out var exception))
         {
            action = func(() => ifFailedMatch(exception)).Some();
         }

         return this;
		}

		public IMatched<TResult> Map(Func<T, TResult> mapping)
		{
			if (matched.If(out var value))
         {
            return mapping(value).Match();
         }
         else
         {
            return matched.Unmatched<TResult>();
         }
      }

		public IMatched<TResult> Map(Func<T, IMatched<TResult>> mapping)
		{
			if (matched.If(out var value))
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
            action = ifDefault.Some();
         }

         return this;
		}

		public TResult Get() => action.Required("Action not set")();

		public Result<TResult> Result() => action.Result("Action not set").Map(a => a());
	}
}