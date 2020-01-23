using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Comparables
{
   public class EnumAssertion : IAssertion<Enum>
   {
      protected Enum value;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public EnumAssertion(Enum value)
      {
         this.value = value;
         constraints = new List<Constraint>();
         not = false;
         name = "Enum";
      }

      public EnumAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected EnumAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name, value));
         not = false;

         return this;
      }

      public EnumAssertion BeOfType<TEnum>(TEnum otherEnum) where TEnum : Enum
      {
         return add(() => value.GetType() == otherEnum.GetType(), $"$name must $not be same type as {otherEnum}");
      }

      public EnumAssertion Equal(Enum otherValue)
      {
         return add(() => value.Equals(otherValue), $"$name must $not equal {otherValue}");
      }

      public EnumAssertion BeGreaterThan(Enum otherValue)
      {
         return add(() => value.CompareTo(otherValue) > 0, $"$name must $not be > {otherValue}");
      }

      public EnumAssertion BeGreaterThanOrEqual(Enum otherValue)
      {
         return add(() => value.CompareTo(otherValue) >= 0, $"$name must $not be >= {otherValue}");
      }

      public EnumAssertion BeLessThan(Enum otherValue)
      {
         return add(() => value.CompareTo(otherValue) < 0, $"$name must $not be < {otherValue}");
      }

      public EnumAssertion BeLessThanOrEqual(Enum otherValue)
      {
         return add(() => value.CompareTo(otherValue) <= 0, $"$name must $not be <= {otherValue}");
      }

      public EnumAssertion EqualInteger(int intValue)
      {
         return add(() => Enum.IsDefined(value.GetType(), intValue), $"$name must $not == {intValue}");
      }

      public bool BeTrue() => beTrue(this);

      public Enum Value => value;

      public IEnumerable<Constraint> Constraints => constraints;

      public IAssertion<Enum> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, Enum>(this, args);

      public Enum Force() => force(this);

      public Enum Force(string message) => force(this, message);

      public Enum Force(Func<string> messageFunc) => force(this, messageFunc);

      public Enum Force<TException>(params object[] args) where TException : Exception => force<TException, Enum>(this, args);

      public TResult Force<TResult>() => forceConvert<Enum, TResult>(this);

      public TResult Force<TResult>(string message) => forceConvert<Enum, TResult>(this, message);

      public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<Enum, TResult>(this, messageFunc);

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception
      {
         return forceConvert<Enum, TException, TResult>(this);
      }

      public IResult<Enum> OrFailure() => orFailure(this);

      public IResult<Enum> OrFailure(string message) => orFailure(this, message);

      public IResult<Enum> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public IMaybe<Enum> OrNone() => orNone(this);

      public async Task<ICompletion<Enum>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<Enum>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<Enum>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}