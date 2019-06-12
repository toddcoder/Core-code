using System;
using Standard.Types.Exceptions;
using static Standard.Types.Monads.MonadFunctions;

namespace Standard.Types.Monads
{
	public class None<T> : IMaybe<T>
	{
		internal None() { }

		public bool IsSome => false;

		public bool IsNone => true;

		public T DefaultTo(Func<T> func) => func();

		public TResult FlatMap<TResult>(Func<T, TResult> ifSome, Func<TResult> ifNone) => ifNone();

		public IMaybe<TResult> Map<TResult>(Func<T, TResult> ifSome) => none<TResult>();

		public IMaybe<TResult> Map<TResult>(Func<T, IMaybe<TResult>> ifSome) => none<TResult>();

		public T Required(string message) => throw new ApplicationException(message);

		public IResult<T> Result(string message) => message.Failure<T>();

		public IMaybe<T> Or(IMaybe<T> other) => other;

		public IMaybe<T> Or(Func<IMaybe<T>> other) => other();

		public IMaybe<T> Or(Func<T> other) => other().Some();

		public IMaybe<T> Or(T other) => other.Some();

		public bool If(out T value)
		{
			value = default;
			return false;
		}

		public bool Else(out T value)
		{
			value = default;
			return true;
		}

		public void Force(string message) => throw message.Throws();

		public void Deconstruct(out bool isSome, out T value)
		{
			isSome = false;
			value = default;
		}

		public IMaybe<T> IfThen(Action<T> action) => this;
	}
}