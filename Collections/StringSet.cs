using System;
using System.Collections.Generic;
using static Core.Collections.CollectionFunctions;

namespace Core.Collections
{
   public class StringSet : Set<string>
   {
      public StringSet(bool ignoreCase) : base(stringComparer(ignoreCase))
      {
      }

      public StringSet(bool ignoreCase, IEnumerable<string> strings) : base(strings, stringComparer(ignoreCase))
      {
      }

      public StringSet(bool ignoreCase, params string[] strings) : base(strings, stringComparer(ignoreCase))
      {
      }
   }
}