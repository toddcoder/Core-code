using System;

namespace Core.Monads
{
   public abstract class Maybe<T>
   {
      public static Maybe<T> operator |(Maybe<T> left, Maybe<T> right)
      {
         if (left.IsSome)
         {
            return left;
         }
         else if (right.IsSome)
         {
            return right;
         }
         else
         {
            return new None<T>();
         }
      }

      public abstract bool IsSome { get; }

      public abstract bool IsNone { get; }

      public abstract T DefaultTo(Func<T> func);

      public abstract Maybe<TResult> Map<TResult>(Func<T, TResult> ifSome);

      public abstract Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> ifSome);

      public abstract T Required(string message);

      public abstract Result<T> Result(string message);

      [Obsolete("Use |")]
      public abstract Maybe<T> Or(Maybe<T> other);

      [Obsolete("Use |")]
      public abstract Maybe<T> Or(Func<Maybe<T>> other);

      [Obsolete("Use |")]
      public abstract Maybe<T> Or(Func<T> other);

      [Obsolete("Use |")]
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

      public Maybe<TResult> SelectMany<TResult>(Func<T, Maybe<TResult>> projection) => Map(projection);

      public Maybe<T2> SelectMany<T1, T2>(Func<T, Maybe<T1>> func, Func<T, T1, T2> projection)
      {
         return Map(outer => func(outer).Map(inner => projection(outer, inner)));
      }

      public Maybe<TResult> Select<TResult>(Func<T, TResult> func) => Map(func);

      public Maybe<T> Tap(Action<Maybe<T>> action)
      {
         action(this);
         return this;
      }
   }
}