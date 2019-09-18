using System;
using System.Collections;
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

namespace Core.Assertions.Collections
{
   public class ListAssertion<T> : IAssertion<List<T>>
   {
      static string listImage(List<T> list)
      {
         if (list == null)
         {
            return "(null)";
         }

         if (list.Count > 10)
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

      protected ListAssertion<T> add(IList otherList, Func<List<T>, bool> constraintFunction, string message)
      {
         switch (otherList)
         {
            case null:
               constraints.Add(Constraint.Failing("RHS must be non-null"));
               break;
            case List<T> tList:
               constraints.Add(new Constraint(() => constraintFunction(tList), message, not));
               break;
            default:
               constraints.Add(Constraint.Failing("Expected list"));
               break;
         }

         not = false;
         return this;
      }

      protected ListAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public ListAssertion<T> Equal(List<T> otherList)
      {
         return add(otherList, a => list.Equals(a), $"{image} must $not equal {listImage(otherList)}");
      }

      public ListAssertion<T> BeNull()
      {
         return add(() => list == null, "This list must $not be null");
      }

      public ListAssertion<T> BeEmpty()
      {
         return add(() => list.Count == 0, "This list must $not be empty");
      }

      public ListAssertion<T> BeNullOrEmpty()
      {
         return add(() => list == null || list.Count == 0, "This list must $not be null or empty");
      }

      public ListAssertion<T> HaveIndexOf(int index)
      {
         return add(() => index > 0 && index < list.Count, $"{image} must $not have an index of {index}");
      }

      public ListAssertion<T> HaveACountOf(int minimumCount)
      {
         return add(() => list.Count >= minimumCount, $"{image} must $not have a count of at least {minimumCount}");
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

      public List<T> Ensure()
      {
         Assert();
         return list;
      }

      public List<T> Ensure(string message)
      {
         Assert(message);
         return list;
      }

      public List<T> Ensure(Func<string> messageFunc)
      {
         Assert(messageFunc);
         return list;
      }

      public List<T> Ensure<TException>(params object[] args) where TException : Exception
      {
         Assert<TException>(args);
         return list;
      }

      public TResult Ensure<TResult>()
      {
         Assert();
         var converter = TypeDescriptor.GetConverter(typeof(List<T>));
         return (TResult)converter.ConvertTo(list, typeof(TResult));
      }

      public TResult Ensure<TResult>(string message)
      {
         Assert(message);
         var converter = TypeDescriptor.GetConverter(typeof(List<T>));
         return (TResult)converter.ConvertTo(list, typeof(TResult));
      }

      public TResult Ensure<TResult>(Func<string> messageFunc)
      {
         Assert(messageFunc);
         var converter = TypeDescriptor.GetConverter(typeof(List<T>));
         return (TResult)converter.ConvertTo(list, typeof(TResult));
      }

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         Assert<TException>(args);
         var converter = TypeDescriptor.GetConverter(typeof(List<T>));
         return (TResult)converter.ConvertTo(list, typeof(TResult));
      }

      public IResult<List<T>> Try()
      {
         if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            return constraint.Message.Failure<List<T>>();
         }
         else
         {
            return list.Success();
         }
      }

      public async Task<ICompletion<List<T>>> TryAsync(CancellationToken token) => await runAsync(t =>
      {
         if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            return constraint.Message.Interrupted<List<T>>();
         }
         else
         {
            return list.Completed(t);
         }
      }, token);
   }
}