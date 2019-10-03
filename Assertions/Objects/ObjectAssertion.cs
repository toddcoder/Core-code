using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Objects
{
   public class ObjectAssertion : IAssertion<object>
   {
      public static implicit operator bool(ObjectAssertion assertion) => assertion.BeTrue();

      public static bool operator &(ObjectAssertion x, ICanBeTrue y) => and(x, y);

      public static bool operator |(ObjectAssertion x, ICanBeTrue y) => or(x, y);


      protected object obj;
      protected List<Constraint> constraints;
      protected bool not;

      public ObjectAssertion(object obj)
      {
         this.obj = obj;
         constraints = new List<Constraint>();
         not = false;
      }

      public ObjectAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected ObjectAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public ObjectAssertion Equal(object other)
      {
         return add(() => obj.Equals(other), $"{obj} must $not equal {other}");
      }

      public ObjectAssertion BeNull()
      {
         return add(() => obj == null, "This value must $not be null");
      }

      public ObjectAssertion BeOfType(Type type)
      {
         return add(() => obj.GetType() == type, $"{obj} must $not be of type {type}");
      }

      public object Value => obj;

      public IEnumerable<Constraint> Constraints => constraints;

      public bool BeTrue() => beTrue(this);

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, object>(this, args);

      public object Ensure() => ensure(this);

      public object Ensure(string message) => ensure(this, message);

      public object Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public object Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, object>(this, args);

      public T Ensure<T>() => (T)Ensure();

      public TResult Ensure<TResult>(string message) => (TResult)Ensure(message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => (TResult)Ensure(messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return (TResult)Ensure<TException>(args);
      }

      public IResult<object> Try() => @try(this);

      public IResult<object> Try(string message) => @try(this, message);

      public IResult<object> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public async Task<ICompletion<object>> TryAsync(CancellationToken token) => await tryAsync(assertion: this, token);

      public async Task<ICompletion<object>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<object>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}