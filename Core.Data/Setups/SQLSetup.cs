using System;
using System.Data.SqlClient;
using Core.Collections;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Monads;
using Core.ObjectGraphs;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups
{
   public class SQLSetup : ISetup, ISetupWithInfo
   {
      public static IResult<SQLSetup> New(DataGraphs dataGraphs, string adapterName)
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
            from connectionString in SQLConnectionString.New(connection)
            from command in Command.New(commandGraph)
            from parameters in Data.Parameters.Parameters.New(adapterGraph.Map("parameters"))
            from fields in Data.Fields.Fields.New(adaptersGraph.Map("fields"))
            select new SQLSetup(dataGraphs)
            {
               CommandText = command.Text,
               CommandTimeout = command.CommandTimeout,
               ConnectionString = connectionString,
               Parameters = parameters,
               Fields = fields
            };
      }

      Hash<string, string> attributes;

      public SQLSetup(DataGraphs dataGraphs, string adapterName)
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
         ConnectionString = new SQLConnectionString(connection);

         var command = new Command(commandGraph);
         CommandTimeout = command.CommandTimeout;
         CommandText = command.Text;

         Parameters = new Parameters.Parameters(parametersGraph);

         Fields = new Fields.Fields(fieldsGraph);

         attributes = new Hash<string, string>();
         loadAttributes(dataGraphs.ConnectionsGraph.Map("attributes"));
      }

      public SQLSetup(ObjectGraph setupGraph)
      {
         var connectionGraph = setupGraph["connection"];
         var connection = new Connection(connectionGraph);
         ConnectionString = new SQLConnectionString(connection);

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

      internal SQLSetup(DataGraphs dataGraphs)
      {
         attributes = new Hash<string, string>();
         loadAttributes(dataGraphs.ConnectionsGraph.Map("attributes"));
      }

      void loadAttributes(IMaybe<ObjectGraph> attributesGraph)
      {
         if (attributesGraph.If(out var ag))
         {
            foreach (var childGraph in ag.Children)
            {
               attributes[childGraph.Name] = childGraph.Value;
            }
         }
      }

      public DataSource DataSource
      {
         get
         {
            var sqlDataSource = new SQLDataSource(ConnectionString.ConnectionString, CommandTimeout);
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