using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Comparables
{
   public class ComparableAssertion<T> : IAssertion<T> where T : struct, IComparable
   {
      public static implicit operator bool(ComparableAssertion<T> assertion) => assertion.BeTrue();

      public static bool operator &(ComparableAssertion<T> x, ICanBeTrue y) => and(x, y);

      public static bool operator |(ComparableAssertion<T> x, ICanBeTrue y) => or(x, y);

      protected static bool inList(IComparable comparable, object[] objects)
      {
         foreach (var obj in objects)
         {
            if (obj is IComparable otherComparable)
            {
               if (comparable.CompareTo(otherComparable) == 0)
               {
                  return true;
               }
            }
            else
            {
               return false;
            }
         }

         return false;
      }

      protected IComparable comparable;
      protected List<Constraint> constraints;
      protected bool not;

      public ComparableAssertion(IComparable comparable)
      {
         this.comparable = comparable;
         constraints = new List<Constraint>();
         not = false;
      }

      public T Comparable => (T)comparable;

      public ComparableAssertion<T> Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected T Comparable1 { get; set; }

      protected ComparableAssertion<T> add(object obj, Func<IComparable, bool> constraintFunction, string message)
      {
         switch (obj)
         {
            case null:
               constraints.Add(Constraint.Failing("RHS must be non-null"));
               break;
            case IComparable otherComparable:
               constraints.Add(new Constraint(() => constraintFunction(otherComparable), message, not));
               break;
            default:
               constraints.Add(Constraint.Failing($"{obj} must be comparable"));
               break;
         }

         not = false;
         return this;
      }

      protected ComparableAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public ComparableAssertion<T> Equal(object obj)
      {
         return add(obj, c => comparable.CompareTo(c) == 0, $"{comparable} must $not equal {obj}");
      }

      public ComparableAssertion<T> BeGreaterThan(object obj)
      {
         return add(obj, c => comparable.CompareTo(c) > 0, $"{comparable} must $not be > {obj}");
      }

      public ComparableAssertion<T> BeGreaterThanOrEqual(object obj)
      {
         return add(obj, c => comparable.CompareTo(c) >= 0, $"{comparable} must $not be >= {obj}");
      }

      public ComparableAssertion<T> BeLessThan(object obj)
      {
         return add(obj, c => comparable.CompareTo(c) < 0, $"{comparable} must $not be < {obj}");
      }

      public ComparableAssertion<T> BeLessThanOrEqual(object obj)
      {
         return add(obj, c => comparable.CompareTo(c) <= 0, $"{comparable} must $not be <= {obj}");
      }

      public ComparableAssertion<T> BeZero()
      {
         return add(() => comparable.CompareTo(0) == 0, $"{comparable} must $not be zero");
      }

      public ComparableAssertion<T> BePositive()
      {
         return add(() => comparable.CompareTo(0) > 0, $"{comparable} must $not be positive");
      }

      public ComparableAssertion<T> BeNegative()
      {
         return add(() => comparable.CompareTo(0) < 0, $"{comparable} must $not be negative");
      }

      public ComparableAssertion<T> BeIn(params object[] objects)
      {
         var objectsString = objects.Select(o => o == null ? "null" : objects.ToString()).Stringify();
         return add(objects, c => inList(comparable, objects), $"{comparable} must $not be in {objectsString}");
      }

      public ComparableAssertion<T> BeOfType(Type type)
      {
         return add(0, c => comparable.GetType() == type, $"{comparable} must $not be of type {type}");
      }

      protected bool betweenAnd(T original, T comparable1, T comparable2)
      {
         return original.CompareTo(comparable1) >= 0 && original.CompareTo(comparable2) <= 0;
      }

      protected bool betweenUntil(T original, T comparable1, T comparable2)
      {
         return original.CompareTo(comparable1) >= 0 && original.CompareTo(comparable2) < 0;
      }

      public ComparableAssertion<T> And(T comparable2)
      {
         var message = $"{comparable} must $not be between {Comparable1} and {comparable2}";
         return add(comparable2, c => betweenAnd(Comparable, Comparable1, comparable2), message);
      }

      public ComparableAssertion<T> Until(T comparable2)
      {
         var message = $"{comparable} must $not be between {Comparable1} and {comparable2} exclusively";
         return add(comparable2, c => betweenUntil(Comparable, Comparable1, comparable2), message);
      }

      public ComparableAssertion<T> BeBetween(T comparable1)
      {
         Comparable1 = comparable1;
         return this;
      }

      public T Value => Comparable;

      public IEnumerable<Constraint> Constraints => constraints;

      public bool BeTrue() => beTrue(this);

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, T>(this, args);

      public T Ensure() => ensure(this);

      public T Ensure(string message) => ensure(this, message);

      public T Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public T Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, T>(this, args);

      TResult IAssertion<T>.Ensure<TResult>() => ensureConvert<T, TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<T, TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<T, TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<T, TException, TResult>(this, args);
      }

      public IResult<T> Try() => @try(this);

      public IResult<T> Try(string message) => @try(this, message);

      public IResult<T> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public async Task<ICompletion<T>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<T>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<T>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}