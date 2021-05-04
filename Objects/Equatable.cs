using System;
using System.Linq;
using System.Reflection;
using Core.Arrays;
using Core.Assertions;
using Core.Collections;
using Core.Enumerables;

namespace Core.Objects
{
   [Obsolete("Do not use this class for performance reasons")]
   public class Equatable<T> : IEquatable<T>
   {
      protected static Hash<string, object> getValues(T obj, string[] signatures)
      {
         var fieldValues = typeof(T).GetRuntimeFields()
            .Where(fi => signatures.Contains(fi.Name))
            .Select(fi => (signature: fi.Name, value: fi.GetValue(obj)))
            .Where(t => t.value is not null)
            .ToStringHash(t => t.signature, t => t.value, true);
         var propertyValues = typeof(T).GetRuntimeProperties()
            .Where(pi => signatures.Contains(pi.Name))
            .Select(pi => (signature: pi.Name, value: pi.GetValue(obj)))
            .Where(t => t.value is not null)
            .ToStringHash(t => t.signature, t => t.value, true);

         var hash = fieldValues.Merge(propertyValues);

         var notFoundSignatures = signatures.Where(signature => !hash.ContainsKey(signature)).ToArray();
         notFoundSignatures.Must().Not.BeEmpty().OrThrow(() => $"Didn't find fields or property {notFoundSignatures.Andify()}");

         return hash;
      }

      public static int HashCode(params object[] values)
      {
         unchecked
         {
            return values.Select(value => value?.GetHashCode() ?? 0).Aggregate(397, (current, value) => current * 397 ^ value);
         }
      }

      protected T obj;
      protected string[] signatures;

      public Equatable(T obj, params string[] signatures)
      {
         this.obj = obj;
         this.signatures = signatures;
      }

      public bool Equals(T other)
      {
         if (other is null)
         {
            return false;
         }
         else
         {
            var values = getValues(obj, signatures);
            var otherValues = getValues(other, signatures);
            return signatures
               .Select(signature => values[signature].Equals(otherValues[signature]))
               .All(b => b);
         }
      }

      public override bool Equals(object other) => other is T tOther && Equals(tOther);

      public override int GetHashCode()
      {
         unchecked
         {
            var values = getValues(obj, signatures);
            return values.Values
               .Select(value => value?.GetHashCode() ?? 0)
               .Aggregate(397, (current, value) => current * 397 ^ value);
         }
      }

      public string Keys => getValues(obj, signatures).Select(i => $"{i.Key}=>{(i.Value is null ? "null" : i.Value)}").ToString(", ");
   }
}