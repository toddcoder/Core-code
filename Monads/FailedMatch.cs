﻿using System;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class FailedMatch<T> : IMatched<T>, IEquatable<FailedMatch<T>>
   {
      public static implicit operator bool(FailedMatch<T> _) => false;

      protected Exception exception;

      internal FailedMatch(Exception exception)
      {
         this.exception = exception is FullStackException ? exception : new FullStackException(exception);
      }

      public Exception Exception => exception;

      public override IMatched<T> Do(Action<T> ifMatched, Action ifNotOrFailed)
      {
         ifNotOrFailed();
         return this;
      }

      public override IMatched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch)
      {
         ifFailedMatch(exception);
         return this;
      }

      public override IMatched<TOther> ExceptionAs<TOther>() => failedMatch<TOther>(exception);

      public override IMatched<T> Or(IMatched<T> other) => other;

      public override IMatched<T> Or(Func<IMatched<T>> other) => other();

      public override IMatched<TResult> SelectMany<TResult>(Func<T, IMatched<TResult>> projection)
      {
         return failedMatch<TResult>(exception);
      }

      public override IMatched<T2> SelectMany<T1, T2>(Func<T, IMatched<T1>> func, Func<T, T1, T2> projection)
      {
         return failedMatch<T2>(exception);
      }

      public override IMatched<TResult> SelectMany<TResult>(Func<T, TResult> func) => failedMatch<TResult>(exception);

      public override IMatched<TResult> Select<TResult>(IMatched<T> result, Func<T, TResult> func)
      {
         return failedMatch<TResult>(exception);
      }

      public override bool If(out T value)
      {
         value = default;
         return false;
      }

      public override bool IfNotMatched() => false;

      public override bool Failed(out Exception exception)
      {
         exception = this.exception;
         return true;
      }

      public override bool ValueOrOriginal(out T value, out IMatched<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      public override bool ValueOrCast<TMatched>(out T value, out IMatched<TMatched> matched)
      {
         value = default;
         matched = failedMatch<TMatched>(exception);

         return false;
      }

      public override bool If(out T value, out Maybe<Exception> exception)
      {
         value = default;
         exception = this.exception.Some();

         return false;
      }

      public override bool IfNot(out Maybe<Exception> anyException)
      {
         anyException = exception.Some();
         return true;
      }

      public override bool Else<TOther>(out IMatched<TOther> result)
      {
         result = failedMatch<TOther>(exception);
         return true;
      }

      public override IMatched<TOther> Unmatched<TOther>() => failedMatch<TOther>(exception);

      public override bool WasMatched(out IMatched<T> matched)
      {
         matched = this;
         return false;
      }

      public override void Force() => throw exception;

      public override T ForceValue() => throw exception;

      public override IMatched<T> UnmatchedOnly() => throw exception;

      public override IMatched<TOther> UnmatchedOnly<TOther>() => throw exception;

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> exception)
      {
         value = none<T>();
         exception = this.exception.Some();
      }

      public override bool EqualToValueOf(IMatched<T> otherMatched) => false;

      public override bool ValueEqualTo(T otherValue) => false;

      public override IMatched<TResult> CastAs<TResult>() => failedMatch<TResult>(exception);

      public override IMatched<T> Where(Predicate<T> predicate) => this;

      public override IMatched<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public override IMatched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public override IMatched<T> ExceptionMessage(string message) => new FailedMatch<T>(new FullStackException(message, exception));

      public override IMatched<T> ExceptionMessage(Func<Exception, string> message)
      {
         return new FailedMatch<T>(new FullStackException(message(exception), exception));
      }

      public override bool IsMatched => false;

      public override bool IsNotMatched => false;

      public override bool IsFailedMatch => true;

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched) => failedMatch<TResult>(exception);

      public override IMatched<TResult> Map<TResult>(Func<T, TResult> ifMatched) => failedMatch<TResult>(exception);

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched)
      {
         return failedMatch<TResult>(exception);
      }

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<Exception,
         IMatched<TResult>> ifFailedMatch)
      {
         return ifFailedMatch(exception);
      }

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched,
         Func<IMatched<TResult>> ifNotMatched, Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return ifFailedMatch(exception);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched,
         Func<Exception, TResult> ifFailedMatch)
      {
         return ifFailedMatch(exception);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed) => ifNotOrFailed();

      public override IMatched<T> If(Action<T> action) => this;

      public override IMatched<T> Else(Action action) => this;

      public override IMatched<T> Else(Action<Exception> action)
      {
         action(exception);
         return this;
      }

      public bool Equals(FailedMatch<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || Equals(exception, other.exception);
      }

      public override bool Equals(object obj) => obj is FailedMatch<T> other && Equals(other);

      public override int GetHashCode() => exception?.GetHashCode() ?? 0;

      public override string ToString() => $"FailedMatch({exception.Message.Elliptical(60, ' ')})";
   }
}