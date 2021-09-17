using System;
using System.Diagnostics;
using Core.Applications.Writers;
using Core.Exceptions;
using Core.Services.Loggers;
using Core.Strings;
using static System.Diagnostics.EventLog;

namespace Core.Services
{
   public class EventWriter : BaseWriter, IServiceMessage
   {
      protected string applicationName;

      public EventWriter(string applicationName)
      {
         this.applicationName = applicationName;
         if (!SourceExists(applicationName))
         {
            CreateEventSource(applicationName, "Application");
         }
      }

      protected override void writeRaw(string text)
      {
         try
         {
            WriteEntry(applicationName, text, EventLogEntryType.Error);
         }
         catch
         {
         }
      }

      public void Begin()
      {
      }

      public void EmitException(Exception exception) => writeRaw(exception.DeepException());

      public void EmitExceptionAttempt(Exception exception, int retry)
      {
         writeRaw((retry == 0 ? "Try" : $"Retry {retry}") + $" exception: {exception.DeepException()}");
      }

      public void EmitMessage(object message) => EmitMessage(message.ToNonNullString());

      public void EmitMessage(string message) => writeRaw(message);

      public void EmitExceptionMessage(object message) => EmitExceptionMessage(message.ToNonNullString());

      public void EmitExceptionMessage(string message) => writeRaw($"Exception: {message}");

      public void EmitWarning(Exception exception) => writeRaw($"Exception: {exception.DeepException()}");

      public void EmitWarningMessage(object message) => writeRaw($"Warning: {message}");

      public void EmitWarningMessage(string message) => writeRaw($"Warning: {message}");

      public void Commit()
      {
      }

      public bool DateEnabled { get; set; }
   }
}