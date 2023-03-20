using System;
using System.Data.SqlClient;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Data.Setups;

public class SqlSetup : ISetup, ISetupWithInfo
{
   public static Result<SqlSetup> FromDataGroups(DataSettings dataSettings, string adapterName)
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
         let connection = new Connection(connectionsSetting)
         from connectionString in SqlConnectionString.FromConnection(connection)
         from command in Command.FromSetting(commandSetting)
         from parameters in Data.Parameters.Parameters.FromSetting(adaptersSetting.Maybe.Setting("parameters"))
         from fields in Data.Fields.Fields.FromSetting(adapterSetting.Maybe.Setting("fields"))
         select new SqlSetup(connectionsSetting.Maybe.Setting("attributes"))
         {
            CommandText = command.Text,
            CommandTimeout = command.CommandTimeout,
            ConnectionString = connectionString,
            Parameters = parameters,
            Fields = fields
         };
   }

   public static Result<SqlSetup> FromGroup(Setting setting, string adapterName)
   {
      return
         from dataGraphs in setting.DataSettings().Result("Data graphs unavailable")
         from setup in FromDataGroups(dataGraphs, adapterName)
         select setup;
   }

   protected StringHash attributes;

   public SqlSetup(Setting setupSetting)
   {
      var setting = setupSetting.Required.Setting("connection");
      var connection = new Connection(setting);
      ConnectionString = new SqlConnectionString(connection);

      var commandSetting = setupSetting.Required.Setting("command");
      var command = new Command(commandSetting);
      CommandText = command.Text;
      CommandTimeout = command.CommandTimeout;

      var _parametersSetting = setupSetting.Maybe.Setting("parameters");
      Parameters = new Parameters.Parameters(_parametersSetting);

      var _fieldsSetting = setupSetting.Maybe.Setting("fields");
      Fields = new Fields.Fields(_fieldsSetting);

      attributes = new StringHash(true);
      Handler = nil;
      loadAttributes(setupSetting.Maybe.Setting("attributes"));
   }

   public SqlSetup(ISetupObject setupObject)
   {
      ConnectionString = new SqlConnectionString(setupObject.ConnectionString, 30.Seconds());
      CommandText = setupObject.CommandSourceType switch
      {
         CommandSourceType.File => ((FileName)setupObject.Command).Text,
         _ => setupObject.Command
      };

      CommandTimeout = setupObject.CommandTimeout;
      Parameters = new Parameters.Parameters(setupObject.Parameters());
      Fields = new Fields.Fields(setupObject.Fields());

      attributes = new StringHash(true);
      Handler = nil;
      loadAttributes(setupObject.Attributes);
   }

   internal SqlSetup(Maybe<Setting> attributesSetting)
   {
      attributes = new StringHash(true);
      Handler = nil;
      loadAttributes(attributesSetting);
   }

   public SqlSetup(StringHash setupData, string parameterSpecifiers = "", string fieldSpecifiers = "")
   {
      var connectionString = setupData.Must().HaveValueAt("connectionString").Value;
      ConnectionString = new SqlConnectionString(connectionString, 30.Seconds());
      CommandText = setupData.Must().HaveValueAt("commandText").Value;
      CommandTimeout = setupData.Maybe("commandTimeout").Map(Maybe.TimeSpan) | (() => 30.Seconds());

      if (parameterSpecifiers.IsNotEmpty())
      {
         Parameters = new Parameters.Parameters(Data.Parameters.Parameters.ParametersFromString(parameterSpecifiers));
      }

      if (fieldSpecifiers.IsNotEmpty())
      {
         Fields = new Fields.Fields(Data.Fields.Fields.FieldsFromString(fieldSpecifiers));
      }

      attributes = new StringHash(true);
      Handler = nil;
   }

   internal SqlSetup()
   {
      attributes = new StringHash(true);
      Handler = nil;
   }

   protected void loadAttributes(Maybe<Setting> _attributesSetting)
   {
      if (_attributesSetting is (true, var attributesSetting))
      {
         foreach (var (key, value) in attributesSetting.Items())
         {
            attributes[key] = value;
         }
      }
   }

   protected void loadAttributes(IHash<string, string> hash)
   {
      var _hash = hash.AnyHash();
      if (_hash is (true, var hashValue))
      {
         foreach (var (key, value) in hashValue)
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

   public Maybe<SqlInfoMessageEventHandler> Handler { get; set; }
}