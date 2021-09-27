using Core.Configurations;
using Core.Monads;

namespace Core.Data.Configurations
{
   public static class ConfigurationExtensions
   {
      public static Maybe<DataGroups> DataGroups(this Group group)
      {
         return
            from connectionsGroup in @group.GetGroup("connections")
            from commandsGroup in @group.GetGroup("commands")
            from adaptersGroup in @group.GetGroup("adapters")
            select new DataGroups { ConnectionsGroup = connectionsGroup, CommandsGroup = commandsGroup, AdaptersGroup = adaptersGroup };
      }
   }
}