using System;
using System.Diagnostics;
using System.Linq;
using Core.Applications.Writers;
using Core.Computers;
using Core.Configurations;
using Core.Dates;
using Core.Dates.DateIncrements;
using Core.Dates.Now;
using Core.Exceptions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Services.Loggers
{
   public class ServiceLogger : BaseWriter, IServiceMessage
   {
      public const string THIN_SEPARATOR = "--------------------------------------------------------------------------------";
      public const string FAT_SEPARATOR = "================================================================================";

      protected static Result<FolderName> getBaseFolder(Configuration configuration, string jobName)
      {
         try
         {
            var _baseFolder = configuration.RequireValue("baseFolder").Map(bf => (FolderName)bf);
            if (_baseFolder.Map(out var baseFolder, out var exception))
            {
               var folder = configuration.GetValue("logs").Map(logs => ((FolderName)logs).Subfolder(baseFolder.Name))
                  .DefaultTo(() => baseFolder[jobName]);
               return folder.Subfolder(jobName);
            }
            else
            {
               return exception;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      protected static Result<ServiceLogger> fromConfiguration(Configuration configuration,
         Func<FolderName, string, int, TimeSpan, Maybe<EventLogger>, Result<ServiceLogger>> creator)
      {
         if (configuration.GetValue("name").Map(out var jobName))
         {
            var _loggingGroup = configuration.GetGroup("logging");
            var sizeLimit = _loggingGroup.Map(g => g.GetValue("sizeLimit")).Map(Maybe.Int32).DefaultTo(() => 1000000);
            var expiry = _loggingGroup.Map(g => g.GetValue("expiry")).Map(Maybe.TimeSpan).DefaultTo(() => 7.Days());
            Maybe<EventLogger> _eventLogger;
            try
            {
               _eventLogger = new EventLogger(jobName);
            }
            catch
            {
               _eventLogger = nil;
            }

            return getBaseFolder(configuration, jobName).Map(baseFolder => creator(baseFolder, jobName, sizeLimit, expiry, _eventLogger));
         }
         else
         {
            return fail("Job name was not found");
         }
      }

      public static Result<ServiceLogger> FromConfiguration(Configuration configuration)
      {
         static Result<ServiceLogger> creator(FolderName baseFolder, string jobName, int sizeLimit, TimeSpan expiry, Maybe<EventLogger> _eventLogger)
         {
            return new ServiceLogger(baseFolder, jobName, sizeLimit, expiry, _eventLogger);
         }

         return fromConfiguration(configuration, creator);
      }

      protected FolderName baseFolder;
      protected string jobName;
      protected string compareTimeStamp;
      protected Maybe<EventLogger> _eventLogger;
      protected Stopwatch stopwatch;
      protected FileName currentLog;
      protected object locker;

      internal ServiceLogger(FolderName baseFolder, string jobName, int sizeLimit, TimeSpan expiry, Maybe<EventLogger> _eventLogger)
      {
         this.baseFolder = baseFolder;
         this.jobName = jobName;
         SizeLimit = sizeLimit;
         Expiry = expiry;
         this._eventLogger = _eventLogger;

         stopwatch = new Stopwatch();
         compareTimeStamp = currentTimeStamp();
         WriteDate = true;
         currentLog = currentLogPath();
         locker = new object();
      }

      protected void appendText(string text)
      {
         if (logTooBig() || logTooOld())
         {
            lock (locker)
            {
               currentLog.Flush();
               currentLog = currentLogPath();
            }
         }

         lock (locker)
         {
            currentLog.Append(text);
         }
      }

      public FolderName BaseFolder => baseFolder;

      public void Begin() => OnDispatch();

      public void BeginTiming() => stopwatch.Start();

      public void EmitWarningMessage(string message) => WriteLine($"Warning: {message}");

      public virtual void Commit()
      {
         WriteLine(FAT_SEPARATOR);
         WriteLine($"Dispatch ends at {NowServer.Now:yyyy/MM/dd HH:mm:ss.ffff}");
         WriteLine(THIN_SEPARATOR);
         WriteLine("");
      }

      protected static string currentDateText() => NowServer.Today.ToString("yyyy-MM-dd");

      protected static string currentTimeStamp() => NowServer.Now.ToString("yyyy-MM-dd HH:mm:ss");

      protected void deleteExpiredLogs()
      {
         foreach (var folder in baseFolder.Folders)
         {
            if (folder.Name != currentDateText())
            {
               var files = folder.Files.ToArray();
               if (files.Any())
               {
                  foreach (var file in files.Where(f => f.LastWriteTime + Expiry < NowServer.Now))
                  {
                     file.Delete();
                  }
               }
               else
               {
                  folder.Delete();
               }
            }
         }
      }

      public void EmitException(Exception exception) => WriteExceptionLine(exception);

      public void EmitExceptionAttempt(Exception exception, int retry)
      {
         Write(retry == 0 ? "First try" : $"Retry {retry}:");
         WriteExceptionLine(exception);
      }

      public void EmitExceptionMessage(object message) => WriteExceptionLine(message);

      public void EmitExceptionMessage(string message) => WriteExceptionLine(message);

      public void EmitWarning(Exception exception)
      {
         Write("Warning: ");
         WriteExceptionLine(exception);
      }

      public void EmitWarningMessage(object message) => WriteLine($"Warning: {message}");

      public void EmitMessage(object message) => WriteLine(message);

      public void EmitMessage(string message) => WriteLine(message);

      public void EndTiming() => stopwatch.Start();

      protected string fileName(int index)
      {
         if (index < 0)
         {
            index = 0;
         }

         return $"{jobName}_{index.ToString().PadLeft(6, '0')}";
      }

      protected bool logTooBig()
      {
         lock (locker)
         {
            return currentLog.Exists() && currentLog.Length > SizeLimit;
         }
      }

      protected bool logTooOld()
      {
         lock (locker)
         {
            return currentLog.Exists() && currentLog.Folder.Name != currentDateText();
         }
      }

      public void OnDispatch()
      {
         WriteLine(THIN_SEPARATOR);
         WriteLine($"Dispatch begins at {NowServer.Now:yyyy/MM/dd HH:mm:ss.ffff}");
         WriteLine(FAT_SEPARATOR);
      }

      public void ResetTiming() => stopwatch.Reset();

      protected void setData(Group group, string name, FolderName folder)
      {
         jobName = name;
         baseFolder = folder;

         if (group.If("logging", out var loggingGroup))
         {
            SizeLimit = loggingGroup.GetValue("sizeLimit").Map(Maybe.Int32).DefaultTo(() => 1000000);
            Expiry = loggingGroup.GetValue("expiry").Map(Maybe.TimeSpan).DefaultTo(() => 7.Days());
         }
         else
         {
            SizeLimit = 1000000;
            Expiry = 7.Days();
         }

         stopwatch = new Stopwatch();
         compareTimeStamp = currentTimeStamp();
         WriteDate = true;
         currentLog = currentLogPath();
         locker = new object();
         try
         {
            _eventLogger = new EventLogger(name);
         }
         catch
         {
            _eventLogger = nil;
         }
      }

      protected FileName currentLogPath() => currentLogPath(baseFolder, jobName);

      protected static FileName currentLogPath(FolderName baseFolder, string jobName)
      {
         var folderName = baseFolder.Subfolder(currentDateText());
         var fileCount = folderName.FileCount;
         var logPath = folderName.File($"{jobName}_{fileCount.ToString().PadLeft(6, '0')}.log");
         logPath.UseBuffer = false;

         return logPath;
      }

      protected void writeCRLF() => WriteRaw("\r\n");

      public override void WriteException(Exception exception)
      {
         writeRaw($"<{exception.DeepException()}:{exception.InnerException?.Message ?? "no inner"}>");
      }

      public override void WriteExceptionLine(Exception exception)
      {
         WriteException(exception);
         writeCRLF();
      }

      public override void WriteLine(string message)
      {
         Write(message);
         writeCRLF();
      }

      protected void writePossibleTimeStamp(bool writeLeadingLine = true)
      {
         var newTimeStamp = currentTimeStamp();
         if (newTimeStamp == compareTimeStamp)
         {
            return;
         }

         if (WriteDate)
         {
            if (writeLeadingLine)
            {
               writeCRLF();
            }

            WriteRaw($"[ {newTimeStamp} ]");
            writeCRLF();
            writeCRLF();
         }

         compareTimeStamp = newTimeStamp;
      }

      protected override void writeRaw(string text)
      {
         if (text.IsEmpty())
         {
            return;
         }

         try
         {
            appendText(text);
         }
         catch (Exception exception)
         {
            if (_eventLogger.Map(out var eventLogger))
            {
               eventLogger.Write(text);
               eventLogger.Write(exception.Message);
            }
         }
      }

      public void WriteTime(bool includeMilliseconds = true) => Write(stopwatch.Elapsed.ToLongString(includeMilliseconds));

      public bool DateEnabled
      {
         get => WriteDate;
         set => WriteDate = value;
      }

      public TimeSpan Expiry { get; set; }

      public int SizeLimit { get; set; }

      public bool WriteDate { get; set; }
   }
}