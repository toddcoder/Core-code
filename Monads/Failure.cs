using System;
using System.Diagnostics;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Failure<T> : IResult<T>
   {
      internal Failure(Exception exception)
      {
         Exception = exception is FullStackException ? exception : new FullStackException(exception);
      }

      public bool If(out T value, out Exception exception)
      {
         value = default;
         exception = Exception;

         return false;
      }

      public bool Out(out T value, out IResult<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      public bool IsSuccessful => false;

      public bool IsFailed => true;

      public Exception Exception { get; }

      public IResult<TOther> ExceptionAs<TOther>() => failure<TOther>(Exception);

      [DebuggerStepThrough]
      public IResult<TResult> Map<TResult>(Func<T, IResult<TResult>> ifSuccessful) => failure<TResult>(Exception);

      [DebuggerStepThrough]
      public IResult<TResult> Map<TResult>(Func<T, TResult> ifSuccessful) => failure<TResult>(Exception);

      [DebuggerStepThrough]
      public TResult FlatMap<TResult>(Func<T, TResult> ifSuccessful, Func<Exception, TResult> ifFailed)
      {
         return ifFailed(Exception);
      }

      [DebuggerStepThrough]
      public IResult<TResult> SelectMany<TResult>(Func<T, IResult<TResult>> projection) => failure<TResult>(Exception);

      [DebuggerStepThrough]
      public IResult<T2> SelectMany<T1, T2>(Func<T, IResult<T1>> func, Func<T, T1, T2> projection)
      {
         return failure<T2>(Exception);
      }

      [DebuggerStepThrough]
      public IResult<TResult> SelectMany<TResult>(Func<T, TResult> func) => failure<TResult>(Exception);

      [DebuggerStepThrough]
      public T Recover(Func<Exception, T> recovery) => recovery(Exception);

      [DebuggerStepThrough]
      public IResult<T> Or(IResult<T> other) => other;

      [DebuggerStepThrough]
      public IResult<T> Or(Func<IResult<T>> other) => tryTo(other);

      [DebuggerStepThrough]
      public IResult<T> Or(T other) => other.Success();

      [DebuggerStepThrough]
      public IResult<T> Or(Func<T> other) => tryTo(other);

      public IResult<Unit> Unit => failure<Unit>(Exception);

      public IResult<T> Always(Action action)
      {
         tryTo(action);
         return this;
      }

      public IMatched<T> Match() => failedMatch<T>(Exception);

      public bool If(out T value)
      {
         value = default;
         return false;
      }

      public bool IfNot(out Exception exception)
      {
         exception = Exception;
         return true;
      }

      public bool IfNot(out T value, out Exception exception)
      {
         value = default;
         exception = Exception;

         return true;
      }

      public void Force() => throw Exception;

      public T ForceValue() => throw Exception;

      public IResult<T> OnSuccess(Action<T> action) => this;

      public IResult<T> OnFailure(Action<Exception> action)
      {
         try
         {
            action(Exception);
         }
         catch { }

         return this;
      }

	   public void Deconstruct(out IMaybe<T> value, out Exception exception)
	   {
		   value = none<T>();
		   exception = Exception;
	   }

	   public IResult<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public IMaybe<T> Maybe() => none<T>();
   }
}