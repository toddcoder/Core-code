using System;
using System.Runtime.InteropServices;
using System.Text;
using Core.Dates.Now;
using Core.Objects;
using static Core.Booleans.Assertions;

namespace Core.Strings
{
   public static class StringFunctions
   {
      const string STRING_ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      const string STRING_NUMERIC = "0123456789";
      const int RPC_S_OK = 0;

      [DllImport("rpcrt4.dll", SetLastError = true)]
      static extern int UuidCreateSequential(out Guid guid);

      public static string guid() => Guid.NewGuid().GUID();

      public static string compressedGUID() => Guid.NewGuid().Compressed();

      public static string guidWithoutBrackets() => Guid.NewGuid().WithoutBrackets();

      public static Guid serialGUID()
      {
         var result = UuidCreateSequential(out var guid);
         if (result != RPC_S_OK)
         {
            return Guid.Empty;
         }
         else
         {
            return guid;
         }
      }

      public static string uniqueID() => serialGUID().ToString();

      public static string uniqueHex() => uniqueID().ToLong().ToString("x").ToUpper();

      public static string randomString(int length, bool alpha, bool numeric)
      {
         Assert(alpha || numeric, "You must specify Alpha and/or Numeric");

         var source = "";
         if (alpha)
         {
            source = STRING_ALPHA;
         }

         if (numeric)
         {
            source = source.Append(STRING_NUMERIC);
         }

         var result = new StringBuilder();
         var random = new Random();
         var sourceLength = source.Length;

         for (var i = 0; i < length; i++)
         {
            result.Append(source[random.Next(sourceLength)]);
         }

         return result.ToString();
      }

      public static string splitLiteral(SplitType split)
      {
         switch (split)
         {
            case SplitType.CRLF:
               return "\r\n";
            case SplitType.CR:
               return "\r";
            case SplitType.LF:
               return "\n";
            default:
               return "";
         }
      }

      public static string splitPattern(SplitType split)
      {
         switch (split)
         {
            case SplitType.CRLF:
               return "/r /n";
            case SplitType.CR:
               return "/r";
            case SplitType.LF:
               return "/n";
            default:
               return "";
         }
      }

      public static string uniqueIDFromTime(int index, int indexPadLength)
      {
         return NowServer.Now.ToString("yyyyMMddHHmmss") + index.FormatAs("D." + indexPadLength);
      }

      public static string uniqueIDFromTime() => uniqueIDFromTime(NowServer.Now.Millisecond, 3);
   }
}