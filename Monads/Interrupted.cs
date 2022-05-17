using System;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Interrupted<T> : Completion<T>, IEquatable<Interrupted<T>>
   {
      public static implicit operator bool(Interrupted<T> _) => false;

      protected Exception exception;

      internal Interrupted(Exception exception)
      {
         this.exception = exception is FullStackException ? exception : new FullStackException(exception);
      }

      public Exception Exception => exception;

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted) => interrupted<TResult>(exception);

      public override Completion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => interrupted<TResult>(exception);

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled)
      {
         return interrupted<TResult>(exception);
      }

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Exception, Completion<TResult>> ifInterrupted)
      {
         return ifInterrupted(exception);
      }

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled,
         Func<Exception, Completion<TResult>> ifInterrupted)
      {
         return ifInterrupted(exception);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
      {
         return ifInterrupted(exception);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifNotCompleted();

      public override Completion<T> Map(Action<T> action) => this;

      public override Completion<T> UnMap(Action action) => this;

      public override Completion<T> UnMap(Action<Exception> action)
      {
         action(exception);
         return this;
      }

      public override Completion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifNotCompleted();
         return this;
      }

      public override Completion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifInterrupted(exception);
         return this;
      }

      [Obsolete("Use exception")]
      public override Completion<TOther> InterruptedAs<TOther>() => interrupted<TOther>(exception);

      public override Completion<T> Or(Completion<T> other) => other;

      public override Completion<T> Or(Func<Completion<T>> other) => other();

      public override Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection) => interrupted<TResult>(exception);

      public override Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection) => interrupted<T2>(exception);

      public override Completion<TResult> SelectMany<TResult>(Func<T, TResult> func) => interrupted<TResult>(exception);

      public override Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func) => interrupted<TResult>(exception);

      public override bool Map(out T value)
      {
         value = default;
         return false;
      }

      public override bool IfCancelled() => false;

      public override bool IfInterrupted(out Exception exception)
      {
         exception = this.exception;
         return true;
      }

      public override bool Map(out T value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = exception.Some();

         return false;
      }

      public override bool UnMap(out Maybe<Exception> _exception)
      {
         _exception = exception.Some();
         return true;
      }

      public override bool UnMap<TOther>(out Completion<TOther> result)
      {
         result = interrupted<TOther>(exception);
         return true;
      }

      public override Completion<TOther> NotCompleted<TOther>() => interrupted<TOther>(exception);

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

      public override void Force() => throw exception;

      public override T ForceValue() => throw exception;

      public override Completion<T> CancelledOnly() => throw exception;

      public override Completion<TOther> CancelledOnly<TOther>() => throw exception;

      public override Completion<TOther> NotCompletedOnly<TOther>() => new Interrupted<TOther>(exception);

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = exception.Some();
      }

      public override Completion<T> OnCompleted(Action<T> action) => this;

      public override Completion<T> OnCancelled(Action action) => this;

      public override Completion<T> OnInterrupted(Action<Exception> action)
      {
         action(exception);
         return this;
      }

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
         completion = interrupted<TCompletion>(exception);

         return false;
      }

      public override bool ValueEqualTo(Completion<T> otherCompletion) => false;

      public override bool EqualToValueOf(T otherValue) => false;

      public Completion<object> AsObject() => interrupted<object>(exception);

      public override Completion<TResult> CastAs<TResult>() => interrupted<TResult>(exception);

      public override Completion<T> Where(Predicate<T> predicate) => this;

      public override Completion<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public override Completion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public override Maybe<T> Maybe() => Maybe<T>.nil;

      public override Result<T> Result() => new Failure<T>(Exception);

      public override Responding<T> Responding() => new FailedResponse<T>(Exception);

      public bool Equals(Interrupted<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || Equals(exception, other.exception);
      }

      public override bool Equals(object obj) => obj is Interrupted<T> other && Equals(other);

      public override int GetHashCode() => exception?.GetHashCode() ?? 0;

      public override string ToString() => $"Interrupted({exception.Message.Elliptical(60, ' ')})";
   }
}