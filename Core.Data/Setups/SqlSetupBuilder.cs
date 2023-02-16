using System;
using System.Collections.Generic;
using Core.Data.ConnectionStrings;
using Core.Data.Fields;
using Core.Data.Parameters;
using Core.Monads;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class SqlSetupBuilder
{
   public static SqlSetupBuilder sqlSetup() => new();

   public static ConnectionStringBuilder operator +(SqlSetupBuilder builder, SqlSetupBuilderParameters.IConnectionStringParameter parameter)
   {
      var connectionStringBuilder = new ConnectionStringBuilder(builder);
      builder.ConnectionStringBuilder(connectionStringBuilder);

      return parameter switch
      {
         SqlSetupBuilderParameters.ApplicationName applicationName => connectionStringBuilder.ApplicationName(applicationName),
         SqlSetupBuilderParameters.ConnectionString connectionString => connectionStringBuilder.ConnectionString(connectionString),
         SqlSetupBuilderParameters.ConnectionTimeout connectionTimeout => connectionStringBuilder.ConnectionTimeout(connectionTimeout),
         SqlSetupBuilderParameters.Database database => connectionStringBuilder.Database(database),
         SqlSetupBuilderParameters.ReadOnly readOnly => connectionStringBuilder.ReadOnly(readOnly),
         SqlSetupBuilderParameters.Server server => connectionStringBuilder.Server(server),
         SqlSetupBuilderParameters.User user => connectionStringBuilder.User(user),
         SqlSetupBuilderParameters.Password password => connectionStringBuilder.Password(password),
         _ => throw new ArgumentOutOfRangeException(nameof(parameter))
      };
   }

   public static CommandTextBuilder operator +(SqlSetupBuilder builder, SqlSetupBuilderParameters.ICommandTextParameter parameter)
   {
      var commandTextBuilder = new CommandTextBuilder(builder);
      builder.CommandTextBuilder(commandTextBuilder);

      return parameter switch
      {
         SqlSetupBuilderParameters.CommandText commandText => commandTextBuilder.CommandText(commandText),
         SqlSetupBuilderParameters.CommandTextFile commandTextFile => commandTextBuilder.CommandTextFile(commandTextFile),
         SqlSetupBuilderParameters.CommandTimeout commandTimeout => commandTextBuilder.CommandTimeout(commandTimeout),
         _ => throw new ArgumentOutOfRangeException(nameof(parameter))
      };
   }

   public static FieldBuilder operator +(SqlSetupBuilder builder, SqlSetupBuilderParameters.IFieldParameter parameter)
   {
      var fieldBuilder = new FieldBuilder(builder);
      builder.FieldBuilder(fieldBuilder);

      return parameter switch
      {
         SqlSetupBuilderParameters.FieldName name => fieldBuilder.Name(name),
         SqlSetupBuilderParameters.Optional optional => fieldBuilder.Optional(optional),
         SqlSetupBuilderParameters.Signature signature => fieldBuilder.Signature(signature),
         SqlSetupBuilderParameters.Type type => fieldBuilder.Type(type),
         _ => throw new ArgumentOutOfRangeException(nameof(parameter))
      };
   }

   public static ParameterBuilder operator +(SqlSetupBuilder builder, SqlSetupBuilderParameters.IParameterParameter parameter)
   {
      var parameterBuilder = new ParameterBuilder(builder);
      builder.ParameterBuilder(parameterBuilder);

      return parameter switch
      {
         SqlSetupBuilderParameters.DefaultValue defaultValue => parameterBuilder.Default(defaultValue),
         SqlSetupBuilderParameters.ParameterName name => parameterBuilder.Name(name),
         SqlSetupBuilderParameters.Output output => parameterBuilder.Output(output),
         SqlSetupBuilderParameters.Signature signature => parameterBuilder.Signature(signature),
         SqlSetupBuilderParameters.Size size => parameterBuilder.Size(size),
         SqlSetupBuilderParameters.Type type => parameterBuilder.Type(type),
         SqlSetupBuilderParameters.ValueParameter valueParameter => parameterBuilder.Value(valueParameter),
         _ => throw new ArgumentOutOfRangeException(nameof(parameter))
      };
   }

   protected Maybe<ConnectionStringBuilder> _connectionStringBuilder;
   protected Maybe<CommandTextBuilder> _commandTextBuilder;
   protected List<ParameterBuilder> parameterBuilders;
   protected List<FieldBuilder> fieldBuilders;

   public SqlSetupBuilder()
   {
      _connectionStringBuilder = nil;
      _commandTextBuilder = nil;
      parameterBuilders = new List<ParameterBuilder>();
      fieldBuilders = new List<FieldBuilder>();
   }

   internal void ConnectionStringBuilder(ConnectionStringBuilder builder) => _connectionStringBuilder = builder;

   internal void CommandTextBuilder(CommandTextBuilder builder) => _commandTextBuilder = builder;

   internal void FieldBuilder(FieldBuilder builder) => fieldBuilders.Add(builder);

   internal void ParameterBuilder(ParameterBuilder builder) => parameterBuilders.Add(builder);

   public Result<SqlSetup> Build()
   {
      var sqlSetup = new SqlSetup();
      var _connectionString = lazy.result<SqlConnectionString>();
      var _commandText = lazy.result<(string, TimeSpan)>();

      if (_connectionStringBuilder is (true, var connectionStringBuilder))
      {
         _connectionString.ValueOf(connectionStringBuilder.Build);
      }
      else
      {
         return fail("Connection string not provided");
      }

      if (_commandTextBuilder is (true, var commandTextBuilder))
      {
         _commandText.ValueOf(commandTextBuilder.Build);
      }
      else
      {
         return fail("Command text not provided");
      }

      if (_connectionString is (true, var connectionString))
      {
         sqlSetup.ConnectionString = connectionString;
      }
      else
      {
         return _connectionString.Exception;
      }

      if (_commandText is (true, var (commandText, commandTimeout)))
      {
         sqlSetup.CommandText = commandText;
         sqlSetup.CommandTimeout = commandTimeout;
      }
      else
      {
         return _commandText.Exception;
      }

      var fields = new List<Field>();
      foreach (var fieldBuilder in fieldBuilders)
      {
         var _field = fieldBuilder.Build();
         if (_field)
         {
            fields.Add(_field);
         }
         else
         {
            return _field.Exception;
         }
      }

      sqlSetup.Fields = new Fields.Fields(fields);

      var parameters = new List<Parameter>();
      foreach (var parameterBuilder in parameterBuilders)
      {
         var _parameter = parameterBuilder.Build();
         if (_parameter)
         {
            parameters.Add(_parameter);
         }
         else
         {
            return _parameter.Exception;
         }
      }

      sqlSetup.Parameters = new Parameters.Parameters(parameters);

      return sqlSetup;
   }
}