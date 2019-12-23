using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Collections
{
   public class ListAssertion<T> : IAssertion<List<T>>
   {
      public static implicit operator bool(ListAssertion<T> assertion) => assertion.BeTrue();

      public static bool operator &(ListAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(ListAssertion<T> x, ICanBeTrue y) => or(x, y);

      protected List<T> list;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public ListAssertion(List<T> list)
      {
         this.list = list;
         constraints = new List<Constraint>();
         not = false;
         name = "List";
      }

      public List<T> List => list;

      public ListAssertion<T> Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected ListAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(Constraint.Formatted(constraintFunction, message, not, name, Value, enumerableImage));
         not = false;

         return this;
      }

      public ListAssertion<T> Equal(List<T> otherList)
      {
         return add(() => list.Equals(otherList), $"$name must $not equal {enumerableImage(otherList)}");
      }

      public ListAssertion<T> BeNull()
      {
         return add(() => list == null, "$name must $not be null");
      }

      public ListAssertion<T> BeEmpty()
      {
         return add(() => list.Count == 0, "$name must $not be empty");
      }

      public ListAssertion<T> BeNullOrEmpty()
      {
         return add(() => list == null || list.Count == 0, "$name must $not be null or empty");
      }

      public ListAssertion<T> HaveIndexOf(int index)
      {
         return add(() => index > 0 && index < list.Count, $"$name must $not have an index of {index}");
      }

      public ListAssertion<T> HaveCountOf(int minimumCount)
      {
         return add(() => list.Count >= minimumCount, $"$name must $not have a count of at least {minimumCount}");
      }

      public List<T> Value => list;

      public IEnumerable<Constraint> Constraints => constraints;

      public bool BeTrue() => beTrue(this);

      public IAssertion<List<T>> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, List<T>>(this, args);

      public List<T> Force() => force(this);

      public List<T> Force(string message) => force(this, message);

      public List<T> Force(Func<string> messageFunc) => force(this, messageFunc);

      public List<T> Force<TException>(params object[] args) where TException : Exception => force<TException, List<T>>(this, args);

      public TResult Force<TResult>() => forceConvert<List<T>, TResult>(this);

      public TResult Force<TResult>(string message) => forceConvert<List<T>, TResult>(this, message);

      public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<List<T>, TResult>(this, messageFunc);

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception
      {
         return forceConvert<List<T>, TException, TResult>(this, args);
      }

      public IResult<List<T>> OrFailure() => orFailure(this);

      public IResult<List<T>> OrFailure(string message) => orFailure(this, message);

      public IResult<List<T>> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public IMaybe<List<T>> OrNone() => orNone(this);

      public async Task<ICompletion<List<T>>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<List<T>>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<List<T>>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }
   }
}