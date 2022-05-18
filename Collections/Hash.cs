using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public class Hash<TKey, TValue> : Dictionary<TKey, TValue>, IHash<TKey, TValue>
   {
      protected ReaderWriterLockSlim locker;

      public event EventHandler<HashArgs<TKey, TValue>> Updated;
      public event EventHandler<HashArgs<TKey, TValue>> Removed;

      public Hash()
      {
         locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      }

      public Hash(int capacity) : base(capacity)
      {
         locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      }

      public Hash(IEqualityComparer<TKey> comparer) : base(comparer)
      {
         locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      }

      public Hash(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
      {
         locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      }

      public Hash(IDictionary<TKey, TValue> dictionary) : base(dictionary)
      {
         locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      }

      public Hash(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
      {
         locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      }

      protected Hash(SerializationInfo info, StreamingContext context) : base(info, context)
      {
         locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      }

      public Hash(IEnumerable<(TKey key, TValue value)> tuples)
      {
         foreach (var (key, value) in tuples)
         {
            this[key] = value;
         }

         locker = new ReaderWriterLockSlim();
      }

      public new void Add(TKey key, TValue value)
      {
         base.Add(key, value);
         Updated?.Invoke(this, new HashArgs<TKey, TValue>(key, value));
      }

      public new bool Remove(TKey key)
      {
         if (ContainsKey(key))
         {
            var value = this[key];
            var result = base.Remove(key);
            Removed?.Invoke(this, new HashArgs<TKey, TValue>(key, value));

            return result;
         }
         else
         {
            return false;
         }
      }

      public new TValue this[TKey key]
      {
         get
         {
            if (ContainsKey(key))
            {
               try
               {
                  locker.EnterReadLock();
                  return base[key];
               }
               finally
               {
                  locker.ExitReadLock();
               }
            }
            else
            {
               return default;
            }
         }
         set
         {
            try
            {
               locker.EnterWriteLock();
               if (ContainsKey(key))
               {
                  base[key] = value;
                  Updated?.Invoke(this, new HashArgs<TKey, TValue>(key, value));
               }
               else
               {
                  Add(key, value);
               }
            }
            finally
            {
               locker.ExitWriteLock();
            }
         }
      }

      public IEnumerable<TValue> ValuesFromKeys(IEnumerable<TKey> keys)
      {
         foreach (var key in keys)
         {
            if (Map(key, out var value))
            {
               yield return value;
            }
         }
      }

      public Result<Hash<TKey, TValue>> AnyHash() => this;

      public TValue Find(TKey key, Func<TKey, TValue> defaultValue, bool addIfNotFound = false)
      {
         if (Map(key, out var result))
         {
            return result;
         }
         else
         {
            var value = defaultValue(key);
            if (addIfNotFound)
            {
               try
               {
                  locker.EnterWriteLock();
                  Add(key, value);
               }
               catch { }
               finally
               {
                  locker.ExitWriteLock();
               }
            }

            return value;
         }
      }

      public TValue Memoize(TKey key, Func<TKey, TValue> defaultValue, bool alwaysUseDefaultValue = false)
      {
         return alwaysUseDefaultValue ? defaultValue(key) : Find(key, defaultValue, true);
      }

      public bool Map(TKey key, out TValue value, Func<TValue> valueToAdd)
      {
         if (Map(key, out value))
         {
            return true;
         }
         else
         {
            value = valueToAdd();
            try
            {
               locker.EnterWriteLock();
               this[key] = value;
            }
            catch { }
            finally
            {
               locker.ExitWriteLock();
            }

            return false;
         }
      }

      public TKey[] KeyArray()
      {
         var keys = new TKey[Count];
         Keys.CopyTo(keys, 0);

         return keys;
      }

      public TValue[] ValueArray()
      {
         var values = new TValue[Count];
         Values.CopyTo(values, 0);

         return values;
      }

      public void SetValueTo(TValue value, params TKey[] keys)
      {
         foreach (var key in keys)
         {
            this[key] = value;
         }
      }

      public HashTrying<TKey, TValue> TryTo => new(this);

      public KeyValuePair<TKey, TValue>[] ItemsArray() => this.ToArray();

      public IEnumerable<(TKey key, TValue value)> Tuples()
      {
         foreach (var (key, value) in this)
         {
            yield return (key, value);
         }
      }

      public void Copy(Hash<TKey, TValue> other)
      {
         foreach (var (key, value) in other)
         {
            this[key] = value;
         }
      }

      public bool Map(TKey key, out TValue value, TValue defaultValue = default)
      {
         if (ContainsKey(key))
         {
            try
            {
               locker.EnterReadLock();
               value = this[key];

               return true;
            }
            finally
            {
               locker.ExitReadLock();
            }
         }
         else
         {
            value = defaultValue;
            return false;
         }
      }

      public Hash<TKey, TValue> Merge(Hash<TKey, TValue> otherHash)
      {
         var result = new Hash<TKey, TValue>();
         foreach (var (key, value) in this)
         {
            result[key] = value;
         }

         foreach (var (key, value) in otherHash)
         {
            result[key] = value;
         }

         return result;
      }

      public Maybe<TValue> Replace(TKey key, TValue newValue)
      {
         var oldValue = this.Map(key);
         this[key] = newValue;

         return oldValue;
      }

      public Maybe<TValue> OneTime(TKey key)
      {
         if (ContainsKey(key))
         {
            var value = this[key];
            Remove(key);
            return value;
         }
         else
         {
            return nil;
         }
      }

      public void Move(TKey oldKey, TKey newKey)
      {
         if (Map(oldKey, out var value))
         {
            this[newKey] = value;
            Remove(oldKey);
         }
      }


      protected virtual Hash<TKey, TValue> getNewHash() => new(Comparer);

      public virtual Hash<TKey, TValue> Subset(params TKey[] keys)
      {
         var newHash = getNewHash();

         foreach (var key in keys)
         {
            newHash[key] = this[key];
         }

         return newHash;
      }

      public virtual Hash<TKey, TValue> Subset(IEnumerable<TKey> keys) => Subset(keys.ToArray());
   }
}