using System;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Completed<T> : ICompletion<T>
   {
      protected T value;

      public Completed(T value) => this.value = value;

      public bool IsCompleted => true;

      public bool IsCancelled => false;

      public bool IsInterrupted => false;

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted) => ifCompleted(value);

      public ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => ifCompleted(value).Completed();

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled)
      {
         return ifCompleted(value);
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted) =>
         ifCompleted(value);

      public TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifCompleted(value);

      public ICompletion<T> If(Action<T> action)
      {
         action(value);
         return this;
      }

      public ICompletion<T> Else(Action action) => this;

      public ICompletion<T> Else(Action<Exception> action) => this;

      public ICompletion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifCompleted(value);
         return this;
      }

      public ICompletion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifCompleted(value);
         return this;
      }

      public ICompletion<TOther> InterruptedAs<TOther>() => throw "There is no exception".Throws();

      public ICompletion<T> Or(ICompletion<T> other) => this;

      public ICompletion<T> Or(Func<ICompletion<T>> other) => this;

      public ICompletion<TResult> SelectMany<TResult>(Func<T, ICompletion<TResult>> projection) => projection(value);

      public ICompletion<T2> SelectMany<T1, T2>(Func<T, ICompletion<T1>> func, Func<T, T1, T2> projection)
      {
         return func(value).Map(t1 => projection(value, t1).Completed(), cancelled<T2>, interrupted<T2>);
      }

      public ICompletion<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(value).Completed();

      public ICompletion<TResult> Select<TResult>(ICompletion<T> result, Func<T, TResult> func) => func(value).Completed();

      public bool If(out T value)
      {
         value = this.value;
         return true;
      }

      public bool IfCancelled() => false;

      public bool IfInterrupted(out Exception exception)
      {
         exception = default;
         return false;
      }

      public bool If(out T value, out IMaybe<Exception> anyException)
      {
         value = this.value;
         anyException = none<Exception>();

         return true;
      }

      public bool Else<TOther>(out ICompletion<TOther> result)
      {
         result = default;
         return false;
      }

      public ICompletion<TOther> NotCompleted<TOther>() => cancelled<TOther>();

      public bool WasCompleted(out ICompletion<T> completed)
      {
         completed = this;
         return true;
      }

      public void Force() { }

      public T ForceValue() => value;

      public ICompletion<T> CancelledOnly() => cancelled<T>();

      public ICompletion<TOther> CancelledOnly<TOther>() => cancelled<TOther>();

      public void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> anyException)
      {
         value = this.value.Some();
         anyException = none<Exception>();
      }
   }
}