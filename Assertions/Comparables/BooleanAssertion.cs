using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Comparables;

public class BooleanAssertion : IAssertion<bool>
{
   public static implicit operator bool(BooleanAssertion assertion) => assertion.BeEquivalentToTrue();

   public static bool operator &(BooleanAssertion x, ICanBeTrue y) => and(x, y);

   public static bool operator |(BooleanAssertion x, ICanBeTrue y) => or(x, y);

   protected bool boolean;
   protected List<Constraint> constraints;
   protected bool not;
   protected string name;

   public BooleanAssertion(bool boolean)
   {
      this.boolean = boolean;
      constraints = new List<Constraint>();
      not = false;
      name = "Boolean";
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
      constraints.Add(new Constraint(constraintFunction, message, not, name, Value));
      not = false;

      return this;
   }

   public BooleanAssertion BeTrue()
   {
      return add(() => boolean, "$name must $not be true");
   }

   public BooleanAssertion And(bool test)
   {
      return add(() => boolean && test, $"$name and $not {test}");
   }

   public BooleanAssertion Or(bool test)
   {
      return add(() => boolean || test, $"$name or $not {test}");
   }

   public bool Value => boolean;

   public IEnumerable<Constraint> Constraints => constraints;

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

   public IAssertion<bool> Named(string name)
   {
      this.name = name;
      return this;
   }

   public void OrThrow() => orThrow(this);

   public void OrThrow(string message) => orThrow(this, message);

   public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

   public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, bool>(this, args);

   public bool Force() => force(this);

   public bool Force(string message) => force(this, message);

   public bool Force(Func<string> messageFunc) => force(this, messageFunc);

   public bool Force<TException>(params object[] args) where TException : Exception => force<TException, bool>(this, args);

   public TResult Force<TResult>() => forceConvert<bool, TResult>(this);

   public TResult Force<TResult>(string message) => forceConvert<bool, TResult>(this, message);

   public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<bool, TResult>(this, messageFunc);

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception
   {
      return forceConvert<bool, TException, TResult>(this, args);
   }

   public Optional<bool> OrFailure() => orFailure(this);

   public Optional<bool> OrFailure(string message) => orFailure(this, message);

   public Optional<bool> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Optional<bool> OrNone() => orNone(this);

   public async Task<Completion<bool>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

   public async Task<Completion<bool>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

   public async Task<Completion<bool>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}