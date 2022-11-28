using System;
using Core.Data.ConnectionStrings;
using Core.Dates.DateIncrements;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class ConnectionStringBuilder
{
   public static ConnectionStringBuilder operator +(ConnectionStringBuilder builder, SqlSetupBuilderParameters.IConnectionStringParameter parameter)
   {
      return parameter switch
      {
         SqlSetupBuilderParameters.ApplicationName applicationName => builder.ApplicationName(applicationName),
         SqlSetupBuilderParameters.ConnectionString connectionString => builder.ConnectionString(connectionString),
         SqlSetupBuilderParameters.ConnectionTimeout connectionTimeout => builder.ConnectionTimeout(connectionTimeout),
         SqlSetupBuilderParameters.Database database => builder.Database(database),
         SqlSetupBuilderParameters.ReadOnly readOnly => builder.ReadOnly(readOnly),
         SqlSetupBuilderParameters.Server server => builder.Server(server),
         _ => throw new ArgumentOutOfRangeException(nameof(parameter))
      };
   }

   protected SqlSetupBuilder setupBuilder;
   protected Maybe<string> _connectionString;
   protected Maybe<TimeSpan> _connectionTimeout;
   protected Maybe<string> _server;
   protected Maybe<string> _database;
   protected Maybe<string> _user;
   protected Maybe<string> _password;
   protected Maybe<string> _applicationName;
   protected Maybe<bool> _readonly;

   public ConnectionStringBuilder(SqlSetupBuilder setupBuilder)
   {
      this.setupBuilder = setupBuilder;
      this.setupBuilder.ConnectionStringBuilder(this);

      _connectionString = nil;
      _connectionTimeout = nil;
      _server = nil;
      _database = nil;
      _user = nil;
      _password = nil;
      _applicationName = nil;
      _readonly = nil;
   }

   public ConnectionStringBuilder ConnectionString(string connectionString)
   {
      _connectionString = connectionString;
      return this;
   }

   public ConnectionStringBuilder ConnectionTimeout(TimeSpan connectionTimeout)
   {
      _connectionTimeout = connectionTimeout;
      return this;
   }

   public ConnectionStringBuilder Server(string server)
   {
      _server = server;
      return this;
   }

   public ConnectionStringBuilder Database(string database)
   {
      _database = database;
      return this;
   }

   public ConnectionStringBuilder User(string user)
   {
      _user = user;
      return this;
   }

   public ConnectionStringBuilder Password(string password)
   {
      _password = password;
      return this;
   }

   public ConnectionStringBuilder ApplicationName(string applicationName)
   {
      _applicationName = applicationName;
      return this;
   }

   public ConnectionStringBuilder ReadOnly(bool readOnly)
   {
      _readonly = readOnly;
      return this;
   }

   public Result<SqlConnectionString> Build()
   {
      var connectionTimeout = _connectionTimeout | (() => 30.Seconds());
      var applicationName = _applicationName | "";
      var readOnly = _readonly | false;

      if (_connectionString)
      {
         return new SqlConnectionString(_connectionString, connectionTimeout);
      }
      else if (_server && _database)
      {
         return new SqlConnectionString(_server, _database, applicationName, connectionTimeout, _user, _password, readOnly);
      }
      else
      {
         return fail("Connection string | server | database not provided");
      }
   }
}