using System;
using Core.Dates.DateIncrements;

namespace Core.Data.ConnectionStrings;

public class ExcelConnectionString : IConnectionString
{
   public string ConnectionString => "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={file};" +
      "Extended Properties=Excel 8.0";

   public TimeSpan ConnectionTimeout => 30.Seconds();
}