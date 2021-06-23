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
      public static implicit operator bool(CompletionAssertion<T> assertion) => assertion.BeEquivalentToTrue();

      public static bool operator &(CompletionAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(CompletionAssertion<T> x, ICanBeTrue y) => or(x, y);

      protected ICompletion<T> completion;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public CompletionAssertion(ICompletion<T> completion)
      {
         this.completion = completion;
         constraints = new List<Constraint>();
         not = false;
         name = "Async result";
      }

      public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

      public T Value => completion.ForceValue();

      public IEnumerable<Constraint> Constraints => constraints;

      protected CompletionAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name, completionImage(completion)));
         not = false;

         return this;
      }

      public CompletionAssertion<T> BeCompleted() => add(() => completion.IsCompleted(out _), "$name must be $not completed");

      public CompletionAssertion<T> BeCancelled() => add(() => completion.IfCancelled(), "$name must be $not cancelled");

      public CompletionAssertion<T> BeInterrupted() => add(() => completion.IfInterrupted(out _), "$name must be $not interrupted");

      public CompletionAssertion<T> ValueEqualTo(ICompletion<T> otherCompletion)
      {
         return add(() => completion.ValueEqualTo(otherCompletion), $"Value of $name must $not equal value of {completionImage(otherCompletion)}");
      }

      public CompletionAssertion<T> EqualToValueOf(T otherValue)
      {
         return add(() => completion.EqualToValueOf(otherValue), $"Value of $name must $not equal {otherValue}");
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

      public Result<T> OrFailure() => orFailure(this);

      public Result<T> OrFailure(string message) => orFailure(this, message);

      public Result<T> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public Maybe<T> OrNone() => orNone(this);

      public async Task<ICompletion<T>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<T>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<T>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}