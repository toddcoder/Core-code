using System;
using System.Collections.Generic;

namespace Core.Collections
{
   public class StringHash<TValue> : Hash<string, TValue>
   {
      protected static StringComparer getStringComparison(bool ignoreCase)
      {
         return ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
      }

      public StringHash(bool ignoreCase) : base(getStringComparison(ignoreCase))
      {
      }

      public StringHash() : this(false)
      {
      }

      public StringHash(bool ignoreCase, int capacity) : base(capacity, getStringComparison(ignoreCase))
      {
      }

      public StringHash(int capacity) : this(false, capacity)
      {
      }

      public StringHash(bool ignoreCase, IDictionary<string, TValue> dictionary) : base(dictionary, getStringComparison(ignoreCase))
      {
      }

      public StringHash(IDictionary<string, TValue> dictionary) : this(false, dictionary)
      {
      }
   }
}