using System;

namespace Core.Monads;

public abstract class Optional<T>
{
   public static Optional<T> operator |(Optional<T> left, Optional<T> right)
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
         return left;
      }
   }

   public static Optional<T> operator |(Optional<T> left, Lazy.LazyOptional<T> right)
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
         return left;
      }
   }

   public static Optional<T> operator |(Optional<T> left, Func<Optional<T>> rightFunc)
   {
      if (left)
      {
         return left;
      }
      else
      {
         var right = rightFunc();
         if (right)
         {
            return right;
         }
      }

      return left;
   }

   public static implicit operator Optional<T>(T value) => new Just<T>(value);

   public static implicit operator Optional<T>(Exception exception) => new Failed<T>(exception);

   public static implicit operator Optional<T>(Nil _) => new Empty<T>();

   public static implicit operator Optional<T>(Maybe<Exception> _exception)
   {
      return _exception.Map(e => (Optional<T>)new Failed<T>(e)) | (() => new Empty<T>());
   }

   public static bool operator true(Optional<T> value) => value is Just<T> || value is Lazy.LazyOptional<T> lazyOptional && lazyOptional;

   public static bool operator false(Optional<T> value)
   {
      return value is not Just<T> || value is Lazy.LazyOptional<T> lazyOptional && !lazyOptional;
   }

   public static bool operator !(Optional<T> value)
   {
      return value is not Just<T> || value is Lazy.LazyOptional<T> lazyOptional && !lazyOptional;
   }

   public static implicit operator bool(Optional<T> value)
   {
      return value is Just<T> || value is Lazy.LazyOptional<T> lazyOptional && lazyOptional;
   }

   public static implicit operator T(Optional<T> value) => value switch
   {
      (true, var internalValue) => internalValue,
      Failed<T> failed => throw failed.Exception,
      _ => throw new InvalidCastException("Must be a Optional to return a value")
   };

   public static T operator |(Optional<T> optional, T defaultValue) => optional ? optional : defaultValue;

   public static T operator |(Optional<T> optional, Func<T> defaultFunc) => optional ? optional : defaultFunc();

   public static T operator |(Optional<T> optional, Func<Maybe<Exception>, T> defaultFunc) => optional.DefaultTo(defaultFunc);

   public abstract Exception Exception { get; }

   public abstract Maybe<Exception> AnyException { get; }

   public abstract Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust);

   public abstract Optional<TResult> Map<TResult>(Func<T, TResult> ifJust);

   public abstract Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust, Func<Optional<TResult>> ifEmpty,
      Func<Exception, Optional<TResult>> ifFailed);

   public abstract Optional<T> OnJust(Action<T> action);

   public abstract Optional<T> OnEmpty(Action action);

   public abstract Optional<T> OnFailed(Action<Exception> action);

   public abstract Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> projection);

   public abstract Optional<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection);

   public abstract Optional<TResult> SelectMany<TResult>(Func<T, TResult> func);

   public abstract Optional<TResult> Select<TResult>(Optional<T> result, Func<T, TResult> func);

   public abstract bool IfEmpty();

   public abstract bool IfFailed(out Exception exception);

   public abstract T Force();

   public abstract T DefaultTo(Func<Maybe<Exception>, T> func);

   public abstract void Deconstruct(out bool isJust, out T value);

   public abstract Maybe<T> Maybe();

   public abstract Result<T> Result();

   public abstract Completion<T> Completion();

   public abstract object ToObject();

   public abstract Optional<T> Initialize(Func<T> initializer);
}