using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
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

      public static bool operator true(Completion<T> value) => value is Completed<T>;

      public static bool operator false(Completion<T> value) => value is not Completed<T>;

      public static bool operator !(Completion<T> value) => value is not Completed<T>;

      public static implicit operator bool(Completion<T> value) => value is Completed<T>;

      public static implicit operator T(Completion<T> value) => value switch
      {
         Completed<T> completed => completed.Value,
         Interrupted<T> interrupted => throw interrupted.Exception,
         _ => throw new InvalidCastException("Must be a Completed to return a value")
      };

      public static implicit operator Exception(Completion<T> value) => value switch
      {
         Interrupted<T> interrupted => interrupted.Exception,
         _ => throw new InvalidCastException("Must be an Interrupted to return a value")
      };

      public static implicit operator Maybe<Exception>(Completion<T> value) => value switch
      {
         Interrupted<T> interrupted => interrupted.Exception,
         _ => nil
      };

      public static T operator |(Completion<T> completion, T defaultValue) => completion ? completion : defaultValue;

      public static T operator |(Completion<T> completion, Func<T> defaultFunc) => completion ? completion : defaultFunc();

      public static Completion<T> operator *(Completion<T> completion, Action<T> action)
      {
         if (completion)
         {
            action(completion);
         }

         return completion;
      }

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

      [Obsolete("Use exception")]
      public abstract Completion<TOther> InterruptedAs<TOther>();

      public abstract Completion<T> Or(Completion<T> other);

      public abstract Completion<T> Or(Func<Completion<T>> other);

      public abstract Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection);

      public abstract Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection);

      public abstract Completion<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func);

      public abstract bool Map(out T value);

      public abstract bool IfCancelled();

      public abstract bool IfInterrupted(out Exception exception);

      public abstract bool Map(out T value, out Maybe<Exception> _exception);

      public abstract bool UnMap(out Maybe<Exception> _exception);

      public abstract bool UnMap<TOther>(out Completion<TOther> result);

      public abstract Completion<TOther> NotCompleted<TOther>();

      public abstract bool IsCompleted(out Completion<T> completed);

      public abstract bool NotCompleted(out Completion<T> notCompleted);

      public abstract void Force();

      public abstract T ForceValue();

      public abstract Completion<T> CancelledOnly();

      public abstract Completion<TOther> CancelledOnly<TOther>();

      public abstract Completion<TOther> NotCompletedOnly<TOther>();

      public abstract void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception);

      public abstract Completion<T> OnCompleted(Action<T> action);

      public abstract Completion<T> OnCancelled(Action action);

      public abstract Completion<T> OnInterrupted(Action<Exception> action);

      [Obsolete("Use Map")]
      public abstract bool ValueOrOriginal(out T value, out Completion<T> original);

      [Obsolete("Use Map")]
      public abstract bool ValueOrCast<TCompletion>(out T value, out Completion<TCompletion> completion);

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
   }
}