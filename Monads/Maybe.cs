using System;

namespace Core.Monads
{
   public abstract class Maybe<T>
   {
      public abstract bool IsSome { get; }

      public abstract bool IsNone { get; }

      public abstract T DefaultTo(Func<T> func);

      public abstract Maybe<TResult> Map<TResult>(Func<T, TResult> ifSome);

      public abstract Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> ifSome);

      public abstract T Required(string message);

      public abstract IResult<T> Result(string message);

      public abstract Maybe<T> Or(Maybe<T> other);

      public abstract Maybe<T> Or(Func<Maybe<T>> other);

      public abstract Maybe<T> Or(Func<T> other);

      public abstract Maybe<T> Or(T other);

      public abstract bool If(out T value);

      public abstract bool Else(out T value);

      public abstract void Force(string message);

      public abstract void Deconstruct(out bool isSome, out T value);

      public abstract Maybe<T> IfThen(Action<T> action);

      public abstract bool EqualToValueOf(Maybe<T> otherMaybe);

      public abstract bool ValueEqualTo(T otherValue);

      public abstract Maybe<TResult> CastAs<TResult>();

      public abstract Maybe<T> Where(Predicate<T> predicate);
   }
}