using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Collections
{
   public class AutoHash<TKey, TValue> : Hash<TKey, TValue>
   {
      public AutoHash() { }

      public AutoHash(int capacity) : base(capacity) { }

      public AutoHash(IEqualityComparer<TKey> comparer) : base(comparer) { }

      public AutoHash(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

      public AutoHash(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

      public AutoHash(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

      protected AutoHash(SerializationInfo info, StreamingContext context) : base(info, context) { }

      public AutoHash(Func<TKey, TValue> defaultLambda, bool autoAddDefault = false)
      {
         Default = DefaultType.Lambda;
         DefaultLambda = defaultLambda;
         AutoAddDefault = autoAddDefault;
      }

      public AutoHash(TValue defaultValue, bool autoAddDefault = false)
      {
         Default = DefaultType.Value;
         DefaultValue = defaultValue;
         AutoAddDefault = autoAddDefault;
      }

      public DefaultType Default { get; set; } = DefaultType.Value;

      public TValue DefaultValue { get; set; }

      public Func<TKey, TValue> DefaultLambda { get; set; } = key => default;

      public bool AutoAddDefault { get; set; }

      public new TValue this[TKey key]
      {
         get
         {
            TValue result;
            switch (Default)
            {
               case DefaultType.None:
                  return base[key];
               case DefaultType.Value:
                  if (ContainsKey(key))
                     return base[key];
                  else
                  {
                     result = DefaultValue;
                     if (AutoAddDefault)
                        this[key] = result;

                     return result;
                  }
               case DefaultType.Lambda:
                  if (ContainsKey(key))
                     return base[key];
                  else
                  {
                     if (DefaultLambda == null)
                        DefaultLambda = k => default;
                     result = DefaultLambda(key);
                     if (AutoAddDefault)
                        this[key] = result;

                     return result;
                  }
               default:
                  return base[key];
            }
         }
         set => base[key] = value;
      }
   }
}