using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Core.Regex.RegexExtensions;

namespace Core.Strings
{
   public class Transformer
   {
      protected InOutside inOutside;

      public Transformer(string beginPattern, string endPattern, string exceptPattern, string exceptReplacement, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         inOutside = new InOutside(beginPattern, endPattern, exceptPattern, exceptReplacement, ignoreCase, multiline, friendly);
         Map = none<Func<string, string>>();
      }

      public Transformer(string beginPattern, string endPattern, string exceptPattern, bool ignoreCase = false, bool multiline = false,
         bool friendly = true)
      {
         inOutside = new InOutside(beginPattern, endPattern, exceptPattern, ignoreCase, multiline, friendly);
         Map = none<Func<string, string>>();
      }

      public string Transform(string source, string pattern, string replacement, bool ignoreCase = false)
      {
         var outside = inOutside.Enumerable(source).Where(t => t.status == InOutsideStatus.Outside).ToArray();
         var startIndex = 0;

         IMaybe<int> findInOutside(string target)
         {
            foreach (var (text, index, _) in outside)
            {
               if (text.Find(target, ignoreCase: ignoreCase).If(out var foundIndex))
               {
                  return (foundIndex + index).Some();
               }
            }

            return none<int>();
         }

         var slices = pattern.SliceSplit(@"\$\d+");
         var values = new List<string>();

         foreach (var (text, sliceIndex, length) in slices.Where(s => s.Length > 0))
         {
            if (sliceIndex == 0)
            {
               startIndex = sliceIndex + length;
            }
            else if (findInOutside(text).If(out var index))
            {
               var item = source.Drop(startIndex).Keep(index - startIndex);
               item = Map.Map(m => m(item)).DefaultTo(() => item);
               values.Add(item);
               startIndex = index + length;
            }
            else
            {
               return source;
            }
         }

         var rest = source.Drop(startIndex);
         if (rest.Length > 0)
         {
            rest = Map.Map(m => m(rest)).DefaultTo(() => rest);
            values.Add(rest);
         }

         var builder = new StringBuilder(replacement);
         for (var i = 0; i < values.Count; i++)
         {
            var target = $"${i}";
            builder.Replace(target, values[i]);
         }

         return builder.ToString();
      }

      public IMaybe<Func<string, string>> Map { get; set; }
   }
}