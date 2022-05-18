﻿using System;
using Core.Collections;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.Strings;
using static Core.Objects.ConversionFunctions;

namespace Core.Data.ConnectionStrings
{
   public class SqlConnectionString : IConnectionString
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

      public static string GetConnectionString(string server, string database, string application, Maybe<string> _user, Maybe<string> _password)
      {
         if (_user.Map(out var user) && _password.Map(out var password))
         {
            return GetConnectionString(server, database, application, user, password);
         }
         else
         {
            return GetConnectionString(server, database, application);
         }
      }

      public static Result<SqlConnectionString> FromConnection(Connection connection)
      {
         if (connection.Map("connection", out var connectionString))
         {
            return new SqlConnectionString(connectionString, connection.Timeout);
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
         var user = connection.Map("user");
         var password = connection.Map("password");
         connectionString = GetConnectionString(server, database, application, user, password);
         connectionTimeout = connection.Timeout;
      }

      public SqlConnectionString(Connection connection)
      {
         var server = connection.Value("server");
         var database = connection.Value("database");
         var application = connection.Value("application");
         var user = connection.Map("user");
         var password = connection.Map("password");
         connectionString = GetConnectionString(server, database, application, user, password);
         connectionTimeout = connection.Timeout;
      }

      public SqlConnectionString(string server, string database, string application, string user = "", string password = "", string timeout = "")
      {
         connectionString = GetConnectionString(server, database, application, user, password);
         connectionTimeout = timeout.IsEmpty() ? 30.Seconds() : Value.TimeSpan(timeout);
      }

      public string ConnectionString => connectionString;

      public TimeSpan ConnectionTimeout => connectionTimeout;
   }
}