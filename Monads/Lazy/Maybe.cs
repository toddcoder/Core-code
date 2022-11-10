using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class Maybe<T> : Monads.Maybe<T>, IEquatable<Maybe<T>>
{
   public static implicit operator bool(Maybe<T> maybe)
   {
      maybe.ensureValue();
      return maybe._value;
   }

   public static implicit operator Maybe<T>(Func<Monads.Maybe<T>> func) => new(func);

   public static bool operator true(Maybe<T> maybe)
   {
      maybe.ensureValue();
      return maybe._value;
   }

   public static bool operator false(Maybe<T> maybe)
   {
      maybe.ensureValue();
      return !maybe._value;
   }

   public static bool operator !(Maybe<T> value) => value is None<T>;

   protected Func<Monads.Maybe<T>> func;
   protected Monads.Maybe<T> _value;

   internal Maybe(Func<Monads.Maybe<T>> func)
   {
      this.func = func;

      _value = nil;
   }

   protected void ensureValue()
   {
      if (!_value)
      {
         _value = func();
      }
   }

   public override T Value
   {
      get
      {
         ensureValue();
         return ~_value;
      }
   }

   public override Monads.Maybe<TResult> Map<TResult>(Func<T, TResult> ifSome)
   {
      ensureValue();
      return _value.Map(ifSome);
   }

   public override Monads.Maybe<TResult> Map<TResult>(Func<T, Monads.Maybe<TResult>> ifSome)
   {
      ensureValue();
      return _value.Map(ifSome);
   }

   public override T Required(string message)
   {
      ensureValue();
      return _value.Required(message);
   }

   public override Monads.Result<T> Result(string message)
   {
      ensureValue();
      return _value.Result(message);
   }

   public override Responding<T> Responding()
   {
      ensureValue();
      return _value.Responding();
   }

   [Obsolete("Use ~")]
   public override bool Map(out T value)
   {
      ensureValue();
      return _value.Map(out value);
   }

   [Obsolete("Use !")]
   public override bool UnMap(out T value)
   {
      ensureValue();
      return _value.UnMap(out value);
   }

   public override void Force(string message)
   {
      ensureValue();
      _value.Force(message);
   }

   public override void Deconstruct(out bool isSome, out T value)
   {
      ensureValue();
      _value.Deconstruct(out isSome, out value);
   }

   public override Monads.Maybe<T> IfThen(Action<T> action)
   {
      ensureValue();
      return _value.IfThen(action);
   }

   public override bool EqualToValueOf(Monads.Maybe<T> otherMaybe)
   {
      ensureValue();
      return _value.EqualToValueOf(otherMaybe);
   }

   public override bool ValueEqualTo(T otherValue)
   {
      ensureValue();
      return _value.ValueEqualTo(otherValue);
   }

   public override Monads.Maybe<TResult> CastAs<TResult>()
   {
      ensureValue();
      return _value.CastAs<TResult>();
   }

   public override Monads.Maybe<T> Where(Predicate<T> predicate)
   {
      ensureValue();
      return _value.Where(predicate);
   }

   public override Monads.Maybe<T> Initialize(Func<T> initializer)
   {
      ensureValue();
      return _value.Initialize(initializer);
   }

   public bool Equals(Maybe<T> other)
   {
      ensureValue();
      return _value == other._value;
   }

   public override bool Equals(object obj)
   {
      ensureValue();
      return obj is Maybe<T> other && Equals(other);
   }

   public override int GetHashCode()
   {
      ensureValue();
      return _value.GetHashCode();
   }

   public static bool operator ==(Maybe<T> left, Maybe<T> right) => Equals(left, right);

   public static bool operator !=(Maybe<T> left, Maybe<T> right) => !Equals(left, right);
}