﻿using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyMaybe<T> : Maybe<T>, IEquatable<LazyMaybe<T>>
{
   public static implicit operator bool(LazyMaybe<T> maybe)
   {
      maybe.ensureValue();
      return maybe._value;
   }

   public static implicit operator LazyMaybe<T>(Func<Maybe<T>> func) => new(func);

   public static bool operator true(LazyMaybe<T> maybe)
   {
      maybe.ensureValue();
      return maybe._value;
   }

   public static bool operator false(LazyMaybe<T> maybe)
   {
      maybe.ensureValue();
      return !maybe._value;
   }

   public static bool operator !(LazyMaybe<T> value) => value is None<T>;

   protected Func<Maybe<T>> func;
   protected Maybe<T> _value;
   protected bool ensured;

   internal LazyMaybe(Func<Maybe<T>> func)
   {
      this.func = func;

      _value = nil;
      ensured = false;
   }

   internal LazyMaybe() : this(() => nil)
   {
   }

   public LazyMaybe<T> ValueOf(Func<Maybe<T>> func)
   {
      this.func = func;
      return this;
   }

   public LazyMaybe<T> ValueOf(Maybe<T> value)
   {
      if (Repeating || !ensured)
      {
         _value = value;
         ensured = true;
      }

      return this;
   }

   public bool Repeating { get; set; }

   protected void ensureValue()
   {
      if (!ensured)
      {
         _value = func();
         ensured = true;
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

   public override Maybe<TResult> Map<TResult>(Func<T, TResult> ifSome)
   {
      ensureValue();
      return _value.Map(ifSome);
   }

   public override Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> ifSome)
   {
      ensureValue();
      return _value.Map(ifSome);
   }

   public override T Required(string message)
   {
      ensureValue();
      return _value.Required(message);
   }

   public override Result<T> Result(string message)
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

   public override Maybe<T> IfThen(Action<T> action)
   {
      ensureValue();
      return _value.IfThen(action);
   }

   public override bool EqualToValueOf(Maybe<T> otherMaybe)
   {
      ensureValue();
      return _value.EqualToValueOf(otherMaybe);
   }

   public override bool ValueEqualTo(T otherValue)
   {
      ensureValue();
      return _value.ValueEqualTo(otherValue);
   }

   public override Maybe<TResult> CastAs<TResult>()
   {
      ensureValue();
      return _value.CastAs<TResult>();
   }

   public override Maybe<T> Where(Predicate<T> predicate)
   {
      ensureValue();
      return _value.Where(predicate);
   }

   public override Maybe<T> Initialize(Func<T> initializer)
   {
      ensureValue();
      return _value.Initialize(initializer);
   }

   public bool Equals(LazyMaybe<T> other)
   {
      ensureValue();
      return _value == other._value;
   }

   public override bool Equals(object obj)
   {
      ensureValue();
      return obj is LazyMaybe<T> other && Equals(other);
   }

   public override int GetHashCode()
   {
      ensureValue();
      return _value.GetHashCode();
   }

   public static bool operator ==(LazyMaybe<T> left, LazyMaybe<T> right) => Equals(left, right);

   public static bool operator !=(LazyMaybe<T> left, LazyMaybe<T> right) => !Equals(left, right);

   public override string ToString() => _value.ToString();
}