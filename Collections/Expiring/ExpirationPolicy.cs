namespace Core.Collections.Expiring;

public abstract class ExpirationPolicy<T>
{
   public abstract bool ItemEvictable(T value);

   public abstract void Reset();
}