using Core.Monads;

namespace Core.Configurations;

public interface IConfigurationItemGetter
{
   Maybe<Setting> GetSetting(string key);

   Maybe<Item> GetItem(string key);
}