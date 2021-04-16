using System;
using System.Collections.Generic;
using static Core.Collections.CollectionFunctions;

namespace Core.Collections
{
   public class AutoStringHash<TValue> : AutoHash<string, TValue>
   {
      public AutoStringHash(bool ignoreCase) : base(stringComparer(ignoreCase))
      {
      }

      public AutoStringHash(bool ignoreCase, int capacity) : base(capacity, stringComparer(ignoreCase))
      {
      }

      public AutoStringHash(bool ignoreCase, IDictionary<string, TValue> dictionary) : base(dictionary, stringComparer(ignoreCase))
      {
      }

      public AutoStringHash(bool ignoreCase, Func<string, TValue> defaultLambda, bool autoAddDefault = false) :
         base(defaultLambda, stringComparer(ignoreCase), autoAddDefault)
      {
      }

      public AutoStringHash(bool ignoreCase, TValue defaultValue, bool autoAddDefault = false) :
         base(defaultValue, autoAddDefault, stringComparer(ignoreCase))
      {
      }
   }
}