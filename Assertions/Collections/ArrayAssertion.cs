using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Collections;

public class ArrayAssertion<T> : IAssertion<T[]>
{
   public static implicit operator bool(ArrayAssertion<T> assertion) => assertion.BeEquivalentToTrue();

   public static bool operator &(ArrayAssertion<T> x, ICanBeTrue y) => and(x, y);

   public static bool operator |(ArrayAssertion<T> x, ICanBeTrue y) => or(x, y);

   protected T[] array;
   protected List<Constraint> constraints;
   protected bool not;
   protected string name;
   protected string image;

   public ArrayAssertion(T[] array)
   {
      this.array = array;
      constraints = new List<Constraint>();
      not = false;
      name = "Array";
      image = enumerableImage(array);
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

   protected ArrayAssertion<T> add(Func<bool> constraintFunction, string message)
   {
      constraints.Add(Constraint.Formatted(constraintFunction, message, not, name, Value, _ => image));
      not = false;

      return this;
   }

   public ArrayAssertion<T> Equal(T[] otherArray)
   {
      return add(() => array.Equals(otherArray), $"$name must $not equal {enumerableImage(otherArray)}");
   }

   public ArrayAssertion<T> BeNull()
   {
      return add(() => array == null, "$name must $not be null");
   }

   public ArrayAssertion<T> BeEmpty()
   {
      return add(() => array.Length == 0, "$name must $not be empty");
   }

   public ArrayAssertion<T> BeNullOrEmpty()
   {
      return add(() => array == null || array.Length == 0, "$name must $not be null or empty");
   }

   public ArrayAssertion<T> HaveIndexOf(int index)
   {
      return add(() => index > 0 && index < array.Length, $"$name must $not have an index of {index}");
   }

   public ArrayAssertion<T> HaveLengthOf(int minimumLength)
   {
      return add(() => array.Length >= minimumLength, $"$name must $not have a length of at least {minimumLength}");
   }

   public ArrayAssertion<T> HaveLengthOfExactly(int length)
   {
      return add(() => array.Length == length, $"$name must $not have a length of exactly {length}");
   }

   public T[] Value => array;

   public IEnumerable<Constraint> Constraints => constraints;

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

   public IAssertion<T[]> Named(string name)
   {
      this.name = name;
      return this;
   }

   public void OrThrow() => orThrow(this);

   public void OrThrow(string message) => orThrow(this, message);

   public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

   public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, T[]>(this, args);

   public T[] Force() => force(this);

   public T[] Force(string message) => force(this, message);

   public T[] Force(Func<string> messageFunc) => force(this, messageFunc);

   public T[] Force<TException>(params object[] args) where TException : Exception => force<TException, T[]>(this, args);

   public TResult Force<TResult>() => forceConvert<T[], TResult>(this);

   public TResult Force<TResult>(string message) => forceConvert<T[], TResult>(this, message);

   public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<T[], TResult>(this, messageFunc);

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception
   {
      return forceConvert<T[], TException, TResult>(this, args);
   }

   public Result<T[]> OrFailure() => orFailure(this);

   public Result<T[]> OrFailure(string message) => orFailure(this, message);

   public Result<T[]> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Maybe<T[]> OrNone() => orNone(this);

   public async Task<Completion<T[]>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

   public async Task<Completion<T[]>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

   public async Task<Completion<T[]>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}