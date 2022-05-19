using System;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class None<T> : Maybe<T>, IEquatable<None<T>>
   {
      public static implicit operator bool(None<T> _) => false;

      internal None()
      {
      }

      public override bool IsSome => false;

      public override bool IsNone => true;

      public override T DefaultTo(Func<T> func) => func();

      public override Maybe<TResult> Map<TResult>(Func<T, TResult> ifSome) => none<TResult>();

      public override Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> ifSome) => nil;

      public override T Required(string message) => throw new ApplicationException(message);

      public override Result<T> Result(string message) => fail(message);

      public override Matched<T> Matched() => nil;

      public override Responding<T> Responding() => nil;

      [Obsolete("Use |")]
      public override Maybe<T> Or(Maybe<T> other) => other;

      [Obsolete("Use |")]
      public override Maybe<T> Or(Func<Maybe<T>> other) => other();

      [Obsolete("Use |")]
      public override Maybe<T> Or(Func<T> other) => other();

      [Obsolete("Use |")]
      public override Maybe<T> Or(T other) => other;

      public override bool Map(out T value)
      {
         value = default;
         return false;
      }

      public override bool UnMap(out T value)
      {
         value = default;
         return true;
      }

      public override void Force(string message) => throw message.Throws();

      public override void Deconstruct(out bool isSome, out T value)
      {
         isSome = false;
         value = default;
      }

      public override Maybe<T> IfThen(Action<T> action) => this;

      public override bool EqualToValueOf(Maybe<T> otherMaybe) => false;

      public override bool ValueEqualTo(T otherValue) => false;

      public override Maybe<TResult> CastAs<TResult>() => nil;

      public override Maybe<T> Where(Predicate<T> predicate) => this;

      public override Maybe<T> Initialize(Func<T> initializer) => initializer();

      public bool Equals(None<T> other) => true;

      public override bool Equals(object obj) => obj is None<T>;

      public override int GetHashCode() => false.GetHashCode();

      public override string ToString() => $"none<{typeof(T).Name}>";
   }
}