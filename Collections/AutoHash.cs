using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Core.Assertions;

namespace Core.Collections
{
   public class AutoHash<TKey, TValue> : Hash<TKey, TValue>
   {
      Func<TKey, TValue> defaultLambda;

      public AutoHash()
      {
         defaultLambda = key => default;
      }

      public AutoHash(int capacity) : base(capacity)
      {
         defaultLambda = key => default;
      }

      public AutoHash(IEqualityComparer<TKey> comparer) : base(comparer)
      {
         defaultLambda = key => default;
      }

      public AutoHash(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
      {
         defaultLambda = key => default;
      }

      public AutoHash(IDictionary<TKey, TValue> dictionary) : base(dictionary)
      {
         defaultLambda = key => default;
      }

      public AutoHash(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
      {
         defaultLambda = key => default;
      }

      protected AutoHash(SerializationInfo info, StreamingContext context) : base(info, context)
      {
         defaultLambda = key => default;
      }

      public AutoHash(Func<TKey, TValue> defaultLambda, bool autoAddDefault = false)
      {
         defaultLambda.Must().Not.BeNull().Assert();

         Default = DefaultType.Lambda;
         this.defaultLambda = defaultLambda;
         AutoAddDefault = autoAddDefault;
      }

      public AutoHash(Func<TKey, TValue> defaultLambda, bool autoAddDefault, IEqualityComparer<TKey> comparer) : this(comparer)
      {
         defaultLambda.Must().Not.BeNull().Assert();
         comparer.Must().Not.BeNull().Assert();

         Default = DefaultType.Lambda;
         this.defaultLambda = defaultLambda;
         AutoAddDefault = autoAddDefault;
      }

      public AutoHash(TValue defaultValue, bool autoAddDefault = false)
      {
         Default = DefaultType.Value;
         DefaultValue = defaultValue;
         AutoAddDefault = autoAddDefault;

         defaultLambda = key => default;
      }

      public AutoHash(TValue defaultValue, bool autoAddDefault, IEqualityComparer<TKey> comparer) : this(comparer)
      {
         comparer.Must().Not.BeNull().Assert();

         Default = DefaultType.Value;
         DefaultValue = defaultValue;
         AutoAddDefault = autoAddDefault;

         defaultLambda = key => default;
      }

      public DefaultType Default { get; set; } = DefaultType.Value;

      public TValue DefaultValue { get; set; }

      public Func<TKey, TValue> DefaultLambda
      {
         get => defaultLambda;
         set
         {
            value.Must().Not.BeNull().Assert();
            defaultLambda = value;
         }
      }

      public bool AutoAddDefault { get; set; }

      public new TValue this[TKey key]
      {
         get
         {
            key.Must().Not.BeNull().Assert();

            TValue result;
            switch (Default)
            {
               case DefaultType.None:
                  return base[key];
               case DefaultType.Value:
                  if (ContainsKey(key))
                  {
                     return base[key];
                  }
                  else
                  {
                     result = DefaultValue;
                     if (AutoAddDefault)
                     {
                        this[key] = result;
                     }

                     return result;
                  }

               case DefaultType.Lambda:
                  if (ContainsKey(key))
                  {
                     return base[key];
                  }
                  else
                  {
                     result = DefaultLambda(key);
                     if (AutoAddDefault)
                     {
                        this[key] = result;
                     }

                     return result;
                  }

               default:
                  return base[key];
            }
         }
         set => base[key] = value;
      }

      public void AddKeys(IEnumerable<TKey> keys)
      {
         Func<TKey, TValue> valueFunc;
         switch (Default)
         {
            case DefaultType.None:
               valueFunc = k => default;
               break;
            case DefaultType.Value:
               valueFunc = k => DefaultValue;
               break;
            case DefaultType.Lambda:
               valueFunc = DefaultLambda;
               break;
            default:
               return;
         }

         foreach (var key in keys.Where(k => k != null && !ContainsKey(k)))
         {
            this[key] = valueFunc(key);
         }
      }
   }
}