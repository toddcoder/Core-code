using System;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Data.Setups
{
   public class OLEDBSetup : ISetup
   {
      static Hash<string, Func<IConnectionString>> registeredTypes;

      static OLEDBSetup() => registeredTypes = new Hash<string, Func<IConnectionString>>();

      public static void RegisterType(string type, Func<IConnectionString> func) => registeredTypes[type.ToLower()] = func;

      IMaybe<FileName> file;

      public OLEDBSetup(DataGraphs dataGraphs, string adapterName, IMaybe<FileName> file)
      {
         this.file = file;

         var connectionsGraph = dataGraphs.ConnectionsGraph;
         var commandsGraph = dataGraphs.CommandsGraph;
         var adaptersGraph = dataGraphs.AdaptersGraph;

         var adapterGraph = adaptersGraph[adapterName];
         var parametersGraph = adapterGraph.Map("parameters");
         var fieldsGraph = adapterGraph.Map("fields");

         var connectionName = adapterGraph["connection"].Value;
         var commandName = adaptersGraph.FlatMap("command", g => g.Value, adapterName);

         var connectionGraph = connectionsGraph[connectionName];
         var commandGraph = commandsGraph[commandName];

         var connection = new Connection(connectionGraph);
         var type = connection.Type.ToLower();
         switch (type)
         {
            case "access":
               ConnectionString = new AccessConnectionString();
               break;
            case "excel":
               ConnectionString = new ExcelConnectionString();
               break;
            case "csv":
               ConnectionString = new CSVConnectionString();
               break;
            default:
               var registeredType = registeredTypes.Value(type);
               ConnectionString = registeredType();
               break;
         }

         var command = new Command(commandGraph);
         CommandTimeout = command.CommandTimeout;
         CommandText = command.Text;

         Parameters = new Parameters.Parameters(parametersGraph);

         Fields = new Fields.Fields(fieldsGraph);
      }

      public DataSource DataSource
      {
         get
         {
            assert(() => (object)ConnectionString).Must().Not.BeNull().OrThrow();
            return new OLEDBDataSource(ConnectionString.ConnectionString, file);
         }
      }

      public IConnectionString ConnectionString { get; set; }

      public string CommandText { get; set; }

      public Fields.Fields Fields { get; set; }

      public Parameters.Parameters Parameters { get; set; }

      public TimeSpan CommandTimeout { get; set; }
   }
}