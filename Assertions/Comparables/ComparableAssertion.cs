using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Comparables;

public class ComparableAssertion<T> : IAssertion<T> where T : struct, IComparable
{
   public static implicit operator bool(ComparableAssertion<T> assertion) => assertion.BeEquivalentToTrue();

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
   protected string name;

   public ComparableAssertion(IComparable comparable)
   {
      this.comparable = comparable;
      constraints = new List<Constraint>();
      not = false;
      name = "Comparable";
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
            constraints.Add(Constraint.Failing("$name must be non-null", name));
            break;
         case IComparable otherComparable:
            constraints.Add(new Constraint(() => constraintFunction(otherComparable), message, not, name, Value));
            break;
         default:
            constraints.Add(Constraint.Failing("$name must be comparable", name));
            break;
      }

      not = false;
      return this;
   }

   protected ComparableAssertion<T> add(Func<bool> constraintFunction, string message)
   {
      constraints.Add(new Constraint(constraintFunction, message, not, name, Value));
      not = false;

      return this;
   }

   public ComparableAssertion<T> Equal(object obj)
   {
      return add(obj, c => comparable.CompareTo(c) == 0, $"$name must $not equal {obj}");
   }

   public ComparableAssertion<T> BeGreaterThan(object obj)
   {
      return add(obj, c => comparable.CompareTo(c) > 0, $"$name must $not be > {obj}");
   }

   public ComparableAssertion<T> BeGreaterThanOrEqual(object obj)
   {
      return add(obj, c => comparable.CompareTo(c) >= 0, $"$name must $not be >= {obj}");
   }

   public ComparableAssertion<T> BeLessThan(object obj)
   {
      return add(obj, c => comparable.CompareTo(c) < 0, $"$name must $not be < {obj}");
   }

   public ComparableAssertion<T> BeLessThanOrEqual(object obj)
   {
      return add(obj, c => comparable.CompareTo(c) <= 0, $"$name must $not be <= {obj}");
   }

   public ComparableAssertion<T> BeZero()
   {
      return add(() => comparable.CompareTo(0) == 0, "$name must $not be zero");
   }

   public ComparableAssertion<T> BePositive()
   {
      return add(() => comparable.CompareTo(0) > 0, "$name must $not be positive");
   }

   public ComparableAssertion<T> BeNegative()
   {
      return add(() => comparable.CompareTo(0) < 0, "$name must $not be negative");
   }

   public ComparableAssertion<T> BeIn(params object[] objects)
   {
      var objectsString = objects.Select(o => o == null ? "null" : objects.ToString()).ToString(", ");
      return add(objects, c => inList(comparable, objects), $"$name must $not be in {objectsString}");
   }

   public ComparableAssertion<T> BeOfType(Type type)
   {
      return add(0, c => comparable.GetType() == type, $"$name must $not be of type {type}");
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
      var message = $"$name must $not be between {Comparable1} and {comparable2}";
      return add(comparable2, c => betweenAnd(Comparable, Comparable1, comparable2), message);
   }

   public ComparableAssertion<T> Until(T comparable2)
   {
      var message = $"$name must $not be between {Comparable1} and {comparable2} exclusively";
      return add(comparable2, c => betweenUntil(Comparable, Comparable1, comparable2), message);
   }

   public ComparableAssertion<T> BeBetween(T comparable1)
   {
      Comparable1 = comparable1;
      return this;
   }

   internal ComparableAssertion<T> IfTrue(Func<T, bool> isTrue, string message)
   {
      return add(() => isTrue(Value), message);
   }

   public T Value => Comparable;

   public IEnumerable<Constraint> Constraints => constraints;

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

   public IAssertion<T> Named(string name)
   {
      this.name = name;
      return this;
   }

   public void OrThrow() => orThrow(this);

   public void OrThrow(string message) => orThrow(this, message);

   public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

   public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, T>(this, args);

   public T Force() => force(this);

   public T Force(string message) => force(this, message);

   public T Force(Func<string> messageFunc) => force(this, messageFunc);

   public T Force<TException>(params object[] args) where TException : Exception => force<TException, T>(this, args);

   TResult IAssertion<T>.Force<TResult>() => forceConvert<T, TResult>(this);

   public TResult Force<TResult>(string message) => forceConvert<T, TResult>(this, message);

   public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<T, TResult>(this, messageFunc);

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception
   {
      return forceConvert<T, TException, TResult>(this, args);
   }

   public Result<T> OrFailure() => orFailure(this);

   public Result<T> OrFailure(string message) => orFailure(this, message);

   public Result<T> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Maybe<T> OrNone() => orNone(this);

   public async Task<Completion<T>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

   public async Task<Completion<T>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

   public async Task<Completion<T>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}