using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.Computers;
using Core.DataStructures;
using Core.Exceptions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Loggers;

public class Logger : IDisposable
{
   protected const string RULE = "--------------------------------------------------------------------------------";

   protected StringWriter writer;
   protected string indentation;
   protected MaybeStack<string> indentations;
   protected DateTime now;

   public Logger(int indentation = 0)
   {
      this.indentation = " ".Repeat(indentation);

      Key = nil;
      MinDateTime = nil;
      MaxDateTime = nil;

      writer = new StringWriter();
      indentations = new MaybeStack<string>();
      now = DateTime.MinValue;
   }

   public Maybe<string> Key { get; set; }

   public Maybe<DateTime> MinDateTime { get; set; }

   public Maybe<DateTime> MaxDateTime { get; set; }

   public void PushIndentation(int amount = 2)
   {
      indentations.Push(indentation);
      var extra = " ".Repeat(amount);
      indentation = $"{indentation}{extra}";
   }

   public void PopIndentation()
   {
      var _oldIndentation = indentations.Pop();
      if (_oldIndentation)
      {
         indentation = _oldIndentation;
      }
   }

   protected void setMinDateTime(DateTime now)
   {
      if (MinDateTime)
      {
         if (now < MinDateTime)
         {
            MinDateTime = now;
         }
      }
      else
      {
         MinDateTime = now;
      }
   }

   protected void setMaxDateTime(DateTime now)
   {
      if (MaxDateTime)
      {
         if (now > MaxDateTime)
         {
            MaxDateTime = now;
         }
      }
      else
      {
         MaxDateTime = now;
      }
   }

   public virtual void WriteRaw(char prefix, string message)
   {
      var now = DateTime.Now;
      setMinDateTime(now);
      setMaxDateTime(now);

      writer.Write($"{now:O} |{prefix}| {indentation}{message}");
   }

   public void Write(LogItemType type, string message)
   {
      var prefix = type switch
      {
         LogItemType.Message => '.',
         LogItemType.Success => '!',
         LogItemType.Failure => '?',
         LogItemType.Exception => '*',
         _ => '~'
      };
      WriteRaw(prefix, message);
   }

   public void WriteMessage(string message) => Write(LogItemType.Message, message);

   public void WriteSuccess(string message) => Write(LogItemType.Success, message);

   public void WriteFailure(string message) => Write(LogItemType.Failure, message);

   public void WriteException(Exception exception)
   {
      var deepStack = exception.DeepStack().Lines();
      if (deepStack.Length > 0)
      {
         Write(LogItemType.Exception, deepStack[0]);
         PushIndentation();

         foreach (var line in deepStack.Skip(1))
         {
            Write(LogItemType.Exception, line);
         }

         PopIndentation();
      }
   }

   public void WriteRule() => writer.WriteLine(RULE);

   public void Flush(FileName logFile)
   {
      logFile.Encoding = Encoding.UTF8;
      logFile.Text = writer.ToString();
   }

   public Result<Unit> TryToFlush(FileName logFile) => tryTo(() => Flush(logFile));

   public void Flush(StringWriter outerWriter) => outerWriter.Write(writer.ToString());

   public void Dispose() => writer?.Dispose();
}