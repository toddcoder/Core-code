using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Assertions.Collections;

public class TypedAssertion<T> : IAssertion<T>
{
   protected T obj;
   protected List<Constraint> constraints;
   protected bool not;
   protected string name;

   public TypedAssertion(T obj)
   {
      this.obj = obj;
      constraints = new List<Constraint>();
      not = false;
      name = "Typed value";
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
      constraints.Add(new Constraint(constraintFunction, message, not, name, Value));
      not = false;

      return this;
   }

   public TypedAssertion<T> Equal(T other)
   {
      return add(() => obj.Equals(other), $"$name must $not equal {other}");
   }

   public TypedAssertion<T> BeNull()
   {
      return add(() => obj == null, "$name must $not be null");
   }

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

   public T Value => obj;

   public IEnumerable<Constraint> Constraints => constraints;

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

   public TResult Force<TResult>() => throw fail("Not implemented");

   public TResult Force<TResult>(string message) => throw fail("Not implemented");

   public TResult Force<TResult>(Func<string> messageFunc) => throw fail("Not implemented");

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception => throw fail("Not implemented");

   public Optional<T> OrFailure() => orFailure(this);

   public Optional<T> OrFailure(string message) => orFailure(this, message);

   public Optional<T> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Optional<T> OrNone() => orNone(this);

   public async Task<Completion<T>> OrFailureAsync(CancellationToken token) => await orFailureAsync(assertion: this, token);

   public async Task<Completion<T>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

   public async Task<Completion<T>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}