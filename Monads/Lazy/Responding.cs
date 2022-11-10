using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class Responding<T> : Monads.Responding<T>, IEquatable<Responding<T>>
{
   public static implicit operator bool(Responding<T> responding)
   {
      responding.ensureValue();
      return responding._value;
   }

   public static implicit operator Responding<T>(Func<Monads.Responding<T>> func) => new(func);

   public static bool operator true(Responding<T> responding)
   {
      responding.ensureValue();
      return responding._value;
   }

   public static bool operator false(Responding<T> responding)
   {
      responding.ensureValue();
      return !responding._value;
   }

   public static bool operator !(Responding<T> responding)
   {
      responding.ensureValue();
      return !responding._value;
   }

   protected Func<Monads.Responding<T>> func;
   protected Monads.Responding<T> _value;
   protected bool ensured;

   public Responding(Func<Monads.Responding<T>> func)
   {
      this.func = func;

      _value = nil;
      ensured = false;
   }

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

   public override Exception Exception
   {
      get
      {
         ensureValue();
         return _value.Exception;
      }
   }

   public override Monads.Maybe<Exception> AnyException
   {
      get
      {
         ensureValue();
         return _value.AnyException;
      }
   }

   public override Monads.Responding<TResult> Map<TResult>(Func<T, Monads.Responding<TResult>> ifResponse)
   {
      ensureValue();
      return _value.Map(ifResponse);
   }

   public override Monads.Responding<TResult> Map<TResult>(Func<T, TResult> ifResponse)
   {
      ensureValue();
      return _value.Map(ifResponse);
   }

   public override Monads.Responding<TResult> Map<TResult>(Func<T, Monads.Responding<TResult>> ifResponse,
      Func<Monads.Responding<TResult>> ifNoResponse, Func<Exception, Monads.Responding<TResult>> ifFailedResponse)
   {
      ensureValue();
      return _value.Map(ifResponse, ifNoResponse, ifFailedResponse);
   }

   public override Monads.Responding<T> OnResponse(Action<T> action)
   {
      ensureValue();
      return _value.OnResponse(action);
   }

   public override Monads.Responding<T> OnNoResponse(Action action)
   {
      ensureValue();
      return _value.OnNoResponse(action);
   }

   public override Monads.Responding<T> OnFailedResponse(Action<Exception> action)
   {
      ensureValue();
      return _value.OnFailedResponse(action);
   }

   public override Monads.Responding<TResult> SelectMany<TResult>(Func<T, Monads.Responding<TResult>> projection)
   {
      ensureValue();
      return _value.SelectMany(projection);
   }

   public override Monads.Responding<T2> SelectMany<T1, T2>(Func<T, Monads.Responding<T1>> func, Func<T, T1, T2> projection)
   {
      ensureValue();
      return _value.SelectMany(func, projection);
   }

   public override Monads.Responding<TResult> SelectMany<TResult>(Func<T, TResult> func)
   {
      ensureValue();
      return _value.SelectMany(func);
   }

   public override Monads.Responding<TResult> Select<TResult>(Monads.Responding<T> result, Func<T, TResult> func)
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

   public override bool IfNoResponse()
   {
      ensureValue();
      return _value.IfNoResponse();
   }

   public override bool IfFailedResponse(out Exception exception)
   {
      ensureValue();
      return _value.IfFailedResponse(out exception);
   }

   [Obsolete("Use ~")]
   public override bool Map(out T value, out Monads.Maybe<Exception> _exception)
   {
      ensureValue();
      return _value.Map(out value, out _exception);
   }

   public override T Force()
   {
      ensureValue();
      return _value.Force();
   }

   public override T DefaultTo(Func<Monads.Maybe<Exception>, T> func)
   {
      ensureValue();
      return _value.DefaultTo(func);
   }

   public override void Deconstruct(out T value, out Monads.Maybe<Exception> _exception)
   {
      ensureValue();
      _value.Deconstruct(out value, out _exception);
   }

   public override Monads.Maybe<T> Maybe()
   {
      ensureValue();
      return _value.Maybe();
   }

   public override Monads.Result<T> Result()
   {
      ensureValue();
      return _value.Result();
   }

   public override Monads.Completion<T> Completion()
   {
      ensureValue();
      return _value.Completion();
   }

   public bool Equals(Responding<T> other) => _value == other._value;

   public override bool Equals(object obj) => obj is Responding<T> other && Equals(other);

   public override int GetHashCode() => _value.GetHashCode();

   public static bool operator ==(Responding<T> left, Responding<T> right) => Equals(left, right);

   public static bool operator !=(Responding<T> left, Responding<T> right) => !Equals(left, right);
}