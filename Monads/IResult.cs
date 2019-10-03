using System;

namespace Core.Monads
{
   public interface IResult<T> : IHasValue
   {
      bool If(out T value, out Exception exception);

      [Obsolete("Use ValueOrOriginal or ValueOrCast")]
      bool Out(out T value, out IResult<T> original);

      bool ValueOrOriginal(out T value, out IResult<T> original);

      bool ValueOrCast<TResult>(out T value, out IResult<TResult> result);

      bool IsSuccessful { get; }

      bool IsFailed { get; }

      IResult<TOther> ExceptionAs<TOther>();

      IResult<TResult> Map<TResult>(Func<T, IResult<TResult>> ifSuccessful);

      IResult<TResult> Map<TResult>(Func<T, TResult> ifSuccessful);

      TResult FlatMap<TResult>(Func<T, TResult> ifSuccessful, Func<Exception, TResult> ifFailed);

      IResult<TResult> SelectMany<TResult>(Func<T, IResult<TResult>> projection);

      IResult<T2> SelectMany<T1, T2>(Func<T, IResult<T1>> func, Func<T, T1, T2> projection);

      IResult<TResult> SelectMany<TResult>(Func<T, TResult> func);

      T Recover(Func<Exception, T> recovery);

      IResult<T> Or(IResult<T> other);

      IResult<T> Or(Func<IResult<T>> other);

      IResult<T> Or(T other);

      IResult<T> Or(Func<T> other);

      IResult<Unit> Unit { get; }

      IResult<T> Always(Action action);

      IMatched<T> Match();

      bool If(out T value);

      bool IfNot(out Exception exception);

      bool IfNot(out T value, out Exception exception);

      void Force();

      T ForceValue();

      IResult<T> OnSuccess(Action<T> action);

      IResult<T> OnFailure(Action<Exception> action);

	   void Deconstruct(out IMaybe<T> value, out Exception exception);

	   IResult<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage);

      IMaybe<T> Maybe();

      bool EqualToValueOf(IResult<T> otherResult);

      bool ValueEqualTo(T otherValue);
   }
}