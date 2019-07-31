using System;
using System.Diagnostics;
using Core.Exceptions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Success<T> : IResult<T>
   {
	   protected T value;

      internal Success(T value) => this.value = value;

      public bool If(out T value, out Exception exception)
      {
         value = this.value;
         exception = null;

         return true;
      }

      public bool Out(out T value, out IResult<T> original)
      {
         value = this.value;
         original = this;

         return true;
      }

      public bool IsSuccessful => true;

      public bool IsFailed => false;

      public Exception Exception => throw "There is no exception".Throws();

      public IResult<TOther> ExceptionAs<TOther>() => throw "There is no exception".Throws();

      [DebuggerStepThrough]
      public IResult<TResult> Map<TResult>(Func<T, IResult<TResult>> ifSuccessful)
      {
         try
         {
            return ifSuccessful(value);
         }
         catch (Exception exception)
         {
            return failure<TResult>(exception);
         }
      }

      [DebuggerStepThrough]
      public IResult<TResult> Map<TResult>(Func<T, TResult> ifSuccessful)
      {
         try
         {
            return ifSuccessful(value).Success();
         }
         catch (Exception exception)
         {
            return failure<TResult>(exception);
         }
      }

      [DebuggerStepThrough]
      public TResult FlatMap<TResult>(Func<T, TResult> ifSuccessful, Func<Exception, TResult> ifFailed)
      {
         return ifSuccessful(value);
      }

      [DebuggerStepThrough]
      public IResult<TResult> SelectMany<TResult>(Func<T, IResult<TResult>> projection)
      {
         try
         {
            return projection(value);
         }
         catch (Exception exception)
         {
            return failure<TResult>(exception);
         }
      }

      [DebuggerStepThrough]
      public IResult<T2> SelectMany<T1, T2>(Func<T, IResult<T1>> func, Func<T, T1, T2> projection)
      {
         var self = this;
         return func(value).FlatMap(t1 => projection(self.value, t1).Success(), failure<T2>);
      }

      [DebuggerStepThrough]
      public IResult<TResult> SelectMany<TResult>(Func<T, TResult> func)
      {
         try
         {
            return func(value).Success();
         }
         catch (Exception exception)
         {
            return failure<TResult>(exception);
         }
      }

      [DebuggerStepThrough]
      public T Recover(Func<Exception, T> recovery) => value;

      [DebuggerStepThrough]
      public IResult<T> Or(IResult<T> other) => this;

      [DebuggerStepThrough]
      public IResult<T> Or(Func<IResult<T>> other) => this;

      [DebuggerStepThrough]
      public IResult<T> Or(T other) => this;

      [DebuggerStepThrough]
      public IResult<T> Or(Func<T> other) => this;

      public IResult<Unit> Unit => Monads.Unit.Success();

      public IResult<T> Always(Action action)
      {
         tryTo(action);
         return this;
      }

      public IMatched<T> Match() => value.Matched();

      public bool If(out T value)
      {
         value = this.value;
         return true;
      }

      public bool IfNot(out Exception exception)
      {
         exception = null;
         return false;
      }

      public bool IfNot(out T value, out Exception exception)
      {
         value = this.value;
         exception = null;

         return false;
      }

      public void Force() { }

      public T ForceValue() => value;

      public IResult<T> OnSuccess(Action<T> action)
      {
         try
         {
            action(value);
         }
         catch { }

         return this;
      }

      public IResult<T> OnFailure(Action<Exception> action) => this;

	   public void Deconstruct(out IMaybe<T> value, out Exception exception)
	   {
		   value = this.value.Some();
		   exception = default;
	   }

	   public IResult<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage)
	   {
		   if (predicate(value))
         {
            return this;
         }
         else
         {
            return exceptionMessage().Failure<T>();
         }
      }

      public IMaybe<T> Maybe() => value.Some();
   }
}