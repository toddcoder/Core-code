using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Monads
{
   public class MaybeAssertion<T> : IAssertion<T>
   {
      public static implicit operator bool(MaybeAssertion<T> assertion) => assertion.BeTrue();

      public static bool operator &(MaybeAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(MaybeAssertion<T> x, ICanBeTrue y) => or(x, y);

      protected IMaybe<T> maybe;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public MaybeAssertion(IMaybe<T> maybe)
      {
         this.maybe = maybe;
         constraints = new List<Constraint>();
         not = false;
         name = "Optional";
      }

      public bool BeTrue() => beTrue(this);

      public T Value => maybe.Required($"{name} value not available");

      public IEnumerable<Constraint> Constraints => constraints;

      public MaybeAssertion<T> Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected MaybeAssertion<T> add(object obj, Func<T, bool> constraintFunction, string message)
      {
         switch (obj)
         {
            case null:
               constraints.Add(Constraint.Failing("$name must be non-null", name));
               break;
            case T otherT:
               constraints.Add(new Constraint(() => constraintFunction(otherT), message, not, name, maybeImage(maybe)));
               break;
            case IMaybe<T> anyValue:
               if (anyValue.If(out var value))
               {
                  constraints.Add(new Constraint(() => constraintFunction(value), message, not, name, maybeImage(maybe)));
               }
               else
               {
                  constraints.Add(Constraint.Failing("$name not available", name));
               }

               break;
            default:
               constraints.Add(Constraint.Failing($"$name must be of type {typeof(T)}", name));
               break;
         }

         not = false;
         return this;
      }

      protected MaybeAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name, maybeImage(maybe)));
         not = false;

         return this;
      }

      public MaybeAssertion<T> HaveValue() => add(() => maybe.IsSome, "$name must $not have a value");

      public MaybeAssertion<T> EqualToValueOf(IMaybe<T> otherMaybe)
      {
         return add(() => maybe.EqualToValueOf(otherMaybe), $"Value of $name must $not equal to value of {maybeImage(otherMaybe)}");
      }

      public MaybeAssertion<T> ValueEqualTo(T otherValue)
      {
         return add(() => maybe.ValueEqualTo(otherValue), $"Value of $name must $not equal to {otherValue}");
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