using System;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Monads;

namespace Core.Data.Setups
{
   public class TextSetup : ISetup
   {
      public static Result<TextSetup> FromDataSettings(DataSettings dataSettings, string adapterName, string fileName)
      {
         var connectionsSetting = dataSettings.ConnectionsSetting;
         var commandsSetting = dataSettings.CommandsSetting;
         var adaptersSetting = dataSettings.AdaptersSetting;

         return
            from adapterSetting in adaptersSetting.Result.Setting(adapterName)
            from connectionName in adapterSetting.Result.String("connection")
            from connectionSetting in connectionsSetting.Result.Setting(connectionName)
            let commandName = adaptersSetting.Maybe.String("command") | adapterName
            from commandSetting in commandsSetting.Result.Setting(commandName)
            let connection = new Connection(connectionSetting) { ["file"] = fileName }
            from connectionString in SqlConnectionString.FromConnection(connection)
            from command in Command.FromSetting(commandsSetting)
            from parameters in Data.Parameters.Parameters.FromSetting(adapterSetting.Maybe.Setting("parameters"))
            from fields in Data.Fields.Fields.FromSetting(adapterSetting.Maybe.Setting("fields"))
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