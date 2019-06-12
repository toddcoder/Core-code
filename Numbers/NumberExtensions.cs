using System;
using System.Runtime.InteropServices;
using System.Text;
using Core.Enumerables;
using Core.RegularExpressions;
using Core.Strings;
using static System.Math;
using static Core.Booleans.Assertions;

namespace Core.Numbers
{
   public static class NumberExtensions
   {
      [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
      public static extern long StrFormatByteSize(long size, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

      public static Between<T> Between<T>(this T number, T minimum)
         where T : IComparable<T> => new Between<T>(number, minimum);

      public static bool IsNumeric(this Type type)
      {
         var code = Type.GetTypeCode(type);
         switch (code)
         {
            case TypeCode.Byte:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
               return true;
            default:
               return false;
         }
      }

      public static bool IsNumeric(this object value) => (value as string)?.IsNumeric() ?? value.GetType().IsNumeric();

      public static bool IsNumeric(this string value)
      {
         if (value.IsNotEmpty())
            if (value.IsMatch("^ ['-+']? /d* '.'? /d* (['eE'] ['-+']? /d+)? $"))
               return value != "." && value != "+." && value != "-." && value != "-" && value != "+";
            else
               return value.IsHex();
         else
            return false;
      }

      public static bool IsCommaNumeric(this string value)
      {
         if (value.IsNotEmpty())
            return value.Replace(",", "").IsNumeric();
         else
            return false;
      }

      public static bool IsIntegral(this Type type)
      {
         var code = Type.GetTypeCode(type);
         switch (code)
         {
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
               return true;
            default:
               return false;
         }
      }

      public static bool IsIntegral(this object value) => (value as string)?.IsIntegral() ?? value.GetType().IsIntegral();

      public static bool IsIntegral(this string value)
      {
         if (value.IsNotEmpty())
            return value.IsMatch("^ ['+-']? /d+ $") || value.IsHex();
         else
            return false;
      }

      public static bool IsFloat(this Type type)
      {
         var code = Type.GetTypeCode(type);
         switch (code)
         {
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
               return true;
            default:
               return false;
         }
      }

      public static bool IsFloat(this object value) => (value as string)?.IsFloat() ?? value.GetType().IsFloat();

      public static bool IsFloat(this string value)
      {
         if (value.IsNotEmpty())
            return value.IsMatch("^ ['-+']? /d*  '.' /d* (['eE'] ['-+']? /d+)? $") && value != "." && value != "+." &&
               value != "-." && value != "-" && value != "+";
         else
            return false;
      }

      public static bool IsDouble(this Type type) => Type.GetTypeCode(type) == TypeCode.Double;

      public static bool IsDouble(this object value) => (value as string)?.IsDouble() ?? value.GetType().IsDouble();

      public static bool IsDouble(this string value)
      {
         if (value.IsNotEmpty())
            return value.IsMatch("^ ['-+']? /d*  '.' /d* (['eE'] ['-+']? /d+)? ['dD']? $") && value != "." &&
               value != "+." && value != "-." && value != "-" && value != "+";
         else
            return false;
      }

      public static bool IsSingle(this Type type) => Type.GetTypeCode(type) == TypeCode.Single;

      public static bool IsSingle(this object value) => (value as string)?.IsSingle() ?? value.GetType().IsSingle();

      public static bool IsSingle(this string value)
      {
         if (value.IsNotEmpty())
            return value.IsMatch("^ ['-+']? /d*  '.' /d* (['eE'] ['-+']? /d+)? ['fF']? $") && value != "." &&
               value != "+." && value != "-." && value != "-" && value != "+";
         else
            return false;
      }

      public static bool IsDecimal(this Type type) => Type.GetTypeCode(type) == TypeCode.Decimal;

      public static bool IsDecimal(this object value) => (value as string)?.IsDecimal() ?? value.GetType().IsDecimal();

      public static bool IsDecimal(this string value)
      {
         if (value.IsNotEmpty())
            return value.IsMatch("^ ['-+']? /d*  '.' /d* (['eE'] ['-+']? /d+)? ['mM']? $") && value != "." &&
               value != "+." && value != "-." && value != "-" && value != "-";
         else
            return false;
      }

      public static bool IsHex(this string value) => value.IsMatch("^ ('0x' | '#') ['0-9a-fA-F']+ $");

      public static bool IsEven(this int value) => Abs(value) % 2 == 0;

      public static bool IsEven(this long value) => Abs(value) % 2 == 0;

      public static bool IsEven(this short value) => Abs(value) % 2 == 0;

      public static bool IsEven(this byte value) => Abs(value) % 2 == 0;

      public static bool IsOdd(this int value) => !value.IsEven();

      public static bool IsOdd(this long value) => !value.IsEven();

      public static bool IsOdd(this short value) => !value.IsEven();

      public static bool IsOdd(this byte value) => !value.IsEven();

      public static (double division, double remainder) DivRem(this double dividend, double divisor)
      {
         Assert(divisor != 0.0, "Can't divide by 0");

         var result = dividend / divisor;
         var floor = Floor(result);
         var remainder = result - floor;

         return (floor, remainder);
      }

      public static (int division, int remainder) DivRem(this int dividend, int divisor)
      {
         Assert(divisor != 0, "Can't divide by 0");
         return (dividend / divisor, dividend % divisor);
      }

      public static Comparison<T> ComparedTo<T>(this T left, T right)
         where T : IComparable<T> => new Comparison<T>(left, right);

      public static Range To(this int start, int stop)
      {
         var range = new Range(start);
         range.To(stop);

         return range;
      }

      public static Range Until(this int start, int stop)
      {
         var range = new Range(start);
         range.Until(stop);

         return range;
      }

      public static T MinOf<T>(this T left, T right)
         where T : IComparable<T> => left.CompareTo(right) < 0 ? left : right;

      public static T MaxOf<T>(this T left, T right)
         where T : IComparable<T> => left.CompareTo(right) > 0 ? left : right;

      public static string ByteSize(this long value)
      {
         var builder = new StringBuilder(11);
         StrFormatByteSize(value, builder, builder.Capacity);

         return builder.ToString();
      }
   }
}