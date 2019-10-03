using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Comparables
{
   public class BooleanAssertion : IAssertion<bool>
   {
      public static implicit operator bool(BooleanAssertion assertion) => assertion.BeTrue();

      public static bool operator &(BooleanAssertion x, ICanBeTrue y) => and(x, y);

      public static bool operator |(BooleanAssertion x, ICanBeTrue y) => or(x, y);

      protected bool boolean;
      protected List<Constraint> constraints;
      protected bool not;

      public BooleanAssertion(bool boolean)
      {
         this.boolean = boolean;
         constraints = new List<Constraint>();
         not = false;
      }

      public bool Boolean => boolean;

      public BooleanAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected BooleanAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public BooleanAssertion Be()
      {
         return add(() => boolean, $"{boolean} must $not be true");
      }

      public BooleanAssertion And(bool test)
      {
         return add(() => boolean && test, $"{boolean} and $not {test}");
      }

      public BooleanAssertion Or(bool test)
      {
         return add(() => boolean || test, $"{boolean} or $not {test}");
      }

      public bool Value => boolean;

      public IEnumerable<Constraint> Constraints => constraints;

      public bool BeTrue() => beTrue(this);

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, bool>(this, args);

      public bool Ensure() => ensure(this);

      public bool Ensure(string message) => ensure(this, message);

      public bool Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public bool Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, bool>(this, args);

      public TResult Ensure<TResult>() => ensureConvert<bool, TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<bool, TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<bool, TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<bool, TException, TResult>(this, args);
      }

      public IResult<bool> Try() => @try(this);

      public IResult<bool> Try(string message) => @try(this, message);

      public IResult<bool> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public async Task<ICompletion<bool>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<bool>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<bool>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}