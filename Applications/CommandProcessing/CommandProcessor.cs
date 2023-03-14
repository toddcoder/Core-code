using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Applications.Writers;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Applications.CommandProcessing;

public abstract class CommandProcessor : IDisposable
{
   protected static IWriter getStandardWriter() => new ConsoleWriter();

   protected static IWriter getExceptionWriter() => new ConsoleWriter
   {
      ForegroundColor = ConsoleColor.Red,
      BackgroundColor = ConsoleColor.White
   };

   protected StringHash configurationDefaults;
   protected StringHash configurationHelp;
   protected string application;
   protected Configuration configuration;

   public event ConsoleCancelEventHandler CancelKeyPress;

   public CommandProcessor(string application)
   {
      application.Must().Not.BeNullOrEmpty().OrThrow();

      this.application = application;
      StandardWriter = getStandardWriter();
      ExceptionWriter = getExceptionWriter();

      Prefix = "--";
      ShortCut = "-";
      Suffix = " ";
      Arguments = string.Empty;

      Console.CancelKeyPress += CancelKeyPress;

      Command = string.Empty;
   }

   public virtual Result<Configuration> InitializeConfiguration()
   {
      FileName configurationFile = $@"~\AppData\Local\{application}\{application}.configuration";

      configurationDefaults = GetConfigurationDefaults();
      configurationHelp = GetConfigurationHelp();

      try
      {
         if (configurationFile.Exists())
         {
            var _configuration = Configuration.Open(configurationFile);
            if (_configuration is (true, var openedConfiguration))
            {
               var _defaultValues = enforceDefaults(openedConfiguration);
               if (_defaultValues is (true, var dirty))
               {
                  foreach (var (key, value) in dirty)
                  {
                     StandardWriter.WriteLine($"Configuration item {key} set to default value of {value}");
                  }
               }
               else if (_defaultValues.AnyException)
               {
                  return _defaultValues.Exception;
               }
            }

            return _configuration;
         }
         else
         {
            configuration = new Setting() + configurationFile;
            configurationFile.Folder.Guarantee();
            ResetConfiguration();

            return configuration;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Optional<StringHash> enforceDefaults(Configuration configuration)
   {
      try
      {
         var defaultValues = new StringHash(true);
         foreach (var (key, defaultValue) in configurationDefaults)
         {
            var _value = configuration.Maybe.String(key);
            if (!_value)
            {
               configuration[key] = defaultValue;
               defaultValues[key] = defaultValue;
            }
         }

         if (defaultValues.Count > 0)
         {
            ConfigurationChanged();
            return configuration.Save().Optional().Map(_ => defaultValues);
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public abstract StringHash GetConfigurationDefaults();

   public abstract StringHash GetConfigurationHelp();

   public virtual void Initialize()
   {
   }

   public virtual void CommandInitialize(CommandAttribute commandAttribute)
   {
   }

   public virtual void CommandCleanUp(CommandAttribute commandAttribute)
   {
   }

   public virtual void CleanUp()
   {
   }

   public IWriter StandardWriter { get; set; }

   public IWriter ExceptionWriter { get; set; }

   public bool Test { get; set; }

   public bool Running { get; set; }

   public string Application => application;

   public int ErrorCode { get; set; }

   public string Prefix { get; set; }

   public string ShortCut { get; set; }

   public string Suffix { get; set; }

   public string Arguments { get; protected set; }

   public string Command { get; protected set; }

   protected static string removeExecutableFromCommandLine(string commandLine)
   {
      return commandLine.Matches("^ (.+? ('.exe' | '.dll') /s* [quote]? /s*) /s* /(.*) $; f")
         .Map(result => result.FirstGroup) | commandLine;
   }

   protected static Maybe<(string command, string rest)> splitCommandFromRest(string commandLine)
   {
      var _help = lazy.maybe<string>();
      var _length = lazy.maybe<int>();
      if (commandLine.IsEmpty())
      {
         return ("help", "");
      }
      else if (_help.ValueOf(commandLine.Matches("^ 'help' (/s+ /(.+))? $; f").Map(r => r.FirstGroup)) is (true, var help))
      {
         return ("help", help);
      }
      else if (commandLine.IsMatch("^ /s* [/w '-']+ /s* $; f"))
      {
         return (commandLine, "");
      }
      else if (_length.ValueOf(commandLine.Matches("^ /('config') /s+ ('get' | 'set') /b; f").Map(r => r.FirstGroup.Length)))
      {
         return ("config", commandLine.Drop(_length).TrimLeft());
      }
      else
      {
         return commandLine.Matches("^ /([/w '-']+) /s+ /(.+) $; f").Map(r => (r.FirstGroup, r.SecondGroup));
      }
   }

   public void Run(string commandLine)
   {
      try
      {
         var _configuration = InitializeConfiguration();
         if (_configuration is (true, var configurationValue))
         {
            configuration = configurationValue;
            ConfigurationLoaded();
            commandLine = removeExecutableFromCommandLine(commandLine);
            Arguments = commandLine;

            Initialize();

            run(commandLine, true);
         }
         else
         {
            HandleException(_configuration.Exception);
         }
      }
      catch (Exception exception)
      {
         HandleException(exception);
      }
      finally
      {
         CleanUp();
      }
   }

   protected void run(string commandLine, bool seekCommandFile)
   {
      var _commandParts = splitCommandFromRest(commandLine);
      if (_commandParts is (true, var (command, rest)))
      {
         Command = command;

         switch (command)
         {
            case "help":
               generateHelp(rest);
               break;
            case "config":
               handleConfiguration(rest);
               break;
            default:
            {
               var _method = getMethod(command);
               if (_method is (true, var (methodInfo, commandAttribute)))
               {
                  if (commandAttribute.Initialize)
                  {
                     CommandInitialize(commandAttribute);
                  }

                  var _result = executeMethod(methodInfo, rest);
                  if (_result)
                  {
                     CommandCleanUp(commandAttribute);
                  }
                  else
                  {
                     HandleException(_result.Exception);
                  }
               }
               else if (seekCommandFile)
               {
                  FileName file = @$"~\AppData\Local\{Application}\{command}.cli";
                  if (file)
                  {
                     var text = file.Text;
                     run(text, false);
                  }
                  else
                  {
                     ExceptionWriter.WriteLine($"Didn't understand command '{command}'");
                  }
               }
               else
               {
                  ExceptionWriter.WriteLine($"Didn't understand command '{command}'");
               }

               break;
            }
         }
      }
      else
      {
         HandleException("Command couldn't be determined");
      }

      if (Test)
      {
         Console.WriteLine();
         Console.Write("Hit any key to exit");
         var _ = Console.ReadKey();
      }
   }

   protected Maybe<(MethodInfo methodInfo, CommandAttribute CommandAttribute)> getMethod(string command)
   {
      return this.MethodsUsing<CommandAttribute>().FirstOrNone(t => t.attribute.Name.Same(command));
   }

   protected void generateHelp(string rest)
   {
      var generator = new HelpGenerator(this);
      var _help = lazy.result<string>();
      string help;
      if (rest.IsEmpty())
      {
         help = generator.Help();
      }
      else if (_help.ValueOf(generator.Help(rest)) is (true, var generatedHelp))
      {
         help = generatedHelp;
      }
      else
      {
         ExceptionWriter.WriteExceptionLine(_help.Exception);
         return;
      }

      StandardWriter.WriteLine(help);
   }

   protected (PropertyInfo propertyInfo, SwitchAttribute attribute)[] getSwitchAttributes()
   {
      return this.PropertiesUsing<SwitchAttribute>().ToArray();
   }

   protected (MethodInfo methodInfo, CommandAttribute attribute)[] getCommandAttributes()
   {
      return this.MethodsUsing<CommandAttribute>().ToArray();
   }

   public StringHash<(Maybe<string> _helpText, Maybe<string> _switchPattern, IHash<string, string> replacements)> GetCommandHelp()
   {
      return getCommandAttributes()
         .Select(a => (a.attribute.Name, a.attribute.HelpText, a.attribute.SwitchPattern, (IHash<string, string>)a.attribute))
         .ToStringHash(t => t.Name, t => (t.HelpText, t.SwitchPattern, t.Item4), true);
   }

   public StringHash<(string type, string argument, Maybe<string> _shortCut)> GetSwitchHelp()
   {
      return getSwitchAttributes()
         .Select(a => (a.attribute.Name, a.attribute.Type, a.attribute.Argument, a.attribute.ShortCut))
         .ToStringHash(t => t.Name, t => (t.Type, t.Argument, t.ShortCut), true);
   }

   public virtual void ConfigurationChanged()
   {
   }

   public virtual void ConfigurationLoaded()
   {
   }

   protected void handleConfiguration(string rest)
   {
      if (rest.IsMatch($"^ '{Prefix}all' | '{ShortCut}a' /b; f"))
      {
         AllConfiguration();
      }
      else if (rest.IsMatch($"^ '{Prefix}reset' | '{ShortCut}r' /b; f"))
      {
         ResetConfiguration();
      }
      else
      {
         var _result = rest.Matches($"^ /('{Prefix}set' | '{Prefix}get' | '{ShortCut}s' | '{ShortCut}g') /s+ /(/w [/w '-']*) /b /(.*) $; f");
         if (_result is (true, var (command, name, value)))
         {
            var _command = lazy.maybe<string>();
            var _command2 = lazy.maybe<string>();
            if (_command.ValueOf(command.Matches($"^ '{Prefix}' /('set' | 'get'); f").Map(r => r.FirstGroup)))
            {
               command = _command;
            }
            else if (_command2.ValueOf(command.Matches($"^ '{ShortCut}' /('s' | 'g'); f").Map(r => r.FirstGroup)))
            {
               command = _command2;
            }

            switch (command)
            {
               case "set" or "s":
                  value = value.TrimLeft().Unquotify();
                  SetConfiguration(name, value);
                  break;
               case "get" or "g":
                  GetConfiguration(name);
                  break;
            }
         }
      }
   }

   public virtual void AllConfiguration()
   {
      var tableMaker = new TableMaker(("Key", Justification.Left), ("Value", Justification.Left), ("Help", Justification.Left))
         { Title = "All Configurations" };
      foreach (var (key, value) in configuration.Items())
      {
         var help = configurationHelp.Maybe[key] | "no help";
         tableMaker.Add(key, value, help);
      }

      StandardWriter.WriteLine(tableMaker);
   }

   public virtual void ResetConfiguration()
   {
      if (configurationDefaults.Count > 0)
      {
         foreach (var (key, value) in configurationDefaults)
         {
            configuration[key] = value;
         }

         var _result = configuration.Save();
         if (_result)
         {
            StandardWriter.WriteLine("Configuration reset");
         }
         else
         {
            ExceptionWriter.WriteExceptionLine(_result.Exception);
         }
      }
   }

   public virtual void SetConfiguration(string key, string value)
   {
      if (configuration.ContainsKey(key) || configurationDefaults.ContainsKey(key))
      {
         configuration[key] = value;
         var _result = configuration.Save();
         if (_result)
         {
            ConfigurationChanged();
            StandardWriter.WriteLine($"Saved {key}");
         }
         else
         {
            ExceptionWriter.WriteExceptionLine(_result.Exception);
         }
      }
      else
      {
         ExceptionWriter.WriteExceptionLine($"Didn't recognize key \"{key}\"");
      }
   }

   public virtual void GetConfiguration(string key)
   {
      var _value = configuration.Maybe.String(key);
      if (_value)
      {
         StandardWriter.WriteLine($"{key} -> {_value}");
      }
      else
      {
         ExceptionWriter.WriteExceptionLine($"Didn't recognize key \"{key}\"");
      }
   }

   protected IEnumerable<(string prefix, string name, Maybe<string> _value)> switchData(string source)
   {
      var delimitedText = DelimitedText.BothQuotes();
      var noStrings = delimitedText.Destringify(source, true);

      while (true)
      {
         var _result = noStrings.Matches($"^ /s* /('{Prefix}' | '{ShortCut}') /(/w [/w '-']*) /b; f");
         if (_result is (true, var result))
         {
         }
         else
         {
            break;
         }

         var (prefix, name) = result;
         noStrings = noStrings.Drop(result.Length);
         if (noStrings.IsEmpty() || noStrings.IsMatch($"^ /s* ('{Prefix}' | '{ShortCut}'); f"))
         {
            yield return (prefix, name, nil);
         }
         else
         {
            var _bareString = lazy.maybe<(string, int)>();
            var _nonSpace = lazy.maybe<(string, int)>();
            if (_bareString.ValueOf(noStrings.Matches("^ /s* /([quote]) /(-[quote]*) /1; f").Map(r => r.SecondGroupAndLength)) is
                (true, var (bareString, bareStringLength)))
            {
               var value = delimitedText.Restringify(bareString, RestringifyQuotes.None);

               yield return (prefix, name, value);

               noStrings = noStrings.Drop(bareStringLength);
            }
            else if (_nonSpace.ValueOf(noStrings.Matches("^ /s* /(-/s+); f").Map(r => r.FirstGroupAndLength)) is
                     (true, var (nonSpace, nonSpaceLength)))
            {
               yield return (prefix, name, nonSpace);

               noStrings = noStrings.Drop(nonSpaceLength);
            }
         }
      }
   }

   protected Result<Unit> executeMethod(MethodInfo methodInfo)
   {
      try
      {
         methodInfo.Invoke(this, Array.Empty<object>());
         return unit;
      }
      catch (Exception exception)
      {
         if (exception.InnerException != null)
         {
            return exception.InnerException;
         }
         else
         {
            return exception;
         }
      }
   }

   protected Result<Unit> executeMethod(MethodInfo methodInfo, string rest)
   {
      if (rest.IsEmpty())
      {
         return executeMethod(methodInfo);
      }

      var switchAttributes = getSwitchAttributes();

      foreach (var (prefix, name, _value) in switchData(rest))
      {
         if (prefix == Prefix)
         {
            var result = fillSwitch(switchAttributes, name, _value);
            if (!result)
            {
               return fail($"Switch {name} not understood");
            }
         }
         else if (prefix == ShortCut)
         {
            var result = fillShortCut(switchAttributes, name, _value);
            if (!result)
            {
               return fail($"Shortcut {name} not understood");
            }
         }
         else
         {
            return fail($"{name} not proceeded by {Prefix} or {ShortCut}");
         }
      }

      return executeMethod(methodInfo);
   }

   protected Maybe<Unit> fillSwitch((PropertyInfo propertyInfo, SwitchAttribute attribute)[] switchAttributes, string name,
      Maybe<string> _value)
   {
      return
         from propertyInfo in switchAttributes.FirstOrNone((_, a) => a.Name == name).Select(t => t.Item1)
         from filled in fillProperty(propertyInfo, _value)
         select filled;
   }

   protected Maybe<Unit> fillShortCut((PropertyInfo propertyInfo, SwitchAttribute attribute)[] switchAttributes, string name,
      Maybe<string> _value)
   {
      return
         from propertyInfo in switchAttributes.FirstOrNone((_, a) => (a.ShortCut | "") == name).Select(t => t.Item1)
         from filled in fillProperty(propertyInfo, _value)
         select filled;
   }

   protected Maybe<Unit> fillProperty(PropertyInfo propertyInfo, Maybe<string> _value)
   {
      var type = propertyInfo.PropertyType;
      Maybe<object> _object = nil;
      if (_value is (true, var value))
      {
         if (type == typeof(bool))
         {
            _object = getBoolean(value);
         }
         else if (type == typeof(string) || type == typeof(Maybe<string>) || type == typeof(FileName) || type == typeof(FolderName) ||
                  type == typeof(Maybe<FileName>) || type == typeof(Maybe<FolderName>))
         {
            _object = getString(value, type);
         }
         else if (type == typeof(int))
         {
            _object = getInt32(value);
         }
         else if (type == typeof(double) || type == typeof(float))
         {
            _object = getFloatingPoint(value, type);
         }
         else if (type == typeof(DateTime))
         {
            _object = getDate(value);
         }
         else if (type.IsEnum)
         {
            _object = getEnum(value, type);
         }
         else if (type == typeof(string[]))
         {
            _object = getStringArray(value);
         }

         if (_object is (true, var @object))
         {
            propertyInfo.SetValue(this, @object);
            return unit;
         }
         else
         {
            return nil;
         }
      }
      else
      {
         propertyInfo.SetValue(this, true);
         return unit;
      }
   }

   protected static Maybe<object> getBoolean(string value)
   {
      if (value.IsMatch("^ /s* $; f"))
      {
         return true.Some<object>();
      }
      else
      {
         var _firstGroup = value.Matches("^ /s* /('true' | 'false' | '+' | '-') /b; fi").Map(r => r.FirstGroup);
         if (_firstGroup is (true, var firstGroup))
         {
            return firstGroup.AnySame("true", "+").Some<object>();
         }
         else
         {
            return true.Some<object>();
         }
      }
   }

   protected static Maybe<object> getInt32(string value) => Maybe.Int32(value).Map(i => (object)i);

   protected static Maybe<object> getFloatingPoint(string value, Type type)
   {
      if (type == typeof(double))
      {
         return Maybe.Double(value).CastAs<object>();
      }
      else if (type == typeof(float))
      {
         return Maybe.Single(value).CastAs<object>();
      }
      else
      {
         return nil;
      }
   }

   protected static Maybe<object> getDate(string value) => Maybe.DateTime(value).CastAs<object>();

   protected static Maybe<object> getString(string value, Type type)
   {
      if (type == typeof(string))
      {
         return value.Some<object>();
      }
      else if (type == typeof(Maybe<string>))
      {
         if (value.IsNotEmpty())
         {
            Maybe<string> _someString = value;
            return ((object)_someString).Some();
         }
         else
         {
            return nil;
         }
      }
      else if (type == typeof(FolderName))
      {
         return value.Some<object>();
      }
      else if (type == typeof(FileName))
      {
         return value.Some<object>();
      }
      else if (type == typeof(Maybe<FolderName>))
      {
         if (value.IsNotEmpty())
         {
            FolderName folder = value;
            Maybe<FolderName> _folder = folder;
            return ((object)_folder).Some();
         }
         else
         {
            return nil;
         }
      }
      else if (type == typeof(Maybe<FileName>))
      {
         if (value.IsNotEmpty())
         {
            FileName file = value;
            Maybe<FileName> _file = file;
            return ((object)_file).Some();
         }
         else
         {
            return nil;
         }
      }
      else
      {
         return nil;
      }
   }

   protected static Maybe<object> getEnum(string value, Type type) => Maybe.Enumeration(type, value);

   protected static Maybe<object> getStringArray(string value)
   {
      var _list = value.Matches("^/s* '[' /s*  /(.*) /s* ']'; f").Map(r => r.FirstGroup);
      if (_list is (true, var list))
      {
         var array = list.Unjoin("/s* ',' /s*; f");
         return array.Some().Map(a => (object)a);
      }
      else
      {
         return nil;
      }
   }

   public virtual void HandleException(Exception exception) => ExceptionWriter.WriteExceptionLine(exception);

   public virtual void HandleException(string message) => ExceptionWriter.WriteExceptionLine(message);

   public void Run() => Run(Environment.CommandLine);

   public void Dispose()
   {
   }
}