using System;

namespace Core.Monads
{
   public interface IMatched<T>
   {
      bool IsMatched { get; }

      bool IsNotMatched { get; }

      bool IsFailedMatch { get; }

      IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched);

      IMatched<TResult> Map<TResult>(Func<T, TResult> ifMatched);

      IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched);

      IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<Exception, IMatched<TResult>> ifFailedMatch);

      IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched,
         Func<Exception, IMatched<TResult>> ifFailedMatch);

      TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched, Func<Exception, TResult> ifFailedMatch);

      TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed);

      IMatched<T> If(Action<T> action);

      IMatched<T> Else(Action action);

      IMatched<T> Else(Action<Exception> action);

      IMatched<T> Do(Action<T> ifMatched, Action ifNotOrFailed);

      IMatched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch);

      IMatched<TOther> ExceptionAs<TOther>();

      IMatched<T> Or(IMatched<T> other);

      IMatched<T> Or(Func<IMatched<T>> other);

      IMatched<TResult> SelectMany<TResult>(Func<T, IMatched<TResult>> projection);

      IMatched<T2> SelectMany<T1, T2>(Func<T, IMatched<T1>> func, Func<T, T1, T2> projection);

      IMatched<TResult> SelectMany<TResult>(Func<T, TResult> func);

      IMatched<TResult> Select<TResult>(IMatched<T> result, Func<T, TResult> func);

      bool If(out T value);

      bool IfNotMatched();

      bool Failed(out Exception exception);

      bool ValueOrOriginal(out T value, out IMatched<T> original);

      bool ValueOrCast<TMatched>(out T value, out IMatched<TMatched> matched);

      bool If(out T value, out IMaybe<Exception> exception);

      bool IfNot(out IMaybe<Exception> anyException);

      bool Else<TOther>(out IMatched<TOther> result);

      IMatched<TOther> Unmatched<TOther>();

      bool WasMatched(out IMatched<T> matched);

      void Force();

      T ForceValue();

      IMatched<T> UnmatchedOnly();

      IMatched<TOther> UnmatchedOnly<TOther>();

	   void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> exception);

      bool EqualToValueOf(IMatched<T> otherMatched);

      bool ValueEqualTo(T otherValue);

      IMatched<TResult> CastAs<TResult>();

      IMatched<T> Where(Predicate<T> predicate);

      IMatched<T> Where(Predicate<T> predicate, string exceptionMessage);

      IMatched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage);

      IMatched<T> ExceptionMessage(string message);

      IMatched<T> ExceptionMessage(Func<Exception, string> message);
   }
}