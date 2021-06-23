using System;

namespace Core.Monads
{
   public abstract class IResult<T>
   {
      public abstract bool If(out T value, out Exception exception);

      public abstract bool ValueOrOriginal(out T value, out IResult<T> original);

      public abstract bool ValueOrCast<TResult>(out T value, out IResult<TResult> result);

      public abstract bool IsSuccessful { get; }

      public abstract bool IsFailed { get; }

      public abstract IResult<TOther> ExceptionAs<TOther>();

      public abstract IResult<TResult> Map<TResult>(Func<T, IResult<TResult>> ifSuccessful);

      public abstract IResult<TResult> Map<TResult>(Func<T, TResult> ifSuccessful);

      public abstract IResult<TResult> SelectMany<TResult>(Func<T, IResult<TResult>> projection);

      public abstract IResult<T2> SelectMany<T1, T2>(Func<T, IResult<T1>> func, Func<T, T1, T2> projection);

      public abstract IResult<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract T Recover(Func<Exception, T> recovery);

      public abstract IResult<T> Or(IResult<T> other);

      public abstract IResult<T> Or(Func<IResult<T>> other);

      public abstract IResult<T> Or(T other);

      public abstract IResult<T> Or(Func<T> other);

      public abstract IResult<Unit> Unit { get; }

      public abstract IResult<T> Always(Action action);

      public abstract IMatched<T> Match();

      public abstract bool If(out T value);

      public abstract bool IfNot(out Exception exception);

      public abstract bool IfNot(out T value, out Exception exception);

      public abstract void Force();

      public abstract T ForceValue();

      public abstract IResult<T> OnSuccess(Action<T> action);

      public abstract IResult<T> OnFailure(Action<Exception> action);

      public abstract void Deconstruct(out Maybe<T> value, out Exception exception);

      public abstract IResult<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage);

      public abstract Maybe<T> Maybe();

      public abstract bool EqualToValueOf(IResult<T> otherResult);

      public abstract bool ValueEqualTo(T otherValue);

      public abstract IResult<T> Otherwise(Func<Exception, T> func);

      public abstract IResult<T> Otherwise(Func<Exception, IResult<T>> func);

      public abstract IResult<TResult> CastAs<TResult>();

      public abstract IResult<T> Where(Predicate<T> predicate, string exceptionMessage);

      public abstract IResult<T> Where(Predicate<T> predicate, Func<string> exceptionMessage);

      public abstract IResult<T> ExceptionMessage(string message);

      public abstract IResult<T> ExceptionMessage(Func<Exception, string> message);

   }
}