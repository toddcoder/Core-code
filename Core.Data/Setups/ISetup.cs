using System;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;

namespace Core.Data.Setups
{
   public interface ISetup
   {
      DataSource DataSource { get; }

      IConnectionString ConnectionString { get; }

      string CommandText { get; set; }

      Fields.Fields Fields { get; set; }

      Parameters.Parameters Parameters { get; set; }

      TimeSpan CommandTimeout { get; set; }
   }
}