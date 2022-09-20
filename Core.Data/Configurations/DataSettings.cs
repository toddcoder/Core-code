using Core.Configurations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Configurations
{
   public class DataSettings
   {
      public Setting ConnectionsSetting { get; set; }

      public Setting CommandsSetting { get; set; }

      public Setting AdaptersSetting { get; set; }

      public Result<string> Command(string adapterName)
      {
         if (AdaptersSetting.Maybe.Setting(adapterName).Map(out var adapterSetting))
         {
            var commandName = adapterSetting.Maybe.String("command") | adapterName;
            if (CommandsSetting.Maybe.Setting(commandName).Map(out var commandSetting))
            {
               var command = new Command(commandSetting);
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
}