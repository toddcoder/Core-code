using System;
using System.Runtime.InteropServices;
using System.Text;
using Core.Assertions;
using Core.Enumerables;
using Core.RegexMatching;
using Core.Strings;
using static System.Math;

namespace Core.Numbers
{
   public static class NumberExtensions
   {
      [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
      public static extern long StrFormatByteSize(long size, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

      public static Between<T> Between<T>(this T number, T minimum) where T : IComparable<T> => new(number, minimum);

      public static bool IsNumeric(this Type type)
      {
         var code = Type.GetTypeCode(type);
         return code switch
         {
            TypeCode.Byte => true,
            TypeCode.Decimal => true,
            TypeCode.Double => true,
            TypeCode.Int16 => true,
            TypeCode.Int32 => true,
            TypeCode.Int64 => true,
            TypeCode.SByte => true,
            TypeCode.Single => true,
            TypeCode.UInt16 => true,
            TypeCode.UInt32 => true,
            TypeCode.UInt64 => true,
            _ => false
         };
      }

      public static bool IsNumeric(this object value) => (value as string)?.IsNumeric() ?? value.GetType().IsNumeric();

      public static bool IsNumeric(this string value)
      {
         if (value.IsNotEmpty())
         {
            if (value.IsMatch("^ ['-+']? /d* '.'? /d* (['eE'] ['-+']? /d+)? $; f"))
            {
               return value != "." && value != "+." && value != "-." && value != "-" && value != "+";
            }
            else
            {
               return value.IsHex();
            }
         }
         else
         {
            return false;
         }
      }

      public static bool IsCommaNumeric(this string value) => value.IsNotEmpty() && value.Replace(",", "").IsNumeric();

      public static bool IsIntegral(this Type type)
      {
         var code = Type.GetTypeCode(type);
         return code switch
         {
            TypeCode.Byte => true,
            TypeCode.Int16 => true,
            TypeCode.Int32 => true,
            TypeCode.Int64 => true,
            TypeCode.SByte => true,
            TypeCode.Single => true,
            TypeCode.UInt16 => true,
            TypeCode.UInt32 => true,
            TypeCode.UInt64 => true,
            _ => false
         };
      }

      public static bool IsIntegral(this object value) => (value as string)?.IsIntegral() ?? value.GetType().IsIntegral();

      public static bool IsIntegral(this string value) => value.IsNotEmpty() && (value.IsMatch("^ ['+-']? /d+ $; f") || value.IsHex());

      public static bool IsFloat(this Type type)
      {
         var code = Type.GetTypeCode(type);
         return code switch
         {
            TypeCode.Decimal => true,
            TypeCode.Double => true,
            TypeCode.Single => true,
            _ => false
         };
      }

      public static bool IsFloat(this object value) => (value as string)?.IsFloat() ?? value.GetType().IsFloat();

      public static bool IsFloat(this string value)
      {
         if (value.IsNotEmpty())
         {
            return value.IsMatch("^ ['-+']? /d*  '.' /d* (['eE'] ['-+']? /d+)? $; f") && value != "." && value != "+." &&
               value != "-." && value != "-" && value != "+";
         }
         else
         {
            return false;
         }
      }

      public static bool IsDouble(this Type type) => Type.GetTypeCode(type) == TypeCode.Double;

      public static bool IsDouble(this object value) => (value as string)?.IsDouble() ?? value.GetType().IsDouble();

      public static bool IsDouble(this string value)
      {
         if (value.IsNotEmpty())
         {
            return value.IsMatch("^ ['-+']? /d*  '.' /d* (['eE'] ['-+']? /d+)? ['dD']? $; f") && value != "." &&
               value != "+." && value != "-." && value != "-" && value != "+";
         }
         else
         {
            return false;
         }
      }

      public static bool IsSingle(this Type type) => Type.GetTypeCode(type) == TypeCode.Single;

      public static bool IsSingle(this object value) => (value as string)?.IsSingle() ?? value.GetType().IsSingle();

      public static bool IsSingle(this string value)
      {
         if (value.IsNotEmpty())
         {
            return value.IsMatch("^ ['-+']? /d*  '.' /d* (['eE'] ['-+']? /d+)? ['fF']? $") && value != "." &&
               value != "+." && value != "-." && value != "-" && value != "+";
         }
         else
         {
            return false;
         }
      }

      public static bool IsDecimal(this Type type) => Type.GetTypeCode(type) == TypeCode.Decimal;

      public static bool IsDecimal(this object value) => (value as string)?.IsDecimal() ?? value.GetType().IsDecimal();

      public static bool IsDecimal(this string value)
      {
         if (value.IsNotEmpty())
         {
            return value.IsMatch("^ ['-+']? /d*  '.' /d* (['eE'] ['-+']? /d+)? ['mM']? $; f") && value != "." &&
               value != "+." && value != "-." && value != "-" && value != "-";
         }
         else
         {
            return false;
         }
      }

      public static bool IsHex(this string value) => value.IsMatch("^ ('0x' | '#') ['0-9a-fA-F']+ $; f");

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
         divisor.Must().Not.BeNearlyEqual(0.0).OrThrow();

         var result = dividend / divisor;
         var floor = Floor(result);
         var remainder = result - floor;

         return (floor, remainder);
      }

      public static (int division, int remainder) DivRem(this int dividend, int divisor)
      {
         divisor.Must().Not.BeZero().OrThrow();
         return (dividend / divisor, dividend % divisor);
      }

      public static Comparison<T> ComparedTo<T>(this T left, T right) where T : IComparable<T> => new(left, right);

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

      public static T MinOf<T>(this T left, T right) where T : IComparable<T> => left.CompareTo(right) < 0 ? left : right;

      public static T MaxOf<T>(this T left, T right) where T : IComparable<T> => left.CompareTo(right) > 0 ? left : right;

      public static string ByteSize(this long value)
      {
         var builder = new StringBuilder(11);
         StrFormatByteSize(value, builder, builder.Capacity);

         return builder.ToString();
      }

      public static string ByteCount(this long bytes, bool si)
      {
         var unit = si ? 1000 : 1024;
         var absBytes = bytes == long.MinValue ? long.MaxValue : Abs(bytes);
         if (absBytes < unit)
         {
            return $"{bytes} B";
         }

         var exp = (int)(Log(absBytes) / Log(unit));
         var th = (long)(Pow(unit, exp) * (unit - 0.05));
         if (exp < 6 && absBytes >= th - ((th & 0xfff) == 0xd00 ? 52 : 0))
         {
            exp++;
         }

         var suffix = si ? "kMGTPE"[exp - 1].ToString() : "KMGTPE"[exp - 1] + "i";

         if (exp > 4)
         {
            bytes /= unit;
            exp -= 1;
         }

         return $"{bytes / Pow(unit, exp):F1} {suffix}";
      }
   }
}