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
   public class ListAssertion<T> : IAssertion<List<T>>
   {
      public static implicit operator bool(ListAssertion<T> assertion) => assertion.BeTrue();

      public static bool operator &(ListAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(ListAssertion<T> x, ICanBeTrue y) => or(x, y);

      static string listImage(List<T> list)
      {
         if (list == null)
         {
            return "(null)";
         }
         else if (list.Count > 10)
         {
            return $"{{{list.Take(10).Stringify()}...}}";
         }
         else
         {
            return $"{{{list.Stringify()}}}";
         }
      }

      protected List<T> list;
      protected List<Constraint> constraints;
      protected bool not;
      protected string image;

      public ListAssertion(List<T> list)
      {
         this.list = list;
         constraints = new List<Constraint>();
         not = false;
         image = listImage(this.list);
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
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public ListAssertion<T> Equal(List<T> otherList)
      {
         return add(() => list.Equals(otherList), $"{image} must $not equal {listImage(otherList)}");
      }

      public ListAssertion<T> BeNull()
      {
         return add(() => list == null, $"{image} must $not be null");
      }

      public ListAssertion<T> BeEmpty()
      {
         return add(() => list.Count == 0, $"{image} must $not be empty");
      }

      public ListAssertion<T> BeNullOrEmpty()
      {
         return add(() => list == null || list.Count == 0, $"{image} must $not be null or empty");
      }

      public ListAssertion<T> HaveIndexOf(int index)
      {
         return add(() => index > 0 && index < list.Count, $"{image} must $not have an index of {index}");
      }

      public ListAssertion<T> HaveCountOf(int minimumCount)
      {
         return add(() => list.Count >= minimumCount, $"{image} must $not have a count of at least {minimumCount}");
      }

      public List<T> Value => list;

      public IEnumerable<Constraint> Constraints => constraints;

      public bool BeTrue() => beTrue(this);

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, List<T>>(this, args);

      public List<T> Ensure() => ensure(this);

      public List<T> Ensure(string message) => ensure(this, message);

      public List<T> Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public List<T> Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, List<T>>(this, args);

      public TResult Ensure<TResult>() => ensureConvert<List<T>, TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<List<T>, TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<List<T>, TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<List<T>, TException, TResult>(this, args);
      }

      public IResult<List<T>> Try() => @try(this);

      public IResult<List<T>> Try(string message) => @try(this, message);

      public IResult<List<T>> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public async Task<ICompletion<List<T>>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<List<T>>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<List<T>>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}