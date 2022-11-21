using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Collections;

public class TwoKeyHash<TKey1, TKey2, TValue> : Hash<TKey1, TValue>
{
   protected BackHash<TKey1, TKey2> backHash;

   public TwoKeyHash() => backHash = new BackHash<TKey1, TKey2>();

   public TwoKeyHash(int capacity) : base(capacity) => backHash = new BackHash<TKey1, TKey2>();

   public TwoKeyHash(IEqualityComparer<TKey1> comparer) : base(comparer) => backHash = new BackHash<TKey1, TKey2>();

   public TwoKeyHash(int capacity, IEqualityComparer<TKey1> comparer) : base(capacity, comparer)
   {
      backHash = new BackHash<TKey1, TKey2>();
   }

   public TwoKeyHash(IDictionary<TKey1, TValue> dictionary) : base(dictionary) => backHash = new BackHash<TKey1, TKey2>();

   public TwoKeyHash(IDictionary<TKey1, TValue> dictionary, IEqualityComparer<TKey1> comparer) : base(dictionary, comparer)
   {
      backHash = new BackHash<TKey1, TKey2>();
   }

   protected TwoKeyHash(SerializationInfo info, StreamingContext context) : base(info, context)
   {
      backHash = new BackHash<TKey1, TKey2>();
   }

   public TValue this[TKey1 key1, TKey2 key2]
   {
      get => base[key1];
      set
      {
         base[key1] = value;
         backHash[key1] = key2;
      }
   }

   public TValue this[TKey2 alternateKey] => base[backHash.Back[alternateKey]];

   public bool ContainsAlternateKey(TKey2 key) => backHash.Back.ContainsKey(key);

   public new void Clear()
   {
      base.Clear();
      backHash.Clear();
   }
}