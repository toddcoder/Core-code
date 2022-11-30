﻿using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyCompletion<T> : Completion<T>
{
   public static implicit operator bool(LazyCompletion<T> completion)
   {
      completion.ensureValue();
      return completion._value;
   }

   public static implicit operator LazyCompletion<T>(Func<Completion<T>> func) => new(func);

   public static bool operator true(LazyCompletion<T> completion)
   {
      completion.ensureValue();
      return completion._value;
   }

   public static bool operator false(LazyCompletion<T> completion)
   {
      completion.ensureValue();
      return !completion._value;
   }

   public static bool operator !(LazyCompletion<T> completion)
   {
      completion.ensureValue();
      return !completion._value;
   }

   protected Func<Completion<T>> func;
   protected Completion<T> _value;
   protected bool ensured;

   internal LazyCompletion(Func<Completion<T>> func)
   {
      this.func = func;

      _value = nil;
      ensured = false;
   }

   internal LazyCompletion() : this(() => nil)
   {
   }

   public LazyCompletion<T> ValueOf(Func<Completion<T>> func)
   {
      this.func = func;
      return this;
   }

   public LazyCompletion<T> ValueOf(Completion<T> value)
   {
      if (Repeating || !ensured)
      {
         _value = value;
         ensured = false;
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

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted)
   {
      ensureValue();
      return _value.Map(ifCompleted);
   }

   public override Completion<TResult> Map<TResult>(Func<T, TResult> ifCompleted)
   {
      ensureValue();
      return _value.Map(ifCompleted);
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted,
      Func<Completion<TResult>> ifCancelled)
   {
      ensureValue();
      return _value.Map(ifCompleted);
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted,
      Func<Exception, Completion<TResult>> ifInterrupted)
   {
      ensureValue();
      return _value.Map(ifCompleted);
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted,
      Func<Completion<TResult>> ifCancelled, Func<Exception, Completion<TResult>> ifInterrupted)
   {
      ensureValue();
      return _value.Map(ifCompleted);
   }

   public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled,
      Func<Exception, TResult> ifInterrupted)
   {
      ensureValue();
      return _value.FlatMap(ifCompleted, ifCancelled, ifInterrupted);
   }

   public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted)
   {
      ensureValue();
      return _value.FlatMap(ifCompleted, ifNotCompleted);
   }

   public override Completion<T> Map(Action<T> action)
   {
      ensureValue();
      return _value.Map(action);
   }

   public override Completion<T> UnMap(Action action)
   {
      ensureValue();
      return _value.UnMap(action);
   }

   public override Completion<T> UnMap(Action<Exception> action)
   {
      ensureValue();
      return _value.UnMap(action);
   }

   public override Completion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
   {
      ensureValue();
      return _value.Do(ifCompleted, ifNotCompleted);
   }

   public override Completion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
   {
      ensureValue();
      return _value.Do(ifCompleted, ifCancelled, ifInterrupted);
   }

   public override Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection)
   {
      ensureValue();
      return _value.SelectMany(projection);
   }

   public override Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection)
   {
      ensureValue();
      return _value.SelectMany(func, projection);
   }

   public override Completion<TResult> SelectMany<TResult>(Func<T, TResult> func)
   {
      ensureValue();
      return _value.SelectMany(func);
   }

   public override Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func)
   {
      ensureValue();
      return _value.Select(result, func);
   }

   [Obsolete("Use ~")]
   public override bool Map(out T value)
   {
      ensureValue();
      return _value.Map(out value);
   }

   public override bool IfCancelled()
   {
      ensureValue();
      return _value.IfCancelled();
   }

   [Obsolete("Use !")]
   public override bool IfInterrupted(out Exception exception)
   {
      ensureValue();
      return _value.IfInterrupted(out exception);
   }

   [Obsolete("Use ~")]
   public override bool Map(out T value, out Maybe<Exception> _exception)
   {
      ensureValue();
      return _value.Map(out value, out _exception);
   }

   [Obsolete("Use !")]
   public override bool UnMap(out Maybe<Exception> _exception)
   {
      ensureValue();
      return _value.UnMap(out _exception);
   }

   [Obsolete("Use !")]
   public override bool UnMap<TOther>(out Completion<TOther> result)
   {
      ensureValue();
      return _value.UnMap(out result);
   }

   public override Completion<TOther> NotCompleted<TOther>()
   {
      ensureValue();
      return _value.NotCompleted<TOther>();
   }

   [Obsolete("Use ~")]
   public override bool IsCompleted(out Completion<T> completed)
   {
      ensureValue();
      return _value.IsCompleted(out completed);
   }

   [Obsolete("Use !")]
   public override bool NotCompleted(out Completion<T> notCompleted)
   {
      ensureValue();
      return _value.NotCompleted(out notCompleted);
   }

   public override void Force()
   {
      ensureValue();
      _value.Force();
   }

   public override T ForceValue()
   {
      ensureValue();
      return _value.ForceValue();
   }

   public override Completion<T> CancelledOnly()
   {
      ensureValue();
      return _value.CancelledOnly();
   }

   public override Completion<TOther> CancelledOnly<TOther>()
   {
      ensureValue();
      return _value.CancelledOnly<TOther>();
   }

   public override Completion<TOther> NotCompletedOnly<TOther>()
   {
      ensureValue();
      return _value.NotCompletedOnly<TOther>();
   }

   public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception)
   {
      ensureValue();
      _value.Deconstruct(out value, out _exception);
   }

   public override Completion<T> OnCompleted(Action<T> action)
   {
      ensureValue();
      return _value.OnCompleted(action);
   }

   public override Completion<T> OnCancelled(Action action)
   {
      ensureValue();
      return _value.OnCancelled(action);
   }

   public override Completion<T> OnInterrupted(Action<Exception> action)
   {
      ensureValue();
      return _value.OnInterrupted(action);
   }

   public override bool ValueEqualTo(Completion<T> otherCompletion)
   {
      ensureValue();
      return _value.ValueEqualTo(otherCompletion);
   }

   public override bool EqualToValueOf(T otherValue)
   {
      ensureValue();
      return _value.EqualToValueOf(otherValue);
   }

   public override Completion<TResult> CastAs<TResult>()
   {
      ensureValue();
      return _value.CastAs<TResult>();
   }

   public override Completion<T> Where(Predicate<T> predicate)
   {
      ensureValue();
      return _value.Where(predicate);
   }

   public override Completion<T> Where(Predicate<T> predicate, string exceptionMessage)
   {
      ensureValue();
      return _value.Where(predicate, exceptionMessage);
   }

   public override Completion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage)
   {
      ensureValue();
      return _value.Where(predicate, exceptionMessage);
   }

   public override T DefaultTo(Func<Maybe<Exception>, T> defaultFunc)
   {
      ensureValue();
      return _value.DefaultTo(defaultFunc);
   }

   public override Maybe<T> Maybe()
   {
      ensureValue();
      return _value.Maybe();
   }

   public override Result<T> Result()
   {
      ensureValue();
      return _value.Result();
   }

   public override Responding<T> Responding()
   {
      ensureValue();
      return _value.Responding();
   }

   public override T Value
   {
      get
      {
         ensureValue();
         return ~_value;
      }
   }

   public override Exception Exception
   {
      get
      {
         ensureValue();
         return _value.Exception;
      }
   }

   public override Maybe<Exception> AnyException
   {
      get
      {
         ensureValue();
         return _value.AnyException;
      }
   }

   public override string ToString() => _value.ToString();
}