using System;

namespace Core.Monads
{
   public abstract class ICompletion<T>
   {
      public abstract ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted);

      public abstract ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted);

      public abstract ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled);

      public abstract ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<Exception, ICompletion<TResult>> ifInterrupted);

      public abstract ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled,
         Func<Exception, ICompletion<TResult>> ifInterrupted);

      public abstract TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted);

      public abstract TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted);

      public abstract ICompletion<T> If(Action<T> action);

      public abstract ICompletion<T> Else(Action action);

      public abstract ICompletion<T> Else(Action<Exception> action);

      public abstract ICompletion<T> Do(Action<T> ifCompleted, Action ifNotCompleted);

      public abstract ICompletion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted);

      public abstract ICompletion<TOther> InterruptedAs<TOther>();

      public abstract ICompletion<T> Or(ICompletion<T> other);

      public abstract ICompletion<T> Or(Func<ICompletion<T>> other);

      public abstract ICompletion<TResult> SelectMany<TResult>(Func<T, ICompletion<TResult>> projection);

      public abstract ICompletion<T2> SelectMany<T1, T2>(Func<T, ICompletion<T1>> func, Func<T, T1, T2> projection);

      public abstract ICompletion<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract ICompletion<TResult> Select<TResult>(ICompletion<T> result, Func<T, TResult> func);

      public abstract bool If(out T value);

      public abstract bool IfCancelled();

      public abstract bool IfInterrupted(out Exception exception);

      public abstract bool If(out T value, out Maybe<Exception> _exception);

      public abstract bool IfNot(out Maybe<Exception> _exception);

      public abstract bool Else<TOther>(out ICompletion<TOther> result);

      public abstract ICompletion<TOther> NotCompleted<TOther>();

      public abstract bool IsCompleted(out ICompletion<T> completed);

      public abstract bool NotCompleted(out ICompletion<T> notCompleted);

      public abstract void Force();

      public abstract T ForceValue();

      public abstract ICompletion<T> CancelledOnly();

      public abstract ICompletion<TOther> CancelledOnly<TOther>();

      public abstract ICompletion<TOther> NotCompletedOnly<TOther>();

      public abstract void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception);

      public abstract ICompletion<T> OnCompleted(Action<T> action);

      public abstract ICompletion<T> OnCancelled(Action action);

      public abstract ICompletion<T> OnInterrupted(Action<Exception> action);

      public abstract bool ValueOrOriginal(out T value, out ICompletion<T> original);

      public abstract bool ValueOrCast<TCompletion>(out T value, out ICompletion<TCompletion> completion);

      public abstract bool ValueEqualTo(ICompletion<T> otherCompletion);

      public abstract bool EqualToValueOf(T otherValue);

      public abstract ICompletion<TResult> CastAs<TResult>();

      public abstract ICompletion<T> Where(Predicate<T> predicate);

      public abstract ICompletion<T> Where(Predicate<T> predicate, string exceptionMessage);

      public abstract ICompletion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage);

   }
}