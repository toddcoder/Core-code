using System;
using System.Collections.Generic;

namespace Core.Collections
{
   public class StringSet : Set<string>
   {
      protected static StringComparer getStringComparison(bool ignoreCase)
      {
         return ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
      }

      public StringSet(bool ignoreCase) : base(getStringComparison(ignoreCase))
      {
      }

      public StringSet(IEnumerable<string> strings, bool ignoreCase) : base(strings, getStringComparison(ignoreCase))
      {
      }
   }
}