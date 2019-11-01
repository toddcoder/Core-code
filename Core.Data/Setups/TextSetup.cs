using System;
using Core.Collections;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;

namespace Core.Data.Setups
{
   public class TextSetup : ISetup
   {
      public TextSetup(DataGraphs dataGraphs, string adapterName, string fileName)
      {
         var adapterGraph = dataGraphs.AdaptersGraph[adapterName];
         var parametersGraph = adapterGraph.Map("parameters");
         var fieldsGraph = adapterGraph.Map("fields");

         var connectionName = adapterGraph["connection"].Value;
         var commandName = dataGraphs.AdaptersGraph.FlatMap("command", g => g.Value, adapterName);

         var connectionGraph = dataGraphs.ConnectionsGraph[connectionName];
         var commandGraph = dataGraphs.ConnectionsGraph[commandName];

         var connection = new Connection(connectionGraph) { ["file"] = fileName };
         ConnectionString = new TextConnectionString(connection);

         var command = new Command(commandGraph);
         CommandTimeout = command.CommandTimeout;
         CommandText = command.Text;

         Parameters = new Parameters.Parameters(parametersGraph);

         Fields = new Fields.Fields(fieldsGraph);
      }

      public DataSource DataSource => new TextDataSource(ConnectionString.ConnectionString, CommandTimeout);

      public IConnectionString ConnectionString { get; set; }

      public string CommandText { get; set; }

      public Fields.Fields Fields { get; set; }

      public Parameters.Parameters Parameters { get; set; }

      public TimeSpan CommandTimeout { get; set; }
   }
}