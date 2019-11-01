namespace Core.Data
{
   public static class AdapterExtensions
   {
      public static AdapterTrying<T> TryTo<T>(this Adapter<T> adapter) where T : class => new AdapterTrying<T>(adapter);
   }
}