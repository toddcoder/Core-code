using System;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Cancelled<T> : ICompletion<T>, IEquatable<Cancelled<T>>
   {
      public static implicit operator bool(Cancelled<T> _) => false;

      internal Cancelled()
      {
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted) => cancelled<TResult>();

      public ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => cancelled<TResult>();

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled)
      {
         return ifCancelled();
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<Exception, ICompletion<TResult>> ifInterrupted)
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

      public bool If(out T value, out IMaybe<Exception> _exception)
      {
         value = default;
         _exception = none<Exception>();
         return false;
      }

      public bool IfNot(out IMaybe<Exception> _exception)
      {
         _exception = none<Exception>();
         return true;
      }

      public bool Else<TOther>(out ICompletion<TOther> result)
      {
         result = cancelled<TOther>();
         return true;
      }

      public ICompletion<TOther> NotCompleted<TOther>() => cancelled<TOther>();

      public bool IsCompleted(out ICompletion<T> completed)
      {
         completed = this;
         return false;
      }

      public bool NotCompleted(out ICompletion<T> notCompleted)
      {
         notCompleted = this;
         return true;
      }

      public void Force()
      {
      }

      public T ForceValue() => throw "There is no value".Throws();

      public ICompletion<T> CancelledOnly() => this;

      public ICompletion<TOther> CancelledOnly<TOther>() => cancelled<TOther>();

      public ICompletion<TOther> NotCompletedOnly<TOther>() => cancelled<TOther>();

      public void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> _exception)
      {
         value = none<T>();
         _exception = none<Exception>();
      }

      public ICompletion<T> OnCompleted(Action<T> action) => this;

      public ICompletion<T> OnCancelled(Action action)
      {
         action();
         return this;
      }

      public ICompletion<T> OnInterrupted(Action<Exception> action) => this;

      public bool ValueOrOriginal(out T value, out ICompletion<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      public bool ValueOrCast<TCompletion>(out T value, out ICompletion<TCompletion> completion)
      {
         value = default;
         completion = cancelled<TCompletion>();

         return false;
      }

      public bool ValueEqualTo(ICompletion<T> otherCompletion) => false;

      public bool EqualToValueOf(T otherValue) => false;

      public ICompletion<object> AsObject() => cancelled<object>();

      public ICompletion<TResult> CastAs<TResult>() => cancelled<TResult>();

      public ICompletion<T> Where(Predicate<T> predicate) => this;

      public ICompletion<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public ICompletion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public bool Equals(Cancelled<T> other) => true;

      public override bool Equals(object obj) => obj is Cancelled<T> other && Equals(other);

      public override int GetHashCode() => false.GetHashCode();

      public override string ToString() => $"cancelled<{typeof(T).Name}>";
   }
}