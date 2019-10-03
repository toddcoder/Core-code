using System;

namespace Core.Monads
{
   public interface IMaybe<T> : IHasValue
   {
      bool IsSome { get; }

      bool IsNone { get; }

      T DefaultTo(Func<T> func);

      TResult FlatMap<TResult>(Func<T, TResult> ifSome, Func<TResult> ifNone);

      IMaybe<TResult> Map<TResult>(Func<T, TResult> ifSome);

      IMaybe<TResult> Map<TResult>(Func<T, IMaybe<TResult>> ifSome);

      T Required(string message);

      IResult<T> Result(string message);

      IMaybe<T> Or(IMaybe<T> other);

      IMaybe<T> Or(Func<IMaybe<T>> other);

      IMaybe<T> Or(Func<T> other);

      IMaybe<T> Or(T other);

      bool If(out T value);

      bool Else(out T value);

      void Force(string message);

      void Deconstruct(out bool isSome, out T value);

	   IMaybe<T> IfThen(Action<T> action);

      bool EqualToValueOf(IMaybe<T> otherMaybe);

      bool ValueEqualTo(T otherValue);
   }
}