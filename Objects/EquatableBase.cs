using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using Core.Collections;

namespace Core.Objects
{
   public class EquatableBase : IEquatable<EquatableBase>
   {
      protected string[] signatures;

      public EquatableBase()
      {
         var type = GetType();
         var fieldSignatures = type.GetRuntimeFields()
            .Where(fi => fi.GetCustomAttributes(typeof(EquatableAttribute), true).Length > 0)
            .Select(fi => fi.Name);
         var propertySignatures = type.GetRuntimeProperties()
            .Where(pi => pi.GetCustomAttributes(typeof(EquatableAttribute), true).Length > 0)
            .Select(pi => pi.Name);
         var all = new List<string>(fieldSignatures);
         all.AddRange(propertySignatures);
         signatures = all.ToArray();
         signatures.Must().Not.BeEmpty().Assert("No fields or properties has an EquatableAttribute");
      }

      protected Hash<string, object> getValues(object obj, string[] signatures)
      {
         var type = GetType();
         var fieldValues = type.GetRuntimeFields()
            .Where(fi => signatures.Contains(fi.Name))
            .Select(fi => (signature: fi.Name, value: fi.GetValue(obj)))
            .ToHash(t => t.signature, t => t.value);
         var propertyValues = type.GetRuntimeProperties()
            .Where(pi => signatures.Contains(pi.Name))
            .Select(pi => (signature: pi.Name, value: pi.GetValue(obj)))
            .ToHash(t => t.signature, t => t.value);

         return fieldValues.Merge(propertyValues);
      }

      public bool Equals(EquatableBase other) => other != null && other.GetType() == GetType() && Equals((object)other);

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, this))
         {
            return false;
         }
         else if (ReferenceEquals(this, obj))
         {
            return true;
         }
         else
         {
            var values = getValues(this, signatures);
            var otherValues = getValues(obj, signatures);
            return signatures
               .Select(signature => values[signature].Equals(otherValues[signature]))
               .All(b => b);
         }
      }

      public override int GetHashCode()
      {
         unchecked
         {
            var values = getValues(this, signatures);
            return values.Values
               .Select(value => value?.GetHashCode() ?? 0)
               .Aggregate(397, (current, value) => current * 397 ^ value);
         }
      }
   }
}