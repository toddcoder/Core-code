using System;
using System.Data;
using System.IO;

namespace Core.Objects
{
   public static class DisposeHelps
   {
      public static void Close(this IDbConnection connection)
      {
         if ((connection?.State ?? ConnectionState.Open) != ConnectionState.Closed)
            connection.Close();
      }

      public static void Close(this Stream stream) => stream?.Close();

      public static bool IsDisposable(this object disposable) => disposable is IDisposable;

      public static void DisposeIfDisposable(this object obj)
      {
         if (obj is IDisposable disp)
            disp.Dispose();
      }
   }
}