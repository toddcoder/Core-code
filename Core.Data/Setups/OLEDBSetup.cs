using System;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups
{
   public class OleDbSetup : ISetup
   {
      protected static StringHash<Func<IConnectionString>> registeredTypes;

      static OleDbSetup() => registeredTypes = new StringHash<Func<IConnectionString>>(true);

      public static void RegisterType(string type, Func<IConnectionString> func) => registeredTypes[type] = func;

      public static IResult<OleDbSetup> FromDataGroups(DataGroups dataGroups, string adapterName, Maybe<FileName> file)
      {
         var result =
            from adapterGroup in dataGroups.AdaptersGroup.RequireGroup(adapterName)
            let _parameters = new Parameters.Parameters(adapterGroup.GetGroup("parameters"))
            let _fields = new Fields.Fields(adapterGroup.GetGroup("fields"))
            from connectionName in adapterGroup.RequireValue("connection")
            let commandName = adapterGroup.GetValue("command").DefaultTo(() => adapterName)
            from connectionGroup in dataGroups.ConnectionsGroup.RequireGroup(connectionName)
            from commandGroup in dataGroups.CommandsGroup.RequireGroup(commandName)
            let _command = new Command(commandGroup)
            let _connection = new Connection(connectionGroup)
            let type = _connection.Type.ToLower()
            select (_parameters, _fields, _command, _connection);
         if (result.If(out var parameters, out var fields, out var command, out var connection, out var exception))
         {
            var type = connection.Type.ToLower();
            var _connectionString = type switch
            {
               "access" => new AccessConnectionString().Some<IConnectionString>(),
               "excel" => new ExcelConnectionString().Some<IConnectionString>(),
               "csv" => new CSVConnectionString().Some<IConnectionString>(),
               _ => registeredTypes.Map(type).Map(f => f())
            };

            return _connectionString.Map(connectionString => new OleDbSetup(file)
            {
               ConnectionString = connectionString, CommandTimeout = command.CommandTimeout, CommandText = command.Text, Parameters = parameters,
               Fields = fields
            }).Result($"Didn't understand type {type}");
         }
         else
         {
            return failure<OleDbSetup>(exception);
         }
      }

      protected Maybe<FileName> file;

      public OleDbSetup(Maybe<FileName> file)
      {
         this.file = file;
      }

      public DataSource DataSource
      {
         get
         {
            ConnectionString.Must().Not.BeNull().OrThrow();
            return new OleDbDataSource(ConnectionString.ConnectionString, file);
         }
      }

      public IConnectionString ConnectionString { get; set; }

      public string CommandText { get; set; }

      public Fields.Fields Fields { get; set; }

      public Parameters.Parameters Parameters { get; set; }

      public TimeSpan CommandTimeout { get; set; }
   }
}