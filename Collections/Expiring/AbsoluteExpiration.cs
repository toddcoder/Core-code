using System;
using Core.Dates;

namespace Core.Collections.Expiring;

public class AbsoluteExpiration<T> : ExpirationPolicy<T>
{
   protected Trigger trigger;

   public AbsoluteExpiration(TimeSpan duration) => trigger = new Trigger(duration);

   public override bool ItemEvictable(T value) => trigger.Triggered;

   public override void Reset() { }
}