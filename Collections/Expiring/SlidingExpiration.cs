using System;

namespace Core.Collections.Expiring;

public class SlidingExpiration<T> : AbsoluteExpiration<T>
{
   public SlidingExpiration(TimeSpan duration) : base(duration) { }

   public override void Reset() => trigger.Reset();
}