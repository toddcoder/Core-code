using System;
using System.IO;
using System.Text;
using Core.Computers;
using Core.Strings;

namespace Core.Applications.Loggers;

public class Logger
{
   protected FileName logFile;
   protected StringWriter writer;
   protected string indentation;

   public Logger(FileName logFile, int indentation = 0)
   {
      this.logFile = logFile;
      this.logFile.Encoding = Encoding.UTF8;
      this.indentation = " ".Repeat(indentation);

      writer = new StringWriter();
   }

   protected void writeRaw(char prefix, string message) => writer.Write($"{DateTime.Now:O} |{prefix}| {indentation}{message}");

   public void Write(LogItemType type, string message)
   {
      var prefix = type switch
      {
         LogItemType.Message => '.',
         LogItemType.Success => '!',
         LogItemType.Failure => '?',
         LogItemType.Exception => 'x',
         _ => '~'
      };
      writeRaw(prefix, message);
   }

   public void WriteMessage(string message) => Write(LogItemType.Message, message);

   public void WriteSuccess(string message) => Write(LogItemType.Success, message);

   public void WriteFailure(string message) => Write(LogItemType.Failure, message);

   public void WriteException(Exception exception)
   {

   }
}