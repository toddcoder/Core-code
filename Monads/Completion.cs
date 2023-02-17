using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public abstract class Completion<T>
{
   protected Exception defaultException() => fail("There is no exception");

   public static implicit operator Completion<T>(T value) => value.Completed();

   public static implicit operator Completion<T>(Exception exception) => new Interrupted<T>(exception);

   public static implicit operator Completion<T>(NilWithMessage nilWithMessage) => new Interrupted<T>(new Exception(nilWithMessage.Message));

   public static implicit operator Completion<T>(Nil _) => new Cancelled<T>();

   public static implicit operator Completion<T>(Maybe<Exception> _exception)
   {
      return _exception.Map(e => (Completion<T>)new Interrupted<T>(e)) | (() => new Cancelled<T>());
   }

   public static bool operator true(Completion<T> value) => value is Completed<T> || value is Lazy.LazyCompletion<T> lazyCompletion && lazyCompletion;

   public static bool operator false(Completion<T> value)
   {
      return value is not Completed<T> || value is Lazy.LazyCompletion<T> lazyCompletion && !lazyCompletion;
   }

   public static bool operator !(Completion<T> value)
   {
      return value is not Completed<T> || value is Lazy.LazyCompletion<T> lazyCompletion && !lazyCompletion;
   }

   public static implicit operator bool(Completion<T> value)
   {
      return value is Completed<T> || value is Lazy.LazyCompletion<T> lazyCompletion && lazyCompletion;
   }

   public static implicit operator T(Completion<T> value) => value switch
   {
      (true, var internalValue) => internalValue,
      Interrupted<T> interrupted => throw interrupted.Exception,
      _ => throw new InvalidCastException("Must be a Completed to return a value")
   };

   public static T operator |(Completion<T> completion, T defaultValue) => completion ? completion : defaultValue;

   public static T operator |(Completion<T> completion, Func<T> defaultFunc) => completion ? completion : defaultFunc();

   public abstract Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted);

   public abstract Completion<TResult> Map<TResult>(Func<T, TResult> ifCompleted);

   public abstract Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled);

   public abstract Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Exception, Completion<TResult>> ifInterrupted);

   public abstract Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled,
      Func<Exception, Completion<TResult>> ifInterrupted);

   public abstract TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted);

   public abstract TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted);

   public abstract Completion<T> Map(Action<T> action);

   public abstract Completion<T> UnMap(Action action);

   public abstract Completion<T> UnMap(Action<Exception> action);

   public abstract Completion<T> Do(Action<T> ifCompleted, Action ifNotCompleted);

   public abstract Completion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted);

   public abstract Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection);

   public abstract Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection);

   public abstract Completion<TResult> SelectMany<TResult>(Func<T, TResult> func);

   public abstract Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func);

   public abstract bool IfCancelled();

   public abstract Completion<TOther> NotCompleted<TOther>();

   public abstract void Force();

   public abstract T ForceValue();

   public abstract Completion<T> CancelledOnly();

   public abstract Completion<TOther> CancelledOnly<TOther>();

   public abstract Completion<TOther> NotCompletedOnly<TOther>();

   public abstract void Deconstruct(out bool isCompleted, out T value);

   public abstract Completion<T> OnCompleted(Action<T> action);

   public abstract Completion<T> OnCancelled(Action action);

   public abstract Completion<T> OnInterrupted(Action<Exception> action);

   public abstract bool ValueEqualTo(Completion<T> otherCompletion);

   public abstract bool EqualToValueOf(T otherValue);

   public abstract Completion<TResult> CastAs<TResult>();

   public abstract Completion<T> Where(Predicate<T> predicate);

   public abstract Completion<T> Where(Predicate<T> predicate, string exceptionMessage);

   public abstract Completion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage);

   public Completion<T> Tap(Action<Completion<T>> action)
   {
      action(this);
      return this;
   }

   public abstract T DefaultTo(Func<Maybe<Exception>, T> defaultFunc);

   public abstract Maybe<T> Maybe();

   public abstract Result<T> Result();

   public abstract Responding<T> Responding();

   public abstract Exception Exception { get; }

   public abstract Maybe<Exception> AnyException { get; }

   public abstract object ToObject();
}