using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyResult<T> : Result<T>, IEquatable<LazyResult<T>>
{
   public static implicit operator bool(LazyResult<T> result)
   {
      result.ensureValue();
      return result._value;
   }

   public static implicit operator LazyResult<T>(Func<Result<T>> func) => new(func);

   public static bool operator true(LazyResult<T> result)
   {
      result.ensureValue();
      return result._value;
   }

   public static bool operator false(LazyResult<T> result)
   {
      result.ensureValue();
      return !result._value;
   }

   public static bool operator !(LazyResult<T> result)
   {
      result.ensureValue();
      return !result._value;
   }

   protected Func<Result<T>> func;
   protected Result<T> _value;
   protected bool ensured;

   internal LazyResult(Func<Result<T>> func)
   {
      this.func = func;

      _value = fail("Uninitialized");
      ensured = false;
   }

   internal LazyResult(Result<T> result) : this(() => result)
   {
   }

   internal LazyResult()
   {
      func = () => fail("Uninitialized");
      _value = func();
      ensured = false;
   }

   public LazyResult<T> ValueOf(Func<Result<T>> func)
   {
      if (Repeating)
      {
         return ValueOf(func());
      }
      else
      {
         this.func = func;
         return this;
      }
   }

   public LazyResult<T> ValueOf(Result<T> value)
   {
      if (Repeating || !ensured)
      {
         _value = value;
         ensured = true;
      }

      return this;
   }

   public LazyResult<TNext> Then<TNext>(Func<T, Result<TNext>> func)
   {
      var _next = new LazyResult<TNext>();
      ensureValue();

      if (_value)
      {
         return _next.ValueOf(() => func(_value.Value));
      }
      else
      {
         return _next.ValueOf(() => _value.Exception);
      }
   }

   public LazyResult<TNext> Then<TNext>(Result<TNext> next) => Then(_ => next);

   public LazyResult<TNext> Then<TNext>(Func<T, TNext> func)
   {
      var _next = new LazyResult<TNext>();
      ensureValue();

      if (_value)
      {
         return _next.ValueOf(() => func(_value.Value));
      }
      else
      {
         return _next.ValueOf(() => _value.Exception);
      }
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

   public override Result<TResult> Map<TResult>(Func<T, Result<TResult>> ifSuccessful)
   {
      ensureValue();
      return _value.Map(ifSuccessful);
   }

   public override Result<TResult> Map<TResult>(Func<T, TResult> ifSuccessful)
   {
      ensureValue();
      return _value.Map(ifSuccessful);
   }

   public override Result<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection)
   {
      ensureValue();
      return _value.SelectMany(projection);
   }

   public override Result<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection)
   {
      ensureValue();
      return _value.SelectMany(func, projection);
   }

   public override Result<TResult> SelectMany<TResult>(Func<T, TResult> func)
   {
      ensureValue();
      return _value.SelectMany(func);
   }

   public override T Recover(Func<Exception, T> recovery)
   {
      ensureValue();
      return _value.Recover(recovery);
   }

   public override Result<Unit> Unit => unit;

   public override T Value
   {
      get
      {
         ensureValue();
         return _value.Value;
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

   public override Result<T> Always(Action action)
   {
      ensureValue();
      return _value.Always(action);
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

   public override Result<T> OnSuccess(Action<T> action)
   {
      ensureValue();
      return _value.OnSuccess(action);
   }

   public override Result<T> OnFailure(Action<Exception> action)
   {
      ensureValue();
      return _value.OnFailure(action);
   }

   [Obsolete("Use (bool, T)")]
   public override void Deconstruct(out Maybe<T> value, out Exception exception)
   {
      ensureValue();
      _value.Deconstruct(out value, out exception);
   }

   public override void Deconstruct(out bool isSuccess, out T value)
   {
      ensureValue();
      _value.Deconstruct(out isSuccess, out value);
   }

   public override Result<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage)
   {
      ensureValue();
      return _value.Assert(predicate, exceptionMessage);
   }

   public override Maybe<T> Maybe()
   {
      ensureValue();
      return _value.Maybe();
   }

   public override Responding<T> Responding()
   {
      ensureValue();
      return _value.Responding();
   }

   public override bool EqualToValueOf(Result<T> otherResult)
   {
      ensureValue();
      return _value.EqualToValueOf(otherResult);
   }

   public override bool ValueEqualTo(T otherValue)
   {
      ensureValue();
      return _value.ValueEqualTo(otherValue);
   }

   public override Result<T> Otherwise(Func<Exception, T> func)
   {
      ensureValue();
      return _value.Otherwise(func);
   }

   public override Result<T> Otherwise(Func<Exception, Result<T>> func)
   {
      ensureValue();
      return _value.Otherwise(func);
   }

   public override Result<TResult> CastAs<TResult>()
   {
      ensureValue();
      return _value.CastAs<TResult>();
   }

   public override Result<T> Where(Predicate<T> predicate, string exceptionMessage)
   {
      ensureValue();
      return _value.Where(predicate, exceptionMessage);
   }

   public override Result<T> Where(Predicate<T> predicate, Func<string> exceptionMessage)
   {
      ensureValue();
      return _value.Where(predicate, exceptionMessage);
   }

   public override Result<T> ExceptionMessage(string message)
   {
      ensureValue();
      return _value.ExceptionMessage(message);
   }

   public override Result<T> ExceptionMessage(Func<Exception, string> message)
   {
      ensureValue();
      return _value.ExceptionMessage(message);
   }

   public bool Equals(LazyResult<T> other)
   {
      ensureValue();
      return _value == other._value;
   }

   public override bool Equals(object obj)
   {
      ensureValue();
      return obj is LazyResult<T> other && Equals(other);
   }

   public override int GetHashCode()
   {
      ensureValue();
      return _value.GetHashCode();
   }

   public static bool operator ==(LazyResult<T> left, LazyResult<T> right) => Equals(left, right);

   public static bool operator !=(LazyResult<T> left, LazyResult<T> right) => !Equals(left, right);

   public override string ToString() => _value.ToString();
}