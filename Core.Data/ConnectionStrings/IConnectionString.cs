using System;

namespace Core.Data.ConnectionStrings
{
   public interface IConnectionString
   {
      string ConnectionString { get; }

      TimeSpan ConnectionTimeout { get; }
   }
}