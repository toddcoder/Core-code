using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class Result<T> : Monads.Result<T>, IEquatable<Result<T>>
{
   public static implicit operator bool(Result<T> result)
   {
      result.ensureValue();
      return result._value;
   }

   public static implicit operator Result<T>(Func<Monads.Result<T>> func) => new(func);

   public static bool operator true(Result<T> result)
   {
      result.ensureValue();
      return result._value;
   }

   public static bool operator false(Result<T> result)
   {
      result.ensureValue();
      return !result._value;
   }

   public static bool operator !(Result<T> result)
   {
      result.ensureValue();
      return !result._value;
   }

   protected Func<Monads.Result<T>> func;
   protected Monads.Result<T> _value;

   internal Result(Func<Monads.Result<T>> func)
   {
      this.func = func;

      _value = fail("Uninitialized");
   }

   protected void ensureValue()
   {
      if (!_value)
      {
         _value = func();
      }
   }

   [Obsolete("Use ~")]
   public override bool Map(out T value, out Exception exception)
   {
      ensureValue();
      return _value.Map(out value, out exception);
   }

   public override Monads.Result<TResult> Map<TResult>(Func<T, Monads.Result<TResult>> ifSuccessful)
   {
      ensureValue();
      return _value.Map(ifSuccessful);
   }

   public override Monads.Result<TResult> Map<TResult>(Func<T, TResult> ifSuccessful)
   {
      ensureValue();
      return _value.Map(ifSuccessful);
   }

   public override Monads.Result<TResult> SelectMany<TResult>(Func<T, Monads.Result<TResult>> projection)
   {
      ensureValue();
      return _value.SelectMany(projection);
   }

   public override Monads.Result<T2> SelectMany<T1, T2>(Func<T, Monads.Result<T1>> func, Func<T, T1, T2> projection)
   {
      ensureValue();
      return _value.SelectMany(func, projection);
   }

   public override Monads.Result<TResult> SelectMany<TResult>(Func<T, TResult> func)
   {
      ensureValue();
      return _value.SelectMany(func);
   }

   public override T Recover(Func<Exception, T> recovery)
   {
      ensureValue();
      return _value.Recover(recovery);
   }

   public override Monads.Result<Unit> Unit => unit;

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

   public override Monads.Result<T> Always(Action action)
   {
      ensureValue();
      return _value.Always(action);
   }

   [Obsolete("Use ~")]
   public override bool Map(out T value)
   {
      ensureValue();
      return _value.Map(out value);
   }

   [Obsolete("Use !")]
   public override bool UnMap(out Exception exception)
   {
      ensureValue();
      return _value.UnMap(out exception);
   }

   [Obsolete("Use !")]
   public override bool UnMap(out T value, out Exception exception)
   {
      ensureValue();
      return _value.UnMap(out value, out exception);
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

   public override Monads.Result<T> OnSuccess(Action<T> action)
   {
      ensureValue();
      return _value.OnSuccess(action);
   }

   public override Monads.Result<T> OnFailure(Action<Exception> action)
   {
      ensureValue();
      return _value.OnFailure(action);
   }

   public override void Deconstruct(out Monads.Maybe<T> value, out Exception exception)
   {
      ensureValue();
      _value.Deconstruct(out value, out exception);
   }

   public override Monads.Result<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage)
   {
      ensureValue();
      return _value.Assert(predicate, exceptionMessage);
   }

   public override Monads.Maybe<T> Maybe()
   {
      ensureValue();
      return _value.Maybe();
   }

   public override Responding<T> Responding()
   {
      ensureValue();
      return _value.Responding();
   }

   public override bool EqualToValueOf(Monads.Result<T> otherResult)
   {
      ensureValue();
      return _value.EqualToValueOf(otherResult);
   }

   public override bool ValueEqualTo(T otherValue)
   {
      ensureValue();
      return _value.ValueEqualTo(otherValue);
   }

   public override Monads.Result<T> Otherwise(Func<Exception, T> func)
   {
      ensureValue();
      return _value.Otherwise(func);
   }

   public override Monads.Result<T> Otherwise(Func<Exception, Monads.Result<T>> func)
   {
      ensureValue();
      return _value.Otherwise(func);
   }

   public override Monads.Result<TResult> CastAs<TResult>()
   {
      ensureValue();
      return _value.CastAs<TResult>();
   }

   public override Monads.Result<T> Where(Predicate<T> predicate, string exceptionMessage)
   {
      ensureValue();
      return _value.Where(predicate, exceptionMessage);
   }

   public override Monads.Result<T> Where(Predicate<T> predicate, Func<string> exceptionMessage)
   {
      ensureValue();
      return _value.Where(predicate, exceptionMessage);
   }

   public override Monads.Result<T> ExceptionMessage(string message)
   {
      ensureValue();
      return _value.ExceptionMessage(message);
   }

   public override Monads.Result<T> ExceptionMessage(Func<Exception, string> message)
   {
      ensureValue();
      return _value.ExceptionMessage(message);
   }

   public bool Equals(Result<T> other)
   {
      ensureValue();
      return _value == other._value;
   }

   public override bool Equals(object obj)
   {
      ensureValue();
      return obj is Result<T> other && Equals(other);
   }

   public override int GetHashCode()
   {
      ensureValue();
      return _value.GetHashCode();
   }

   public static bool operator ==(Result<T> left, Result<T> right) => Equals(left, right);

   public static bool operator !=(Result<T> left, Result<T> right) => !Equals(left, right);
}