using Core.Configurations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Configurations;

public class DataSettings
{
   public Setting ConnectionsSetting { get; set; }

   public Setting CommandsSetting { get; set; }

   public Setting AdaptersSetting { get; set; }

   public Result<string> Command(string adapterName)
   {
      var _adapterSetting = AdaptersSetting.Maybe.Setting(adapterName);
      if (_adapterSetting is (true, var adapterSetting))
      {
         var commandName = adapterSetting.Maybe.String("command") | adapterName;
         var _commandSetting = CommandsSetting.Maybe.Setting(commandName);
         if (_commandSetting)
         {
            var command = new Command(_commandSetting);
            return command.Text;
         }
         else
         {
            return fail($"Didn't find command setting '{commandName}'");
         }
      }
      else
      {
         return fail($"Didn't find adapter setting '{adapterName}'");
      }
   }
}