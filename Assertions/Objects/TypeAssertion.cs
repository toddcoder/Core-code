﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Objects
{
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

      protected TypeAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name));
         not = false;

         return this;
      }

      public TypeAssertion Equal(Type otherType)
      {
         return add(() => type == otherType, $"$name must $not equal {otherType}");
      }

      public TypeAssertion EqualToTypeOf(object obj)
      {
         return add(() => type == obj.GetType(), $"$name must $not equal {obj.GetType()}");
      }

      public TypeAssertion BeNull()
      {
         return add(() => type == null, "$name must $not be null");
      }

      public TypeAssertion BeAssignableFrom(Type otherType)
      {
         return add(() => type.IsAssignableFrom(otherType), $"$name must $not be assignable from {otherType}");
      }

      public TypeAssertion BeAssignableTo(Type otherType)
      {
         return add(() => otherType.IsAssignableFrom(type), $"$name must $not be assignable to {otherType}");
      }

      public TypeAssertion BeConvertibleFrom(Type otherType)
      {
         return add(() => TypeDescriptor.GetConverter(type).CanConvertFrom(otherType), $"$name must $not be convertible from {otherType}");
      }

      public TypeAssertion BeConvertibleTo(Type otherType)
      {
         return add(() => TypeDescriptor.GetConverter(type).CanConvertTo(otherType), $"$name must $not be convertible to {otherType}");
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
         var message = $"$name must $not contain generic argument {otherType}";
         return add(() => type.IsGenericType && type.GetGenericArguments().Contains(otherType), message);
      }

      public TypeAssertion BeConstructedGeneric()
      {
         return add(() => type.IsConstructedGenericType, "$name must $not be a constructed generic");
      }

      public bool BeTrue() => beTrue(this);

      public Type Value => type;

      public IEnumerable<Constraint> Constraints => constraints;

      public IAssertion<Type> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, Type>(this, args);

      public Type Ensure() => ensure(this);

      public Type Ensure(string message) => ensure(this, message);

      public Type Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public Type Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, Type>(this, args);

      public TResult Ensure<TResult>() => ensureConvert<Type, TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<Type, TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<Type, TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<Type, TException, TResult>(this, args);
      }

      public IResult<Type> Try() => @try(this);

      public IResult<Type> Try(string message) => @try(this, message);

      public IResult<Type> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public IMaybe<Type> Maybe() => maybe(this);

      public async Task<ICompletion<Type>> TryAsync(CancellationToken token) => await tryAsync(assertion: this, token);

      public async Task<ICompletion<Type>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<Type>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}