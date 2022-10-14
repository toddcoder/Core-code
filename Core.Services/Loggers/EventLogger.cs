using System.Diagnostics;

namespace Core.Services.Loggers;

public class EventLogger
{
   protected string applicationName;

   public EventLogger(string applicationName)
   {
      this.applicationName = applicationName;
      if (!EventLog.SourceExists(this.applicationName))
      {
         EventLog.CreateEventSource(this.applicationName, "Application");
      }
   }

   public void Write(string format, params object[] args)
   {
      var message = string.Format(format, args);
      try
      {
         EventLog.WriteEntry(applicationName, message, EventLogEntryType.Error);
      }
      catch { }
   }
}