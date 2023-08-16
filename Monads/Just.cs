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

   public override Maybe<Exception> Exception => nil;

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

   public override Optional<TResult> SelectMany<TResult>(Func<T, Maybe<TResult>> projection)
   {
      if (projection(value) is (true, var result))
      {
         return result;
      }
      else
      {
         return nil;
      }
   }

   public override Optional<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection)
   {
      var _result = projection(value);
      if (_result is (true, var result))
      {
         return result;
      }
      else
      {
         return _result.Exception;
      }
   }

   public override Optional<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection)
   {
      var _completion = projection(value);
      if (_completion is (true, var completion))
      {
         return completion;
      }
      else if (_completion.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection)
   {
      return func(value).Map(t1 => projection(value, t1).Just(), () => nil, e => e);
   }

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Maybe<T1>> func, Func<T, T1, T2> projection)
   {
      var _t1 = func(value);
      if (_t1 is (true, var t1))
      {
         return projection(value, t1);
      }
      else
      {
         return nil;
      }
   }

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection)
   {
      var _t1 = func(value);
      if (_t1 is (true, var t1))
      {
         return projection(value, t1);
      }
      else
      {
         return _t1.Exception;
      }
   }

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection)
   {
      var _t1 = func(value);
      if (_t1 is (true, var t1))
      {
         return projection(value, t1);
      }
      else if (_t1.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
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

   public override T DefaultTo(Func<Maybe<Exception>, T> func) => value;

   public override void Deconstruct(out bool isJust, out T value)
   {
      isJust = true;
      value = this.value;
   }

   public override Maybe<T> Maybe() => value;

   public override Result<T> Result() => value;

   public override Completion<T> Completion() => value;

   public override object ToObject() => value;

   public override Optional<T> Initialize(Func<T> initializer) => this;

   public override void MapOf(Action<T> action) => action(value);

   public bool Equals(Just<T> other) => value.Equals(other.value);

   public override bool Equals(object obj) => obj is Just<T> other && Equals(other);

   public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(value);

   public static bool operator ==(Just<T> left, Just<T> right) => Equals(left, right);

   public static bool operator !=(Just<T> left, Just<T> right) => !Equals(left, right);

   public override string ToString() => value.ToString();
}