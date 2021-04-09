using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Collections;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Collections
{
   public class SetAssertion<T> : IAssertion<Set<T>>
   {
      protected Set<T> set;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;
      protected string image;

      public SetAssertion(Set<T> set)
      {
         this.set = set;

         constraints = new List<Constraint>();
         not = false;
         name = "Set";
         image = enumerableImage(set);
      }

      public Set<T> Set => set;

      public SetAssertion<T> Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected SetAssertion<T> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(Constraint.Formatted(constraintFunction, message, not, name, Value, _ => image));
         not = false;

         return this;
      }

      public SetAssertion<T> Equal(Set<T> other) => add(() => set.Equals(other), $"$name must $not equal {enumerableImage(other)}");

      public SetAssertion<T> BeNull() => add(() => set is null, "$name must $not be null");

      public SetAssertion<T> BeEmpty() => add(() => set.Count == 0, "$name must $not be empty");

      public SetAssertion<T> BeNullOrEmpty() => add(() => set is null || set.Count == 0, "$name must $not be null or empty");

      public SetAssertion<T> HaveCountOf(int minimumCount)
      {
         return add(() => set.Count >= minimumCount, $"$name must $not have a length of at least {minimumCount}");
      }

      public SetAssertion<T> HaveCountOfExactly(int count)
      {
         return add(() => set.Count == count, $"$name must $not have a length of exactly {count}");
      }

      public SetAssertion<T> Contain(T item) => add(() => set.Contains(item), $"$name must $not contain {item}");

      public SetAssertion<T> Overlap(IEnumerable<T> items) => add(() => set.Overlaps(items), $"$name must $not overlap {enumerableImage(items)}");

      public SetAssertion<T> BeASubsetOf(IEnumerable<T> items)
      {
         return add(() => set.IsSubsetOf(items), $"$name must $not be a subset of {enumerableImage(items)}");
      }

      public SetAssertion<T> BeAProperSubsetOf(IEnumerable<T> items)
      {
         return add(() => set.IsProperSubsetOf(items), $"$name must $not be a proper subset of {enumerableImage(items)}");
      }

      public SetAssertion<T> BeASupersetOf(IEnumerable<T> items)
      {
         return add(() => set.IsSupersetOf(items), $"$name must $not be a superset of {enumerableImage(items)}");
      }

      public SetAssertion<T> BeAProperSupersetOf(IEnumerable<T> items)
      {
         return add(() => set.IsProperSupersetOf(items), $"$name must $not be a proper superset of {enumerableImage(items)}");
      }

      public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

      public Set<T> Value => set;

      public IEnumerable<Constraint> Constraints => constraints;

      public IAssertion<Set<T>> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, Set<T>>(this, args);

      public Set<T> Force() => force(this);

      public Set<T> Force(string message) => force(this, message);

      public Set<T> Force(Func<string> messageFunc) => force(this, messageFunc);

      public Set<T> Force<TException>(params object[] args) where TException : Exception => force<TException, Set<T>>(this, args);

      public TResult Force<TResult>() => forceConvert<Set<T>, TResult>(this);

      public TResult Force<TResult>(string message) => forceConvert<Set<T>, TResult>(this, message);

      public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<Set<T>, TResult>(this, messageFunc);

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception
      {
         return forceConvert<Set<T>, TException, TResult>(this, args);
      }

      public IResult<Set<T>> OrFailure() => orFailure(this);

      public IResult<Set<T>> OrFailure(string message) => orFailure(this, message);

      public IResult<Set<T>> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public IMaybe<Set<T>> OrNone() => orNone(this);

      public async Task<ICompletion<Set<T>>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<Set<T>>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<Set<T>>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}