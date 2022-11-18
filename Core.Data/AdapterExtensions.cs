using Core.Data.Setups;

namespace Core.Data;

public static class AdapterExtensions
{
   public static AdapterTrying<T> TryTo<T>(this Adapter<T> adapter) where T : class => new(adapter);

   public static Adapter<T> SqlAdapter<T>(this T setupObject) where T : class, ISetupObject
   {
      var setup = setupObject.Setup();
      return new Adapter<T>(setupObject, setup);
   }
}