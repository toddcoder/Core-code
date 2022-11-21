using Core.Assertions;
using Core.Matching;
using Core.Strings;

namespace Core.Collections;

public class StringStringHash : Hash<string, string>
{
   public static implicit operator StringStringHash(string keyValues)
   {
      keyValues.Must().Not.BeNull().OrThrow();
      if (keyValues.IsEmpty())
      {
         return new StringStringHash();
      }

      var destringifier = DelimitedText.BothQuotes();
      var parsed = destringifier.Destringify(keyValues);
      var hash = new StringStringHash();

      foreach (var keyValue in parsed.Unjoin("/s* ',' /s*; f"))
      {
         var elements = keyValue.Unjoin("/s* '->' /s*; f");
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