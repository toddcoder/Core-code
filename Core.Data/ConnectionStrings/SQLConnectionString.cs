using System;
using Core.Collections;
using Core.Dates.DateIncrements;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Objects.ConversionFunctions;

namespace Core.Data.ConnectionStrings;

public class SqlConnectionString : IConnectionString
{
   public static string GetConnectionString(string server, string database, string application, bool integratedSecurity = true,
      bool readOnly = false)
   {
      var connectionString = $"Data Source={server};Initial Catalog={database};Application Name={application};";
      if (integratedSecurity)
      {
         connectionString = $"{connectionString}Integrated Security=SSPI;";
      }

      if (readOnly)
      {
         connectionString = $"{connectionString}ApplicationIntent=ReadOnly;";
      }

      return connectionString;
   }

   public static string GetConnectionString(string server, string database, string application, string user, string password,
      bool readOnly = false)
   {
      var baseValue = GetConnectionString(server, database, application, user.IsEmpty() && password.IsEmpty(), readOnly);
      return user.IsNotEmpty() && password.IsNotEmpty() ? $"{baseValue}User ID={user}; Password={password}" : baseValue;
   }

   public static string GetConnectionString(string server, string database, string application, Optional<string> _user, Optional<string> _password,
      bool readOnly = false)
   {
      if (_user && _password)
      {
         return GetConnectionString(server, database, application, _user, _password, readOnly);
      }
      else
      {
         return GetConnectionString(server, database, application, true, readOnly);
      }
   }

   public static (string server, string database, string application, string user, string password) GetArguments(string connectionString)
   {
      Pattern getPattern(string firstWord, string secondWord) => $"/b '{firstWord}' /s+ '{secondWord}' /s* '=' /s* /(-[';']+); fi";
      Pattern getPattern1(string word) => $"/b '{word}' /s* '=' /s* /(-[';']+); fi";
      string getValue(string firstWord, string secondWord) => connectionString.Matches(getPattern(firstWord, secondWord)).Map(r => r.FirstGroup) | "";
      string getValue1(string word) => connectionString.Matches(getPattern1(word)).Map(r => r.FirstGroup) | "";

      var server = getValue("data", "source");
      var database = getValue("initial", "catalog");
      var application = getValue("application", "name");
      var user = getValue("user", "id");
      var password = getValue1("password");

      return (server, database, application, user, password);
   }

   public static Optional<SqlConnectionString> FromConnection(Connection connection)
   {
      var _connectionString = connection.Maybe()["connection"];
      if (_connectionString)
      {
         return new SqlConnectionString(_connectionString, connection.Timeout);
      }
      else
      {
         return
            from server in connection.Require("server")
            from database in connection.Require("database")
            from application in connection.Require("application")
            select new SqlConnectionString(server, database, application, connection);
      }
   }

   protected string connectionString;
   protected TimeSpan connectionTimeout;

   internal SqlConnectionString(string connectionString, TimeSpan connectionTimeout)
   {
      this.connectionString = connectionString;
      this.connectionTimeout = connectionTimeout;
   }

   internal SqlConnectionString(string server, string database, string application, Connection connection)
   {
      var user = connection.Maybe("user");
      var password = connection.Maybe("password");
      connectionString = GetConnectionString(server, database, application, user, password, connection.ReadOnly);
      connectionTimeout = connection.Timeout;
   }

   public SqlConnectionString(Connection connection)
   {
      var server = connection.Value("server");
      var database = connection.Value("database");
      var application = connection.Value("application");
      var user = connection.Maybe("user");
      var password = connection.Maybe("password");
      var readOnly = connection.ReadOnly;
      connectionString = GetConnectionString(server, database, application, user, password, readOnly);
      connectionTimeout = connection.Timeout;
   }

   public SqlConnectionString(string server, string database, string application, string user = "", string password = "", string timeout = "",
      bool readOnly = false)
   {
      connectionString = GetConnectionString(server, database, application, user, password, readOnly);
      connectionTimeout = timeout.IsEmpty() ? 30.Seconds() : Value.TimeSpan(timeout);
   }

   public SqlConnectionString(string server, string database, string application, TimeSpan timeout, string user = "", string password = "",
      bool readOnly = false) : this(server, database, application, user, password, "", readOnly)
   {
      connectionString = GetConnectionString(server, database, application, user, password, readOnly);
      connectionTimeout = timeout;
   }

   public SqlConnectionString(string server, string database, string application, TimeSpan timeout, Optional<string> _user, Optional<string> _password,
      bool readOnly = false)
   {
      connectionString = GetConnectionString(server, database, application, _user, _password, readOnly);
      connectionTimeout = timeout;
   }

   public string ConnectionString => connectionString;

   public TimeSpan ConnectionTimeout => connectionTimeout;
}