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

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted) => cancelled<TResult>();

      public override ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => cancelled<TResult>();

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled)
      {
         return ifCancelled();
      }

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return cancelled<TResult>();
      }

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifCancelled();
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
      {
         return ifCancelled();
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifNotCompleted();

      public override ICompletion<T> If(Action<T> action) => this;

      public override ICompletion<T> Else(Action action)
      {
         action();
         return this;
      }

      public override ICompletion<T> Else(Action<Exception> action) => this;

      public override ICompletion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifNotCompleted();
         return this;
      }

      public override ICompletion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifCancelled();
         return this;
      }

      public override ICompletion<TOther> InterruptedAs<TOther>() => throw "There is no exception".Throws();

      public override ICompletion<T> Or(ICompletion<T> other) => other;

      public override ICompletion<T> Or(Func<ICompletion<T>> other) => other();

      public override ICompletion<TResult> SelectMany<TResult>(Func<T, ICompletion<TResult>> projection) => cancelled<TResult>();

      public override ICompletion<T2> SelectMany<T1, T2>(Func<T, ICompletion<T1>> func, Func<T, T1, T2> projection) => cancelled<T2>();

      public override ICompletion<TResult> SelectMany<TResult>(Func<T, TResult> func) => cancelled<TResult>();

      public override ICompletion<TResult> Select<TResult>(ICompletion<T> result, Func<T, TResult> func) => cancelled<TResult>();

      public override bool If(out T value)
      {
         value = default;
         return false;
      }

      public override bool IfCancelled() => true;

      public override bool IfInterrupted(out Exception exception)
      {
         exception = default;
         return false;
      }

      public override bool If(out T value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = none<Exception>();
         return false;
      }

      public override bool IfNot(out Maybe<Exception> _exception)
      {
         _exception = none<Exception>();
         return true;
      }

      public override bool Else<TOther>(out ICompletion<TOther> result)
      {
         result = cancelled<TOther>();
         return true;
      }

      public override ICompletion<TOther> NotCompleted<TOther>() => cancelled<TOther>();

      public override bool IsCompleted(out ICompletion<T> completed)
      {
         completed = this;
         return false;
      }

      public override bool NotCompleted(out ICompletion<T> notCompleted)
      {
         notCompleted = this;
         return true;
      }

      public override void Force()
      {
      }

      public override T ForceValue() => throw "There is no value".Throws();

      public override ICompletion<T> CancelledOnly() => this;

      public override ICompletion<TOther> CancelledOnly<TOther>() => cancelled<TOther>();

      public override ICompletion<TOther> NotCompletedOnly<TOther>() => cancelled<TOther>();

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception)
      {
         value = none<T>();
         _exception = none<Exception>();
      }

      public override ICompletion<T> OnCompleted(Action<T> action) => this;

      public override ICompletion<T> OnCancelled(Action action)
      {
         action();
         return this;
      }

      public override ICompletion<T> OnInterrupted(Action<Exception> action) => this;

      public override bool ValueOrOriginal(out T value, out ICompletion<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      public override bool ValueOrCast<TCompletion>(out T value, out ICompletion<TCompletion> completion)
      {
         value = default;
         completion = cancelled<TCompletion>();

         return false;
      }

      public override bool ValueEqualTo(ICompletion<T> otherCompletion) => false;

      public override bool EqualToValueOf(T otherValue) => false;

      public ICompletion<object> AsObject() => cancelled<object>();

      public override ICompletion<TResult> CastAs<TResult>() => cancelled<TResult>();

      public override ICompletion<T> Where(Predicate<T> predicate) => this;

      public override ICompletion<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public override ICompletion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public bool Equals(Cancelled<T> other) => true;

      public override bool Equals(object obj) => obj is Cancelled<T> other && Equals(other);

      public override int GetHashCode() => false.GetHashCode();

      public override string ToString() => $"cancelled<{typeof(T).Name}>";
   }
}