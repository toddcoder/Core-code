using System;
using Core.Collections;
using Core.Dates.DateIncrements;

namespace Core.Data.ConnectionStrings
{
   public class TextConnectionString : IConnectionString
   {
      string fileName;
      string header;
      string delimited;

      public TextConnectionString(Connection connection)
      {
         fileName = connection.Value("file");
         header = connection.DefaultTo("header", "true") == "true" ? "YES" : "NO";
         delimited = connection.Value("delimited");
         switch (delimited)
         {
            case "comma":
            case ",":
               delimited = "CSVDelimited";
               break;
            case "tab":
            case "\t":
               delimited = "TabDelimited";
               break;
            default:
               delimited = $"Delimited({delimited.Substring(0, 1)})";
               break;
         }
      }

      public string ConnectionString => $"Provider=Microsoft.Jet.OLEDB.4.0; Data Source={fileName};" +
         $" Extended Properties=\"text;HDR={header};FMT={delimited}";

      public TimeSpan ConnectionTimeout => 30.Seconds();
   }
}