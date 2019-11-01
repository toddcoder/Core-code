using System;
using Core.Dates.DateIncrements;

namespace Core.Data.ConnectionStrings
{
   public class AccessConnectionString : IConnectionString
   {
      public string ConnectionString => "Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source={file};";

      public TimeSpan ConnectionTimeout => 30.Seconds();
   }
}