using System;

namespace Standard.Types.Monads
{
   public interface IResult<T>
   {
      bool If(out T value, out Exception exception);

      bool Out(out T value, out IResult<T> original);

      bool IsSuccessful { get; }

      bool IsFailed { get; }

      Exception Exception { get; }

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

      IResult<T> OnSuccess(Action<T> action);

      IResult<T> OnFailure(Action<Exception> action);

	   void Deconstruct(out IMaybe<T> value, out Exception exception);

	   IResult<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage);
   }
}