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
      public static string GetConnectionString(string server, string database, string application, bool integratedSecurity = true)
      {
         var baseValue = $"Data Source={server};Initial Catalog={database};Application Name={application};";
         return integratedSecurity ? $"{baseValue}Integrated Security=SSPI;" : baseValue;
      }

      public static string GetConnectionString(string server, string database, string application, string user, string password)
      {
         var baseValue = GetConnectionString(server, database, application, false);
         return user.IsNotEmpty() && password.IsNotEmpty() ? $"{baseValue}User ID={user}; Password={password}" : baseValue;
      }

      public static string GetConnectionString(string server, string database, string application, IMaybe<string> _user, IMaybe<string> _password)
      {
         if (_user.If(out var user) && _password.If(out var password))
         {
            return GetConnectionString(server, database, application, user, password);
         }
         else
         {
            return GetConnectionString(server, database, application);
         }
      }

      public static IResult<SQLConnectionString> FromConnection(Connection connection)
      {
         if (connection.If("connection", out var connectionString))
         {
            return new SQLConnectionString(connectionString, connection.Timeout).Success();
         }
         else
         {
            return
               from server in connection.Require("server")
               from database in connection.Require("database")
               from application in connection.Require("application")
               select new SQLConnectionString(server, database, application, connection);
         }
      }

      protected string connectionString;
      protected TimeSpan connectionTimeout;

      internal SQLConnectionString(string connectionString, TimeSpan connectionTimeout)
      {
         this.connectionString = connectionString;
         this.connectionTimeout = connectionTimeout;
      }

      internal SQLConnectionString(string server, string database, string application, Connection connection)
      {
         var user = connection.Map("user");
         var password = connection.Map("password");
         connectionString = GetConnectionString(server, database, application, user, password);
         connectionTimeout = connection.Timeout;
      }

      public SQLConnectionString(Connection connection)
      {
         var server = connection.Value("server");
         var database = connection.Value("database");
         var application = connection.Value("application");
         var user = connection.Map("user");
         var password = connection.Map("password");
         connectionString = GetConnectionString(server, database, application, user, password);
         connectionTimeout = connection.Timeout;
      }

      public SQLConnectionString(string server, string database, string application, string user = "", string password = "", string timeout = "")
      {
         connectionString = GetConnectionString(server, database, application, user, password);
         connectionTimeout = timeout.IsEmpty() ? 30.Seconds() : timeout.ToTimeSpan();
      }

      public string ConnectionString => connectionString;

      public TimeSpan ConnectionTimeout => connectionTimeout;
   }
}