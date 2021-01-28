using System;
using System.Data.SqlClient;
using Core.Collections;
using Core.Computers;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.ObjectGraphs;
using Core.ObjectGraphs.Configurations;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups
{
   public class SqlSetup : ISetup, ISetupWithInfo
   {
      public static IResult<SqlSetup> FromDataGraphs(DataGraphs dataGraphs, string adapterName)
      {
         var connectionsGraph = dataGraphs.ConnectionsGraph;
         var commandsGraph = dataGraphs.CommandsGraph;
         var adaptersGraph = dataGraphs.AdaptersGraph;
         var commandName = commandsGraph.FlatMap("command", g => g.Value, adapterName);

         return
            from adapterGraph in adaptersGraph.TryTo[adapterName]
            from connectionName in adapterGraph.TryTo["connection"].Map(g => g.Value)
            from connectionGraph in connectionsGraph.TryTo[connectionName]
            from commandGraph in commandsGraph.TryTo[commandName]
            from connection in tryTo(() => new Connection(connectionGraph))
            from connectionString in SqlConnectionString.FromConnection(connection)
            from command in Command.FromObjectGraph(commandGraph)
            from parameters in Data.Parameters.Parameters.FromObjectGraph(adapterGraph.Map("parameters"))
            from fields in Data.Fields.Fields.FromObjectGraph(adapterGraph.Map("fields"))
            select new SqlSetup(dataGraphs)
            {
               CommandText = command.Text,
               CommandTimeout = command.CommandTimeout,
               ConnectionString = connectionString,
               Parameters = parameters,
               Fields = fields
            };
      }

      public static IResult<SqlSetup> FromConfiguration(Configuration configuration, string adapterName)
      {
         return
            from dataGraphs in configuration.DataGraphs().Result("Data graphs unavailable")
            from setup in FromDataGraphs(dataGraphs, adapterName)
            select setup;
      }

      protected Hash<string, string> attributes;

      public SqlSetup(DataGraphs dataGraphs, string adapterName)
      {
         var connectionsGraph = dataGraphs.ConnectionsGraph;
         var commandsGraph = dataGraphs.CommandsGraph;
         var adaptersGraph = dataGraphs.AdaptersGraph;

         var adapterGraph = adaptersGraph[adapterName];
         var parametersGraph = adapterGraph.Map("parameters");
         var fieldsGraph = adapterGraph.Map("fields");

         var connectionName = adapterGraph["connection"].Value;
         var commandName = adapterGraph.FlatMap("command", g => g.Value, adapterName);

         var connectionGraph = connectionsGraph[connectionName];
         var commandGraph = commandsGraph[commandName];

         var connection = new Connection(connectionGraph);
         ConnectionString = new SqlConnectionString(connection);

         var command = new Command(commandGraph);
         CommandTimeout = command.CommandTimeout;
         CommandText = command.Text;

         Parameters = new Parameters.Parameters(parametersGraph);

         Fields = new Fields.Fields(fieldsGraph);

         attributes = new Hash<string, string>();
         loadAttributes(dataGraphs.ConnectionsGraph.Map("attributes"));
      }

      public SqlSetup(ObjectGraph setupGraph)
      {
         var connectionGraph = setupGraph["connection"];
         var connection = new Connection(connectionGraph);
         ConnectionString = new SqlConnectionString(connection);

         var commandGraph = setupGraph["command"];
         var command = new Command(commandGraph);
         CommandText = command.Text;
         CommandTimeout = command.CommandTimeout;

         var parametersGraph = setupGraph.Map("parameters");
         Parameters = new Parameters.Parameters(parametersGraph);

         var fieldsGraph = setupGraph.Map("fields");
         Fields = new Fields.Fields(fieldsGraph);

         attributes = new Hash<string, string>();
         loadAttributes(setupGraph.Map("attributes"));
      }

      public SqlSetup(ISetupObject setupObject)
      {
         ConnectionString = new SqlConnectionString(setupObject.ConnectionString, 30.Seconds());
         switch (setupObject.CommandSourceType)
         {
            case CommandSourceType.File:
               CommandText = ((FileName)setupObject.Command).Text;
               break;
            default:
               CommandText = setupObject.Command;
               break;
         }
         CommandTimeout = setupObject.CommandTimeout;
         Parameters = new Parameters.Parameters(setupObject.Parameters());
         Fields = new Fields.Fields(setupObject.Fields());

         attributes = new Hash<string, string>();
         loadAttributes(setupObject.Attributes);
      }

      internal SqlSetup(DataGraphs dataGraphs)
      {
         attributes = new Hash<string, string>();
         loadAttributes(dataGraphs.ConnectionsGraph.Map("attributes"));
      }

      protected void loadAttributes(IMaybe<ObjectGraph> attributesGraph)
      {
         if (attributesGraph.If(out var ag))
         {
            foreach (var childGraph in ag.Children)
            {
               attributes[childGraph.Name] = childGraph.Value;
            }
         }
      }

      protected void loadAttributes(IHash<string, string> hash)
      {
         if (hash.AnyHash().If(out var actualHash))
         {
            foreach (var (key, value) in actualHash)
            {
               attributes[key] = value;
            }
         }
      }

      public DataSource DataSource
      {
         get
         {
            var sqlDataSource = new SqlDataSource(ConnectionString.ConnectionString, CommandTimeout);
            foreach (var (key, value) in attributes)
            {
               sqlDataSource[key] = value;
            }

            return sqlDataSource;
         }
      }

      public IConnectionString ConnectionString { get; set; }

      public string CommandText { get; set; }

      public Fields.Fields Fields { get; set; }

      public Parameters.Parameters Parameters { get; set; }

      public TimeSpan CommandTimeout { get; set; }

      public IMaybe<SqlInfoMessageEventHandler> Handler { get; set; } = none<SqlInfoMessageEventHandler>();
   }
}