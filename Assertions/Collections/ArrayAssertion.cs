using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Collections
{
   public class ArrayAssertion<T> : IAssertion<T[]>
   {
      public static implicit operator bool(ArrayAssertion<T> assertion) => assertion.BeTrue();

      public static bool operator &(ArrayAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(ArrayAssertion<T> x, ICanBeTrue y) => or(x, y);

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

      public T[] Value => array;

      public IEnumerable<Constraint> Constraints => constraints;

      public bool BeTrue() => beTrue(this);

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, T[]>(this, args);

      public T[] Ensure() => ensure(this);

      public T[] Ensure(string message) => ensure(this, message);

      public T[] Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public T[] Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, T[]>(this, args);

      public TResult Ensure<TResult>() => ensureConvert<T[], TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<T[], TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<T[], TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<T[], TException, TResult>(this, args);
      }

      public IResult<T[]> Try() => @try(this);

      public IResult<T[]> Try(string message) => @try(this, message);

      public IResult<T[]> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public async Task<ICompletion<T[]>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<T[]>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<T[]>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}