using Core.Assertions;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Assertions.AssertionFunctions;

namespace Core.Collections
{
   public class StringHash : Hash<string, string>
   {
      public static implicit operator StringHash(string keyValues)
      {
         assert(() => keyValues).Must().Not.BeNull().OrThrow();
         if (keyValues.IsEmpty())
         {
            return new StringHash();
         }

         var destringifier = DelimitedText.BothQuotes();
         var parsed = destringifier.Destringify(keyValues);
         var hash = new StringHash();

         foreach (var keyValue in parsed.Split("/s* ',' /s*"))
         {
            var elements = keyValue.Split("/s* '->' /s*");
            if (elements.Length == 2)
            {
               var key = destringifier.Restringify(elements[0], RestringifyQuotes.None);
               var value = destringifier.Restringify(elements[1], RestringifyQuotes.None);
               hash[key] = value;
            }
         }

         return hash;
      }
   }
}