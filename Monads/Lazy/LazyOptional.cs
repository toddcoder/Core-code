using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyOptional<T> : Optional<T>, IEquatable<LazyOptional<T>>
{
   public static implicit operator bool(LazyOptional<T> optional)
   {
      optional.ensureValue();
      return optional._value;
   }

   public static implicit operator LazyOptional<T>(Func<Optional<T>> func) => new(func);

   public static bool operator true(LazyOptional<T> optional)
   {
      optional.ensureValue();
      return optional._value;
   }

   public static bool operator false(LazyOptional<T> optional)
   {
      optional.ensureValue();
      return !optional._value;
   }

   public static bool operator !(LazyOptional<T> optional)
   {
      optional.ensureValue();
      return !optional._value;
   }

   protected Func<Optional<T>> func;
   protected Optional<T> _value;
   protected bool ensured;

   internal LazyOptional(Func<Optional<T>> func)
   {
      this.func = func;

      _value = nil;
      ensured = false;
   }

   internal LazyOptional(Optional<T> optional) : this(() => optional)
   {
   }

   internal LazyOptional() : this(() => nil)
   {
   }

   public void Activate()
   {
      if (Repeating || !ensured)
      {
         _value = func();
         ensured = _value;
      }
   }

   public void Activate(Func<Optional<T>> func)
   {
      if (Repeating)
      {
         Activate(func());
      }
      else
      {
         this.func = func;
      }
   }

   public void Activate(Optional<T> value)
   {
      if (Repeating || !ensured)
      {
         _value = value;
         ensured = _value;
      }
   }

   public LazyOptional<T> ValueOf(Func<Optional<T>> func)
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

   public LazyOptional<T> ValueOf(Optional<T> value)
   {
      if (Repeating || !ensured)
      {
         _value = value;
         ensured = true;
      }

      return this;
   }

   public LazyOptional<TNext> Then<TNext>(Func<T, Optional<TNext>> func)
   {
      var _next = new LazyOptional<TNext>();
      ensureValue();

      if (_value is (true, var value))
      {
         return _next.ValueOf(() => func(value));
      }
      else if (_value.Exception is (true, var exception))
      {
         return _next.ValueOf(() => exception);
      }
      else
      {
         return _next.ValueOf(() => nil);
      }
   }

   public LazyOptional<TNext> Then<TNext>(Optional<TNext> next) => Then(_ => next);

   public LazyOptional<TNext> Then<TNext>(Func<T, TNext> func)
   {
      var _next = new LazyOptional<TNext>();
      ensureValue();

      if (_value is (true, var value))
      {
         return _next.ValueOf(() => func(value));
      }
      else if (_value.Exception is (true, var exception))
      {
         return _next.ValueOf(() => exception);
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

   public void Reset()
   {
      ensured = false;
      _value = nil;
   }

   public override Maybe<Exception> Exception
   {
      get
      {
         ensureValue();
         return _value.Exception;
      }
   }

   public override Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust)
   {
      ensureValue();
      return _value.Map(ifJust);
   }

   public override Optional<TResult> Map<TResult>(Func<T, TResult> ifJust)
   {
      ensureValue();
      return _value.Map(ifJust);
   }

   public override Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust,
      Func<Optional<TResult>> ifEmpty, Func<Exception, Optional<TResult>> ifFailed)
   {
      ensureValue();
      return _value.Map(ifJust, ifEmpty, ifFailed);
   }

   public override Optional<T> OnJust(Action<T> action)
   {
      ensureValue();
      return _value.OnJust(action);
   }

   public override Optional<T> OnEmpty(Action action)
   {
      ensureValue();
      return _value.OnEmpty(action);
   }

   public override Optional<T> OnFailed(Action<Exception> action)
   {
      ensureValue();
      return _value.OnFailed(action);
   }

   public override Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> projection)
   {
      ensureValue();
      return _value.SelectMany(projection);
   }

   public override Optional<TResult> SelectMany<TResult>(Func<T, Maybe<TResult>> projection)
   {
      ensureValue();
      return _value.SelectMany(projection);
   }

   public override Optional<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection)
   {
      ensureValue();
      return _value.SelectMany(projection);
   }

   public override Optional<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection)
   {
      ensureValue();
      return _value.SelectMany(projection);
   }

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection)
   {
      ensureValue();
      return _value.SelectMany(func, projection);
   }

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Maybe<T1>> func, Func<T, T1, T2> projection)
   {
      ensureValue();
      return _value.SelectMany(func, projection);
   }

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection)
   {
      ensureValue();
      return _value.SelectMany(func, projection);
   }

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection)
   {
      ensureValue();
      return _value.SelectMany(func, projection);
   }

   public override Optional<TResult> SelectMany<TResult>(Func<T, TResult> func)
   {
      ensureValue();
      return _value.SelectMany(func);
   }

   public override Optional<TResult> Select<TResult>(Optional<T> result, Func<T, TResult> func)
   {
      ensureValue();
      return _value.Select(result, func);
   }

   public override bool IfEmpty()
   {
      ensureValue();
      return _value.IfEmpty();
   }

   public override bool IfFailed(out Exception exception)
   {
      ensureValue();
      return _value.IfFailed(out exception);
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

   public override void Deconstruct(out bool isJust, out T value)
   {
      ensureValue();
      _value.Deconstruct(out isJust, out value);
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

   public override object ToObject()
   {
      ensureValue();
      return _value.ToObject();
   }

   public override Optional<T> Initialize(Func<T> initializer)
   {
      ensureValue();
      return _value.Initialize(initializer);
   }

   public override void MapOf(Action<T> action)
   {
      ensureValue();
      _value.MapOf(action);
   }

   public bool Equals(LazyOptional<T> other) => _value == other._value;

   public override bool Equals(object obj) => obj is LazyOptional<T> other && Equals(other);

   public override int GetHashCode() => _value.GetHashCode();

   public static bool operator ==(LazyOptional<T> left, LazyOptional<T> right) => Equals(left, right);

   public static bool operator !=(LazyOptional<T> left, LazyOptional<T> right) => !Equals(left, right);

   public override string ToString() => _value.ToString();
}