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

   public class AutoStringHash : AutoStringHash<string>
   {
      public AutoStringHash(bool ignoreCase) : base(ignoreCase)
      {
      }

      public AutoStringHash(bool ignoreCase, int capacity) : base(ignoreCase, capacity)
      {
      }

      public AutoStringHash(bool ignoreCase, IDictionary<string, string> dictionary) : base(ignoreCase, dictionary)
      {
      }

      public AutoStringHash(bool ignoreCase, Func<string, string> defaultLambda, bool autoAddDefault = false) : base(ignoreCase, defaultLambda,
         autoAddDefault)
      {
      }

      public AutoStringHash(bool ignoreCase, string defaultValue, bool autoAddDefault = false) : base(ignoreCase, defaultValue, autoAddDefault)
      {
      }
   }
}