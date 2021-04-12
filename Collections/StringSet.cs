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

      public StringSet(bool ignoreCase, IEnumerable<string> strings) : base(strings, getStringComparison(ignoreCase))
      {
      }

      public StringSet(bool ignoreCase, params string[] strings) : base(strings, getStringComparison(ignoreCase))
      {
      }
   }
}