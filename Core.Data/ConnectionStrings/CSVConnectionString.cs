using System;
using Core.Dates.DateIncrements;

namespace Core.Data.ConnectionStrings;

public class CSVConnectionString : IConnectionString
{
   public string ConnectionString => "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{file}';" +
      " Extended Properties='text;HDR=Yes;FMT=Delimited';";

   public TimeSpan ConnectionTimeout => 30.Seconds();
}