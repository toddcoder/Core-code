using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Collections
{
   public class TypedAssertion<T> : IAssertion<T>
   {
      protected T obj;
      protected List<Constraint> constraints;
      protected bool not;

      public TypedAssertion(T obj)
      {
         this.obj = obj;
         constraints = new List<Constraint>();
         not = false;
      }

      public TypedAssertion<T> Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected TypedAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public TypedAssertion<T> Equal(T other)
      {
         return add(() => obj.Equals(other), $"{obj} must $not equal {other}");
      }

      public TypedAssertion<T> BeNull()
      {
         return add(() => obj == null, "This value must $not be null");
      }

      public bool BeTrue() => beTrue(this);

      public T Value => obj;

      public IEnumerable<Constraint> Constraints => constraints;

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, T>(this, args);

      public T Ensure() => ensure(this);

      public T Ensure(string message) => ensure(this, message);

      public T Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public T Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, T>(this, args);

      public TResult Ensure<TResult>() => throw "Not implemented".Throws();

      public TResult Ensure<TResult>(string message) => throw "Not implemented".Throws();

      public TResult Ensure<TResult>(Func<string> messageFunc) => throw "Not implemented".Throws();

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception => throw "Not implemented".Throws();

      public IResult<T> Try() => @try(this);

      public IResult<T> Try(string message) => @try(this, message);

      public IResult<T> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public IMaybe<T> Maybe() => maybe(this);

      public async Task<ICompletion<T>> TryAsync(CancellationToken token) => await tryAsync(assertion: this, token);

      public async Task<ICompletion<T>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<T>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}