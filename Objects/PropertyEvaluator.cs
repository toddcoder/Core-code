using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Booleans.Assertions;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
   public class PropertyEvaluator : IEvaluator, IHash<string, object>, IHash<Signature, object>
   {
      public static void SetValue(object obj, string signature, object value)
      {
         new PropertyEvaluator(obj) { [signature] = value };
      }

      public static IMaybe<object> GetValue(object obj, string signature)
      {
         var evaluator = new PropertyEvaluator(obj);
         return ((IHash<string, object>)evaluator).Map(signature, o => o);
      }

      object obj;
      Type type;

      public PropertyEvaluator(object obj)
      {
         Assert<ArgumentNullException>(obj.IsNotNull(), "Object set to evaluator can't be null");

         this.obj = obj;
         type = this.obj.GetType();
      }

      public IHash<string, object> Hash => this;

      public object this[string signature]
      {
         get
         {
            var current = obj;

            foreach (var s in new SignatureCollection(signature))
            {
               if (current.IsNull())
               {
                  return null;
               }
               else if (new ObjectInfo(current, s).Value.If(out var value))
               {
                  current = value;
               }
               else
               {
                  return null;
               }
            }

            return current;
         }
         set
         {
            if (value.IsNotNull())
            {
               var current = obj;

               var lastInfo = none<ObjectInfo>();

               foreach (var info in new SignatureCollection(signature).Select(s => new ObjectInfo(current, s)))
               {
                  Assert(current.IsNotNull(), $"Object at signature {signature} is null; can't continue the chain");
                  Assert(info.PropertyType.IsSome, $"Couldn't determine object at {signature}");
                  var infoValue = info.Value.Required($"Signature {signature} doesn't exist");
                  current = infoValue;
                  lastInfo = info.Some();
               }

               var li = lastInfo.Required($"Couldn't derive {signature}");
               li.Value = value.Some();
            }
         }
      }

      public bool ContainsKey(string key) => Contains(key);

      public object this[Signature signature]
      {
         get => this[signature.Name];
         set => this[signature.Name] = value;
      }

      public bool ContainsKey(Signature key) => Contains(key.Name);

      public object Object
      {
         get => obj;
         set => obj = value;
      }

      public Type Type(string signature)
      {
         var signatures = new SignatureCollection(signature);
         var result = obj;
         var info = new ObjectInfo();

         foreach (var singleSignature in signatures)
         {
            Assert(result.IsNotNull(), $"Null value returned by {singleSignature}");

            info = new ObjectInfo(result, singleSignature);
            var value = info.Value;
            result = value.Required($"Signature {singleSignature} not found");
         }

         var propertyType = info.PropertyType.Required($"Signature {signature} not found");

         return Ensure(propertyType, pt => pt.IsNotNull(), $"Null value returned by {signature}");
      }

      public Type Type(Signature signature) => Type(signature.ToString());

      public bool Contains(string signature)
      {
         if (signature.IsMatch("[/w '.[]']+"))
         {
            if (signature.Split("'.'").All(s => s.IsMatch(Signature.REGEX_FORMAT)))
            {
               var current = obj;

               foreach (var singleSignature in new SignatureCollection(signature))
               {
                  if (current.IsNotNull())
                  {
                     if (ObjectInfo.PropertyInfo(current, singleSignature).If(out var info))
                     {
                        if (new ObjectInfo(current, singleSignature, info).Value.If(out var value))
                        {
                           current = value;
                        }
                        else
                        {
                           return false;
                        }
                     }
                     else
                     {
                        return false;
                     }
                  }
                  else
                  {
                     return false;
                  }
               }

               return true;
            }
            else
            {
               return false;
            }
         }
         else
         {
            return false;
         }
      }

      public Hash<string, object> ToHash()
      {
         var hash = new Hash<string, object>();
         var info = obj.GetType().GetProperties();

         foreach (var pInfo in info)
         {
            hash[pInfo.Name] = this[pInfo.Name];
         }

         return hash;
      }

      public Signature[] Signatures => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
         .Select(i => new Signature(i.Name))
         .ToArray();

      static bool attributeMatches<TAttribute>(PropertyInfo info)
         where TAttribute : Attribute => info.GetCustomAttributes(true).OfType<TAttribute>().Any();

      public IMaybe<T> ValueAtAttribute<TAttribute, T>()
         where TAttribute : Attribute
      {
         var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(attributeMatches<TAttribute>)
            .Select(i => new Signature(i.Name));

         foreach (var signature in properties)
         {
            return ((IHash<Signature, object>)this).Map(signature, o => (T)o);
         }

         return none<T>();
      }

      public IEnumerable<Signature> ValuesAtAttribute<TAttribute>()
         where TAttribute : Attribute
      {
         return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(attributeMatches<TAttribute>)
            .Select(i => new Signature(i.Name));
      }

      public IMaybe<TAttribute> Attribute<TAttribute>(string signature)
         where TAttribute : Attribute => Attribute<TAttribute>(new Signature(signature));

      public IMaybe<TAttribute> Attribute<TAttribute>(Signature signature)
         where TAttribute : Attribute
      {
         var info = ObjectInfo.PropertyInfo(obj, signature);
         return info.Map(i => i.GetCustomAttributes(true).OfType<TAttribute>().FirstOrNone());
      }

      public IEnumerable<Signature> WithAttribute<TAttribute>()
         where TAttribute : Attribute
      {
         return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(attributeMatches<TAttribute>)
            .Select(p => new Signature(p.Name));
      }

      public PropertyEvaluatorTrying TryTo => new PropertyEvaluatorTrying(this);
   }
}