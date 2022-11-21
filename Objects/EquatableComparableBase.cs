using System;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public class EquatableComparableBase : EquatableBase, IComparable
{
   public static bool operator <(EquatableComparableBase lhs, EquatableComparableBase rhs) => lhs.CompareTo(rhs) < 0;

   public static bool operator <=(EquatableComparableBase lhs, EquatableComparableBase rhs) => lhs.CompareTo(rhs) <= 0;

   public static bool operator >(EquatableComparableBase lhs, EquatableComparableBase rhs) => lhs.CompareTo(rhs) > 0;

   public static bool operator >=(EquatableComparableBase lhs, EquatableComparableBase rhs) => lhs.CompareTo(rhs) >= 0;

   protected MemberInfo[] comparableInfo;

   public EquatableComparableBase()
   {
      var type = GetType();
      var fieldSignatures = type.GetRuntimeFields()
         .Select(fi => (info: fi, attr: fi.GetCustomAttributes(typeof(ComparableAttribute), true)))
         .Where(t => t.attr.Length > 0)
         .OrderBy(t => ((ComparableAttribute)t.attr[0]).Order)
         .Select(t => t.info)
         .Cast<MemberInfo>();
      var propertySignatures = type.GetRuntimeProperties()
         .Select(pi => (info: pi, attr: pi.GetCustomAttributes(typeof(ComparableAttribute), true)))
         .Where(t => t.attr.Length > 0)
         .OrderBy(t => ((ComparableAttribute)t.attr[0]).Order)
         .Select(t => t.info)
         .Cast<MemberInfo>();
      comparableInfo = fieldSignatures.Union(propertySignatures).ToArray();
      comparableInfo.Must().Not.BeEmpty().OrThrow("No fields or properties has a ComparableAttribute");
   }

   protected static int compareField(object left, object right, MemberInfo memberInfo)
   {
      switch (memberInfo)
      {
         case FieldInfo fieldInfo:
         {
            var leftValue = fieldInfo.GetValue(left);
            var rightValue = fieldInfo.GetValue(right);
            if (leftValue is IComparable leftComparable)
            {
               return leftComparable.CompareTo(rightValue);
            }
            else
            {
               throw fail($"{leftValue} must be comparable");
            }
         }
         case PropertyInfo propertyInfo:
         {
            var leftValue = propertyInfo.GetValue(left);
            var rightValue = propertyInfo.GetValue(right);
            if (leftValue is IComparable leftComparable)
            {
               return leftComparable.CompareTo(rightValue);
            }
            else
            {
               throw fail($"{leftValue} must be comparable");
            }
         }
         default:
            throw fail("Must be field info or property info");
      }
   }

   public virtual int CompareTo(object obj)
   {
      obj.Must().BeOfType(GetType()).OrThrow();

      foreach (var memberInfo in comparableInfo)
      {
         var result = compareField(this, obj, memberInfo);
         if (result != 0)
         {
            return result;
         }
      }

      return 0;
   }

   protected bool Equals(EquatableComparableBase other) => base.Equals(other);

   // ReSharper disable once RedundantOverriddenMember
   public override bool Equals(object obj) => base.Equals(obj);

   public override int GetHashCode() => base.GetHashCode();
}