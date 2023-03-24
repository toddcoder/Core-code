using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Just<T> : Optional<T>, IEquatable<Just<T>>
{
   protected T value;

   internal Just(T value)
   {
      this.value = value;
   }

   public override Exception Exception => throw fail("Response has no Exception");

   public override Optional<Exception> AnyException => nil;

   public override Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust)
   {
      try
      {
         return ifJust(value);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override Optional<TResult> Map<TResult>(Func<T, TResult> ifJust)
   {
      try
      {
         return ifJust(value);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust, Func<Optional<TResult>> ifEmpty,
      Func<Exception, Optional<TResult>> ifFailed)
   {
      return ifJust(value);
   }

   public override Optional<T> OnJust(Action<T> action)
   {
      action(value);
      return this;
   }

   public override Optional<T> OnEmpty(Action action) => this;

   public override Optional<T> OnFailed(Action<Exception> action) => this;

   public override Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> projection) => projection(value);

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection)
   {
      return func(value).Map(t1 => projection(value, t1).Just(), () => nil, e => e);
   }

   public override Optional<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(value);

   public override Optional<TResult> Select<TResult>(Optional<T> result, Func<T, TResult> func) => func(value);

   public override bool IfEmpty() => false;

   public override bool IfFailed(out Exception exception)
   {
      exception = fail("There is no exception");
      return false;
   }

   public override T Force() => value;

   public override T DefaultTo(Func<Optional<Exception>, T> func) => value;

   public override void Deconstruct(out bool isJust, out T value)
   {
      isJust = true;
      value = this.value;
   }

   public override Completion<T> Completion() => value;

   public override object ToObject() => value;

   public override Optional<T> Initialize(Func<T> initializer) => this;

   public bool Equals(Just<T> other) => value.Equals(other.value);

   public override bool Equals(object obj) => obj is Just<T> other && Equals(other);

   public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(value);

   public static bool operator ==(Just<T> left, Just<T> right) => Equals(left, right);

   public static bool operator !=(Just<T> left, Just<T> right) => !Equals(left, right);

   public override string ToString() => value.ToString();
}