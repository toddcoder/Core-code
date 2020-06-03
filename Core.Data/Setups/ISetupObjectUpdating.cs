namespace Core.Data.Setups
{
   public interface ISetupObjectUpdating
   {
      string ConnectionString { set; }

      string Command { set; }
   }
}