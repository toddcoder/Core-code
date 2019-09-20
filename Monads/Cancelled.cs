using System;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Cancelled<T> : ICompletion<T>
   {
      public static implicit operator bool(Cancelled<T> _) => false;

      public static bool operator &(Cancelled<T> x, IHasValue y) => false;

      public static bool operator |(Cancelled<T> x, IHasValue y) => y.HasValue;

      internal Cancelled() { }

      public bool IsCompleted => false;

      public bool IsCancelled => true;

      public bool IsInterrupted => false;

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted) => cancelled<TResult>();

      public ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => cancelled<TResult>();

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled)
      {
         return ifCancelled();
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return cancelled<TResult>();
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifCancelled();
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
      {
         return ifCancelled();
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifNotCompleted();

      public ICompletion<T> If(Action<T> action) => this;

      public ICompletion<T> Else(Action action)
      {
         action();
         return this;
      }

      public ICompletion<T> Else(Action<Exception> action) => this;

      public ICompletion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifNotCompleted();
         return this;
      }

      public ICompletion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifCancelled();
         return this;
      }

      public ICompletion<TOther> InterruptedAs<TOther>() => throw "There is no exception".Throws();

      public ICompletion<T> Or(ICompletion<T> other) => other;

      public ICompletion<T> Or(Func<ICompletion<T>> other) => other();

      public ICompletion<TResult> SelectMany<TResult>(Func<T, ICompletion<TResult>> projection) => cancelled<TResult>();

      public ICompletion<T2> SelectMany<T1, T2>(Func<T, ICompletion<T1>> func, Func<T, T1, T2> projection) => cancelled<T2>();

      public ICompletion<TResult> SelectMany<TResult>(Func<T, TResult> func) => cancelled<TResult>();

      public ICompletion<TResult> Select<TResult>(ICompletion<T> result, Func<T, TResult> func) => cancelled<TResult>();

      public bool If(out T value)
      {
         value = default;
         return false;
      }

      public bool IfCancelled() => true;

      public bool IfInterrupted(out Exception exception)
      {
         exception = default;
         return false;
      }

      public bool If(out T value, out IMaybe<Exception> anyException)
      {
         value = default;
         anyException = none<Exception>();
         return false;
      }

      public bool Else<TOther>(out ICompletion<TOther> result)
      {
         result = cancelled<TOther>();
         return true;
      }

      public ICompletion<TOther> NotCompleted<TOther>() => cancelled<TOther>();

      public bool WasCompleted(out ICompletion<T> completed)
      {
         completed = this;
         return false;
      }

      public void Force() { }

      public T ForceValue() => throw "There is no value".Throws();

      public ICompletion<T> CancelledOnly() => this;

      public ICompletion<TOther> CancelledOnly<TOther>() => cancelled<TOther>();

      public void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> anyException)
      {
         value = none<T>();
         anyException = none<Exception>();
      }

      public ICompletion<T> OnCompleted(Action<T> action) => this;

      public ICompletion<T> OnCancelled(Action action)
      {
         action();
         return this;
      }

      public ICompletion<T> OnInterrupted(Action<Exception> action) => this;

      public bool HasValue => false;
   }
}