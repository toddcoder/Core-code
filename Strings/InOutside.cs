using System.Collections.Generic;
using System.Text;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Monads.MonadFunctions;

namespace Core.Strings
{
   public class InOutside
   {
      protected string beginPattern;
      protected string endPattern;
      protected string exceptPattern;
      protected IMaybe<string> exceptReplacement;
      protected bool ignoreCase;
      protected bool multiline;
      protected bool friendly;

      protected InOutside(string beginPattern, string endPattern, string exceptPattern, IMaybe<string> exceptReplacement, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         this.beginPattern = $"^{beginPattern}";
         this.endPattern = $"^{endPattern}";
         this.exceptPattern = $"^{exceptPattern}";
         this.exceptReplacement = exceptReplacement;
         this.ignoreCase = ignoreCase;
         this.multiline = multiline;
         this.friendly = friendly;
      }

      public InOutside(string beginPattern, string endPattern, string exceptPattern, string exceptReplacement, bool ignoreCase = false,
         bool multiline = false, bool friendly = true) :
         this(beginPattern, endPattern, exceptPattern, exceptReplacement.Some(), ignoreCase, multiline, friendly) { }

      public InOutside(string beginPattern, string endPattern, string exceptPattern, bool ignoreCase = false, bool multiline = false,
         bool friendly = true) : this(beginPattern, endPattern, exceptPattern, none<string>(), ignoreCase, multiline, friendly) { }

      public IEnumerable<(string segment, int index, InOutsideStatus status)> Enumerable(string source)
      {
         var builder = new StringBuilder();
         var inside = false;
         var current = source;
         var insideStart = 0;
         var outsideStart = 0;
         var matcher = new Matcher(friendly);

         var i = 0;
         while (i < source.Length)
         {
            var ch = source[i];
            current = source.Drop(i);
            if (inside)
            {
               if (matcher.IsMatch(current, exceptPattern, ignoreCase, multiline))
               {
                  builder.Append(exceptReplacement.DefaultTo(() => matcher[0]));
                  i += matcher.Length;
                  continue;
               }
               else if (matcher.IsMatch(current, endPattern, ignoreCase, multiline))
               {
                  yield return (builder.ToString(), insideStart, InOutsideStatus.Outside);
                  yield return (matcher[0], i, InOutsideStatus.EndDelimiter);

                  builder.Clear();
                  inside = false;
                  i += matcher.Length;
                  outsideStart = i;
                  continue;
               }
               else
               {
                  builder.Append(ch);
               }
            }
            else
            {
               if (matcher.IsMatch(current, beginPattern, ignoreCase, multiline))
               {
                  yield return (builder.ToString(), outsideStart, InOutsideStatus.Inside);
                  yield return (matcher[0], i, InOutsideStatus.BeginDelimiter);

                  builder.Clear();
                  inside = true;
                  i += matcher.Length;
                  insideStart = i;
                  continue;
               }
               else
               {
                  builder.Append(ch);
               }
            }

            i++;
         }

         if (builder.Length > 0)
         {
            var rest = builder.ToString();
            if (inside)
            {
               yield return (rest, insideStart, InOutsideStatus.Outside);
            }
            else
            {
               yield return (rest, outsideStart, InOutsideStatus.Inside);
            }
         }
      }
   }
}