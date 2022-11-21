namespace Core.Collections.Expiring;

public class NonExpiration<T> : ExpirationPolicy<T>
{
   public override bool ItemEvictable(T value) => false;

   public override void Reset() { }
}