﻿using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public abstract class Maybe<T>
{
   public class If
   {
      public static If operator &(If @if, bool test) => @if.test && test ? new If(test) : new If(false);

      public static Maybe<T> operator &(If @if, T value) => @if.test ? value : nil;

      public static Maybe<T> operator &(If @if, Func<T> func) => @if.test ? func() : nil;

      public static Maybe<T> operator &(If @if, Maybe<T> maybe) => @if.test ? maybe : nil;

      public static Maybe<T> operator &(If @if, Func<Maybe<T>> maybe) => @if.test ? maybe() : nil;

      protected bool test;

      internal If(bool test)
      {
         this.test = test;
      }
   }

   public static Maybe<T> operator |(Maybe<T> left, Maybe<T> right)
   {
      if (left)
      {
         return left;
      }
      else if (right)
      {
         return right;
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<T> operator |(Maybe<T> left, Lazy.LazyMaybe<T> right)
   {
      if (left)
      {
         return left;
      }
      else if (right)
      {
         return right;
      }
      else
      {
         return nil;
      }
   }

   public static T operator |(Maybe<T> maybe, T defaultValue) => maybe ? maybe : defaultValue;

   public static T operator |(Maybe<T> maybe, Func<T> defaultFunc) => maybe ? maybe : defaultFunc();

   public static implicit operator Maybe<T>(T value) => value.Some();

   public static implicit operator Maybe<T>(Nil _) => new None<T>();

   public static bool operator true(Maybe<T> value) => value is Some<T> || value is Lazy.LazyMaybe<T> lazyMaybe && lazyMaybe;

   public static bool operator false(Maybe<T> value) => value is None<T> || value is Lazy.LazyMaybe<T> lazyMaybe && !lazyMaybe;

   public static bool operator !(Maybe<T> value) => value is None<T> || value is Lazy.LazyMaybe<T> lazyMaybe && !lazyMaybe;

   public static implicit operator bool(Maybe<T> value) => value is Some<T> || value is Lazy.LazyMaybe<T> lazyMaybe && lazyMaybe;

   public static implicit operator T(Maybe<T> value) => value switch
   {
      (true, var internalValue) => internalValue,
      _ => throw new InvalidCastException("Must be a Some to return a value")
   };

   public abstract Maybe<TResult> Map<TResult>(Func<T, TResult> ifSome);

   public abstract Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> ifSome);

   public abstract T Required(string message);

   public abstract Result<T> Result(string message);

   public abstract Optional<T> Optional();

   public abstract void Force(string message);

   public abstract void Deconstruct(out bool isSome, out T value);

   [Obsolete("Use if")]
   public abstract Maybe<T> IfThen(Action<T> action);

   public abstract bool EqualToValueOf(Maybe<T> otherMaybe);

   public abstract bool ValueEqualTo(T otherValue);

   public abstract Maybe<TResult> CastAs<TResult>();

   public abstract Maybe<T> Where(Predicate<T> predicate);

   public virtual Maybe<TResult> SelectMany<TResult>(Func<T, Maybe<TResult>> projection) => Map(projection);

   public abstract Maybe<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection);

   public virtual Maybe<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> projection) => Map(v => projection(v).Maybe());

   public virtual Maybe<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection) => Map(v => projection(v).Maybe());

   public Maybe<T2> SelectMany<T1, T2>(Func<T, Maybe<T1>> func, Func<T, T1, T2> projection)
   {
      return Map(outer => func(outer).Map(inner => projection(outer, inner)));
   }

   public abstract Maybe<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection);

   public abstract Maybe<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection);

   public abstract Maybe<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection);

   public Maybe<TResult> Select<TResult>(Func<T, TResult> func) => Map(func);

   public Maybe<T> Tap(Action<Maybe<T>> action)
   {
      action(this);
      return this;
   }

   public abstract Maybe<T> Initialize(Func<T> initializer);

   public abstract object ToObject();
}