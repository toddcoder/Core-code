using System;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Interrupted<T> : Completion<T>, IEquatable<Interrupted<T>>
{
   // ReSharper disable once UnusedParameter.Global
   public static implicit operator bool(Interrupted<T> _) => false;

   protected Exception exception;

   internal Interrupted(Exception exception)
   {
      this.exception = exception is FullStackException ? exception : new FullStackException(exception);
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted) => exception;

   public override Completion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => exception;

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled)
   {
      return exception;
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

   public override Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection) => exception;

   public override Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection) => exception;

   public override Completion<TResult> SelectMany<TResult>(Func<T, TResult> func) => exception;

   public override Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func) => exception;

   [Obsolete("Use ~")]
   public override bool Map(out T value)
   {
      value = default;
      return false;
   }

   public override bool IfCancelled() => false;

   [Obsolete("Use !")]
   public override bool IfInterrupted(out Exception exception)
   {
      exception = this.exception;
      return true;
   }

   [Obsolete("Use ~")]
   public override bool Map(out T value, out Maybe<Exception> _exception)
   {
      value = default;
      _exception = exception;

      return false;
   }

   [Obsolete("Use !")]
   public override bool UnMap(out Maybe<Exception> _exception)
   {
      _exception = exception;
      return true;
   }

   [Obsolete("Use !")]
   public override bool UnMap<TOther>(out Completion<TOther> result)
   {
      result = exception;
      return true;
   }

   public override Completion<TOther> NotCompleted<TOther>() => exception;

   [Obsolete("Use ~")]
   public override bool IsCompleted(out Completion<T> completed)
   {
      completed = this;
      return false;
   }

   [Obsolete("Use !")]
   public override bool NotCompleted(out Completion<T> notCompleted)
   {
      notCompleted = this;
      return true;
   }

   public override void Force() => throw exception;

   public override T ForceValue() => throw exception;

   public override Completion<T> CancelledOnly() => throw exception;

   public override Completion<TOther> CancelledOnly<TOther>() => throw exception;

   public override Completion<TOther> NotCompletedOnly<TOther>() => exception;

   public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception)
   {
      value = default;
      _exception = exception;
   }

   public override Completion<T> OnCompleted(Action<T> action) => this;

   public override Completion<T> OnCancelled(Action action) => this;

   public override Completion<T> OnInterrupted(Action<Exception> action)
   {
      action(exception);
      return this;
   }

   public override bool ValueEqualTo(Completion<T> otherCompletion) => false;

   public override bool EqualToValueOf(T otherValue) => false;

   public Completion<object> AsObject() => exception;

   public override Completion<TResult> CastAs<TResult>() => exception;

   public override Completion<T> Where(Predicate<T> predicate) => this;

   public override Completion<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

   public override Completion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

   public override T DefaultTo(Func<Maybe<Exception>, T> defaultFunc) => defaultFunc(exception);

   public override Maybe<T> Maybe() => nil;

   public override Result<T> Result() => exception;

   public override Responding<T> Responding() => exception;

   public override T Value => throw exception;

   public override Exception Exception => exception;

   public override Maybe<Exception> AnyException => exception;

   public bool Equals(Interrupted<T> other)
   {
      return other is not null && ReferenceEquals(this, other) || Equals(exception, other.exception);
   }

   public override bool Equals(object obj) => obj is Interrupted<T> other && Equals(other);

   public override int GetHashCode() => exception?.GetHashCode() ?? 0;

   public override string ToString() => $"Interrupted({exception.Message.Elliptical(60, ' ')})";
}