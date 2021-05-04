using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Exceptions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Success<T> : IResult<T>, IEquatable<Success<T>>
   {
      public static implicit operator bool(Success<T> _) => true;

      protected T value;

      internal Success(T value) => this.value = value;

      public bool If(out T value, out Exception exception)
      {
         value = this.value;
         exception = null;

         return true;
      }

      public bool ValueOrOriginal(out T value, out IResult<T> original)
      {
         value = this.value;
         original = this;

         return true;
      }

      public bool ValueOrCast<TResult>(out T value, out IResult<TResult> result)
      {
         value = this.value;
         result = "Do not use this".Failure<TResult>();

         return true;
      }

      public bool IsSuccessful => true;

      public bool IsFailed => false;

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
      public IResult<T2> SelectMany<T1, T2>(Func<T, IResult<T1>> func, Func<T, T1, T2> projection) => func(value).Map(t1 => projection(value, t1));

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

      public void Force()
      {
      }

      public T ForceValue() => value;

      public IResult<T> OnSuccess(Action<T> action)
      {
         try
         {
            action(value);
         }
         catch
         {
         }

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
         return predicate(value) ? this : exceptionMessage().Failure<T>();
      }

      public IMaybe<T> Maybe() => value.Some();

      public bool EqualToValueOf(IResult<T> otherResult) => otherResult.If(out var otherValue) && ValueEqualTo(otherValue);

      public bool ValueEqualTo(T otherValue) => value.Equals(otherValue);

      public IResult<T> Otherwise(Func<Exception, T> func) => this;

      public IResult<T> Otherwise(Func<Exception, IResult<T>> func) => this;

      public IResult<TResult> CastAs<TResult>()
      {
         if (value is TResult result)
         {
            return result.Success();
         }
         else
         {
            return $"Invalid cast from {typeof(T).Name} to {typeof(TResult).Name}".Failure<TResult>();
         }
      }

      public IResult<T> Where(Predicate<T> predicate, string exceptionMessage) => predicate(value) ? this : exceptionMessage.Failure<T>();

      public IResult<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => predicate(value) ? this : exceptionMessage().Failure<T>();

      public IResult<T> ExceptionMessage(string message) => this;

      public IResult<T> ExceptionMessage(Func<Exception, string> message) => this;

      public bool Equals(Success<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(value, other.value);
      }

      public override bool Equals(object obj) => obj is Success<T> other && Equals(other);

      public override int GetHashCode() => value.GetHashCode();

      public override string ToString() => $"Success({value})";
   }
}