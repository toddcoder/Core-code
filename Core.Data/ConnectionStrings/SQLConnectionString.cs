using System;
using Core.Collections;
using Core.Dates;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.Strings;

namespace Core.Data.ConnectionStrings
{
   public class SQLConnectionString : IConnectionString
   {
      const string CONNECTION_BASE = "Data Source={0};Initial Catalog={1};";
      const string CONNECTION_INTEGRATED_SECURITY = CONNECTION_BASE + "Integrated Security=SSPI;";
      const string CONNECTION_LOGIN = CONNECTION_BASE + "User ID={2}; Password={3}";

      public static IResult<SQLConnectionString> FromConnection(Connection connection) =>
         from server in connection.Require("server")
         from database in connection.Require("database")
         select new SQLConnectionString(server, database, connection);

      string connectionString;
      TimeSpan connectionTimeout;

      internal SQLConnectionString(string server, string database, Connection connection)
      {
         var user = connection.Map("user");
         var password = connection.Map("password");
         connectionString = getString(server, database, user, password);
         connectionTimeout = connection.Timeout;
      }

      static string getString(string server, string database, IMaybe<string> user, IMaybe<string> password)
      {
         if (user.IsNone || password.IsNone)
         {
            return integratedSecurity(server, database);
         }
         else
         {
            return login(server, database, user, password);
         }
      }

      static string getString(string server, string database, string user, string password)
      {
         if (user.IsEmpty() || password.IsEmpty())
         {
            return integratedSecurity(server, database);
         }
         else
         {
            return login(server, database, user.Some(), password.Some());
         }
      }

      static string login(string server, string database, IMaybe<string> user, IMaybe<string> password)
      {
         if (user.If(out var u) && password.If(out var p))
         {
            return string.Format(CONNECTION_LOGIN, server, database, u, p);
         }
         else
         {
            return integratedSecurity(server, database);
         }
      }

      static string integratedSecurity(string server, string database)
      {
         return string.Format(CONNECTION_INTEGRATED_SECURITY, server, database);
      }

      public SQLConnectionString(Connection connection)
      {
         var server = connection.Value("server");
         var database = connection.Value("database");
         var user = connection.Map("user");
         var password = connection.Map("password");
         connectionString = getString(server, database, user, password);
         connectionTimeout = connection.Timeout;
      }

      public SQLConnectionString(string server, string database, string user = "", string password = "", string timeout = "")
      {
         connectionString = getString(server, database, user, password);
         connectionTimeout = timeout.IsEmpty() ? 30.Seconds() : timeout.ToTimeSpan();
      }

      public string ConnectionString => connectionString;

      public TimeSpan ConnectionTimeout => connectionTimeout;
   }
}