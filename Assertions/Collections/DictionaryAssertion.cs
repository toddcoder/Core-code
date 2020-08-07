using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Assertions.Monads;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Assertions.Collections
{
   public class DictionaryAssertion<TKey, TValue> : IAssertion<Dictionary<TKey, TValue>>
   {
      static string keyValueImage(KeyValuePair<TKey, TValue> item) => $"[{item.Key}] => {item.Value}";

      protected Dictionary<TKey, TValue> dictionary;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public DictionaryAssertion(Dictionary<TKey, TValue> dictionary)
      {
         this.dictionary = dictionary;
         constraints = new List<Constraint>();
         not = false;
         name = "Dictionary";
      }

      public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

      public Dictionary<TKey, TValue> Value => dictionary;

      public IEnumerable<Constraint> Constraints => constraints;

      public DictionaryAssertion<TKey, TValue> Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected DictionaryAssertion<TKey, TValue> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(Constraint.Formatted(constraintFunction, message, not, name, Value, dictionaryImage));
         not = false;

         return this;
      }

      protected DictionaryAssertion<TKey, TValue> addWithoutValue(Func<bool> constraintFunction, string message)
      {
         constraints.Add(Constraint.Formatted(constraintFunction, message, not, name));
         not = false;

         return this;
      }

      public DictionaryAssertion<TKey, TValue> Equal(Dictionary<TKey, TValue> otherHash)
      {
         return addWithoutValue(() => dictionary.Equals(otherHash), $"$name must $not equal {dictionaryImage(otherHash)}");
      }

      public DictionaryAssertion<TKey, TValue> BeNull()
      {
         return addWithoutValue(() => dictionary == null, "$name must $not be null");
      }

      public DictionaryAssertion<TKey, TValue> BeEmpty()
      {
         return addWithoutValue(() => dictionary.Count == 0, "$name must $not be empty");
      }

      public DictionaryAssertion<TKey, TValue> BeNullOrEmpty()
      {
         return addWithoutValue(() => dictionary == null || dictionary.Count == 0, "$name must $not be null or empty");
      }

      public DictionaryAssertion<TKey, TValue> HaveKeyOf(TKey key)
      {
         return addWithoutValue(() => dictionary.ContainsKey(key), $"$name must $not have key of {key}");
      }

      public DictionaryAssertion<TKey, TValue> HaveValueOf(TValue value)
      {
         return add(() => dictionary.ContainsValue(value), $"$name must $not have value of {value}");
      }

      public DictionaryAssertion<TKey, TValue> HaveCountOf(int minimumCount)
      {
         return addWithoutValue(() => dictionary.Count >= minimumCount, $"$name must $not have a count of at least {minimumCount}");
      }

      public MaybeAssertion<TValue> HaveValueAt(TKey key)
      {
         if (dictionary.ContainsKey(key))
         {
            return dictionary[key].Some().Must();
         }
         else
         {
            return none<TValue>().Must();
         }
      }

      public ResultAssertion<TValue> HaveValueAt(TKey key, Func<string> failureMessage)
      {
         if (dictionary.ContainsKey(key))
         {
            return dictionary[key].Success().Must();
         }
         else
         {
            return failureMessage().Failure<TValue>().Must();
         }
      }

      public IAssertion<Dictionary<TKey, TValue>> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, Dictionary<TKey, TValue>>(this, args);

      public Dictionary<TKey, TValue> Force() => force(this);

      public Dictionary<TKey, TValue> Force(string message) => force(this, message);

      public Dictionary<TKey, TValue> Force(Func<string> messageFunc) => force(this, messageFunc);

      public Dictionary<TKey, TValue> Force<TException>(params object[] args) where TException : Exception
      {
         return force<TException, Dictionary<TKey, TValue>>(this, args);
      }

      public TResult Force<TResult>() => forceConvert<Dictionary<TKey, TValue>, TResult>(this);

      public TResult Force<TResult>(string message) => forceConvert<Dictionary<TKey, TValue>, TResult>(this, message);

      public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<Dictionary<TKey, TValue>, TResult>(this, messageFunc);

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception
      {
         return forceConvert<Dictionary<TKey, TValue>, TException, TResult>(this, args);
      }

      public IResult<Dictionary<TKey, TValue>> OrFailure() => orFailure(this);

      public IResult<Dictionary<TKey, TValue>> OrFailure(string message) => orFailure(this, message);

      public IResult<Dictionary<TKey, TValue>> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public IMaybe<Dictionary<TKey, TValue>> OrNone() => orNone(this);

      public async Task<ICompletion<Dictionary<TKey, TValue>>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<Dictionary<TKey, TValue>>> OrFailureAsync(string message, CancellationToken token)
      {
         return await orFailureAsync(this, message, token);
      }

      public async Task<ICompletion<Dictionary<TKey, TValue>>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}