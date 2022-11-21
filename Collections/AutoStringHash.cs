using System;
using System.Collections.Generic;
using static Core.Collections.CollectionFunctions;

namespace Core.Collections;

public class AutoStringHash<TValue> : AutoHash<string, TValue>
{
   public static implicit operator StringHash<TValue>(AutoStringHash<TValue> autoStringHash)
   {
      var stringHash = new StringHash<TValue>(autoStringHash.IgnoreCase);
      foreach (var (key, value) in autoStringHash)
      {
         stringHash[key] = value;
      }

      return stringHash;
   }

   public AutoStringHash(bool ignoreCase) : base(stringComparer(ignoreCase))
   {
      IgnoreCase = ignoreCase;
   }

   public AutoStringHash(bool ignoreCase, int capacity) : base(capacity, stringComparer(ignoreCase))
   {
      IgnoreCase = ignoreCase;
   }

   public AutoStringHash(bool ignoreCase, IDictionary<string, TValue> dictionary) : base(dictionary, stringComparer(ignoreCase))
   {
      IgnoreCase = ignoreCase;
   }

   public AutoStringHash(bool ignoreCase, Func<string, TValue> defaultLambda, bool autoAddDefault = false) :
      base(defaultLambda, stringComparer(ignoreCase), autoAddDefault)
   {
      IgnoreCase = ignoreCase;
   }

   public AutoStringHash(bool ignoreCase, TValue defaultValue, bool autoAddDefault = false) :
      base(defaultValue, autoAddDefault, stringComparer(ignoreCase))
   {
      IgnoreCase = ignoreCase;
   }

   public bool IgnoreCase { get; }
}

public class AutoStringHash : AutoStringHash<string>
{
   public static implicit operator StringHash(AutoStringHash autoStringHash)
   {
      var stringHash = new StringHash(autoStringHash.IgnoreCase);
      foreach (var (key, value) in autoStringHash)
      {
         stringHash[key] = value;
      }

      return stringHash;
   }

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