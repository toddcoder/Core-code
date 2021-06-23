using Core.Configurations;
using Core.Monads;

namespace Core.Data.Configurations
{
   public static class ConfigurationExtensions
   {
      public static Maybe<DataGroups> DataGroups(this Configuration configuration)
      {
         return
            from connectionsGroup in configuration.GetGroup("connections")
            from commandsGroup in configuration.GetGroup("commands")
            from adaptersGroup in configuration.GetGroup("adapters")
            select new DataGroups { ConnectionsGroup = connectionsGroup, CommandsGroup = commandsGroup, AdaptersGroup = adaptersGroup };
      }
   }
}