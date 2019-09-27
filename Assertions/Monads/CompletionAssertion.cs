using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Monads
{
   public class CompletionAssertion<T> : IAssertion<T>
   {
      public static implicit operator bool(CompletionAssertion<T> assertion) => assertion.BeTrue();

      public static bool operator &(CompletionAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(CompletionAssertion<T> x, ICanBeTrue y) => or(x, y);

      protected ICompletion<T> completion;
      protected List<Constraint> constraints;
      protected bool not;

      public CompletionAssertion(ICompletion<T> completion)
      {
         this.completion = completion;
         constraints = new List<Constraint>();
         not = false;
      }

      public bool BeTrue() => beTrue(this);

      public T Value => completion.ForceValue();

      public IEnumerable<Constraint> Constraints => constraints;

      protected CompletionAssertion<T> add(object obj, Func<T, bool> constraintFunction, string message)
      {
         switch (obj)
         {
            case null:
               constraints.Add(Constraint.Failing("RHS must be non-null"));
               break;
            case T otherT:
               constraints.Add(new Constraint(() => constraintFunction(otherT), message, not));
               break;
            case ICompletion<T> anyValue:
               if (anyValue.If(out var value, out var anyException))
               {
                  constraints.Add(new Constraint(() => constraintFunction(value), message, not));
               }
               else if (anyException.If(out var exception))
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

      protected CompletionAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public CompletionAssertion<T> BeCompleted() => add(() => completion.IsCompleted(out _), "Must be $not completed");

      public CompletionAssertion<T> BeCancelled() => add(() => completion.IfCancelled(), "Must be $not cancelled");

      public CompletionAssertion<T> BeInterrupted() => add(() => completion.IfInterrupted(out _), "Must be $not interrupted");

      public CompletionAssertion<T> Equal(object obj)
      {
         return add(obj, other => completion.If(out var value) && value.Equals(other), $"Completion must $not equal {obj}");
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