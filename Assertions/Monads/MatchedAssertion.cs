using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Monads
{
   public class MatchedAssertion<T> : IAssertion<T>
   {
      public static implicit operator bool(MatchedAssertion<T> assertion) => assertion.BeEquivalentToTrue();

      public static bool operator &(MatchedAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(MatchedAssertion<T> x, ICanBeTrue y) => or(x, y);

      protected Matched<T> matched;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public MatchedAssertion(Matched<T> matched)
      {
         this.matched = matched;
         constraints = new List<Constraint>();
         not = false;
         name = "Match";
      }

      public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

      public T Value => matched.ForceValue();

      public IEnumerable<Constraint> Constraints => constraints;

      protected MatchedAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name, matchedImage(matched)));
         not = false;

         return this;
      }

      public MatchedAssertion<T> BeMatched() => add(() => matched.IsMatched, "$name must be $not matched");

      public MatchedAssertion<T> BeUnmatched() => add(() => matched.IsNotMatched, "$name must be $not unmatched");

      public MatchedAssertion<T> BeFailedMatch() => add(() => matched.IsFailedMatch, "$name must be $not a failed match");

      public MatchedAssertion<T> EqualToValueOf(Matched<T> otherMatched)
      {
         return add(() => matched.EqualToValueOf(otherMatched), $"Value of $name must $not equal value of {matchedImage(otherMatched)}");
      }

      public MatchedAssertion<T> ValueEqualTo(T otherValue)
      {
         return add(() => matched.ValueEqualTo(otherValue), $"Value of $name must $not equal {otherValue}");
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

      public async Task<Completion<T>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<Completion<T>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<Completion<T>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}