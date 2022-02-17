using System;
using System.Linq;
using System.Reflection;
using Core.Applications.Writers;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

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
         ErrorCode = 1;

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

      public int ErrorCode { get; set; }

      public virtual void HandleException(Exception exception)
      {
         ExceptionWriter.WriteExceptionLine(exception);
         Environment.ExitCode = ErrorCode;
      }

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

      protected Result<(MethodInfo, EntryPointType)> getEntryPoint()
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
         if (name.Matches("['A-Z']; f").If(out var result))
         {
            for (var i = 0; i < result.MatchCount; i++)
            {
               result[i, 0] = $"-{result[i, 0].ToLower()}";
            }

            return $"('{name}' | '{result}')";
         }
         else
         {
            return $"'{name}'";
         }
      }

      protected static object getBoolean(string rest, string suffix)
      {
         if (suffix == " " && !rest.IsMatch("^ /s* 'false' | 'true' | '+' | '-'; f"))
         {
            return true;
         }
         else if (rest.IsMatch("^ /s* '+'; f"))
         {
            return true;
         }
         else if (rest.IsMatch("^ /s* '-'; f"))
         {
            return false;
         }
         else
         {
            return Value.Boolean(rest.Keep("^ /s* ('false' | 'true') /b; f").TrimStart());
         }
      }

      protected object getString(string rest, Type type)
      {
         var source = rest.IsMatch("^ /s* [quote]; f") ? rest.Keep("^ /s* /([quote]) .*? /1; f").TrimStart().Drop(1).Drop(-1)
            : rest.Keep("^ /s* -/s+; f").TrimStart();

         if (type == typeof(string))
         {
            return source;
         }
         else if (type == typeof(Maybe<string>))
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
         else if (type == typeof(Maybe<FolderName>))
         {
            return maybe(source.IsNotEmpty(), () => (FolderName)source);
         }
         else if (type == typeof(Maybe<FileName>))
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
         return Value.Int32(rest.Keep("^ /s* -/s+; f").TrimStart());
      }

      protected static object getFloatingPoint(string rest, Type type)
      {
         var source = rest.Keep("^ /s* -/s+; f").TrimStart();
         return type == typeof(float) ? Value.Single(source) : Value.Double(source);
      }

      protected static object getEnum(string rest, Type type)
      {
         var source = rest.Keep("^ /s* -/s+; f").TrimStart();
         return Enum.Parse(type, source.Replace("-", ""), true);
      }

      protected static object getStringArray(string rest)
      {
         if (rest.Matches("^/s* '[' /s*  /(.*) /s* ']'; f").If(out var result))
         {
            var list = result.FirstGroup;
            var array = list.Unjoin("/s* ',' /s*; f");
            return array;
         }
         else
         {
            throw "String arrays must be delimited by []".Throws();
         }
      }

      protected Result<object> retrieveItem(string name, Type type, Maybe<object> _defaultValue, string prefix, string suffix, string commandLine)
      {
         return tryTo(() =>
         {
            if (prefix == "/")
            {
               prefix = "//";
            }

            var itemPrefix = $"'{prefix}' {namePattern(name)} '{suffix}' /(.*); f";

            if (commandLine.IsMatch($"'{prefix}' {namePattern(name)} $; f") && type == typeof(bool))
            {
               return true.Success<object>();
            }

            if (commandLine.Matches(itemPrefix).If(out var result))
            {
               var rest = result.FirstGroup;
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
               else if (type == typeof(string[]))
               {
                  return getStringArray(rest).Success();
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
         return commandLine.Matches("^ (.+? ('.exe' | '.dll') /s* [quote]? /s*) /s* /(.*) $; f")
            .Map(result => result.FirstGroup)
            .DefaultTo(() => commandLine);
      }

      protected static Hash<char, string> getShortcuts(string source)
      {
         return source.Unjoin("/s* ';' /s*; f").Select(s =>
         {
            var pair = s.Unjoin("/s* '=' /s*; f");
            return (key: pair[0][0], value: pair[1]);
         }).ToHash(i => i.key, i => i.value);
      }

      protected string fixCommand(string commandLine, string prefix, string suffix)
      {
         if (commandLine.Matches($"^ /({REGEX_PARAMETER}) /s*; f").If(out var result))
         {
            var command = result.FirstGroup;
            result.FirstGroup = $"{prefix}{command}{suffix}true";

            commandLine = result.ToString();
         }

         if (Shortcuts.IsEmpty())
         {
            return commandLine;
         }

         if (commandLine.Matches($"'{ShortPrefix}' /['a-zA-Z0-9'] '{ShortSuffix}'; f").If(out result))
         {
            var shortcuts = getShortcuts(Shortcuts);
            for (var matchIndex = 0; matchIndex < result.MatchCount; matchIndex++)
            {
               var key = result[matchIndex, 1][0];
               if (shortcuts.If(key, out var replacement))
               {
                  result[matchIndex, 0] = $"{prefix}{replacement}{suffix}";
               }
               else
               {
                  result[matchIndex, 0] = "";
               }
            }

            commandLine = result.ToString();
         }

         return commandLine;
      }

      protected bool setPossibleAlias(string commandLine)
      {
         if (commandLine.Matches("^ /s+ /([/w '-']+) /s+ /(.*) $; f").If(out var result))
         {
            var aliasName = result.FirstGroup;
            var command = result.SecondGroup;
            aliases[aliasName] = command;

            return true;
         }
         else
         {
            return false;
         }
      }

      protected static Maybe<string> getCommand(string commandLine)
      {
         return commandLine.Matches($"^ /({REGEX_PARAMETER}) /b; f").Map(result => result.FirstGroup);
      }

      protected Maybe<string> getCommandsFromFile(string prefix, string suffix, string commandLine)
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
            if (commandLine.Matches($"^ /({REGEX_PARAMETER})'{suffix}'; f").If(out var result))
            {
               var commandName = result.FirstGroup;
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
         if (name.Matches("'-' /(/w); f").If(out var result))
         {
            for (var matchIndex = 0; matchIndex < result.MatchCount; matchIndex++)
            {
               var letter = result.FirstGroup;
               result[matchIndex] = letter.ToUpper();
            }

            return result.ToString().ToPascal();
         }
         else
         {
            return name.ToPascal();
         }
      }

      protected Result<object> fillObject(object emptyObject, string prefix, string suffix, string commandLine)
      {
         try
         {
            var evaluator = new PropertyEvaluator(emptyObject);
            if (prefix == "/")
            {
               prefix = "//";
            }

            var pattern = $"'{prefix}' /({REGEX_PARAMETER}) ('{suffix}' | ['+-'] ('{suffix}' | $) | $); f";
            if (commandLine.Matches(pattern).If(out var result))
            {
               var isFirstMatch = true;
               foreach (var match in result)
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
                     else if (type == typeof(string) || type == typeof(Maybe<string>) || type == typeof(FileName) || type == typeof(FolderName) ||
                        type == typeof(Maybe<FileName>) || type == typeof(Maybe<FolderName>))
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
                     else if (type == typeof(string[]))
                     {
                        evaluator[name] = getStringArray(rest);
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