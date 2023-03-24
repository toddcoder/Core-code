using Core.Monads;

namespace Core.Configurations;

public interface IConfigurationItemGetter
{
   Optional<Setting> GetSetting(string key);

   Optional<Item> GetItem(string key);
}