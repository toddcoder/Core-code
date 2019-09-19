using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Collections
{
   public class ArrayAssertion<T> : IAssertion<T[]>
   {
	   public static implicit operator bool(ArrayAssertion<T> assertion) => assertion.BeTrue();

      protected T[] array;
      protected List<Constraint> constraints;
      protected bool not;
      protected string image;

      public ArrayAssertion(T[] array)
      {
         this.array = array;
         constraints = new List<Constraint>();
         not = false;
         image = arrayImage(this.array);
      }

      public T[] Array => array;

      public ArrayAssertion<T> Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected ArrayAssertion<T> add(Array otherArray, Func<T[], bool> constraintFunction, string message)
      {
         switch (otherArray)
         {
            case null:
               constraints.Add(Constraint.Failing("RHS must be non-null"));
               break;
            case T[] tArray:
               constraints.Add(new Constraint(() => constraintFunction(tArray), message, not));
               break;
            default:
               constraints.Add(Constraint.Failing($"{otherArray}"));
               break;
         }

         not = false;
         return this;
      }

      protected ArrayAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      static string arrayImage(T[] array)
      {
         if (array == null)
         {
            return "(null)";
         }

         if (array.Length > 10)
         {
            return $"{{{array.Take(10).Stringify()}...}}";
         }
         else
         {
            return $"{{{array.Stringify()}}}";
         }
      }

      public ArrayAssertion<T> Equal(T[] otherArray)
      {
         return add(otherArray, a => array.Equals(a), $"{image} must $not equal {arrayImage(otherArray)}");
      }

      public ArrayAssertion<T> BeNull()
      {
         return add(() => array == null, "This array must $not be null");
      }

      public ArrayAssertion<T> BeEmpty()
      {
         return add(() => array.Length == 0, "This array must $not be empty");
      }

      public ArrayAssertion<T> BeNullOrEmpty()
      {
         return add(() => array == null || array.Length == 0, "This array must $not be null or empty");
      }

      public ArrayAssertion<T> HaveIndexOf(int index)
      {
         return add(() => index > 0 && index < array.Length, $"{image} must $not have an index of {index}");
      }

      public ArrayAssertion<T> HaveLengthOf(int minimumLength)
      {
         return add(() => array.Length >= minimumLength, $"{image} must $not have a length of at least {minimumLength}");
      }

      public bool BeTrue() => constraints.All(constraint => constraint.IsTrue());

      public void Assert()
      {
         if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            throw constraint.Message.Throws();
         }
      }

      public void Assert(string message)
      {
         if (constraints.Any(c => !c.IsTrue()))
         {
            throw message.Throws();
         }
      }

      public void Assert(Func<string> messageFunc)
      {
         if (constraints.Any(c => !c.IsTrue()))
         {
            throw messageFunc().Throws();
         }
      }

      public void Assert<TException>(params object[] args) where TException : Exception
      {
         if (constraints.Any(c => !c.IsTrue()))
         {
            throw getException<TException>(args);
         }
      }

      public T[] Ensure()
      {
         Assert();
         return array;
      }

      public T[] Ensure(string message)
      {
         Assert(message);
         return array;
      }

      public T[] Ensure(Func<string> messageFunc)
      {
         Assert(messageFunc);
         return array;
      }

      public T[] Ensure<TException>(params object[] args) where TException : Exception
      {
         Assert<TException>(args);
         return array;
      }

      public TResult Ensure<TResult>()
      {
         Assert();
         var converter = TypeDescriptor.GetConverter(typeof(T[]));
         return (TResult)converter.ConvertTo(array, typeof(TResult));
      }

      public TResult Ensure<TResult>(string message)
      {
         Assert(message);
         var converter = TypeDescriptor.GetConverter(typeof(T[]));
         return (TResult)converter.ConvertTo(array, typeof(TResult));
      }

      public TResult Ensure<TResult>(Func<string> messageFunc)
      {
         Assert(messageFunc);
         var converter = TypeDescriptor.GetConverter(typeof(T[]));
         return (TResult)converter.ConvertTo(array, typeof(TResult));
      }

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         Assert<TException>(args);
         var converter = TypeDescriptor.GetConverter(typeof(T[]));
         return (TResult)converter.ConvertTo(array, typeof(TResult));
      }

      public IResult<T[]> Try()
      {
         if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            return constraint.Message.Failure<T[]>();
         }
         else
         {
            return array.Success();
         }
      }

      public async Task<ICompletion<T[]>> TryAsync(CancellationToken token) => await AttemptFunctions.runAsync(t =>
      {
         if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            return constraint.Message.Interrupted<T[]>();
         }
         else
         {
            return array.Completed(t);
         }
      }, token);
   }
}