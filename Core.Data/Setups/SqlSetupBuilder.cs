using System;
using System.Collections.Generic;
using Core.Computers;
using Core.Data.ConnectionStrings;
using Core.Data.Fields;
using Core.Data.Parameters;
using Core.Dates.DateIncrements;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class SqlSetupBuilder
{
   protected Maybe<SqlConnectionString> _connectionString;
   protected Maybe<string> _commandText;
   protected List<Field> fields;
   protected List<Parameter> parameters;
   protected Maybe<TimeSpan> _commandTimeout;

   public SqlSetupBuilder()
   {
      _connectionString = nil;
      _commandText = nil;
      fields = new List<Field>();
      parameters = new List<Parameter>();
      _commandTimeout = nil;
   }

   public SqlSetupBuilder ConnectionString(string connectionString)
   {
      _connectionString = new SqlConnectionString(connectionString, 30.Seconds());
      return this;
   }

   public SqlSetupBuilder ConnectionString(string connectionString, TimeSpan connectionTimeout)
   {
      _connectionString = new SqlConnectionString(connectionString, connectionTimeout);
      return this;
   }

   public SqlSetupBuilder ConnectionString(string server, string database, string application, bool integratedSecurity = true, bool readOnly = false)
   {
      return ConnectionString(SqlConnectionString.GetConnectionString(server, database, application, integratedSecurity, readOnly));
   }

   public SqlSetupBuilder ConnectionString(string server, string database, string application, string user, string password, bool readOnly = false)
   {
      return ConnectionString(SqlConnectionString.GetConnectionString(server, database, application, user, password, readOnly));
   }

   public SqlSetupBuilder ConnectionString(string server, string database, string application, Maybe<string> _user, Maybe<string> _password,
      bool readOnly = false)
   {
      return ConnectionString(SqlConnectionString.GetConnectionString(server, database, application, _user, _password, readOnly));
   }

   public SqlSetupBuilder ConnectionString(Connection connection)
   {
      _connectionString = SqlConnectionString.FromConnection(connection).Maybe();
      return this;
   }

   public SqlSetupBuilder CommandText(string commandText)
   {
      _commandText = commandText;
      return this;
   }

   public SqlSetupBuilder CommandText(FileName sqlFile)
   {
      _commandText = sqlFile.TryTo.Text.Maybe();
      return this;
   }

   public SqlSetupBuilder CommandTimeout(TimeSpan commandTimeout)
   {
      _commandTimeout = commandTimeout;
      return this;
   }

   public FieldBuilder Field(string name)
   {
      var fieldBuilder = new FieldBuilder(name, this);
      return fieldBuilder;
   }

   internal void AddField(Field field) => fields.Add(field);

   public ParameterBuilder Parameter(string name)
   {
      var parameterBuilder = new ParameterBuilder(name, this);
      return parameterBuilder;
   }

   internal void AddParameter(Parameter parameter) => parameters.Add(parameter);

   public SqlSetup Setup() => new()
   {
      ConnectionString = _connectionString.Required("ConnectionString must be called"),
      CommandText = _commandText.Required("CommandText must be called"),
      Fields = new Fields.Fields(fields),
      Parameters = new Parameters.Parameters(parameters),
      CommandTimeout = _commandTimeout | (() => 30.Seconds())
   };

   public Result<SqlSetup> TrySetup() => tryTo(Setup);
}