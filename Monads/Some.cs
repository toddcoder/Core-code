using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Some<T> : IMaybe<T>, IEquatable<Some<T>>
   {
      public static implicit operator bool(Some<T> _) => true;

      public static bool operator &(Some<T> x, IHasValue y) => y.HasValue;

      public static bool operator |(Some<T> x, IHasValue y) => true;

      public static bool operator !(Some<T> _) => false;

      protected T value;

      internal Some(T value) => this.value = value;

      public bool IsSome => true;

      public bool IsNone => false;

      public T DefaultTo(Func<T> func) => value;

      public TResult FlatMap<TResult>(Func<T, TResult> ifSome, Func<TResult> ifNone) => ifSome(value);

      public IMaybe<TResult> Map<TResult>(Func<T, TResult> ifSome) => ifSome(value).Some();

      public IMaybe<TResult> Map<TResult>(Func<T, IMaybe<TResult>> ifSome) => ifSome(value);

      public IMaybe<T> If(Action<T> action)
      {
         action(value);
         return this;
      }

      public T Required(string message) => value;

      public IResult<T> Result(string message) => value.Success();

      public IMaybe<T> Or(IMaybe<T> other) => this;

      public IMaybe<T> Or(Func<IMaybe<T>> other) => this;

      public IMaybe<T> Or(Func<T> other) => this;

      public IMaybe<T> Or(T other) => this;

      public bool If(out T value)
      {
         value = this.value;
         return true;
      }

      public bool Else(out T value)
      {
         value = this.value;
         return false;
      }

      public void Force(string message) { }

      public void Deconstruct(out bool isSome, out T value)
      {
         isSome = true;
         value = this.value;
      }

	   public IMaybe<T> IfThen(Action<T> action)
	   {
		   action(value);
         return this;
	   }

      public bool EqualToValueOf(IMaybe<T> otherMaybe) => otherMaybe.If(out var otherValue) && ValueEqualTo(otherValue);

      public bool ValueEqualTo(T otherValue) => value.Equals(otherValue);

      public IMaybe<object> AsObject() => value.Some<object>();

      public IMaybe<TResult> CastAs<TResult>()
      {
         if (value is TResult result)
         {
            return result.Some();
         }
         else
         {
            return none<TResult>();
         }
      }

      public IMaybe<T> Where(Predicate<T> predicate) => predicate(value) ? this : none<T>();

      public bool HasValue => true;

      public bool Equals(Some<T> other)
      {
         if (ReferenceEquals(null, other))
         {
            return false;
         }

         if (ReferenceEquals(this, other))
         {
            return true;
         }

         return EqualityComparer<T>.Default.Equals(value, other.value);
      }

      public override bool Equals(object obj) => obj is Some<T> other && Equals(other);

      public override int GetHashCode() => value.GetHashCode();
   }
}