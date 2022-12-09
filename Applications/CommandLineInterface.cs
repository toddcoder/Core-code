using System;
using System.Linq;
using System.Reflection;
using Core.Applications.Writers;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Applications;

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
      var _matches = name.Matches("['A-Z']; f");
      if (_matches)
      {
         foreach (var match in ~_matches)
         {
            match.ZerothGroup = $"-{match.ZerothGroup.ToLower()}";
         }

         return $"('{name}' | '{~_matches}')";
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
            return args.Result | (object)source;
         }
         else
         {
            return source;
         }
      }
   }

   protected static object getInt32(string rest) => Value.Int32(rest.Keep("^ /s* -/s+; f").TrimStart());

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
      var _list = rest.Matches("^/s* '[' /s*  /(.*) /s* ']'; f").Map(r => r.FirstGroup);
      if (_list)
      {
         var array = (~_list).Unjoin("/s* ',' /s*; f");
         return array;
      }
      else
      {
         throw fail("String arrays must be delimited by []");
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

         var _rest = commandLine.Matches(itemPrefix).Map(r => r.FirstGroup);
         if (_rest)
         {
            if (type == typeof(bool))
            {
               return getBoolean(_rest, suffix).Success();
            }
            else if (type == typeof(string) || type == typeof(FileName) || type == typeof(FolderName))
            {
               return getString(_rest, type).Success();
            }
            else if (type == typeof(int))
            {
               return getInt32(_rest).Success();
            }
            else if (type == typeof(float) || type == typeof(double))
            {
               return getFloatingPoint(_rest, type).Success();
            }
            else if (type.IsEnum)
            {
               return getEnum(_rest, type).Success();
            }
            else if (type == typeof(string[]))
            {
               return getStringArray(_rest).Success();
            }
            else if (this is IMissingArgument missingArgument)
            {
               return missingArgument.BadType(name, type, _rest);
            }
            else
            {
               return $"Can't handle type {type.Name}".Failure<object>();
            }
         }
         else if (_defaultValue)
         {
            return _defaultValue.Result($"No value for {name}");
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
         .Map(result => result.FirstGroup) | commandLine;
   }

   protected static Hash<char, string> getShortcuts(string source) => source.Unjoin("/s* ';' /s*; f").Select(s =>
   {
      var pair = s.Unjoin("/s* '=' /s*; f");
      return (key: pair[0][0], value: pair[1]);
   }).ToHash(i => i.key, i => i.value);

   protected string fixCommand(string commandLine, string prefix, string suffix)
   {
      var _result = commandLine.Matches($"^ /({REGEX_PARAMETER}) /s*; f");
      if (_result)
      {
         var result = ~_result;
         var command = result.FirstGroup;
         result.FirstGroup = $"{prefix}{command}{suffix}true";

         commandLine = result.ToString();
      }

      if (Shortcuts.IsEmpty())
      {
         return commandLine;
      }

      var _matches = commandLine.Matches($"'{ShortPrefix}' /['a-zA-Z0-9'] '{ShortSuffix}'; f");
      if (_matches)
      {
         var shortcuts = getShortcuts(Shortcuts);
         foreach (var match in ~_matches)
         {
            var key = match.FirstGroup[0];
            var _replacement = shortcuts.Maybe[key];
            if (_replacement)
            {
               match.Text = $"{prefix}{_replacement}{suffix}";
            }
            else
            {
               match.Text = "";
            }
         }

         commandLine = (~_matches).ToString();
      }

      return commandLine;
   }

   protected bool setPossibleAlias(string commandLine)
   {
      var _result = commandLine.Matches("^ /s+ /([/w '-']+) /s+ /(.*) $; f");
      if (_result)
      {
         var (aliasName, command) = ~_result;
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
         var _commandName = commandLine.Matches($"^ /({REGEX_PARAMETER})'{suffix}'; f").Map(r => r.FirstGroup);
         if (_commandName)
         {
            var remainder = commandLine.Drop((~_commandName).Length);
            if (this is ICommandFile commandFile)
            {
               var file = commandFile.CommandFile(_commandName);
               if (file)
               {
                  var text = file.Text;
                  return remainder.IsNotEmpty() ? $"{text} {remainder}".Some() : text.Some();
               }
            }
         }

         return nil;
      }
   }

   public void runUsingParameters(string prefix, string suffix, string commandLine)
   {
      commandLine = removeExecutableFromCommandLine(commandLine);

      var _newCommandLine = getCommandsFromFile(prefix, suffix, commandLine);
      if (_newCommandLine)
      {
         commandLine = _newCommandLine;
      }

      var _command = getCommand(commandLine);
      if (_command)
      {
         if (_command == "alias")
         {
            if (setPossibleAlias(commandLine.Drop((~_command).Length)))
            {
               return;
            }
         }
         else
         {
            var _replacement = aliases.Maybe[_command];
            if (_replacement)
            {
               commandLine = _replacement;
            }
         }
      }

      commandLine = fixCommand(commandLine, prefix, suffix);

      var _entryPoint = getEntryPoint();
      if (_entryPoint)
      {
         var (methodInfo, type) = ~_entryPoint;
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
         HandleException(_entryPoint.Exception);
      }
   }

   protected static void pressAKeyToEnd()
   {
      Console.Write("Press a key to end: ");
      Console.ReadKey();
   }

   protected void useWithParameters(MethodInfo methodInfo, string prefix, string suffix, string commandLine)
   {
      var _arguments = methodInfo.GetParameters()
         .Select(p => (p.Name, p.ParameterType, defaultValue: maybe(p.HasDefaultValue, () => p.DefaultValue)))
         .Select(t => retrieveItem(t.Name, t.ParameterType, t.defaultValue, prefix, suffix, commandLine))
         .ToArray();
      var _failure = _arguments.FirstOrNone(p => !p);
      if (_failure)
      {
         var failure = ~_failure;
         if (!failure)
         {
            HandleException(failure.Exception);
         }
      }
      else
      {
         try
         {
            Running = true;
            methodInfo.Invoke(this, _arguments.Select(a => a.ForceValue()).ToArray());
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
      if (_argument)
      {
         try
         {
            Running = true;
            methodInfo.Invoke(this, new[] { ~_argument });
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
         HandleException(_argument.Exception);
      }

      if (Test)
      {
         pressAKeyToEnd();
      }
   }

   protected void useWithThis(MethodInfo methodInfo, string prefix, string suffix, string commandLine)
   {
      var _filledObject = fillObject(this, prefix, suffix, commandLine);
      if (_filledObject)
      {
         try
         {
            Running = true;
            methodInfo.Invoke(this, Array.Empty<object>());
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
         HandleException(_filledObject.Exception);
      }

      if (Test)
      {
         pressAKeyToEnd();
      }
   }

   protected static string xmlToPascal(string name)
   {
      var _matches = name.Matches("'-' /(/w); f");
      if (_matches)
      {
         var result = ~_matches;
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
         var _matches = commandLine.Matches(pattern);
         if (_matches)
         {
            var isFirstMatch = true;
            foreach (var match in ~_matches)
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
         return exception;
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