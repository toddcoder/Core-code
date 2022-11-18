namespace Core.Data.Setups;

public interface ISetupObjectWithSetters
{
   string ConnectionString { set; }

   string Command { set; }
}