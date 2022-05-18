using System;
using System.Collections.Generic;

namespace Core.Collections
{
   [Obsolete("Use StringSet")]
   public class FastStringSet : FastSet<string>
   {
      protected bool ignoreCase;

      protected static IEqualityComparer<string> getEqualityComparer(bool ignoreCase)
      {
         return ignoreCase ? new IgnoreCaseEqualityComparer() : EqualityComparer<string>.Default;
      }

      public FastStringSet(bool ignoreCase) : base(getEqualityComparer(ignoreCase))
      {
         this.ignoreCase = ignoreCase;
      }

      public FastStringSet(bool ignoreCase, IEnumerable<string> items) : base(getEqualityComparer(ignoreCase), items)
      {
         this.ignoreCase = ignoreCase;
      }

      public FastStringSet(bool ignoreCase, params string[] items) : base(getEqualityComparer(ignoreCase), items)
      {
         this.ignoreCase = ignoreCase;
      }

      public bool IgnoreCase => ignoreCase;
   }
}