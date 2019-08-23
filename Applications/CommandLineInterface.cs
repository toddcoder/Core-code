using System;
using System.Linq;
using System.Reflection;
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
   public abstract class CommandLineInterface : IDisposable
   {
      protected static IWriter getStandardWriter() => new ConsoleWriter();

      protected static IWriter getExceptionWriter() => new ConsoleWriter
      {
         ForegroundColor = ConsoleColor.Red,
         BackgroundColor = ConsoleColor.White
      };

      public CommandLineInterface() : this(getStandardWriter()) { }

      public CommandLineInterface(IWriter standardWriter) : this(standardWriter, getExceptionWriter()) { }

      public CommandLineInterface(IWriter standardWriter, IWriter exceptionWriter)
      {
         StandardWriter = standardWriter;
         ExceptionWriter = exceptionWriter;
         Test = false;
         Running = false;
      }

      public IWriter StandardWriter { get; set; }

      public IWriter ExceptionWriter { get; set; }

      public bool Test { get; set; }

      public bool Running { get; set; }

      public virtual void HandleException(Exception exception) => ExceptionWriter.WriteExceptionLine(exception);

      public void Run(string prefix = "/", string suffix = ":")
      {
         Run(Environment.CommandLine, prefix, suffix);
      }

      public void Run(string commandLine, string prefix, string suffix)
      {
         runUsingParameters(prefix, suffix, commandLine);
      }

      IResult<MethodInfo> getEntryPoint()
      {
         return GetType().GetMethods()
            .Where(m => m.GetCustomAttribute<EntryPointAttribute>() != null)
            .FirstOrFail(() => "No method with EntryPoint attribute found");
      }

      static string namePattern(string name)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(name, "['A-Z']"))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               matcher[i, 0] = $"-{matcher[i, 0].ToLower()}";
            }

            return $"('{name}' | '{matcher}')";
         }
         else
         {
            return $"'{name}'";
         }
      }

      static IResult<object> retrieveItem(string name, Type type, IMaybe<object> anyDefaultValue, string prefix, string suffix, string commandLine)
      {
         return tryTo(() =>
         {
            if (prefix == "/")
            {
               prefix = "//";
            }

            var itemPrefix = $"'{prefix}' {namePattern(name)} '{suffix}' /(.*)";
            var matcher = new Matcher();
            if (matcher.IsMatch(commandLine, itemPrefix))
            {
               var rest = matcher.FirstGroup;
               if (type == typeof(bool))
               {
                  if (suffix == " " && !rest.IsMatch("^ /s* 'false' | 'true'"))
                  {
                     return true.Success<object>();
                  }
                  else
                  {
                     return rest.Keep("^ /s* ('false' | 'true') /b").TrimStart().Boolean().Map(b => (object)b);
                  }
               }
               else if (type == typeof(string))
               {
                  if (rest.IsMatch("^ /s* [quote]"))
                  {
                     var source = rest.Keep("^ /s* /([quote]) .*? /1").TrimStart().Drop(1).Drop(-1);
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
                  return tryTo(() => Enum.Parse(type, source.Replace("-", ""), true));
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

      static string getCommand(string commandLine, string prefix, string suffix)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(commandLine, "^ .+ '.exe' /s* /([/w '-']+)"))
         {
            var command = matcher.FirstGroup;
            matcher.FirstGroup = $"{prefix}{command}{suffix}true";

            return matcher.ToString();
         }
         else if (matcher.IsMatch(commandLine, "^ /([/w '-']+)"))
         {
            var command = matcher.FirstGroup;
            matcher.FirstGroup = $"{prefix}{command}{suffix}true";

            return matcher.ToString();
         }
         else
         {
            return commandLine;
         }
      }

      public void runUsingParameters(string prefix, string suffix, string commandLine)
      {
         commandLine = getCommand(commandLine, prefix, suffix);
         if (getEntryPoint().If(out var methodInfo, out var entryPointException))
         {
            var arguments = methodInfo.GetParameters()
               .Select(p => (p.Name, p.ParameterType, anyDefaultValue: maybe(p.HasDefaultValue, () => p.DefaultValue)))
               .Select(t => retrieveItem(t.Name, t.ParameterType, t.anyDefaultValue, prefix, suffix, commandLine))
               .ToArray();
            if (arguments.FirstOrNone(p => p.IsFailed).If(out var failure))
            {
               HandleException(failure.Exception);
            }
            else
            {
               try
               {
                  Running = true;
                  methodInfo.Invoke(this, arguments.Select(a => a.ForceValue()).ToArray());
               }
               catch (Exception exception)
               {
                  HandleException(exception);
               }
               finally
               {
                  Running = false;
               }

               if (Test)
               {
                  Console.ReadLine();
               }
            }
         }
         else
         {
            HandleException(entryPointException);
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

      ~CommandLineInterface() => dispose();
   }
}