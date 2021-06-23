using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Enums;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Comparables
{
   public class EnumAssertion<TEnum> : IAssertion<TEnum> where TEnum : struct, Enum
   {
      protected TEnum value;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public EnumAssertion(TEnum value)
      {
         this.value = value;
         constraints = new List<Constraint>();
         not = false;
         name = "Enum";
      }

      public EnumAssertion<TEnum> Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected EnumAssertion<TEnum> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name, value));
         not = false;

         return this;
      }

      public EnumAssertion<TEnum> BeOfType<TOtherEnum>(TOtherEnum otherEnum) where TOtherEnum : struct, Enum
      {
         return add(() => value.GetType() == otherEnum.GetType(), $"$name must $not be same type as {otherEnum}");
      }

      public EnumAssertion<TEnum> Equal(TEnum otherValue)
      {
         return add(() => value.Equals(otherValue), $"$name must $not equal {otherValue}");
      }

      public EnumAssertion<TEnum> BeGreaterThan(TEnum otherValue)
      {
         return add(() => value.CompareTo(otherValue) > 0, $"$name must $not be > {otherValue}");
      }

      public EnumAssertion<TEnum> BeGreaterThanOrEqual(TEnum otherValue)
      {
         return add(() => value.CompareTo(otherValue) >= 0, $"$name must $not be >= {otherValue}");
      }

      public EnumAssertion<TEnum> BeLessThan(TEnum otherValue)
      {
         return add(() => value.CompareTo(otherValue) < 0, $"$name must $not be < {otherValue}");
      }

      public EnumAssertion<TEnum> BeLessThanOrEqual(Enum otherValue)
      {
         return add(() => value.CompareTo(otherValue) <= 0, $"$name must $not be <= {otherValue}");
      }

      public EnumAssertion<TEnum> EqualInteger(int intValue)
      {
         return add(() => Enum.IsDefined(value.GetType(), intValue), $"$name must $not == {intValue}");
      }

      public EnumAssertion<TEnum> HaveAnyOf(params TEnum[] args)
      {
         return add(() => value.Any(args), $"$name must $not have any of {args.ToString(" | ")}");
      }

      public EnumAssertion<TEnum> HaveAllOf(params TEnum[] args)
      {
         return add(() => value.All(args), $"$name must $not have all of {args.ToString(" | ")}");
      }

      public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

      public TEnum Value => value;

      public IEnumerable<Constraint> Constraints => constraints;

      public IAssertion<TEnum> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, TEnum>(this, args);

      public TEnum Force() => force(this);

      public TEnum Force(string message) => force(this, message);

      public TEnum Force(Func<string> messageFunc) => force(this, messageFunc);

      public TEnum Force<TException>(params object[] args) where TException : Exception => force<TException, TEnum>(this, args);

      public TResult Force<TResult>() => forceConvert<TEnum, TResult>(this);

      public TResult Force<TResult>(string message) => forceConvert<TEnum, TResult>(this, message);

      public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<TEnum, TResult>(this, messageFunc);

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception
      {
         return forceConvert<TEnum, TException, TResult>(this);
      }

      public Result<TEnum> OrFailure() => orFailure(this);

      public Result<TEnum> OrFailure(string message) => orFailure(this, message);

      public Result<TEnum> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public Maybe<TEnum> OrNone() => orNone(this);

      public async Task<ICompletion<TEnum>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<TEnum>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<TEnum>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}