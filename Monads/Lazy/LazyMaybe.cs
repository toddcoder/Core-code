using System;
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

   internal LazyMaybe(Maybe<T> maybe) : this(() => maybe)
   {
   }

   internal LazyMaybe() : this(() => nil)
   {
   }

   public void Activate()
   {
      if (Repeating || !_value)
      {
         _value = func();
         ensured = _value;
      }
   }

   public void Activate(Func<Maybe<T>> func)
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

   public void Activate(Maybe<T> value)
   {
      if (Repeating || !_value)
      {
         _value = value;
         ensured = _value;
      }
   }

   public LazyMaybe<T> ValueOf(Func<Maybe<T>> func)
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

   public LazyMaybe<T> ValueOf(Maybe<T> value)
   {
      if (Repeating || !ensured)
      {
         _value = value;
         ensured = true;
      }

      return this;
   }

   public LazyMaybe<TNext> Then<TNext>(Func<T, Maybe<TNext>> func)
   {
      var _next = new LazyMaybe<TNext>();
      ensureValue();

      if (_value is (true, var value))
      {
         return _next.ValueOf(() => func(value));
      }
      else
      {
         return _next.ValueOf(() => nil);
      }
   }

   public LazyMaybe<TNext> Then<TNext>(Maybe<TNext> next) => Then(_ => next);

   public LazyMaybe<TNext> Then<TNext>(Func<T, TNext> func)
   {
      var _next = new LazyMaybe<TNext>();
      ensureValue();

      if (_value is (true, var value))
      {
         return _next.ValueOf(() => func(value));
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
         ensured = _value;
      }
   }

   public void Reset()
   {
      ensured = false;
      _value = nil;
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

   public override object ToObject()
   {
      ensureValue();
      return _value.ToObject();
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