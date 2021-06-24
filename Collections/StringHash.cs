using System.Collections.Generic;
using static Core.Collections.CollectionFunctions;

namespace Core.Collections
{
   public class StringHash<TValue> : Hash<string, TValue>
   {
      protected bool ignoreCase;

      public StringHash(bool ignoreCase) : base(stringComparer(ignoreCase))
      {
         this.ignoreCase = ignoreCase;
      }

      public StringHash(bool ignoreCase, int capacity) : base(capacity, stringComparer(ignoreCase))
      {
         this.ignoreCase = ignoreCase;
      }

      public StringHash(bool ignoreCase, IDictionary<string, TValue> dictionary) : base(dictionary, stringComparer(ignoreCase))
      {
         this.ignoreCase = ignoreCase;
      }

      public bool IgnoreCase => ignoreCase;

      public Hash<string, TValue> AsHash => this;
   }

   public class StringHash : StringHash<string>
   {
      public StringHash(bool ignoreCase) : base(ignoreCase)
      {
      }

      public StringHash(bool ignoreCase, int capacity) : base(ignoreCase, capacity)
      {
      }

      public StringHash(bool ignoreCase, IDictionary<string, string> dictionary) : base(ignoreCase, dictionary)
      {
      }
   }
}