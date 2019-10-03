using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Monads
{
   public class ResultAssertion<T> : IAssertion<T>
   {
      public static implicit operator bool(ResultAssertion<T> assertion) => assertion.BeTrue();

      public static bool operator &(ResultAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(ResultAssertion<T> x, ICanBeTrue y) => or(x, y);

      protected IResult<T> result;
      protected List<Constraint> constraints;
      protected bool not;

      public ResultAssertion(IResult<T> result)
      {
         this.result = result;
         constraints = new List<Constraint>();
         not = false;
      }

      public bool BeTrue() => beTrue(this);

      public T Value => result.ForceValue();

      public IEnumerable<Constraint> Constraints => constraints;

      protected ResultAssertion<T> add(object obj, Func<T, bool> constraintFunction, string message)
      {
         switch (obj)
         {
            case null:
               constraints.Add(Constraint.Failing("RHS must be non-null"));
               break;
            case T otherT:
               constraints.Add(new Constraint(() => constraintFunction(otherT), message, not));
               break;
            case IResult<T> anyValue:
               if (anyValue.If(out var value, out var exception))
               {
                  constraints.Add(new Constraint(() => constraintFunction(value), message, not));
               }
               else
               {
                  constraints.Add(Constraint.Failing(exception.Message));
               }

               break;
            default:
               constraints.Add(Constraint.Failing($"{obj} must be of type {typeof(T)}"));
               break;
         }

         not = false;
         return this;
      }

      protected ResultAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public ResultAssertion<T> BeSuccessful() => add(() => result.IsSuccessful, "Result must $not be successful");

      public ResultAssertion<T> BeFailed() => add(() => result.IsFailed, "Result be $not be failed");

      public ResultAssertion<T> EqualToValueOf(IResult<T> otherResult)
      {
         return add(() => result.EqualToValueOf(otherResult), $"Value of result must $not equal to value of {otherResult}");
      }

      public ResultAssertion<T> ValueEqualTo(T otherValue)
      {
         return add(() => result.ValueEqualTo(otherValue), $"Value of result must $not equal to {otherValue}");
      }

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, T>(this, args);

      public T Ensure() => ensure(this);

      public T Ensure(string message) => ensure(this, message);

      public T Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public T Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, T>(this, args);

      public TResult Ensure<TResult>() => ensureConvert<T, TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<T, TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<T, TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<T, TimeoutException, TResult>(this);
      }

      public IResult<T> Try() => @try(this);

      public IResult<T> Try(string message) => @try(this, message);

      public IResult<T> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public async Task<ICompletion<T>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<T>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<T>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}