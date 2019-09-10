using System;

namespace Core.Monads
{
   public interface ICompletion<T>
   {
      bool IsCompleted { get; }

      bool IsCancelled { get; }

      bool IsInterrupted { get; }

      ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted);

      ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted);

      ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled);

      ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<Exception, ICompletion<TResult>> ifInterrupted);

      ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled,
         Func<Exception, ICompletion<TResult>> ifInterrupted);

      TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted);

      TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted);

      ICompletion<T> If(Action<T> action);

      ICompletion<T> Else(Action action);

      ICompletion<T> Else(Action<Exception> action);

      ICompletion<T> Do(Action<T> ifCompleted, Action ifNotCompleted);

      ICompletion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted);

      ICompletion<TOther> InterruptedAs<TOther>();

      ICompletion<T> Or(ICompletion<T> other);

      ICompletion<T> Or(Func<ICompletion<T>> other);

      ICompletion<TResult> SelectMany<TResult>(Func<T, ICompletion<TResult>> projection);

      ICompletion<T2> SelectMany<T1, T2>(Func<T, ICompletion<T1>> func, Func<T, T1, T2> projection);

      ICompletion<TResult> SelectMany<TResult>(Func<T, TResult> func);

      ICompletion<TResult> Select<TResult>(ICompletion<T> result, Func<T, TResult> func);

      bool If(out T value);

      bool IfCancelled();

      bool IfInterrupted(out Exception exception);

      bool Out(out T value, out IMaybe<Exception> anyException);

      bool Else<TOther>(out ICompletion<TOther> result);

      ICompletion<TOther> NotCompleted<TOther>();

      bool WasCompleted(out ICompletion<T> completed);

      void Force();

      T ForceValue();

      ICompletion<T> CancelledOnly();

      ICompletion<TOther> CancelledOnly<TOther>();

      void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> anyException);
   }
}