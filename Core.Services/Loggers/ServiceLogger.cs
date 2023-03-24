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

namespace Core.Services.Loggers;

public class ServiceLogger : BaseWriter, IServiceMessage
{
   public const string THIN_SEPARATOR = "--------------------------------------------------------------------------------";
   public const string FAT_SEPARATOR = "================================================================================";

   protected static Optional<FolderName> getBaseFolder(Configuration configuration, string jobName)
   {
      try
      {
         var _baseFolder = configuration.Result.FolderName("baseFolder");
         if (_baseFolder is (true, var baseFolder))
         {
            var folder = configuration.Maybe.FolderName("logs").Map(logs => logs.Subfolder(baseFolder.Name)) | (() => baseFolder[jobName]);
            return folder.Subfolder(jobName);
         }
         else
         {
            return _baseFolder.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected static Optional<ServiceLogger> fromConfiguration(Configuration configuration,
      Func<FolderName, string, int, TimeSpan, Optional<EventLogger>, Optional<ServiceLogger>> creator)
   {
      var _jobName = configuration.Maybe.String("name");
      if (_jobName)
      {
         var _loggingSetting = configuration.Maybe.Setting("logging");
         var sizeLimit = _loggingSetting.Map(g => g.Maybe.Int32("sizeLimit")) | 1000000;
         var expiry = _loggingSetting.Map(g => g.Maybe.TimeSpan("expiry")) | (() => 7.Days());
         Optional<EventLogger> _eventLogger;
         try
         {
            _eventLogger = new EventLogger(_jobName);
         }
         catch
         {
            _eventLogger = nil;
         }

         return getBaseFolder(configuration, _jobName).Map(baseFolder => creator(baseFolder, _jobName, sizeLimit, expiry, _eventLogger));
      }
      else
      {
         return fail("Job name was not found");
      }
   }

   public static Optional<ServiceLogger> FromConfiguration(Configuration configuration)
   {
      static Optional<ServiceLogger> creator(FolderName baseFolder, string jobName, int sizeLimit, TimeSpan expiry, Optional<EventLogger> _eventLogger)
      {
         return new ServiceLogger(baseFolder, jobName, sizeLimit, expiry, _eventLogger);
      }

      return fromConfiguration(configuration, creator);
   }

   protected FolderName baseFolder;
   protected string jobName;
   protected string compareTimeStamp;
   protected Optional<EventLogger> _eventLogger;
   protected Stopwatch stopwatch;
   protected FileName currentLog;
   protected object locker;

   internal ServiceLogger(FolderName baseFolder, string jobName, int sizeLimit, TimeSpan expiry, Optional<EventLogger> _eventLogger)
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

   protected void setData(Setting setting, string name, FolderName folder)
   {
      jobName = name;
      baseFolder = folder;

      var _loggingSetting = setting.Maybe.Setting("logging");
      if (_loggingSetting is (true, var loggingSetting))
      {
         SizeLimit = loggingSetting.Maybe.Int32("sizeLimit") | 1000000;
         Expiry = loggingSetting.Maybe.TimeSpan("expiry") | (() => 7.Days());
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
         if (_eventLogger is (true, var eventLogger))
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