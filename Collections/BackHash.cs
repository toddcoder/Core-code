using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Collections
{
   public class BackHash<TKey, TValue> : Hash<TKey, TValue>
   {
      protected Hash<TValue, TKey> backHash;

      public BackHash()
      {
         backHash = new Hash<TValue, TKey>();
      }

      public BackHash(BackHash<TValue, TKey> backHash)
      {
         foreach (var item in backHash)
         {
            this[item.Value] = item.Key;
         }
      }

      public BackHash(int capacity) : base(capacity)
      {
         backHash = new Hash<TValue, TKey>();
      }

      public BackHash(IEqualityComparer<TKey> comparer) : base(comparer)
      {
         backHash = new Hash<TValue, TKey>();
      }

      public BackHash(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
      {
         backHash = new Hash<TValue, TKey>();
      }

      public BackHash(IDictionary<TKey, TValue> dictionary) : base(dictionary)
      {
         backHash = new Hash<TValue, TKey>();
      }

      public BackHash(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
      {
         backHash = new Hash<TValue, TKey>();
      }

      protected BackHash(SerializationInfo info, StreamingContext context) : base(info, context)
      {
         backHash = new Hash<TValue, TKey>();
      }

      public new TValue this[TKey key]
      {
         get => base[key];
         set
         {
            base[key] = value;
            backHash[value] = key;
         }
      }

      public virtual TKey ValueToKey(TValue value) => backHash[value];

      public virtual Hash<TValue, TKey> Back => backHash;

      public new void Clear()
      {
         base.Clear();
         backHash.Clear();
      }

      protected override Hash<TKey, TValue> getNewHash() => new BackHash<TKey, TValue>(Comparer);

      public override Hash<TKey, TValue> Subset(params TKey[] keys)
      {
         var newHash = (BackHash<TKey, TValue>)getNewHash();
         foreach (var key in keys)
         {
            newHash[key] = this[key];
         }

         return newHash;
      }

      public void Deconstruct(out Hash<TKey, TValue> forward, out Hash<TValue, TKey> backward)
      {
         forward = new Hash<TKey, TValue>(this, Comparer);
         backward = backHash;
      }
   }
}