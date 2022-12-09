using System;

namespace Core.Applications.Writers;

public interface IWriter
{
   void WriteRaw(string text);

   void Write(string message);

   void Write(object message);

   void WriteLine(string message);

   void WriteLine(object message);

   void WriteException(Exception exception);

   void WriteException(string message);

   void WriteException(object message);

   void WriteExceptionLine(Exception exception);

   void WriteExceptionLine(string message);

   void WriteExceptionLine(object message);
}