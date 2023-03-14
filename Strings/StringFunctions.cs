using System;
using System.Runtime.InteropServices;
using System.Text;
using Core.Assertions;
using Core.Dates.Now;
using Core.Objects;
using static Core.Objects.ConversionFunctions;

namespace Core.Strings;

public static class StringFunctions
{
   private const string STRING_ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
   private const string STRING_NUMERIC = "0123456789";
   private const int RPC_S_OK = 0;

   [DllImport("rpcrt4.dll", SetLastError = true)]
   private static extern int UuidCreateSequential(out Guid guid);

   public static string guid() => Guid.NewGuid().GUID();

   public static string compressedGUID() => Guid.NewGuid().Compressed();

   public static string guidWithoutBrackets() => Guid.NewGuid().WithoutBrackets();

   public static Guid serialGUID()
   {
      var result = UuidCreateSequential(out var guid);
      return result == RPC_S_OK ? guid : Guid.Empty;
   }

   public static string uniqueID() => serialGUID().ToString();

   public static string uniqueHex() => Value.Int64(uniqueID()).ToString("x").ToUpper();

   public static string randomString(int length, bool alpha, bool numeric)
   {
      alpha.Must().Or(numeric).OrThrow("You must specify Alpha and/or Numeric");

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

   public static string splitLiteral(SplitType split) => split switch
   {
      SplitType.CRLF => "\r\n",
      SplitType.CR => "\r",
      SplitType.LF => "\n",
      _ => ""
   };

   public static string splitPattern(SplitType split) => split switch
   {
      SplitType.CRLF => "/r /n; f",
      SplitType.CR => "/r; f",
      SplitType.LF => "/n; f",
      _ => ""
   };

   public static string uniqueIDFromTime(int index, int indexPadLength)
   {
      return NowServer.Now.ToString("yyyyMMddHHmmss") + index.FormatAs("D." + indexPadLength);
   }

   public static string uniqueIDFromTime() => uniqueIDFromTime(NowServer.Now.Millisecond, 3);
}