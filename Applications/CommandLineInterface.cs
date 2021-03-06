﻿using System;
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
      protected const string REGEX_PARAMETER = "[/w '-']* /w";

      protected static IWriter getStandardWriter() => new ConsoleWriter();

      protected static IWriter getExceptionWriter() => new ConsoleWriter
      {
         ForegroundColor = ConsoleColor.Red,
         BackgroundColor = ConsoleColor.White
      };

      protected Hash<string, string> aliases;

      public event EventHandler<ConvertArgs> Convert;

      public CommandLineInterface() : this(getStandardWriter())
      {
      }

      public CommandLineInterface(IWriter standardWriter) : this(standardWriter, getExceptionWriter())
      {
      }

      public CommandLineInterface(IWriter standardWriter, IWriter exceptionWriter)
      {
         StandardWriter = standardWriter;
         ExceptionWriter = exceptionWriter;
         Test = false;
         Running = false;
         aliases = new Hash<string, string>();
         Application = string.Empty;
         Shortcuts = string.Empty;
         ShortPrefix = "-";
         ShortSuffix = " ";

         Console.CancelKeyPress += cancelKeyPress;
      }

      protected virtual void cancelKeyPress(object sender, ConsoleCancelEventArgs e)
      {
      }

      public IWriter StandardWriter { get; set; }

      public IWriter ExceptionWriter { get; set; }

      public bool Test { get; set; }

      public bool Running { get; set; }

      public string Application { get; set; }

      public string Shortcuts { get; set; }

      public string ShortPrefix { get; set; }

      public string ShortSuffix { get; set; }

      public virtual void HandleException(Exception exception) => ExceptionWriter.WriteExceptionLine(exception);

      public void Run(string prefix = "--", string suffix = " ")
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

      protected IResult<(MethodInfo, EntryPointType)> getEntryPoint()
      {
         return GetType()
            .GetMethods()
            .Select(mi => (methodInfo: mi, attr: mi.GetCustomAttribute<EntryPointAttribute>()))
            .Where(t => t.attr != null)
            .FirstOrFail("")
            .Map(t => (t.methodInfo, t.attr.Type));
      }

      protected static string namePattern(string name)
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

      protected static object getBoolean(string rest, string suffix)
      {
         if (suffix == " " && !rest.IsMatch("^ /s* 'false' | 'true' | '+' | '-'"))
         {
            return true;
         }
         else if (rest.IsMatch("^ /s* '+'"))
         {
            return true;
         }
         else if (rest.IsMatch("^ /s* '-'"))
         {
            return false;
         }
         else
         {
            return rest.Keep("^ /s* ('false' | 'true') /b").TrimStart().ToBool();
         }
      }

      protected object getString(string rest, Type type)
      {
         var source = rest.IsMatch("^ /s* [quote]") ? rest.Keep("^ /s* /([quote]) .*? /1").TrimStart().Drop(1).Drop(-1)
            : rest.Keep("^ /s* -/s+").TrimStart();

         if (type == typeof(string))
         {
            return source;
         }
         else if (type == typeof(IMaybe<string>))
         {
            return maybe(source.IsNotEmpty(), () => source);
         }
         else if (type == typeof(FolderName))
         {
            return (FolderName)source;
         }
         else if (type == typeof(FileName))
         {
            return (FileName)source;
         }
         else if (type == typeof(IMaybe<FolderName>))
         {
            return maybe(source.IsNotEmpty(), () => (FolderName)source);
         }
         else if (type == typeof(IMaybe<FileName>))
         {
            return maybe(source.IsNotEmpty(), () => (FileName)source);
         }
         else
         {
            if (Convert != null)
            {
               var args = new ConvertArgs(source);
               Convert.Invoke(this, args);
               return args.Result.DefaultTo(() => source);
            }
            else
            {
               return source;
            }
         }
      }

      protected static object getInt32(string rest)
      {
         return rest.Keep("^ /s* -/s+").TrimStart().ToInt();
      }

      protected static object getFloatingPoint(string rest, Type type)
      {
         var source = rest.Keep("^ /s* -/s+").TrimStart();
         return type == typeof(float) ? source.ToFloat() : source.ToDouble();
      }

      protected static object getEnum(string rest, Type type)
      {
         var source = rest.Keep("^ /s* -/s+").TrimStart();
         return Enum.Parse(type, source.Replace("-", ""), true);
      }

      protected IResult<object> retrieveItem(string name, Type type, IMaybe<object> _defaultValue, string prefix, string suffix, string commandLine)
      {
         return tryTo(() =>
         {
            if (prefix == "/")
            {
               prefix = "//";
            }

            var matcher = new Matcher();

            var itemPrefix = $"'{prefix}' {namePattern(name)} '{suffix}' /(.*)";

            if (matcher.IsMatch(commandLine, $"'{prefix}' {namePattern(name)} $") && type == typeof(bool))
            {
               return true.Success<object>();
            }

            if (matcher.IsMatch(commandLine, itemPrefix))
            {
               var rest = matcher.FirstGroup;
               if (type == typeof(bool))
               {
                  return getBoolean(rest, suffix).Success();
               }
               else if (type == typeof(string) || type == typeof(FileName) || type == typeof(FolderName))
               {
                  return getString(rest, type).Success();
               }
               else if (type == typeof(int))
               {
                  return getInt32(rest).Success();
               }
               else if (type == typeof(float) || type == typeof(double))
               {
                  return getFloatingPoint(rest, type).Success();
               }
               else if (type.IsEnum)
               {
                  return getEnum(rest, type).Success();
               }
               else if (this is IMissingArgument missingArgument)
               {
                  return missingArgument.BadType(name, type, rest);
               }
               else
               {
                  return $"Can't handle type {type.Name}".Failure<object>();
               }
            }
            else if (_defaultValue.If(out var defaultValue))
            {
               return defaultValue.Success();
            }
            else
            {
               return $"No value for {name}".Failure<object>();
            }
         });
      }

      protected static string removeExecutableFromCommandLine(string commandLine)
      {
         var matcher = new Matcher();
         return matcher.IsMatch(commandLine, "^ (.+ '.exe' /s* [quote]? /s*) /s* /(.*) $") ? matcher.FirstGroup : commandLine;
      }

      protected static Hash<char, string> getShortcuts(string source)
      {
         return source.Split("/s* ';' /s*").Select(s =>
         {
            var pair = s.Split("/s* '=' /s*");
            return (key: pair[0][0], value: pair[1]);
         }).ToHash(i => i.key, i => i.value);
      }

      protected string fixCommand(string commandLine, string prefix, string suffix)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(commandLine, $"^ /({REGEX_PARAMETER}) /s*"))
         {
            var command = matcher.FirstGroup;
            matcher.FirstGroup = $"{prefix}{command}{suffix}true";

            commandLine = matcher.ToString();
         }

         if (Shortcuts.IsEmpty())
         {
            return commandLine;
         }

         if (matcher.IsMatch(commandLine, $"'{ShortPrefix}' /['a-zA-Z0-9'] '{ShortSuffix}'"))
         {
            var shortcuts = getShortcuts(Shortcuts);
            for (var matchIndex = 0; matchIndex < matcher.MatchCount; matchIndex++)
            {
               var key = matcher[matchIndex, 1][0];
               if (shortcuts.If(key, out var replacement))
               {
                  matcher[matchIndex, 0] = $"{prefix}{replacement}{suffix}";
               }
               else
               {
                  matcher[matchIndex, 0] = "";
               }
            }

            commandLine = matcher.ToString();
         }

         return commandLine;
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
         return commandLine.Matcher($"^ /({REGEX_PARAMETER}) /b").Map(matcher => matcher.FirstGroup);
      }

      protected IMaybe<string> getCommandsFromFile(string prefix, string suffix, string commandLine)
      {
         var opensCommandFile = $"{prefix}cmd{suffix}";
         if (commandLine.StartsWith(opensCommandFile))
         {
            var rest = commandLine.Drop(opensCommandFile.Length).TrimStart();
            var command = rest.KeepUntil(" ");
            var remainder = rest.Drop(command.Length).TrimStart();

            FileName file;
            if (this is ICommandFile commandFile)
            {
               file = commandFile.CommandFile(command);
            }
            else
            {
               file = rest;
            }

            var text = file.Text;
            return remainder.IsNotEmpty() ? $"{text} {remainder}".Some() : text.Some();
         }
         else
         {
            if (commandLine.Matcher($"^ /({REGEX_PARAMETER})'{suffix}'").If(out var matcher))
            {
               var commandName = matcher.FirstGroup;
               var remainder = commandLine.Drop(commandName.Length);
               if (this is ICommandFile commandFile)
               {
                  var file = commandFile.CommandFile(commandName);
                  if (file.Exists())
                  {
                     var text = file.Text;
                     return remainder.IsNotEmpty() ? $"{text} {remainder}".Some() : text.Some();
                  }
               }
            }

            return none<string>();
         }
      }

      public void runUsingParameters(string prefix, string suffix, string commandLine)
      {
         commandLine = removeExecutableFromCommandLine(commandLine);

         if (getCommandsFromFile(prefix, suffix, commandLine).If(out var newCommandLine))
         {
            commandLine = newCommandLine;
         }

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
            var (methodInfo, type) = tuple;
            switch (type)
            {
               case EntryPointType.Parameters:
                  useWithParameters(methodInfo, prefix, suffix, commandLine);
                  break;
               case EntryPointType.Object:
                  useWithObject(methodInfo, prefix, suffix, commandLine);
                  break;
               case EntryPointType.This:
                  useWithThis(methodInfo, prefix, suffix, commandLine);
                  break;
            }
         }
         else
         {
            HandleException(entryPointException);
         }
      }

      protected static void pressAKeyToEnd()
      {
         Console.Write("Press a key to end: ");
         Console.ReadKey();
      }

      protected void useWithParameters(MethodInfo methodInfo, string prefix, string suffix, string commandLine)
      {
         var arguments = methodInfo.GetParameters()
            .Select(p => (p.Name, p.ParameterType, defaultValue: maybe(p.HasDefaultValue, () => p.DefaultValue)))
            .Select(t => retrieveItem(t.Name, t.ParameterType, t.defaultValue, prefix, suffix, commandLine))
            .ToArray();
         if (arguments.FirstOrNone(p => p.IsFailed).If(out var failure))
         {
            if (failure.IfNot(out var exception))
            {
               HandleException(exception);
            }
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
               pressAKeyToEnd();
            }
         }
      }

      protected void useWithObject(MethodInfo methodInfo, string prefix, string suffix, string commandLine)
      {
         var _argument =
            from parameterInfo in methodInfo.GetParameters().Take(1).FirstOrFail("Couldn't retrieve object information")
            from obj in parameterInfo.ParameterType.TryCreate()
            from filledObject in fillObject(obj, prefix, suffix, commandLine)
            select filledObject;
         if (_argument.If(out var argument, out var argumentException))
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
            finally
            {
               Running = false;
            }
         }
         else
         {
            HandleException(argumentException);
         }

         if (Test)
         {
            pressAKeyToEnd();
         }
      }

      protected void useWithThis(MethodInfo methodInfo, string prefix, string suffix, string commandLine)
      {
         if (fillObject(this, prefix, suffix, commandLine).If(out _, out var filledException))
         {
            try
            {
               Running = true;
               methodInfo.Invoke(this, new object[0]);
            }
            catch (Exception exception)
            {
               HandleException(exception);
            }
            finally
            {
               Running = false;
            }
         }
         else
         {
            HandleException(filledException);
         }

         if (Test)
         {
            pressAKeyToEnd();
         }
      }

      protected static string xmlToPascal(string name)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(name, "'-' /(/w)"))
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

      protected IResult<object> fillObject(object emptyObject, string prefix, string suffix, string commandLine)
      {
         try
         {
            var evaluator = new PropertyEvaluator(emptyObject);
            if (prefix == "/")
            {
               prefix = "//";
            }

            var pattern = $"'{prefix}' /({REGEX_PARAMETER}) ('{suffix}' | ['+-'] ('{suffix}' | $) | $)";
            var matcher = new Matcher();
            if (matcher.IsMatch(commandLine, pattern))
            {
               var isFirstMatch = true;
               foreach (var match in matcher)
               {
                  var name = xmlToPascal(match.FirstGroup);
                  var (text, index, length) = match;
                  if (text.EndsWith("-") || text.EndsWith("+"))
                  {
                     length--;
                  }

                  var rest = commandLine.Drop(index + length);
                  if (evaluator.ContainsKey(name))
                  {
                     var type = evaluator.Type(name);
                     if (type == typeof(bool))
                     {
                        evaluator[name] = getBoolean(rest, suffix);
                     }
                     else if (type == typeof(string) || type == typeof(IMaybe<string>) || type == typeof(FileName) || type == typeof(FolderName) ||
                        type == typeof(IMaybe<FileName>) || type == typeof(IMaybe<FolderName>))
                     {
                        evaluator[name] = getString(rest, type);
                     }
                     else if (type == typeof(int))
                     {
                        evaluator[name] = getInt32(rest);
                     }
                     else if (type == typeof(float) || type == typeof(double))
                     {
                        evaluator[name] = getFloatingPoint(rest, type);
                     }
                     else if (type.IsEnum)
                     {
                        evaluator[name] = getEnum(rest, type);
                     }
                     else
                     {
                        return $"No value for {name}".Failure<object>();
                     }
                  }
                  else
                  {
                     if (emptyObject is IMissingArgument missingArgument && missingArgument.Handled(name, rest, isFirstMatch))
                     {
                        continue;
                     }

                     if (isFirstMatch)
                     {
                        isFirstMatch = false;
                        return $"Did not understand command '{name}'".Failure<object>();
                     }
                     else
                     {
                        return $"Did not understand argument '{name}".Failure<object>();
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