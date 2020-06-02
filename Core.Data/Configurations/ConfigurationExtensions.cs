using Core.Collections;
using Core.Monads;
using Core.ObjectGraphs.Configurations;

namespace Core.Data.Configurations
{
   public static class ConfigurationExtensions
   {
      public static IMaybe<DataGraphs> DataGraphs(this Configuration configuration)
      {
         return
            from connectionsGraph in configuration.Map("connections")
            from commandsGraph in configuration.Map("commands")
            from adaptersGraph in configuration.Map("adapters")
            select new DataGraphs { ConnectionsGraph = connectionsGraph, CommandsGraph = commandsGraph, AdaptersGraph = adaptersGraph };
      }
   }
}