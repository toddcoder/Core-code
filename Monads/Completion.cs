using System;

namespace Core.Monads
{
   public abstract class Completion<T>
   {
      public static implicit operator Completion<T>(T value) => value.Completed();

      public abstract Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted);

      public abstract Completion<TResult> Map<TResult>(Func<T, TResult> ifCompleted);

      public abstract Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled);

      public abstract Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Exception, Completion<TResult>> ifInterrupted);

      public abstract Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled,
         Func<Exception, Completion<TResult>> ifInterrupted);

      public abstract TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted);

      public abstract TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted);

      public abstract Completion<T> If(Action<T> action);

      public abstract Completion<T> Else(Action action);

      public abstract Completion<T> Else(Action<Exception> action);

      public abstract Completion<T> Do(Action<T> ifCompleted, Action ifNotCompleted);

      public abstract Completion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted);

      public abstract Completion<TOther> InterruptedAs<TOther>();

      public abstract Completion<T> Or(Completion<T> other);

      public abstract Completion<T> Or(Func<Completion<T>> other);

      public abstract Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection);

      public abstract Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection);

      public abstract Completion<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func);

      public abstract bool If(out T value);

      public abstract bool IfCancelled();

      public abstract bool IfInterrupted(out Exception exception);

      public abstract bool If(out T value, out Maybe<Exception> _exception);

      public abstract bool IfNot(out Maybe<Exception> _exception);

      public abstract bool Else<TOther>(out Completion<TOther> result);

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

      public abstract bool ValueOrOriginal(out T value, out Completion<T> original);

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
   }
}