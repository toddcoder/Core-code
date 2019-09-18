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
using static Core.Monads.AttemptFunctions;

namespace Core.Assertions.Comparables
{
   public class ComparableAssertion<T> : IAssertion<T> where T : struct, IComparable
   {
      public static implicit operator bool(ComparableAssertion<T> assertion) => assertion.BeTrue();

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

      public T Ensure()
      {
         Assert();
         return Comparable;
      }

      public T Ensure(string message)
      {
         Assert(message);
         return Comparable;
      }

      public T Ensure(Func<string> messageFunc)
      {
         Assert(messageFunc);
         return Comparable;
      }

      public T Ensure<TException>(params object[] args) where TException : Exception
      {
         Assert<TException>(args);
         return Comparable;
      }

      TResult IAssertion<T>.Ensure<TResult>()
      {
         Assert();
         var converter = TypeDescriptor.GetConverter(typeof(T));
         return (TResult)converter.ConvertTo(comparable, typeof(TResult));
      }

      public TResult Ensure<TResult>(string message)
      {
         Assert(message);
         var converter = TypeDescriptor.GetConverter(typeof(T));
         return (TResult)converter.ConvertTo(comparable, typeof(TResult));
      }

      public TResult Ensure<TResult>(Func<string> messageFunc)
      {
         Assert(messageFunc);
         var converter = TypeDescriptor.GetConverter(typeof(T));
         return (TResult)converter.ConvertTo(comparable, typeof(TResult));
      }

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         Assert<TException>(args);
         var converter = TypeDescriptor.GetConverter(typeof(T));
         return (TResult)converter.ConvertTo(comparable, typeof(TResult));
      }

      public IResult<T> Try()
      {
         if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            return constraint.Message.Failure<T>();
         }
         else
         {
            return Comparable.Success();
         }
      }

      public async Task<ICompletion<T>> TryAsync(CancellationToken token) => await runAsync(t =>
      {
         if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            return constraint.Message.Interrupted<T>();
         }
         else
         {
            return Comparable.Completed(t);
         }
      }, token);
   }
}