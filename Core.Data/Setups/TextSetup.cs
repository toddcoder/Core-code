using System;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Monads;

namespace Core.Data.Setups
{
   public class TextSetup : ISetup
   {
      public static IResult<TextSetup> FromDataGroups(DataGroups dataGroups, string adapterName, string fileName)
      {
         var connectionsGroup = dataGroups.ConnectionsGroup;
         var commandsGroup = dataGroups.CommandsGroup;
         var adaptersGroup = dataGroups.AdaptersGroup;

         return
            from adapterGraph in adaptersGroup.RequireGroup(adapterName)
            from connectionName in adapterGraph.RequireValue("connection")
            from connectionGroup in connectionsGroup.RequireGroup(connectionName)
            let commandName = adaptersGroup.GetValue("command").DefaultTo(() => adapterName)
            from commandGraph in commandsGroup.RequireGroup(commandName)
            let connection = new Connection(connectionGroup) { ["file"] = fileName }
            from connectionString in SqlConnectionString.FromConnection(connection)
            from command in Command.FromGroup(commandGraph)
            from parameters in Data.Parameters.Parameters.FromGroup(adapterGraph.GetGroup("parameters"))
            from fields in Data.Fields.Fields.FromGroup(adapterGraph.GetGroup("fields"))
            select new TextSetup
            {
               CommandText = command.Text,
               CommandTimeout = command.CommandTimeout,
               ConnectionString = connectionString,
               Parameters = parameters,
               Fields = fields
            };
      }

      public DataSource DataSource => new TextDataSource(ConnectionString.ConnectionString, CommandTimeout);

      public IConnectionString ConnectionString { get; set; }

      public string CommandText { get; set; }

      public Fields.Fields Fields { get; set; }

      public Parameters.Parameters Parameters { get; set; }

      public TimeSpan CommandTimeout { get; set; }
   }
}