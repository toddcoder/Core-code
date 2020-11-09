using Core.Computers;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.Setups;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data
{
   public static class DataGraphExtensions
   {
      public static Adapter<T> SQLAdapter<T>(this DataGraphs dataGraphs, string adapterName, T entity)
         where T : class
      {
         var setup = new SQLSetup(dataGraphs, adapterName);
         return new Adapter<T>(entity, setup);
      }

      public static Adapter<T> OLEDBAdapter<T>(this DataGraphs dataGraphs, string adapterName, T entity, IMaybe<FileName> file)
         where T : class
      {
         var setup = new OLEDBSetup(dataGraphs, adapterName, file);
         return new Adapter<T>(entity, setup);
      }

      public static IResult<string> SQLConnectionString(this DataGraphs dataGraphs, string adapterName)
      {
         return
            from adapterGraph in dataGraphs.AdaptersGraph.Result[adapterName]
            from connectionNameGraph in adapterGraph.Result["connection"]
            from connectionGraph in dataGraphs.ConnectionsGraph.Result[connectionNameGraph.Value]
            from connection in tryTo(() => new Connection(connectionGraph))
            from sqlConnection in tryTo(() => new SQLConnectionString(connection))
            select sqlConnection.ConnectionString;
      }

      public static IResult<string> OLEDBConnectionString(this DataGraphs dataGraphs, string adapterName)
      {
         return
            from adapterGraph in dataGraphs.AdaptersGraph.Result[adapterName]
            from connectionNameGraph in adapterGraph.Result["connection"]
            from connectionGraph in dataGraphs.ConnectionsGraph.Result[connectionNameGraph.Value]
            from connection in tryTo(() => new Connection(connectionGraph))
            from type in connection.Type.ToLower().Success()
            from connectionString in oledbConnectionString(type)
            select connectionString.ConnectionString;
      }

      private static IResult<IConnectionString> oledbConnectionString(string type)
      {
         switch (type)
         {
            case "access":
               return success<IConnectionString>(new AccessConnectionString());
            case "excel":
               return success<IConnectionString>(new ExcelConnectionString());
            case "csv":
               return success<IConnectionString>(new CSVConnectionString());
            default:
               return $"Unknown OLDEDB type {type}".Failure<IConnectionString>();
         }
      }
   }
}