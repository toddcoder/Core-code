using System;
using Core.Dates.DateIncrements;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class ConnectionStringBuilder
{
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

   public SqlSetupBuilder EndConnectionString()
   {
      var connectionTimeout = _connectionTimeout | (() => 30.Seconds());
      var applicationName = _applicationName | "";
      var readOnly = _readonly | false;

      if (_connectionString)
      {
         return setupBuilder.ConnectionString(_connectionString, connectionTimeout);
      }
      else if (_server && _database)
      {
         if (_user && _password)
         {
            return setupBuilder.ConnectionString(_server, _database, applicationName, _user, _password, readOnly);
         }
         else
         {
            return setupBuilder.ConnectionString(_server, _database, applicationName, readOnly);
         }
      }
      else
      {
         return setupBuilder;
      }
   }
}