using Core.Configurations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Configurations
{
   public class DataGroups
   {
      public Group ConnectionsGroup { get; set; }

      public Group CommandsGroup { get; set; }

      public Group AdaptersGroup { get; set; }

      public Result<string> Command(string adapterName)
      {
         if (AdaptersGroup.GetGroup(adapterName).Map(out var adapterGroup))
         {
            var commandName = adapterGroup.GetValue("command").DefaultTo(() => adapterName);
            if (CommandsGroup.GetGroup(commandName).Map(out var commandGroup))
            {
               var command = new Command(commandGroup);
               return command.Text;
            }
            else
            {
               return fail($"Didn't find command group '{commandName}'");
            }
         }
         else
         {
            return fail($"Didn't find adapter group '{adapterName}'");
         }
      }
   }
}