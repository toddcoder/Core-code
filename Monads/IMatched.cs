using System;

namespace Core.Monads
{
   public abstract class IMatched<T>
   {
      public abstract bool IsMatched { get; }

      public abstract bool IsNotMatched { get; }

      public abstract bool IsFailedMatch { get; }

      public abstract IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched);

      public abstract IMatched<TResult> Map<TResult>(Func<T, TResult> ifMatched);

      public abstract IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched);

      public abstract IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<Exception, IMatched<TResult>> ifFailedMatch);

      public abstract IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched,
         Func<Exception, IMatched<TResult>> ifFailedMatch);

      public abstract TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched, Func<Exception, TResult> ifFailedMatch);

      public abstract TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed);

      public abstract IMatched<T> If(Action<T> action);

      public abstract IMatched<T> Else(Action action);

      public abstract IMatched<T> Else(Action<Exception> action);

      public abstract IMatched<T> Do(Action<T> ifMatched, Action ifNotOrFailed);

      public abstract IMatched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch);

      public abstract IMatched<TOther> ExceptionAs<TOther>();

      public abstract IMatched<T> Or(IMatched<T> other);

      public abstract IMatched<T> Or(Func<IMatched<T>> other);

      public abstract IMatched<TResult> SelectMany<TResult>(Func<T, IMatched<TResult>> projection);

      public abstract IMatched<T2> SelectMany<T1, T2>(Func<T, IMatched<T1>> func, Func<T, T1, T2> projection);

      public abstract IMatched<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract IMatched<TResult> Select<TResult>(IMatched<T> result, Func<T, TResult> func);

      public abstract bool If(out T value);

      public abstract bool IfNotMatched();

      public abstract bool Failed(out Exception exception);

      public abstract bool ValueOrOriginal(out T value, out IMatched<T> original);

      public abstract bool ValueOrCast<TMatched>(out T value, out IMatched<TMatched> matched);

      public abstract bool If(out T value, out Maybe<Exception> exception);

      public abstract bool IfNot(out Maybe<Exception> anyException);

      public abstract bool Else<TOther>(out IMatched<TOther> result);

      public abstract IMatched<TOther> Unmatched<TOther>();

      public abstract bool WasMatched(out IMatched<T> matched);

      public abstract void Force();

      public abstract T ForceValue();

      public abstract IMatched<T> UnmatchedOnly();

      public abstract IMatched<TOther> UnmatchedOnly<TOther>();

      public abstract void Deconstruct(out Maybe<T> value, out Maybe<Exception> exception);

      public abstract bool EqualToValueOf(IMatched<T> otherMatched);

      public abstract bool ValueEqualTo(T otherValue);

      public abstract IMatched<TResult> CastAs<TResult>();

      public abstract IMatched<T> Where(Predicate<T> predicate);

      public abstract IMatched<T> Where(Predicate<T> predicate, string exceptionMessage);

      public abstract IMatched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage);

      public abstract IMatched<T> ExceptionMessage(string message);

      public abstract IMatched<T> ExceptionMessage(Func<Exception, string> message);
   }
}