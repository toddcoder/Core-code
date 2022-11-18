using System;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;

namespace Core.Data.DataSources;

public class TextDataSource : DataSource
{
   public TextDataSource(string connectionString, TimeSpan commandTimeout) : base(connectionString, commandTimeout)
   {
   }

   public override IDbConnection GetConnection()
   {
      var newConnection = new OleDbConnection(ConnectionString);
      newConnection.Open();

      return newConnection;
   }

   public override IDbCommand GetCommand() => new OdbcCommand
   {
      CommandTimeout = (int)commandTimeout.TotalSeconds
   };

   public override void AddParameters(object entity, Parameters.Parameters parameters)
   {
      SqlDataSource.AddParametersToCommand(Command.Required("Command hasn't been set"), entity, parameters);
   }

   public override void ClearAllPools()
   {
   }

   public override DataSource WithNewConnectionString(string newConnectionString) => new TextDataSource(newConnectionString, commandTimeout);
}