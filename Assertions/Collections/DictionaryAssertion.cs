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
   public class DictionaryAssertion<TKey, TValue> : IAssertion<Dictionary<TKey, TValue>>
   {
      static string keyValueImage(KeyValuePair<TKey, TValue> item) => $"[{item.Key}] => {item.Value}";

      static string hashImage(Dictionary<TKey, TValue> hash)
      {
         if (hash == null)
         {
            return "(null)";
         }
         else if (hash.Count > 10)
         {
            return $"{{{hash.Take(10).Select(keyValueImage).Stringify()}}}";
         }
         else
         {
            return $"{{{hash.Select(keyValueImage).Stringify()}}}";
         }
      }

      protected Dictionary<TKey, TValue> dictionary;
      protected List<Constraint> constraints;
      protected bool not;
      protected string image;

      public DictionaryAssertion(Dictionary<TKey, TValue> dictionary)
      {
         this.dictionary = dictionary;
         constraints = new List<Constraint>();
         not = false;
         image = hashImage(this.dictionary);
      }

      public bool BeTrue() => beTrue(this);

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
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public DictionaryAssertion<TKey, TValue> Equal(Dictionary<TKey, TValue> otherHash)
      {
         return add(() => dictionary.Equals(otherHash), $"{image} must $not equal {hashImage(otherHash)}");
      }

      public DictionaryAssertion<TKey, TValue> BeNull()
      {
         return add(() => dictionary == null, $"{image} must $not be null");
      }

      public DictionaryAssertion<TKey, TValue> BeEmpty()
      {
         return add(() => dictionary.Count == 0, $"{image} must $not be empty");
      }

      public DictionaryAssertion<TKey, TValue> BeNullOrEmpty()
      {
         return add(() => dictionary == null || dictionary.Count == 0, $"{image} must $not be null or empty");
      }

      public DictionaryAssertion<TKey, TValue> HaveKeyOf(TKey key)
      {
         return add(() => dictionary.ContainsKey(key), $"{image} must $not have key of {key}");
      }

      public DictionaryAssertion<TKey, TValue> HaveValueOf(TValue value)
      {
         return add(() => dictionary.ContainsValue(value), $"{image} must $not have value of {value}");
      }

      public DictionaryAssertion<TKey, TValue> HaveCountOf(int minimumCount)
      {
         return add(() => dictionary.Count >= minimumCount, $"{image} must $not have a count of at least {minimumCount}");
      }

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, Dictionary<TKey, TValue>>(this, args);

      public Dictionary<TKey, TValue> Ensure() => ensure(this);

      public Dictionary<TKey, TValue> Ensure(string message) => ensure(this, message);

      public Dictionary<TKey, TValue> Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public Dictionary<TKey, TValue> Ensure<TException>(params object[] args) where TException : Exception
      {
         return ensure<TException, Dictionary<TKey, TValue>>(this, args);
      }

      public TResult Ensure<TResult>() => ensureConvert<Dictionary<TKey, TValue>, TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<Dictionary<TKey, TValue>, TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<Dictionary<TKey, TValue>, TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<Dictionary<TKey, TValue>, TException, TResult>(this, args);
      }

      public IResult<Dictionary<TKey, TValue>> Try() => @try(this);

      public IResult<Dictionary<TKey, TValue>> Try(string message) => @try(this, message);

      public IResult<Dictionary<TKey, TValue>> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public IMaybe<Dictionary<TKey, TValue>> Maybe() => maybe(this);

      public async Task<ICompletion<Dictionary<TKey, TValue>>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<Dictionary<TKey, TValue>>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<Dictionary<TKey, TValue>>> TryAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await tryAsync(this, messageFunc, token);
      }
   }
}