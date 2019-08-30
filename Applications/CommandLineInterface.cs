using System;
using System.Linq;
using System.Reflection;
using Core.Applications.Writers;
using Core.Collections;
using Core.Computers;
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

      protected Hash<string, string> aliases;

      public CommandLineInterface() : this(getStandardWriter()) { }

      public CommandLineInterface(IWriter standardWriter) : this(standardWriter, getExceptionWriter()) { }

      public CommandLineInterface(IWriter standardWriter, IWriter exceptionWriter)
      {
         StandardWriter = standardWriter;
         ExceptionWriter = exceptionWriter;
         Test = false;
         Running = false;
         aliases = new Hash<string, string>();
      }

      public IWriter StandardWriter { get; set; }

      public IWriter ExceptionWriter { get; set; }

      public bool Test { get; set; }

      public bool Running { get; set; }

      public string Application { get; set; } = "";

      public virtual void HandleException(Exception exception) => ExceptionWriter.WriteExceptionLine(exception);

      public void Run(string prefix = "/", string suffix = ":")
      {
         Run(Environment.CommandLine, prefix, suffix);
      }

      public void Run(string commandLine, string prefix, string suffix)
      {
         var aliasFile = FolderName.LocalApplicationData.Subfolder(Application) + "aliases.txt";
         if (Application.IsNotEmpty() && aliasFile.Exists() && aliasFile.Length > 0)
         {
            LoadAliases(aliasFile);
         }

         runUsingParameters(prefix, suffix, commandLine);

         if (Application.IsNotEmpty())
         {
            SaveAliases(aliasFile);
         }
      }

      public virtual void LoadAliases(FileName aliasFile)
      {
         try
         {
            aliases = aliasFile.Lines.Select(line => line.Split('~').ToTuple2()).ToHash(i => i.Item1, i => i.Item2);
         }
         catch (Exception exception)
         {
            throw new ApplicationException($"Alias file {aliasFile} couldn't be loaded", exception);
         }
      }

      public virtual void SaveAliases(FileName aliasFile)
      {
         aliasFile.Lines = aliases.Select(kv => $"{kv.Key}~{kv.Value}").ToArray();
      }

      IResult<(MethodInfo, bool)> getEntryPoint()
      {
         return GetType()
            .GetMethods()
            .Select(mi => (methodInfo: mi, attr: mi.GetCustomAttribute<EntryPointAttribute>()))
            .Where(t => t.attr != null)
            .FirstOrFail("")
            .Map(t => (t.methodInfo, t.attr.UsesObject));
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

            if (matcher.IsMatch(commandLine, $"'{prefix}' {namePattern(name)} $") && type == typeof(bool))
            {
               return true.Success<object>();
            }

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

      static string removeExecutableFromCommandLine(string commandLine)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(commandLine, "^ (.+ '.exe' /s* [quote]? /s*) /s* /(.*) $"))
         {
            return matcher.FirstGroup;
         }
         else
         {
            return commandLine;
         }
      }

      static string fixCommand(string commandLine, string prefix, string suffix)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(commandLine, "^ /([/w '-']+) /s*"))
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

      protected bool setPossibleAlias(string commandLine)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(commandLine, "^ /s+ /([/w '-']+) /s+ /(.*) $"))
         {
            var aliasName = matcher.FirstGroup;
            var command = matcher.SecondGroup;
            aliases[aliasName] = command;

            return true;
         }

         else
         {
            return false;
         }
      }

      protected static IMaybe<string> getCommand(string commandLine)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(commandLine, "^ /([/w '-']+) /b"))
         {
            return matcher.FirstGroup.Some();
         }
         else
         {
            return none<string>();
         }
      }

      public void runUsingParameters(string prefix, string suffix, string commandLine)
      {
         commandLine = removeExecutableFromCommandLine(commandLine);

         if (getCommand(commandLine).If(out var command))
         {
            if (command == "alias")
            {
               if (setPossibleAlias(commandLine.Drop(command.Length)))
               {
                  return;
               }
            }
            else if (aliases.If(command, out var replacement))
            {
               commandLine = replacement;
            }
         }

         commandLine = fixCommand(commandLine, prefix, suffix);

         if (getEntryPoint().If(out var tuple, out var entryPointException))
         {
            var (methodInfo, usesObject) = tuple;
            if (usesObject)
            {
               useWithObject(methodInfo, prefix, suffix, commandLine);
            }
            else
            {
               useWithParameters(methodInfo, prefix, suffix, commandLine);
            }
         }
         else
         {
            HandleException(entryPointException);
         }
      }

      void useWithParameters(MethodInfo methodInfo, string prefix, string suffix, string commandLine)
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

      void useWithObject(MethodInfo methodInfo, string prefix, string suffix, string commandLine)
      {
         var anyArgument =
            from parameterInfo in methodInfo.GetParameters().Take(1).FirstOrFail("Couldn't retrieve object information")
            from obj in parameterInfo.ParameterType.TryCreate()
            from filledObject in fillObject(obj, prefix, suffix, commandLine)
            select filledObject;
         if (anyArgument.If(out var argument, out var argumentException))
         {
            try
            {
               Running = true;
               methodInfo.Invoke(this, new[] { argument });
            }
            catch (Exception exception)
            {
               HandleException(exception);
            }
         }
         else
         {
            HandleException(argumentException);
         }
      }

      static string xmlToPascal(string name)
      {
         var matcher = new Matcher();
         if (name.IsMatch("'-' /(/w)"))
         {
            for (var matchIndex = 0; matchIndex < matcher.MatchCount; matchIndex++)
            {
               var letter = matcher.FirstGroup;
               matcher[matchIndex] = letter.ToUpper();
            }

            return matcher.ToString().ToPascal();
         }
         else
         {
            return name.ToPascal();
         }
      }

      static IResult<object> fillObject(object emptyObject, string prefix, string suffix, string commandLine)
      {
         try
         {
            var evaluator = new PropertyEvaluator(emptyObject);
            if (prefix == "/")
            {
               prefix = "//";
            }

            var pattern = $"'{prefix}' /([/w '-']) '{suffix}'";
            var matcher = new Matcher();
            if (matcher.IsMatch(commandLine, pattern))
            {
               foreach (var match in matcher)
               {
                  var name = xmlToPascal(match.FirstGroup);
                  var rest = commandLine.Drop(matcher.Index + matcher.Length);
                  if (evaluator.ContainsKey(name))
                  {
                     var type = evaluator.Type(name);
                     if (type == typeof(bool))
                     {
                        if (suffix == " " && (!rest.IsMatch("^ /s* 'false' | 'true'") || rest.IsEmpty()))
                        {
                           evaluator[name] = true;
                        }
                        else
                        {
                           evaluator[name] = rest.Keep("^ /s* ('false' | 'true') /b").TrimStart().ToBool();
                        }
                     }
                     else if (type == typeof(string))
                     {
                        if (rest.IsMatch("^ /s* [quote]"))
                        {
                           evaluator[name] = rest.Keep("^ /s* /([quote]) .*? /1").TrimStart().Drop(1).Drop(-1);
                        }
                        else
                        {
                           evaluator[name] = rest.Keep("^ /s* -/s+").TrimStart();
                        }
                     }
                     else if (type == typeof(int))
                     {
                        evaluator[name] = rest.Keep("^ /s* -/s+").TrimStart().ToInt();
                     }
                     else if (type == typeof(float) || type == typeof(double))
                     {
                        var source = rest.Keep("^ /s* -/s+").TrimStart();
                        if (type == typeof(float))
                        {
                           evaluator[name] = source.ToFloat();
                        }
                        else
                        {
                           evaluator[name] = source.ToDouble();
                        }
                     }
                     else if (type.IsEnum)
                     {
                        evaluator[name] = Enum.Parse(type, rest.Keep("^ /s* -/s+").TrimStart());
                     }
                     else
                     {
                        return $"No value for {name}".Failure<object>();
                     }
                  }
               }
            }

            return evaluator.Object.Success();
         }
         catch (Exception exception)
         {
            return failure<object>(exception);
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