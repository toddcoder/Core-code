using System;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using Core.Collections;

namespace Core.Objects
{
   public class EquatableBase : IEquatable<EquatableBase>
   {
      public static bool operator ==(EquatableBase lhs, EquatableBase rhs) => lhs.Equals(rhs);

      public static bool operator !=(EquatableBase lhs, EquatableBase rhs) => !lhs.Equals(rhs);

      protected MemberInfo[] equatableInfo;

      public EquatableBase()
      {
         var type = GetType();
         var fieldSignatures = type.GetRuntimeFields()
            .Where(fi => fi.GetCustomAttributes(typeof(EquatableAttribute), true).Length > 0)
            .Cast<MemberInfo>();
         var propertySignatures = type.GetRuntimeProperties()
            .Where(pi => pi.GetCustomAttributes(typeof(EquatableAttribute), true).Length > 0)
            .Cast<MemberInfo>();
         equatableInfo = fieldSignatures.Union(propertySignatures).ToArray();
         equatableInfo.Must().Not.BeEmpty().Assert("No fields or properties has an EquatableAttribute");
      }

      protected Hash<string, object> getValues(object obj)
      {
         var hash = new Hash<string, object>();

         foreach (var memberInfo in equatableInfo)
         {
            switch (memberInfo)
            {
               case FieldInfo fieldInfo:
               {
                  hash[fieldInfo.Name] = fieldInfo.GetValue(obj);
               }
                  break;
               case PropertyInfo propertyInfo:
               {
                  hash[propertyInfo.Name] = propertyInfo.GetValue(obj);
               }
                  break;
            }
         }

         return hash;
      }

      public virtual bool Equals(EquatableBase other) => other != null && other.GetType() == GetType() && Equals((object)other);

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
            var values = getValues(this);
            var otherValues = getValues(obj);
            return equatableInfo
               .Select(mi => mi.Name)
               .Select(signature => values[signature].Equals(otherValues[signature]))
               .All(b => b) && equals(obj);
         }
      }

      protected virtual bool equals(object other) => true;

      public override int GetHashCode()
      {
         unchecked
         {
            var values = getValues(this);
            return values.Values
               .Select(value => value?.GetHashCode() ?? 0)
               .Aggregate(397, (current, value) => current * 397 ^ value);
         }
      }
   }
}