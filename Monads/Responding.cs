using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public abstract class Responding<T>
{
   public static Responding<T> operator |(Responding<T> left, Responding<T> right)
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

   public static Responding<T> operator |(Responding<T> left, Func<Responding<T>> rightFunc)
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

   public static implicit operator Responding<T>(T value) => new Response<T>(value);

   public static implicit operator Responding<T>(Exception exception) => new FailedResponse<T>(exception);

   public static implicit operator Responding<T>(Nil _) => new NoResponse<T>();

   public static implicit operator Responding<T>(Maybe<Exception> _exception)
   {
      return _exception.Map(e => (Responding<T>)new FailedResponse<T>(e)) | (() => new NoResponse<T>());
   }

   public static bool operator true(Responding<T> value) => value is Response<T>;

   public static bool operator false(Responding<T> value) => value is not Response<T>;

   public static bool operator !(Responding<T> value) => value is not Response<T>;

   public static implicit operator bool(Responding<T> value) => value is Response<T>;

   public static implicit operator T(Responding<T> value) => value switch
   {
      Response<T> response => response.Value,
      FailedResponse<T> failedResponse => throw failedResponse.Exception,
      Lazy.Responding<T> responding => responding.Value,
      _ => throw new InvalidCastException("Must be a Response to return a value")
   };

   [Obsolete("Use Exception property")]
   public static implicit operator Exception(Responding<T> value) => value switch
   {
      FailedResponse<T> failedResponse => failedResponse.Exception,
      _ => throw new InvalidCastException("Must be a FailedResponse to return a value")
   };

   [Obsolete("Use AnyException property")]
   public static implicit operator Maybe<Exception>(Responding<T> value) => value switch
   {
      FailedResponse<T> failedResponse => failedResponse.Exception,
      _ => nil
   };

   public static T operator |(Responding<T> responding, T defaultValue) => responding ? responding : defaultValue;

   public static T operator |(Responding<T> responding, Func<T> defaultFunc) => responding ? responding : defaultFunc();

   public static T operator |(Responding<T> responding, Func<Maybe<Exception>, T> defaultFunc) => responding.DefaultTo(defaultFunc);

   public static T operator ~(Responding<T> responding) => responding.Value;

   public abstract T Value { get; }

   public abstract Exception Exception { get; }

   public abstract Maybe<Exception> AnyException { get; }

   public abstract Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse);

   public abstract Responding<TResult> Map<TResult>(Func<T, TResult> ifResponse);

   public abstract Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse, Func<Responding<TResult>> ifNoResponse,
      Func<Exception, Responding<TResult>> ifFailedResponse);

   public abstract Responding<T> OnResponse(Action<T> action);

   public abstract Responding<T> OnNoResponse(Action action);

   public abstract Responding<T> OnFailedResponse(Action<Exception> action);

   public abstract Responding<TResult> SelectMany<TResult>(Func<T, Responding<TResult>> projection);

   public abstract Responding<T2> SelectMany<T1, T2>(Func<T, Responding<T1>> func, Func<T, T1, T2> projection);

   public abstract Responding<TResult> SelectMany<TResult>(Func<T, TResult> func);

   public abstract Responding<TResult> Select<TResult>(Responding<T> result, Func<T, TResult> func);

   [Obsolete("Use ~")]
   public abstract bool Map(out T value);

   public abstract bool IfNoResponse();

   public abstract bool IfFailedResponse(out Exception exception);

   [Obsolete("Use ~")]
   public abstract bool Map(out T value, out Maybe<Exception> _exception);

   public abstract T Force();

   public abstract T DefaultTo(Func<Maybe<Exception>, T> func);

   public abstract void Deconstruct(out T value, out Maybe<Exception> _exception);

   public abstract Maybe<T> Maybe();

   public abstract Result<T> Result();

   public abstract Completion<T> Completion();
}