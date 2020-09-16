using System;
using System.Linq;
using System.Reflection;
using Core.Numbers;

namespace Core.Enums
{
   public static class EnumExtensions
   {
      public static bool In<TEnum>(this TEnum @enum, params TEnum[] args) where TEnum : struct, Enum
      {
         if (typeof(TEnum).IsDefined(typeof(FlagsAttribute)))
         {
            Bits32<TEnum> bits = @enum;
            return args.Any(e => bits[e]);
         }
         else
         {
            return args.Any(e => e.Equals(@enum));
         }
      }
   }
}