using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Some<T> : Maybe<T>, IEquatable<Some<T>>
{
   public static implicit operator bool(Some<T> _) => true;

   protected T value;

   internal Some(T value) => this.value = value;

   public override Maybe<TResult> Map<TResult>(Func<T, TResult> ifSome) => ifSome(value).Some();

   public override Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> ifSome) => ifSome(value);

   public override T Required(string message) => value;

   public override Result<T> Result(string message) => value;

   public override Responding<T> Responding() => value;

   public override void Force(string message)
   {
   }

   public override void Deconstruct(out bool isSome, out T value)
   {
      isSome = true;
      value = this.value;
   }

   public override Maybe<T> IfThen(Action<T> action)
   {
      action(value);
      return this;
   }

   public override bool EqualToValueOf(Maybe<T> otherMaybe) => otherMaybe.Map(ValueEqualTo) | false;

   public override bool ValueEqualTo(T otherValue) => value.Equals(otherValue);

   public override Maybe<TResult> CastAs<TResult>()
   {
      if (value is TResult result)
      {
         return result;
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<T> Where(Predicate<T> predicate) => predicate(value) ? this : nil;

   public override Maybe<T> Initialize(Func<T> initializer) => this;

   public override object ToObject() => value;

   public bool Equals(Some<T> other)
   {
      return other is not null && ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(value, other.value);
   }

   public override bool Equals(object obj) => obj is Some<T> other && Equals(other);

   public override int GetHashCode() => value.GetHashCode();

   public override string ToString() => value.ToString();
}