using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class None<T> : Maybe<T>, IEquatable<None<T>>
{
   public static implicit operator bool(None<T> _) => false;

   internal None()
   {
   }

   public override Maybe<TResult> Map<TResult>(Func<T, TResult> ifSome) => nil;

   public override Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> ifSome) => nil;

   public override T Required(string message) => throw new ApplicationException(message);

   public override Result<T> Result(string message) => fail(message);

   public override Responding<T> Responding() => nil;

   public override void Force(string message) => throw fail(message);

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