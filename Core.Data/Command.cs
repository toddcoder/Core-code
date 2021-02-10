using System;
using Core.Dates.DateIncrements;

namespace Core.Data
{
   public class Command
   {
      internal Command()
      {
         Name = string.Empty;
         CommandTimeout = 30.Seconds();
         Text = string.Empty;
      }

      public string Name { get; set; }

      public TimeSpan CommandTimeout { get; set; }

      public string Text { get; set; }
   }
}