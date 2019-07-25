using System;
using System.Linq;
using System.Threading;
using Core.Applications.Writers;
using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Applications
{
   public abstract class CommandLine : IDisposable
   {
      static IWriter getStandardWriter() => new ConsoleWriter();

      static IWriter getExceptionWriter() => new ConsoleWriter
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

      public string EntryPointName { get; set; } = "Entry";

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

      public virtual void Run(string prefix = "/", string suffix=":")
      {
         runUsingParameters(prefix, suffix, Environment.CommandLine);
      }

      void run(Arguments arguments)
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

      public void runUsingParameters(string prefix, string suffix, string commandLine)
      {
         IResult<object> retrieveItem(string name, Type type, IMaybe<object> anyDefaultValue)
         {
            return tryTo(() =>
            {
               if (prefix == "/")
               {
                  prefix = "//";
               }

               var itemPrefix = $"'{prefix}' '{name}' '{suffix}' /(.*)";
               var matcher = new Matcher();
               if (matcher.IsMatch(commandLine, itemPrefix))
               {
                  var rest = matcher.FirstGroup;
                  if (type == typeof(string))
                  {
                     if (rest.IsMatch("^ /s* [quote]"))
                     {
                        var source = rest.Keep("^ /s* /([quote]) .* /1").TrimStart().Drop(1).Drop(-1);
                        return source.Success<object>();
                     }
                     else
                     {
                        return rest.Keep("^ /s* -/s+").TrimStart().Success<object>();
                     }
                  }
                  else if (type == typeof(int))
                  {
                     var value = rest.Keep("^ /s* -/s+").TrimStart().Int32();
                     return value.Map(i => (object)i);
                  }
                  else if (type == typeof(float) || type == typeof(double))
                  {
                     var source = rest.Keep("^ /s* -/s+").TrimStart();
                     if (type == typeof(float))
                     {
                        return source.Single().Map(f => (object)f);
                     }
                     else
                     {
                        return source.Double().Map(d => (object)d);
                     }
                  }
                  else if (type.IsEnum)
                  {
                     var source = rest.Keep("^ /s* -/s+").TrimStart();
                     return tryTo(() => Enum.Parse(type, source, true));
                  }
                  else
                  {
                     return $"Can't handle type {type.Name}".Failure<object>();
                  }
               }
               else if (anyDefaultValue.If(out var defaultValue))
               {
                  return defaultValue.Success();
               }
               else
               {
                  return $"No value for {name}".Failure<object>();
               }
            });
         }

         if (GetType().GetMethod(EntryPointName).SomeIfNotNull().If(out var methodInfo))
         {
            var arguments = methodInfo.GetParameters()
               .Select(p => (p.Name, p.ParameterType, anyDefaultValue: maybe(p.HasDefaultValue, () => p.DefaultValue)))
               .Select(t => retrieveItem(t.Name, t.ParameterType, t.anyDefaultValue));
            var firstFailure = arguments.FirstOrNone(p => p.IsFailed);
            if (firstFailure.If(out var failure))
            {
               HandleException(failure.Exception);
            }
            else
            {
               try
               {
                  methodInfo.Invoke(this, arguments.Select(a => a.ForceValue()).ToArray());
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
         }
         else
         {
            HandleException(new ApplicationException($"Couldn't find {EntryPointName}"));
         }
      }

      //public void Run(string prefix, string suffix) => runUsingParameters(prefix, suffix, Environment.CommandLine);

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

      void runInLoop(Arguments arguments, TimeSpan interval)
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

      void dispose()
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