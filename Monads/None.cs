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

   public override Optional<T> Optional() => nil;

   public override void Force(string message) => throw fail(message);

   public override void Deconstruct(out bool isSome, out T value)
   {
      isSome = false;
      value = default;
   }

   [Obsolete("Use if")]
   public override Maybe<T> IfThen(Action<T> action) => this;

   public override void MapOf(Action<T> action)
   {
   }

   public override bool EqualToValueOf(Maybe<T> otherMaybe) => false;

   public override bool ValueEqualTo(T otherValue) => false;

   public override Maybe<TResult> CastAs<TResult>() => nil;

   public override Maybe<T> Where(Predicate<T> predicate) => this;
   public override Maybe<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection) => nil;

   public override Maybe<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection) => nil;

   public override Maybe<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection) => nil;

   public override Maybe<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection) => nil;

   public override Maybe<T> Initialize(Func<T> initializer) => initializer();

   public override object ToObject() => nil;

   public bool Equals(None<T> other) => true;

   public override bool Equals(object obj) => obj is None<T>;

   public override int GetHashCode() => false.GetHashCode();

   public override string ToString() => $"none<{typeof(T).Name}>";
}