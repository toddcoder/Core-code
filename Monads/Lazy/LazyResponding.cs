using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyResponding<T> : Responding<T>, IEquatable<LazyResponding<T>>
{
   public static implicit operator bool(LazyResponding<T> responding)
   {
      responding.ensureValue();
      return responding._value;
   }

   public static implicit operator LazyResponding<T>(Func<Responding<T>> func) => new(func);

   public static bool operator true(LazyResponding<T> responding)
   {
      responding.ensureValue();
      return responding._value;
   }

   public static bool operator false(LazyResponding<T> responding)
   {
      responding.ensureValue();
      return !responding._value;
   }

   public static bool operator !(LazyResponding<T> responding)
   {
      responding.ensureValue();
      return !responding._value;
   }

   protected Func<Responding<T>> func;
   protected Responding<T> _value;
   protected bool ensured;

   internal LazyResponding(Func<Responding<T>> func)
   {
      this.func = func;

      _value = nil;
      ensured = false;
   }

   internal LazyResponding(Responding<T> responding) : this(() => responding)
   {
   }

   internal LazyResponding() : this(() => nil)
   {
   }

   public LazyResponding<T> ValueOf(Func<Responding<T>> func)
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

   public LazyResponding<T> ValueOf(Responding<T> value)
   {
      if (Repeating || !ensured)
      {
         _value = value;
         ensured = true;
      }

      return this;
   }

   public LazyResponding<TNext> Then<TNext>(Func<T, Responding<TNext>> func)
   {
      var _next = new LazyResponding<TNext>();
      ensureValue();

      if (_value)
      {
         return _next.ValueOf(() => func(~_value));
      }
      else if (_value.AnyException)
      {
         return _next.ValueOf(() => _value.Exception);
      }
      else
      {
         return _next.ValueOf(() => nil);
      }
   }

   public LazyResponding<TNext> Then<TNext>(Responding<TNext> next) => Then(_ => next);

   public LazyResponding<TNext> Then<TNext>(Func<T, TNext> func)
   {
      var _next = new LazyResponding<TNext>();
      ensureValue();

      if (_value)
      {
         return _next.ValueOf(() => func(~_value));
      }
      else if (_value.AnyException)
      {
         return _next.ValueOf(() => _value.Exception);
      }
      else
      {
         return _next.ValueOf(() => nil);
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

   public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse)
   {
      ensureValue();
      return _value.Map(ifResponse);
   }

   public override Responding<TResult> Map<TResult>(Func<T, TResult> ifResponse)
   {
      ensureValue();
      return _value.Map(ifResponse);
   }

   public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse,
      Func<Responding<TResult>> ifNoResponse, Func<Exception, Responding<TResult>> ifFailedResponse)
   {
      ensureValue();
      return _value.Map(ifResponse, ifNoResponse, ifFailedResponse);
   }

   public override Responding<T> OnResponse(Action<T> action)
   {
      ensureValue();
      return _value.OnResponse(action);
   }

   public override Responding<T> OnNoResponse(Action action)
   {
      ensureValue();
      return _value.OnNoResponse(action);
   }

   public override Responding<T> OnFailedResponse(Action<Exception> action)
   {
      ensureValue();
      return _value.OnFailedResponse(action);
   }

   public override Responding<TResult> SelectMany<TResult>(Func<T, Responding<TResult>> projection)
   {
      ensureValue();
      return _value.SelectMany(projection);
   }

   public override Responding<T2> SelectMany<T1, T2>(Func<T, Responding<T1>> func, Func<T, T1, T2> projection)
   {
      ensureValue();
      return _value.SelectMany(func, projection);
   }

   public override Responding<TResult> SelectMany<TResult>(Func<T, TResult> func)
   {
      ensureValue();
      return _value.SelectMany(func);
   }

   public override Responding<TResult> Select<TResult>(Responding<T> result, Func<T, TResult> func)
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
   public override bool Map(out T value, out Maybe<Exception> _exception)
   {
      ensureValue();
      return _value.Map(out value, out _exception);
   }

   public override T Force()
   {
      ensureValue();
      return _value.Force();
   }

   public override T DefaultTo(Func<Maybe<Exception>, T> func)
   {
      ensureValue();
      return _value.DefaultTo(func);
   }

   public override void Deconstruct(out T value, out Maybe<Exception> _exception)
   {
      ensureValue();
      _value.Deconstruct(out value, out _exception);
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

   public override Completion<T> Completion()
   {
      ensureValue();
      return _value.Completion();
   }

   public bool Equals(LazyResponding<T> other) => _value == other._value;

   public override bool Equals(object obj) => obj is LazyResponding<T> other && Equals(other);

   public override int GetHashCode() => _value.GetHashCode();

   public static bool operator ==(LazyResponding<T> left, LazyResponding<T> right) => Equals(left, right);

   public static bool operator !=(LazyResponding<T> left, LazyResponding<T> right) => !Equals(left, right);

   public override string ToString() => _value.ToString();
}