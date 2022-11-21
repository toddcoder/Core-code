using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Objects;

public class TypeAssertion : IAssertion<Type>
{
   protected Type type;
   protected List<Constraint> constraints;
   protected bool not;
   protected string name;

   public TypeAssertion(Type type)
   {
      this.type = type;
      constraints = new List<Constraint>();
      not = false;
      name = "Type";
   }

   public TypeAssertion Not
   {
      get
      {
         not = true;
         return this;
      }
   }

   protected static string format(Type type) => type.FullName;

   protected TypeAssertion add(Func<bool> constraintFunction, string message)
   {
      constraints.Add(Constraint.Formatted(constraintFunction, message, not, name, Value, format));
      not = false;

      return this;
   }

   public TypeAssertion Equal(Type otherType)
   {
      return add(() => type == otherType, $"$name must $not equal {format(otherType)}");
   }

   public TypeAssertion EqualToTypeOf(object obj)
   {
      return add(() => type == obj.GetType(), $"$name must $not equal {format(obj.GetType())}");
   }

   public TypeAssertion BeNull()
   {
      return add(() => type == null, "$name must $not be null");
   }

   public TypeAssertion BeAssignableFrom(Type otherType)
   {
      return add(() => type.IsAssignableFrom(otherType), $"$name must $not be assignable from {format(otherType)}");
   }

   public TypeAssertion BeAssignableTo(Type otherType)
   {
      return add(() => otherType.IsAssignableFrom(type), $"$name must $not be assignable to {format(otherType)}");
   }

   public TypeAssertion BeConvertibleFrom(Type otherType)
   {
      return add(() => TypeDescriptor.GetConverter(type).CanConvertFrom(otherType), $"$name must $not be convertible from {format(otherType)}");
   }

   public TypeAssertion BeConvertibleTo(Type otherType)
   {
      return add(() => TypeDescriptor.GetConverter(type).CanConvertTo(otherType), $"$name must $not be convertible to {format(otherType)}");
   }

   public TypeAssertion BeClass()
   {
      return add(() => type.IsClass, "$name must $not be a class");
   }

   public TypeAssertion BeValue()
   {
      return add(() => type.IsValueType, "$name must $not be a value");
   }

   public TypeAssertion BeEnumeration()
   {
      return add(() => type.IsEnum, "$name must $not be an enumeration");
   }

   public TypeAssertion BeGeneric()
   {
      return add(() => type.IsGenericType, "$name must $not be a generic");
   }

   public TypeAssertion ContainGenericArgument(Type otherType)
   {
      var message = $"$name must $not contain generic argument {format(otherType)}";
      return add(() => type.IsGenericType && type.GetGenericArguments().Contains(otherType), message);
   }

   public TypeAssertion BeConstructedGeneric()
   {
      return add(() => type.IsConstructedGenericType, "$name must $not be a constructed generic");
   }

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

   public Type Value => type;

   public IEnumerable<Constraint> Constraints => constraints;

   public IAssertion<Type> Named(string name)
   {
      this.name = name;
      return this;
   }

   public void OrThrow() => orThrow(this);

   public void OrThrow(string message) => orThrow(this, message);

   public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

   public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, Type>(this, args);

   public Type Force() => force(this);

   public Type Force(string message) => force(this, message);

   public Type Force(Func<string> messageFunc) => force(this, messageFunc);

   public Type Force<TException>(params object[] args) where TException : Exception => force<TException, Type>(this, args);

   public TResult Force<TResult>() => forceConvert<Type, TResult>(this);

   public TResult Force<TResult>(string message) => forceConvert<Type, TResult>(this, message);

   public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<Type, TResult>(this, messageFunc);

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception
   {
      return forceConvert<Type, TException, TResult>(this, args);
   }

   public Result<Type> OrFailure() => orFailure(this);

   public Result<Type> OrFailure(string message) => orFailure(this, message);

   public Result<Type> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Maybe<Type> OrNone() => orNone(this);

   public async Task<Completion<Type>> OrFailureAsync(CancellationToken token) => await orFailureAsync(assertion: this, token);

   public async Task<Completion<Type>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

   public async Task<Completion<Type>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}