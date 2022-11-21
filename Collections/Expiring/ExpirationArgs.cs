using System;

namespace Core.Collections.Expiring;

public class ExpirationArgs<TKey, TValue> : EventArgs
{
   public ExpirationArgs(TKey key, TValue value)
   {
      Key = key;
      Value = value;
   }

   public TKey Key { get; }

   public TValue Value { get; }

   public bool CancelEviction { get; set; } = false;
}