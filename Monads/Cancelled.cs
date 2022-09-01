using System;
using Core.Applications;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Cancelled<T> : Completion<T>, IEquatable<Cancelled<T>>
   {
      public static implicit operator bool(Cancelled<T> _) => false;

      internal Cancelled()
      {
      }

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted) => nil;

      public override Completion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => nil;

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled)
      {
         return ifCancelled();
      }

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted,
         Func<Exception, Completion<TResult>> ifInterrupted)
      {
         return nil;
      }

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled,
         Func<Exception, Completion<TResult>> ifInterrupted)
      {
         return ifCancelled();
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
      {
         return ifCancelled();
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifNotCompleted();

      public override Completion<T> Map(Action<T> action) => this;

      public override Completion<T> UnMap(Action action)
      {
         action();
         return this;
      }

      public override Completion<T> UnMap(Action<Exception> action) => this;

      public override Completion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifNotCompleted();
         return this;
      }

      public override Completion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifCancelled();
         return this;
      }

      [Obsolete("Use exception")]
      public override Completion<TOther> InterruptedAs<TOther>() => throw "There is no exception".Throws();

      public override Completion<T> Or(Completion<T> other) => other;

      public override Completion<T> Or(Func<Completion<T>> other) => other();

      public override Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection) => nil;

      public override Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection) => nil;

      public override Completion<TResult> SelectMany<TResult>(Func<T, TResult> func) => nil;

      public override Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func) => nil;

      public override bool Map(out T value)
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

      public override bool Map(out T value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = nil;
         return false;
      }

      public override bool UnMap(out Maybe<Exception> _exception)
      {
         _exception = nil;
         return true;
      }

      public override bool UnMap<TOther>(out Completion<TOther> result)
      {
         result = nil;
         return true;
      }

      public override Completion<TOther> NotCompleted<TOther>() => nil;

      public override bool IsCompleted(out Completion<T> completed)
      {
         completed = this;
         return false;
      }

      public override bool NotCompleted(out Completion<T> notCompleted)
      {
         notCompleted = this;
         return true;
      }

      public override void Force()
      {
      }

      public override T ForceValue() => throw "There is no value".Throws();

      public override Completion<T> CancelledOnly() => this;

      public override Completion<TOther> CancelledOnly<TOther>() => nil;

      public override Completion<TOther> NotCompletedOnly<TOther>() => nil;

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception)
      {
         value = nil;
         _exception = nil;
      }

      public override Completion<T> OnCompleted(Action<T> action) => this;

      public override Completion<T> OnCancelled(Action action)
      {
         action();
         return this;
      }

      public override Completion<T> OnInterrupted(Action<Exception> action) => this;

      [Obsolete("Use If")]
      public override bool ValueOrOriginal(out T value, out Completion<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      [Obsolete("Use If")]
      public override bool ValueOrCast<TCompletion>(out T value, out Completion<TCompletion> completion)
      {
         value = default;
         completion = cancelled<TCompletion>();

         return false;
      }

      public override bool ValueEqualTo(Completion<T> otherCompletion) => false;

      public override bool EqualToValueOf(T otherValue) => false;

      public Completion<object> AsObject() => nil;

      public override Completion<TResult> CastAs<TResult>() => nil;

      public override Completion<T> Where(Predicate<T> predicate) => this;

      public override Completion<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public override Completion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public override T DefaultTo(Func<Maybe<Exception>, T> defaultFunc) => defaultFunc(nil);

      public override Maybe<T> Maybe() => nil;

      public override Result<T> Result() => new Failure<T>(new CancelException());

      public override Responding<T> Responding() => nil;

      public bool Equals(Cancelled<T> other) => true;

      public override bool Equals(object obj) => obj is Cancelled<T> other && Equals(other);

      public override int GetHashCode() => false.GetHashCode();

      public override string ToString() => $"cancelled<{typeof(T).Name}>";
   }
}