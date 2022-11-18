using Core.Configurations;
using Core.Monads;

namespace Core.Data.Configurations;

public static class ConfigurationExtensions
{
   public static Maybe<DataSettings> DataSettings(this Setting setting)
   {
      return
         from connectionsSetting in setting.Maybe.Setting("connections")
         from commandsSetting in setting.Maybe.Setting("commands")
         from adaptersSetting in setting.Maybe.Setting("adapters")
         select new DataSettings { ConnectionsSetting = connectionsSetting, CommandsSetting = commandsSetting, AdaptersSetting = adaptersSetting };
   }
}