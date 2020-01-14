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
         equatableInfo.Must().Not.BeEmpty().OrThrow("No fields or properties has an EquatableAttribute");
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
                  var value = fieldInfo.GetValue(obj);
                  if (value.IsNotNull())
                  {
                     hash[fieldInfo.Name] = value;
                  }
               }
                  break;
               case PropertyInfo propertyInfo:
               {
                  var value = propertyInfo.GetValue(obj);
                  if (value.IsNotNull())
                  {
                     hash[propertyInfo.Name] = value;
                  }
               }
                  break;
            }
         }

         return hash;
      }

      public virtual bool Equals(EquatableBase other) => other != null && other.GetType() == GetType() && Equals((object)other);

      public override bool Equals(object obj)
      {
         if (obj.IsNull())
         {
            return false;
         }
         else
         {
            var values = getValues(this);
            var otherValues = getValues(obj);
            return equatableInfo
               .Select(mi => mi.Name)
               .Select(signature => values[signature]?.Equals(otherValues[signature]) ?? false)
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