using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Interrupted<T> : ICompletion<T>
   {
      public static implicit operator bool(Interrupted<T> _) => false;

      public static bool operator &(Interrupted<T> x, IHasValue y) => false;

      public static bool operator |(Interrupted<T> x, IHasValue y) => y.HasValue;

      public static bool operator !(Interrupted<T> _) => true;

      protected Exception exception;

      internal Interrupted(Exception exception)
      {
         this.exception = exception is FullStackException ? exception : new FullStackException(exception);
      }

      public bool IsCompleted => false;

      public bool IsCancelled => false;

      public bool IsInterrupted => true;

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted) => interrupted<TResult>(exception);

      public ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => interrupted<TResult>(exception);

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled)
      {
         return interrupted<TResult>(exception);
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifInterrupted(exception);
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifInterrupted(exception);
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
      {
         return ifInterrupted(exception);
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifNotCompleted();

      public ICompletion<T> If(Action<T> action) => this;

      public ICompletion<T> Else(Action action) => this;

      public ICompletion<T> Else(Action<Exception> action)
      {
         action(exception);
         return this;
      }

      public ICompletion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifNotCompleted();
         return this;
      }

      public ICompletion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifInterrupted(exception);
         return this;
      }

      public ICompletion<TOther> InterruptedAs<TOther>() => interrupted<TOther>(exception);

      public ICompletion<T> Or(ICompletion<T> other) => other;

      public ICompletion<T> Or(Func<ICompletion<T>> other) => other();

      public ICompletion<TResult> SelectMany<TResult>(Func<T, ICompletion<TResult>> projection) => interrupted<TResult>(exception);

      public ICompletion<T2> SelectMany<T1, T2>(Func<T, ICompletion<T1>> func, Func<T, T1, T2> projection) => interrupted<T2>(exception);

      public ICompletion<TResult> SelectMany<TResult>(Func<T, TResult> func) => interrupted<TResult>(exception);

      public ICompletion<TResult> Select<TResult>(ICompletion<T> result, Func<T, TResult> func) => interrupted<TResult>(exception);

      public bool If(out T value)
      {
         value = default;
         return false;
      }

      public bool IfCancelled() => false;

      public bool IfInterrupted(out Exception exception)
      {
         exception = this.exception;
         return true;
      }

      public bool If(out T value, out IMaybe<Exception> anyException)
      {
         value = default;
         anyException = exception.Some();

         return false;
      }

      public bool Else<TOther>(out ICompletion<TOther> result)
      {
         result = interrupted<TOther>(exception);
         return true;
      }

      public ICompletion<TOther> NotCompleted<TOther>() => interrupted<TOther>(exception);

      public bool WasCompleted(out ICompletion<T> completed)
      {
         completed = this;
         return false;
      }

      public void Force() => throw exception;

      public T ForceValue() => throw exception;

      public ICompletion<T> CancelledOnly() => throw exception;

      public ICompletion<TOther> CancelledOnly<TOther>() => throw exception;

      public void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> anyException)
      {
         value = default;
         anyException = exception.Some();
      }

      public ICompletion<T> OnCompleted(Action<T> action) => this;

      public ICompletion<T> OnCancelled(Action action) => this;

      public ICompletion<T> OnInterrupted(Action<Exception> action)
      {
         action(exception);
         return this;
      }

      public bool HasValue => false;
   }
}