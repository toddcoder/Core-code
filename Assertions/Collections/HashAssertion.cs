using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Collections
{
   public class HashAssertion<TKey, TValue> : IAssertion<Hash<TKey, TValue>>
   {
      static string keyValueImage(KeyValuePair<TKey, TValue> item) => $"[{item.Key}] => {item.Value}";

      static string hashImage(Hash<TKey, TValue> hash)
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

      protected Hash<TKey, TValue> hash;
      protected List<Constraint> constraints;
      protected bool not;
      protected string image;

      public HashAssertion(Hash<TKey, TValue> hash)
      {
         this.hash = hash;
         constraints = new List<Constraint>();
         not = false;
         image = hashImage(this.hash);
      }

      public bool BeTrue() => beTrue(this);

      public Hash<TKey, TValue> Value => hash;

      public IEnumerable<Constraint> Constraints => constraints;

      public HashAssertion<TKey, TValue> Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected HashAssertion<TKey, TValue> add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public HashAssertion<TKey, TValue> Equal(Hash<TKey, TValue> otherHash)
      {
         return add(() => hash.Equals(otherHash), $"{image} must $not equal {hashImage(otherHash)}");
      }

      public HashAssertion<TKey, TValue> BeNull()
      {
         return add(() => hash == null, $"{image} must $not be null");
      }

      public HashAssertion<TKey, TValue> BeEmpty()
      {
         return add(() => hash.Count == 0, $"{image} must $not be empty");
      }

      public HashAssertion<TKey, TValue> BeNullOrEmpty()
      {
         return add(() => hash == null || hash.Count == 0, $"{image} must $not be null or empty");
      }

      public HashAssertion<TKey, TValue> HaveKeyOf(TKey key)
      {
         return add(() => hash.ContainsKey(key), $"{image} must $not have key of {key}");
      }

      public HashAssertion<TKey, TValue> HaveValueOf(TValue value)
      {
         return add(() => hash.ContainsValue(value), $"{image} must $not have value of {value}");
      }

      public HashAssertion<TKey, TValue> HaveCountOf(int minimumCount)
      {
         return add(() => hash.Count >= minimumCount, $"{image} must $not have a count of at least {minimumCount}");
      }

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, Hash<TKey, TValue>>(this, args);

      public Hash<TKey, TValue> Ensure() => ensure(this);

      public Hash<TKey, TValue> Ensure(string message) => ensure(this, message);

      public Hash<TKey, TValue> Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public Hash<TKey, TValue> Ensure<TException>(params object[] args) where TException : Exception
      {
         return ensure<TException, Hash<TKey, TValue>>(this, args);
      }

      public TResult Ensure<TResult>() => ensureConvert<Hash<TKey, TValue>, TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<Hash<TKey, TValue>, TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<Hash<TKey, TValue>, TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<Hash<TKey, TValue>, TException, TResult>(this, args);
      }

      public IResult<Hash<TKey, TValue>> Try() => @try(this);

      public IResult<Hash<TKey, TValue>> Try(string message) => @try(this, message);

      public IResult<Hash<TKey, TValue>> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public async Task<ICompletion<Hash<TKey, TValue>>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<Hash<TKey, TValue>>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<Hash<TKey, TValue>>> TryAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await tryAsync(this, messageFunc, token);
      }
   }
}