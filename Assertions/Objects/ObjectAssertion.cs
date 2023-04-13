using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Objects;

public class ObjectAssertion : IAssertion<object>
{
   public static implicit operator bool(ObjectAssertion assertion) => assertion.BeEquivalentToTrue();

   public static bool operator &(ObjectAssertion x, ICanBeTrue y) => and(x, y);

   public static bool operator |(ObjectAssertion x, ICanBeTrue y) => or(x, y);

   protected object obj;
   protected List<Constraint> constraints;
   protected bool not;
   protected string name;

   public ObjectAssertion(object obj)
   {
      this.obj = obj;
      constraints = new List<Constraint>();
      not = false;
      name = "Object";
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
      constraints.Add(new Constraint(constraintFunction, message, not, name, Value));
      not = false;

      return this;
   }

   public ObjectAssertion Equal(object other)
   {
      return add(() => obj.Equals(other), $"$name must $not equal {other}");
   }

   public ObjectAssertion BeNull()
   {
      return add(() => obj == null, "$name must $not be null");
   }

   public ObjectAssertion BeOfType(Type type)
   {
      return add(() => obj.GetType() == type, $"$name must $not be of type {type}");
   }

   public object Value => obj;

   public IEnumerable<Constraint> Constraints => constraints;

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

   public IAssertion<object> Named(string name)
   {
      this.name = name;
      return this;
   }

   public void OrThrow() => orThrow(this);

   public void OrThrow(string message) => orThrow(this, message);

   public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

   public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, object>(this, args);

   public object Force() => force(this);

   public object Force(string message) => force(this, message);

   public object Force(Func<string> messageFunc) => force(this, messageFunc);

   public object Force<TException>(params object[] args) where TException : Exception => force<TException, object>(this, args);

   public T Force<T>() => (T)Force();

   public TResult Force<TResult>(string message) => (TResult)Force(message);

   public TResult Force<TResult>(Func<string> messageFunc) => (TResult)Force(messageFunc);

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception
   {
      return (TResult)Force<TException>(args);
   }

   public Result<object> OrFailure() => orFailure(this);

   public Result<object> OrFailure(string message) => orFailure(this, message);

   public Result<object> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Maybe<object> OrNone() => orNone(this);

   public Optional<object> OrEmpty() => orEmpty(this);

   public Optional<object> OrFailed() => orFailed(this);

   public Optional<object> OrFailed(string message) => orFailed(this, message);

   public Optional<object> OrFailed(Func<string> messageFunc) => orFailed(this, messageFunc);

   public async Task<Completion<object>> OrFailureAsync(CancellationToken token) => await orFailureAsync(assertion: this, token);

   public async Task<Completion<object>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

   public async Task<Completion<object>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}