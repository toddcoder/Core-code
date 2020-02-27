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
      public static implicit operator bool(ResultAssertion<T> assertion) => assertion.BeEquivalentToTrue();

      public static bool operator &(ResultAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(ResultAssertion<T> x, ICanBeTrue y) => or(x, y);

      protected IResult<T> result;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public ResultAssertion(IResult<T> result)
      {
         this.result = result;
         constraints = new List<Constraint>();
         not = false;
         name = "Result";
      }

      public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

      public T Value => result.ForceValue();

      public IEnumerable<Constraint> Constraints => constraints;

      protected ResultAssertion<T> add(object obj, Func<T, bool> constraintFunction, string message)
      {
         switch (obj)
         {
            case null:
               constraints.Add(Constraint.Failing("$name must be non-null", name));
               break;
            case T otherT:
               constraints.Add(new Constraint(() => constraintFunction(otherT), message, not, name, resultImage(result)));
               break;
            case IResult<T> anyValue:
               if (anyValue.If(out var value, out var exception))
               {
                  constraints.Add(new Constraint(() => constraintFunction(value), message, not, name, resultImage(result)));
               }
               else
               {
                  constraints.Add(Constraint.Failing(exception.Message, name));
               }

               break;
            default:
               constraints.Add(Constraint.Failing($"$name must be of type {typeof(T)}", name));
               break;
         }

         not = false;
         return this;
      }

      protected ResultAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name, resultImage(result)));
         not = false;

         return this;
      }

      public ResultAssertion<T> BeSuccessful() => add(() => result.IsSuccessful, "$name must $not be successful");

      public ResultAssertion<T> BeFailed() => add(() => result.IsFailed, "$name be $not be failed");

      public ResultAssertion<T> EqualToValueOf(IResult<T> otherResult)
      {
         return add(() => result.EqualToValueOf(otherResult), $"Value of $name must $not equal to value of {resultImage(otherResult)}");
      }

      public ResultAssertion<T> ValueEqualTo(T otherValue)
      {
         return add(() => result.ValueEqualTo(otherValue), $"Value of $name must $not equal to {otherValue}");
      }

      public IAssertion<T> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, T>(this, args);

      public T Force() => force(this);

      public T Force(string message) => force(this, message);

      public T Force(Func<string> messageFunc) => force(this, messageFunc);

      public T Force<TException>(params object[] args) where TException : Exception => force<TException, T>(this, args);

      public TResult Force<TResult>() => forceConvert<T, TResult>(this);

      public TResult Force<TResult>(string message) => forceConvert<T, TResult>(this, message);

      public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<T, TResult>(this, messageFunc);

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception
      {
         return forceConvert<T, TimeoutException, TResult>(this);
      }

      public IResult<T> OrFailure() => orFailure(this);

      public IResult<T> OrFailure(string message) => orFailure(this, message);

      public IResult<T> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public IMaybe<T> OrNone() => orNone(this);

      public async Task<ICompletion<T>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<T>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<T>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}