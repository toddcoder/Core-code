using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
	public class FailedMatch<T> : IMatched<T>
	{
		internal FailedMatch(Exception exception)
		{
			Exception = Exception = exception is FullStackException ? exception : new FullStackException(exception);
		}

		public IMatched<T> Do(Action<T> ifMatched, Action ifNotOrFailed)
		{
			ifNotOrFailed();
			return this;
		}

		public IMatched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch)
		{
			ifFailedMatch(Exception);
			return this;
		}

		public Exception Exception { get; }

		public IMatched<TOther> ExceptionAs<TOther>() => failedMatch<TOther>(Exception);

		public IMatched<T> Or(IMatched<T> other) => other;

		public IMatched<T> Or(Func<IMatched<T>> other) => other();

		public IMatched<TResult> SelectMany<TResult>(Func<T, IMatched<TResult>> projection)
		{
			return failedMatch<TResult>(Exception);
		}

		public IMatched<T2> SelectMany<T1, T2>(Func<T, IMatched<T1>> func, Func<T, T1, T2> projection)
		{
			return failedMatch<T2>(Exception);
		}

		public IMatched<TResult> SelectMany<TResult>(Func<T, TResult> func) => failedMatch<TResult>(Exception);

		public IMatched<TResult> Select<TResult>(IMatched<T> result, Func<T, TResult> func)
		{
			return failedMatch<TResult>(Exception);
		}

		public bool If(out T value)
		{
			value = default;
			return false;
		}

		public bool IfNotMatched() => false;

		public bool Failed(out Exception exception)
		{
			exception = Exception;
			return true;
		}

		public bool Out(out T value, out IMatched<T> original)
		{
			value = default;
			original = this;

			return false;
		}

		public bool If(out T value, out bool isNotMatched, out Exception exception)
		{
			value = default;
			isNotMatched = false;
			exception = Exception;

			return false;
		}

		public bool If(out T value, out IMaybe<Exception> exception)
		{
			value = default;
			exception = Exception.Some();

			return false;
		}

		public bool Else<TOther>(out IMatched<TOther> result)
		{
			result = failedMatch<TOther>(Exception);
			return true;
		}

		public IMatched<TOther> Unmatched<TOther>() => failedMatch<TOther>(Exception);

		public bool WasMatched(out IMatched<T> matched)
		{
			matched = this;
			return false;
		}

		public void Force() => throw Exception;

      public T ForceValue() => throw Exception;

      public IMatched<T> UnmatchedOnly() => throw Exception;

		public IMatched<TOther> UnmatchedOnly<TOther>() => throw Exception;

		public void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> exception)
		{
			value = none<T>();
			exception = Exception.Some();
		}

		public bool IsMatched => false;

		public bool IsNotMatched => false;

		public bool IsFailedMatch => true;

		public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched) => failedMatch<TResult>(Exception);

		public IMatched<TResult> Map<TResult>(Func<T, TResult> ifMatched) => failedMatch<TResult>(Exception);

		public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched)
		{
			return failedMatch<TResult>(Exception);
		}

		public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<Exception,
			IMatched<TResult>> ifFailedMatch)
		{
			return ifFailedMatch(Exception);
		}

		public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched,
			Func<IMatched<TResult>> ifNotMatched, Func<Exception, IMatched<TResult>> ifFailedMatch)
		{
			return ifFailedMatch(Exception);
		}

		public TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched,
			Func<Exception, TResult> ifFailedMatch)
		{
			return ifFailedMatch(Exception);
		}

		public TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed) => ifNotOrFailed();

		public IMatched<T> If(Action<T> action) => this;

		public IMatched<T> Else(Action action) => this;

		public IMatched<T> Else(Action<Exception> action)
		{
			action(Exception);
			return this;
		}
	}
}