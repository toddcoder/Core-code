using System;

namespace Core.Collections;

public class HashArgs<TKey, TValue> : EventArgs
{
   public HashArgs(TKey key, TValue value)
   {
      Key = key;
      Value = value;
   }

   public TKey Key { get; }

   public TValue Value { get; }
}