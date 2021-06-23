using System;
using System.IO;
using System.Text;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Io
{
   public static class StreamExtensions
   {
      public static Result<string> FromStream(this Stream stream, Encoding encoding)
      {
         try
         {
            stream.Must().Not.BeNull().OrThrow();

            stream.Position = 0;

            using var reader = new StreamReader(stream, encoding);
            return reader.ReadToEnd().Success();
         }
         catch (Exception exception)
         {
            return failure<string>(exception);
         }
      }
   }
}