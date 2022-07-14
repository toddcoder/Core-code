using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Enumerables;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
   public class PropertyEvaluator : IEvaluator, IHash<string, object>, IHash<Signature, object>
   {
      public static void SetValue(object obj, string signature, object value) => new PropertyEvaluator(obj) { [signature] = value };

      public static Maybe<object> GetValue(object obj, string signature)
      {
         var evaluator = new PropertyEvaluator(obj);
         return ((IHash<string, object>)evaluator).Map(signature, o => o);
      }

      protected object obj;
      protected Type type;

      public PropertyEvaluator(object obj)
      {
         this.obj = obj.Must().Not.BeNull().Force<ArgumentNullException, object>();
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
               if (current is null)
               {
                  return null;
               }
               else if (new ObjectInfo(current, s).Value.Map(out var value))
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
            if (value is not null)
            {
               var current = obj;

               Maybe<ObjectInfo> _lastInfo = nil;

               foreach (var info in new SignatureCollection(signature).Select(s => new ObjectInfo(current, s)))
               {
                  if (current == null)
                  {
                     $"{signature} is null; can't continue the chain".Throws();
                  }

                  var infoValue = info.Value.Required($"Signature {signature} doesn't exist");
                  if (!info.PropertyType)
                  {
                     throw $"Couldn't determine object at {signature}".Throws();
                  }

                  current = infoValue;
                  _lastInfo = info;
               }

               var li = _lastInfo.Required($"Couldn't derive {signature}");
               li.Value = value;
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

      Result<Hash<Signature, object>> IHash<Signature, object>.AnyHash()
      {
         var hash = new Hash<Signature, object>();
         var info = obj.GetType().GetProperties();

         foreach (var pInfo in info)
         {
            hash[new Signature(pInfo.Name)] = this[pInfo.Name];
         }

         return hash;
      }

      public object Object
      {
         get => obj;
         set
         {
            value.Must().Not.BeNull().OrThrow();
            obj = value;
         }
      }

      public Type Type(string signature)
      {
         var signatures = new SignatureCollection(signature);
         var result = obj;
         var info = new ObjectInfo();

         foreach (var singleSignature in signatures)
         {
            info = new ObjectInfo(result, singleSignature);
            var value = info.Value;
            result = value.Required($"Signature {singleSignature} not found");
         }

         var propertyType = info.PropertyType.Required($"Signature {signature} not found");

         return propertyType.Must().Not.BeNull().Force();
      }

      public Type Type(Signature signature) => Type(signature.ToString());

      public bool Contains(string signature)
      {
         if (signature.IsMatch("[/w '.[]']+; f"))
         {
            if (signature.Unjoin("'.'; f").All(s => s.IsMatch(Signature.REGEX_FORMAT)))
            {
               var current = obj;

               foreach (var singleSignature in new SignatureCollection(signature))
               {
                  if (current is not null)
                  {
                     if (ObjectInfo.PropertyInfo(current, singleSignature).Map(out var info))
                     {
                        if (new ObjectInfo(current, singleSignature, info).Value.Map(out var value))
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

      public Result<Hash<string, object>> AnyHash()
      {
         var hash = new Hash<string, object>();
         var info = obj.GetType().GetProperties();

         foreach (var pInfo in info)
         {
            hash[pInfo.Name] = this[pInfo.Name];
         }

         return hash;
      }

      public Signature[] Signatures
      {
         get
         {
            return type
               .GetProperties(BindingFlags.Instance | BindingFlags.Public)
               .Select(i => new Signature(i.Name))
               .ToArray();
         }
      }

      protected static bool attributeMatches<TAttribute>(PropertyInfo info) where TAttribute : Attribute
      {
         return info.GetCustomAttributes(true).OfType<TAttribute>().Any();
      }

      public Maybe<T> ValueAtAttribute<TAttribute, T>() where TAttribute : Attribute
      {
         var properties = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(attributeMatches<TAttribute>)
            .Select(i => new Signature(i.Name));

         foreach (var signature in properties)
         {
            return ((IHash<Signature, object>)this).Map(signature, o => (T)o);
         }

         return nil;
      }

      public IEnumerable<Signature> ValuesAtAttribute<TAttribute>() where TAttribute : Attribute
      {
         return type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(attributeMatches<TAttribute>)
            .Select(i => new Signature(i.Name));
      }

      public Maybe<TAttribute> Attribute<TAttribute>(string signature) where TAttribute : Attribute
      {
         return Attribute<TAttribute>(new Signature(signature));
      }

      public Maybe<TAttribute> Attribute<TAttribute>(Signature signature) where TAttribute : Attribute
      {
         var info = ObjectInfo.PropertyInfo(obj, signature);
         return info.Map(i => i.GetCustomAttributes(true).OfType<TAttribute>().FirstOrNone());
      }

      public IEnumerable<Signature> WithAttribute<TAttribute>() where TAttribute : Attribute
      {
         return type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(attributeMatches<TAttribute>)
            .Select(p => new Signature(p.Name));
      }

      public PropertyEvaluatorTrying TryTo => new(this);

      public PropertyEvaluator Evaluator(Signature signature) => new(this[signature]);

      public PropertyEvaluator Evaluator(string signature) => new(this[signature]);
   }
}