using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace Core.Collections
{
   public class Hash<TKey, TValue> : Dictionary<TKey, TValue>, IHash<TKey, TValue>
   {
      protected ReaderWriterLockSlim locker;

      public Hash()
      {
         locker = new ReaderWriterLockSlim();
      }

      public Hash(int capacity) : base(capacity)
      {
         locker = new ReaderWriterLockSlim();
      }

      public Hash(IEqualityComparer<TKey> comparer) : base(comparer)
      {
         locker = new ReaderWriterLockSlim();
      }

      public Hash(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
      {
         locker = new ReaderWriterLockSlim();
      }

      public Hash(IDictionary<TKey, TValue> dictionary) : base(dictionary)
      {
         locker = new ReaderWriterLockSlim();
      }

      public Hash(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
      {
         locker = new ReaderWriterLockSlim();
      }

      protected Hash(SerializationInfo info, StreamingContext context) : base(info, context)
      {
         locker = new ReaderWriterLockSlim();
      }

      public Hash(IEnumerable<(TKey key, TValue value)> tuples)
      {
         foreach (var (key, value) in tuples)
         {
            this[key] = value;
         }

         locker = new ReaderWriterLockSlim();
      }

      public new TValue this[TKey key]
      {
         get
         {
            if (ContainsKey(key))
            {
               return base[key];
            }
            else
            {
               return default;
            }
         }
         set
         {
            if (ContainsKey(key))
            {
               base[key] = value;
            }
            else
            {
               Add(key, value);
            }
         }
      }

      public TValue Find(TKey key, Func<TKey, TValue> defaultValue, bool addIfNotFound = false)
      {
         if (If(key, out var result))
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
               catch (ArgumentException) { }
               finally
               {
                  locker.ExitWriteLock();
               }
            }

            return value;
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

      public HashTrying<TKey, TValue> TryTo => new HashTrying<TKey, TValue>(this);

      public KeyValuePair<TKey, TValue>[] ItemsArray() => this.ToArray();

      public void Copy(Hash<TKey, TValue> other)
      {
         foreach (var (key, value) in other)
         {
            this[key] = value;
         }
      }

      public bool If(TKey key, out TValue value, TValue defaultValue = default)
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
   }
}