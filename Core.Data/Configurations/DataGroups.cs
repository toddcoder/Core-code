using Core.Configurations;
using Core.Monads;

namespace Core.Data.Configurations
{
   public class DataGroups
   {
      public Group ConnectionsGroup { get; set; }

      public Group CommandsGroup { get; set; }

      public Group AdaptersGroup { get; set; }

      public IResult<string> Command(string adapterName)
      {
         if (AdaptersGroup.GetGroup(adapterName).If(out var adapterGroup))
         {
            var commandName = adapterGroup.GetValue("command").DefaultTo(() => adapterName);
            if (CommandsGroup.GetGroup(commandName).If(out var commandGroup))
            {
               var command = new Command(commandGroup);
               return command.Text.Success();
            }
            else
            {
               return $"Didn't find command group '{commandName}'".Failure<string>();
            }
         }
         else
         {
            return $"Didn't find adapter group '{adapterName}'".Failure<string>();
         }
      }
   }
}