﻿using System;
using System.Data.SqlClient;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Data.Fields;
using Core.Data.Parameters;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Data.Setups
{
   public class SqlSetup : ISetup, ISetupWithInfo
   {
      public static Result<SqlSetup> FromDataGroups(DataGroups dataGroups, string adapterName)
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
            let connection = new Connection(connectionGroup)
            from connectionString in SqlConnectionString.FromConnection(connection)
            from command in Command.FromGroup(commandGraph)
            from parameters in Data.Parameters.Parameters.FromGroup(adapterGraph.GetGroup("parameters"))
            from fields in Data.Fields.Fields.FromGroup(adapterGraph.GetGroup("fields"))
            select new SqlSetup(connectionsGroup.GetGroup("attributes"))
            {
               CommandText = command.Text,
               CommandTimeout = command.CommandTimeout,
               ConnectionString = connectionString,
               Parameters = parameters,
               Fields = fields
            };
      }

      public static Result<SqlSetup> FromGroup(Group group, string adapterName)
      {
         return
            from dataGraphs in @group.DataGroups().Result("Data graphs unavailable")
            from setup in FromDataGroups(dataGraphs, adapterName)
            select setup;
      }

      protected StringHash attributes;

      public SqlSetup(Group setupGroup)
      {
         var connectionGroup = setupGroup.RequireGroup("connection").ForceValue();
         var connection = new Connection(connectionGroup);
         ConnectionString = new SqlConnectionString(connection);

         var commandGroup = setupGroup.RequireGroup("command").ForceValue();
         var command = new Command(commandGroup);
         CommandText = command.Text;
         CommandTimeout = command.CommandTimeout;

         var parametersGroup = setupGroup.GetGroup("parameters");
         Parameters = new Parameters.Parameters(parametersGroup);

         var fieldsGroup = setupGroup.GetGroup("fields");
         Fields = new Fields.Fields(fieldsGroup);

         attributes = new StringHash(true);
         loadAttributes(setupGroup.GetGroup("attributes"));
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
         loadAttributes(setupObject.Attributes);
      }

      internal SqlSetup(Maybe<Group> attributesGroup)
      {
         attributes = new StringHash(true);
         loadAttributes(attributesGroup);
      }

      public SqlSetup(StringHash setupData, string parameterSpecifiers="", string fieldSpecifiers = "")
      {
         var connectionString = setupData.Must().HaveValueAt("connectionString").Value;
         ConnectionString = new SqlConnectionString(connectionString, 30.Seconds());
         CommandText = setupData.Must().HaveValueAt("commandText").Value;
         CommandTimeout = setupData.Map("commandTimeout").Map(Maybe.TimeSpan).DefaultTo(() => 30.Seconds());

         if (parameterSpecifiers.IsNotEmpty())
         {
            Parameters = new Parameters.Parameters(Parameter.ParametersFromString(parameterSpecifiers));
         }

         if (fieldSpecifiers.IsNotEmpty())
         {
            Fields = new Fields.Fields(Field.FieldsFromString(fieldSpecifiers));
         }

         attributes = new StringHash(true);
      }

      protected void loadAttributes(Maybe<Group> _attributesGroup)
      {
         if (_attributesGroup.Map(out var attributesGroup))
         {
            foreach (var (key, value) in attributesGroup.Values())
            {
               attributes[key] = value;
            }
         }
      }

      protected void loadAttributes(IHash<string, string> hash)
      {
         if (hash.AnyHash().Map(out var actualHash))
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

      public Maybe<SqlInfoMessageEventHandler> Handler { get; set; } = nil;
   }
}