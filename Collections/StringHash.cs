using System.Collections.Generic;
using static Core.Collections.CollectionFunctions;

namespace Core.Collections
{
   public class StringHash<TValue> : Hash<string, TValue>
   {
      public StringHash(bool ignoreCase) : base(stringComparer(ignoreCase))
      {
      }

      public StringHash() : this(false)
      {
      }

      public StringHash(bool ignoreCase, int capacity) : base(capacity, stringComparer(ignoreCase))
      {
      }

      public StringHash(int capacity) : this(false, capacity)
      {
      }

      public StringHash(bool ignoreCase, IDictionary<string, TValue> dictionary) : base(dictionary, stringComparer(ignoreCase))
      {
      }

      public StringHash(IDictionary<string, TValue> dictionary) : this(false, dictionary)
      {
      }
   }
}