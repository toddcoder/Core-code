using System;
using System.Threading;
using Core.Applications.Writers;
using Core.Objects;

namespace Core.Applications
{
   public abstract class CommandLine : IDisposable
   {
      protected static IWriter getStandardWriter() => new ConsoleWriter();

      protected static IWriter getExceptionWriter() => new ConsoleWriter
      {
         ForegroundColor = ConsoleColor.Red,
         BackgroundColor = ConsoleColor.White
      };

      protected ManualResetEvent resetEvent;
      protected bool threading;

      public CommandLine(bool threading = false) : this(getStandardWriter(), threading) { }

      public CommandLine(IWriter standardWriter, bool threading = false) : this(standardWriter, getExceptionWriter(), threading) { }

      public CommandLine(IWriter standardWriter, IWriter exceptionWriter, bool threading = false)
      {
         StandardWriter = standardWriter;
         ExceptionWriter = exceptionWriter;
         Test = false;
         Running = true;
         this.threading = threading;
         if (threading)
         {
            resetEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, e) =>
            {
               resetEvent.Set();
               e.Cancel = true;
            };
         }
      }

      public void Wait()
      {
         if (threading)
         {
            resetEvent.WaitOne();
         }
      }

      public IWriter StandardWriter { get; set; }

      public IWriter ExceptionWriter { get; set; }

      public bool Test { get; set; }

      public bool Running { get; set; }

      public abstract void Execute(Arguments arguments);

      public virtual void HandleException(Exception exception) => ExceptionWriter.WriteExceptionLine(exception);

      public virtual void Deinitialize() { }

      public virtual void Run(string[] args)
      {
         var arguments = new Arguments(args);
         run(arguments);
      }

      protected void run(Arguments arguments)
      {
         try
         {
            Execute(arguments);
         }
         catch (Exception exception)
         {
            HandleException(exception);
         }

         if (Test)
         {
            Console.ReadLine();
         }
      }

      public virtual void RunInLoop(string[] args, TimeSpan interval)
      {
         var arguments = new Arguments(args);

         runInLoop(arguments, interval);
      }

      public virtual void RunInLoop(TimeSpan interval)
      {
         var arguments = new Arguments(Environment.CommandLine);
         runInLoop(arguments, interval);
      }

      protected void runInLoop(Arguments arguments, TimeSpan interval)
      {
         try
         {
            while (Running)
            {
               Execute(arguments);
               Thread.Sleep(interval);
            }
         }
         catch (Exception ex)
         {
            HandleException(ex);
         }

         if (Test)
         {
            Console.ReadLine();
         }
      }

      protected void dispose()
      {
         StandardWriter?.DisposeIfDisposable();
         ExceptionWriter?.DisposeIfDisposable();
      }

      public void Dispose()
      {
         dispose();
         GC.SuppressFinalize(this);
      }

      ~CommandLine() => dispose();
   }
}