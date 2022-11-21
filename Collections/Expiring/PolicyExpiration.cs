using System;

namespace Core.Collections.Expiring;

public class PolicyExpiration<T> : ExpirationPolicy<T>
{
   protected Func<T, bool> mustExpire;

   public PolicyExpiration(Func<T, bool> mustExpire) => this.mustExpire = mustExpire;

   public override bool ItemEvictable(T value) => mustExpire(value);

   public override void Reset() { }
}