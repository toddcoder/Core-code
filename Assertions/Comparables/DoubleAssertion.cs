using System;
using System.ComponentModel;

namespace Core.Assertions.Comparables
{
   public class DoubleAssertion : ComparableAssertion<double>
   {
      static bool nearlyEqual(double d1, object obj, double epsilon)
      {
         var converter = TypeDescriptor.GetConverter(typeof(double));
         if (converter.CanConvertFrom(obj.GetType()))
         {
            var f2 = (double)converter.ConvertTo(obj, typeof(double));
            return nearlyEqual(d1, f2, epsilon);
         }
         else
         {
            return false;
         }
      }

      static bool nearlyEqual(double d1, double d2, double epsilon) => Math.Abs(d1 - d2) < epsilon;

      public DoubleAssertion(IComparable comparable) : base(comparable) { }

      public new DoubleAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      public DoubleAssertion BeNearlyEqual(object obj, double epsilon = 0.00001)
      {
         return (DoubleAssertion)add(obj, c => nearlyEqual(Comparable, obj, epsilon), $"{obj} must $not nearly be equal {comparable}");
      }
   }
}