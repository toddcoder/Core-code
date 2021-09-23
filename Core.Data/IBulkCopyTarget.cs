using System;
using System.Data;

namespace Core.Data
{
   public interface IBulkCopyTarget
   {
      string TableName { get; set; }

      void Copy<T>(Adapter<T> sourceAdapter) where T : class;

      void Copy(IDataReader reader, TimeSpan timeout);
   }
}