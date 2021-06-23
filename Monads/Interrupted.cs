using System;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Interrupted<T> : ICompletion<T>, IEquatable<Interrupted<T>>
   {
      public static implicit operator bool(Interrupted<T> _) => false;

      protected Exception exception;

      internal Interrupted(Exception exception)
      {
         this.exception = exception is FullStackException ? exception : new FullStackException(exception);
      }

      public Exception Exception => exception;

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted) => interrupted<TResult>(exception);

      public override ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => interrupted<TResult>(exception);

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled)
      {
         return interrupted<TResult>(exception);
      }

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifInterrupted(exception);
      }

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifInterrupted(exception);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
      {
         return ifInterrupted(exception);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifNotCompleted();

      public override ICompletion<T> If(Action<T> action) => this;

      public override ICompletion<T> Else(Action action) => this;

      public override ICompletion<T> Else(Action<Exception> action)
      {
         action(exception);
         return this;
      }

      public override ICompletion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifNotCompleted();
         return this;
      }

      public override ICompletion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifInterrupted(exception);
         return this;
      }

      public override ICompletion<TOther> InterruptedAs<TOther>() => interrupted<TOther>(exception);

      public override ICompletion<T> Or(ICompletion<T> other) => other;

      public override ICompletion<T> Or(Func<ICompletion<T>> other) => other();

      public override ICompletion<TResult> SelectMany<TResult>(Func<T, ICompletion<TResult>> projection) => interrupted<TResult>(exception);

      public override ICompletion<T2> SelectMany<T1, T2>(Func<T, ICompletion<T1>> func, Func<T, T1, T2> projection) => interrupted<T2>(exception);

      public override ICompletion<TResult> SelectMany<TResult>(Func<T, TResult> func) => interrupted<TResult>(exception);

      public override ICompletion<TResult> Select<TResult>(ICompletion<T> result, Func<T, TResult> func) => interrupted<TResult>(exception);

      public override bool If(out T value)
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

      public override bool If(out T value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = exception.Some();

         return false;
      }

      public override bool IfNot(out Maybe<Exception> _exception)
      {
         _exception = exception.Some();
         return true;
      }

      public override bool Else<TOther>(out ICompletion<TOther> result)
      {
         result = interrupted<TOther>(exception);
         return true;
      }

      public override ICompletion<TOther> NotCompleted<TOther>() => interrupted<TOther>(exception);

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

      public override void Force() => throw exception;

      public override T ForceValue() => throw exception;

      public override ICompletion<T> CancelledOnly() => throw exception;

      public override ICompletion<TOther> CancelledOnly<TOther>() => throw exception;

      public override ICompletion<TOther> NotCompletedOnly<TOther>() => new Interrupted<TOther>(exception);

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = exception.Some();
      }

      public override ICompletion<T> OnCompleted(Action<T> action) => this;

      public override ICompletion<T> OnCancelled(Action action) => this;

      public override ICompletion<T> OnInterrupted(Action<Exception> action)
      {
         action(exception);
         return this;
      }

      public override bool ValueOrOriginal(out T value, out ICompletion<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      public override bool ValueOrCast<TCompletion>(out T value, out ICompletion<TCompletion> completion)
      {
         value = default;
         completion = interrupted<TCompletion>(exception);

         return false;
      }

      public override bool ValueEqualTo(ICompletion<T> otherCompletion) => false;

      public override bool EqualToValueOf(T otherValue) => false;

      public ICompletion<object> AsObject() => interrupted<object>(exception);

      public override ICompletion<TResult> CastAs<TResult>() => interrupted<TResult>(exception);

      public override ICompletion<T> Where(Predicate<T> predicate) => this;

      public override ICompletion<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public override ICompletion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public bool Equals(Interrupted<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || Equals(exception, other.exception);
      }

      public override bool Equals(object obj) => obj is Interrupted<T> other && Equals(other);

      public override int GetHashCode() => exception?.GetHashCode() ?? 0;

      public override string ToString() => $"Interrupted({exception.Message.Elliptical(60, ' ')})";
   }
}