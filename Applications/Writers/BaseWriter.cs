using System;
using Core.Exceptions;
using Core.Strings;

namespace Core.Applications.Writers;

public abstract class BaseWriter : IWriter
{
   protected string lineEnding;
   protected bool deepMessage;

   public BaseWriter()
   {
      lineEnding = "\r\n";
      deepMessage = true;
   }

   protected abstract void writeRaw(string text);

   public virtual void WriteRaw(string text) => writeRaw(text);

   public virtual void Write(string message) => writeRaw(message);

   public virtual void Write(object message) => Write(message.ToNonNullString());

   public virtual void WriteLine(string message) => Write(message + lineEnding);

   public virtual void WriteLine(object message) => WriteLine(message.ToNonNullString());

   public virtual void WriteException(Exception exception) => Write(deepMessage ? exception.DeepMessage() : exception.Message);

   public virtual void WriteExceptionLine(Exception exception)
   {
      WriteLine(deepMessage ? exception.DeepMessage() : exception.Message);
   }

   public virtual void WriteException(string message) => Write(message);

   public virtual void WriteException(object message) => Write(message);

   public virtual void WriteExceptionLine(string message) => WriteLine(message);

   public virtual void WriteExceptionLine(object message) => WriteLine(message);
}