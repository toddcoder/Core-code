using System;

namespace Core.Monads
{
   public abstract class Matched<T>
   {
      public static implicit operator Matched<T>(T value) => value.Match();

      public static implicit operator Matched<T>(Exception exception) => new FailedMatch<T>(exception);

      public abstract bool IsMatched { get; }

      public abstract bool IsNotMatched { get; }

      public abstract bool IsFailedMatch { get; }

      public abstract Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched);

      public abstract Matched<TResult> Map<TResult>(Func<T, TResult> ifMatched);

      public abstract Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched, Func<Matched<TResult>> ifNotMatched);

      public abstract Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched, Func<Exception, Matched<TResult>> ifFailedMatch);

      public abstract Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched, Func<Matched<TResult>> ifNotMatched,
         Func<Exception, Matched<TResult>> ifFailedMatch);

      public abstract TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched, Func<Exception, TResult> ifFailedMatch);

      public abstract TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed);

      public abstract Matched<T> If(Action<T> action);

      public abstract Matched<T> Else(Action action);

      public abstract Matched<T> Else(Action<Exception> action);

      public abstract Matched<T> Do(Action<T> ifMatched, Action ifNotOrFailed);

      public abstract Matched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch);

      public abstract Matched<TOther> ExceptionAs<TOther>();

      public abstract Matched<T> Or(Matched<T> other);

      public abstract Matched<T> Or(Func<Matched<T>> other);

      public abstract Matched<TResult> SelectMany<TResult>(Func<T, Matched<TResult>> projection);

      public abstract Matched<T2> SelectMany<T1, T2>(Func<T, Matched<T1>> func, Func<T, T1, T2> projection);

      public abstract Matched<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract Matched<TResult> Select<TResult>(Matched<T> result, Func<T, TResult> func);

      public abstract bool If(out T value);

      public abstract bool IfNotMatched();

      public abstract bool Failed(out Exception exception);

      public abstract bool ValueOrOriginal(out T value, out Matched<T> original);

      public abstract bool ValueOrCast<TMatched>(out T value, out Matched<TMatched> matched);

      public abstract bool If(out T value, out Maybe<Exception> exception);

      public abstract bool IfNot(out Maybe<Exception> anyException);

      public abstract bool Else<TOther>(out Matched<TOther> result);

      public abstract Matched<TOther> Unmatched<TOther>();

      public abstract bool WasMatched(out Matched<T> matched);

      public abstract void Force();

      public abstract T ForceValue();

      public abstract Matched<T> UnmatchedOnly();

      public abstract Matched<TOther> UnmatchedOnly<TOther>();

      public abstract void Deconstruct(out Maybe<T> value, out Maybe<Exception> exception);

      public abstract bool EqualToValueOf(Matched<T> otherMatched);

      public abstract bool ValueEqualTo(T otherValue);

      public abstract Matched<TResult> CastAs<TResult>();

      public abstract Matched<T> Where(Predicate<T> predicate);

      public abstract Matched<T> Where(Predicate<T> predicate, string exceptionMessage);

      public abstract Matched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage);

      public abstract Matched<T> ExceptionMessage(string message);

      public abstract Matched<T> ExceptionMessage(Func<Exception, string> message);

      public Matched<T> Tap(Action<Matched<T>> action)
      {
         action(this);
         return this;
      }
   }
}