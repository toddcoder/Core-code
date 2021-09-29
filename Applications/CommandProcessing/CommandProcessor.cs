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
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using Group = Core.Configurations.Group;

namespace Core.Applications.CommandProcessing
{
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
               return Configuration.Open(configurationFile);
            }
            else
            {
               configuration = new Group() + configurationFile;
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

      public abstract StringHash GetConfigurationDefaults();

      public abstract StringHash GetConfigurationHelp();

      public virtual void Initialize()
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
            .Map(result => result.FirstGroup)
            .DefaultTo(() => commandLine);
      }

      protected static Maybe<(string command, string rest)> splitCommandFromRest(string commandLine)
      {
         if (commandLine.IsMatch("^ 'help' $; f"))
         {
            return ("help", "");
         }
         else if (commandLine.IsMatch("^ /s* [/w '-']+ /s* $; f"))
         {
            return (commandLine, "");
         }
         else if (commandLine.Matches("^ /('config') /s+ ('get' | 'set') /b; f").If(out var result))
         {
            return ("config", commandLine.Drop(result.FirstGroup.Length).TrimLeft());
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
            if (InitializeConfiguration().IfNot(out configuration, out var exception))
            {
               HandleException(exception);
            }

            commandLine = removeExecutableFromCommandLine(commandLine);
            Arguments = commandLine;
            run(commandLine, true);
         }
         catch (Exception exception)
         {
            HandleException(exception);
         }
      }

      protected void run(string commandLine, bool seekCommandFile)
      {
         if (splitCommandFromRest(commandLine).If(out var command, out var rest))
         {
            Command = command;

            switch (command)
            {
               case "help":
                  generateHelp();
                  break;
               case "config":
                  handleConfiguration(rest);
                  break;
               default:
               {
                  if (rest.IsEmpty())
                  {
                     if (seekCommandFile)
                     {
                        FileName file = @$"~\AppData\Local\{Application}\{command}.cli";
                        if (!file.Exists())
                        {
                           ExceptionWriter.WriteLine($"Command file {file} doesn't exist");
                        }

                        var text = file.Text;
                        run(text, false);
                     }
                     else
                     {
                        ExceptionWriter.WriteLine($"No switches provided for {command}");
                     }
                  }
                  else if (this.MethodsUsing<CommandAttribute>().FirstOrNone(t => command == t.attribute.Name)
                     .If(out var methodInfo, out var commandAttribute))
                  {
                     if (commandAttribute.Initialize)
                     {
                        Initialize();
                     }

                     var result = executeMethod(methodInfo, rest);
                     if (result.If(out _, out var exception))
                     {
                        CleanUp();
                     }
                     else
                     {
                        HandleException(exception);
                     }
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

      protected void generateHelp()
      {
         var commandHelps = getCommandHelps().ToStringHash(t => t.methodName, t => (t.command, t.attribute), true);
         var switches = getSwitchAttributes()
            .Select(t => (switchName: t.attribute.Name, propertyName: t.propertyInfo.Name))
            .ToStringHash(t => t.switchName, t => t.propertyName, false);
         var switchHelps = getSwitchHelpAttributes()
            .Select(t => (t.propertyInfo.Name, t.attribute))
            .ToStringHash(t => t.Name, t => t.attribute.HelpText, true);
         var shortCuts = getShortCutAttributes()
            .Select(t => (t.propertyInfo.Name, t.attribute))
            .ToStringHash(t => t.Name, t => t.attribute.Name, true);

         Console.WriteLine("Help");
         foreach (var (methodInfo, commandHelpAttribute) in getCommandHelpAttributes())
         {
            if (commandHelps.If(methodInfo.Name, out var tuple))
            {
               Console.Write($"   {tuple.command}: {commandHelpAttribute.HelpText}");
               foreach (var @switch in commandHelpAttribute.Switches)
               {
                  Console.Write(" ");
                  var optional = @switch.EndsWith("?");
                  var name = optional ? @switch.Drop(-1) : @switch;
                  if (switches.If(name, out var propertyName))
                  {
                     if (optional)
                     {
                        Console.Write("[");
                     }

                     var _shortCut = shortCuts.Map(propertyName);

                     if (_shortCut.IsSome)
                     {
                        Console.Write("(");
                     }

                     Console.Write(Prefix);
                     Console.Write(name);

                     if (_shortCut.If(out var shortCut))
                     {
                        Console.Write($" | {ShortCut}{shortCut})");
                     }

                     Console.Write(Suffix);
                     if (switchHelps.If(propertyName, out var switchHelp))
                     {
                        Console.Write($"<{switchHelp}>");
                     }

                     if (optional)
                     {
                        Console.Write("]");
                     }
                  }
               }

               if (configurationHelp.Count > 0)
               {
                  Console.WriteLine();

                  Console.WriteLine("config");
                  var maxLength = configurationHelp.Keys.Max(k => k.Length);
                  foreach (var (key, value) in configurationHelp)
                  {
                     Console.WriteLine($"   {key.PadRight(maxLength)} - {value}");
                  }
               }

               Console.WriteLine();
            }
         }
      }

      protected (PropertyInfo propertyInfo, SwitchAttribute attribute)[] getSwitchAttributes()
      {
         return this.PropertiesUsing<SwitchAttribute>().ToArray();
      }

      protected (PropertyInfo propertyInfo, ShortCutAttribute attribute)[] getShortCutAttributes()
      {
         return this.PropertiesUsing<ShortCutAttribute>().ToArray();
      }

      protected (MethodInfo methodInfo, CommandHelpAttribute attribute)[] getCommandHelpAttributes()
      {
         return this.MethodsUsing<CommandHelpAttribute>().ToArray();
      }

      protected (PropertyInfo propertyInfo, SwitchHelpAttribute attribute)[] getSwitchHelpAttributes()
      {
         return this.PropertiesUsing<SwitchHelpAttribute>().ToArray();
      }

      protected IEnumerable<(string methodName, string command, CommandHelpAttribute attribute)> getCommandHelps()
      {
         var commandHelpAttributes = getCommandHelpAttributes()
            .Select(t => (t.methodInfo.Name, t.attribute))
            .ToStringHash(t => t.Name, t => t.attribute, true);
         foreach (var (methodInfo, commandAttribute) in this.MethodsUsing<CommandAttribute>())
         {
            if (commandHelpAttributes.If(methodInfo.Name, out var attribute))
            {
               yield return (methodInfo.Name, commandAttribute.Name, attribute);
            }
         }
      }

      protected void handleConfiguration(string rest)
      {
         if (rest.IsMatch("^ 'all' /b"))
         {
            AllConfiguration();
         }
         else if (rest.IsMatch("^ 'reset' /b"))
         {
            ResetConfiguration();
         }
         else if (rest.Matches("^ /('set' | 'get') /s+ /(/w [/w '-']*) /b /(.*) $; f").If(out var result))
         {
            var (command, name, value) = result;
            switch (command)
            {
               case "set":
                  value = value.TrimLeft().Unquotify();
                  SetConfiguration(name, value);
                  break;
               case "get":
                  GetConfiguration(name);
                  break;
            }
         }
      }

      public virtual void AllConfiguration()
      {
         var tableMaker = new TableMaker(("Key", Justification.Left), ("Value", Justification.Left));
         foreach (var (key, value) in configuration.Values())
         {
            tableMaker.Add(key, value);
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

            configuration.Save()
               .OnSuccess(_ => StandardWriter.WriteLine("Configuration reset"))
               .OnFailure(e => ExceptionWriter.WriteExceptionLine(e));
         }
      }

      public virtual void SetConfiguration(string key, string value)
      {
         if (configuration.ContainsKey(key))
         {
            configuration[key] = value;
         }
         else
         {
            ExceptionWriter.WriteExceptionLine($"Didn't recognize key \"{key}\"");
         }
      }

      public virtual void GetConfiguration(string key)
      {
         if (configuration.ContainsKey(key))
         {
            StandardWriter.WriteLine($"{key} -> {configuration[key]}");
         }
         else
         {
            ExceptionWriter.WriteExceptionLine($"Didn't recognize key \"{key}\"");
         }
      }

      public virtual StringHash ConfigurationHelp() => configurationHelp;

      protected IEnumerable<(string prefix, string name, Maybe<string> _value)> switchData(string source)
      {
         var delimitedText = DelimitedText.BothQuotes();
         var noStrings = delimitedText.Destringify(source, true);

         while (noStrings.Matches($"^ /s* /('{Prefix}' | '{ShortCut}') /(/w [/w '-']*) /b; f").If(out var result))
         {
            var (prefix, name) = result;
            noStrings = noStrings.Drop(result.Length);
            if (noStrings.IsEmpty() || noStrings.IsMatch($"^ /s* ('{Prefix}' | '{ShortCut}'); f"))
            {
               yield return (prefix, name, none<string>());
            }
            else if (noStrings.Matches("^ /s* /([quote]) /(-[quote]*) /1; f").If(out result))
            {
               var value = delimitedText.Restringify(result.SecondGroup, RestringifyQuotes.None);

               yield return (prefix, name, value);

               noStrings = noStrings.Drop(result.Length);
            }
            else if (noStrings.Matches("^ /s* /(-/s+); f").If(out result))
            {
               yield return (prefix, name, result.FirstGroup);

               noStrings = noStrings.Drop(result.Length);
            }
         }
      }

      protected Result<Unit> executeMethod(MethodInfo methodInfo, string rest)
      {
         var switchAttributes = getSwitchAttributes();
         var shortCutAttributes = getShortCutAttributes();

         foreach (var (prefix, name, _value) in switchData(rest))
         {
            if (prefix == Prefix)
            {
               var result = fill(switchAttributes, name, _value);
               if (result.IsNone)
               {
                  return fail($"Switch {name} not successful");
               }
            }
            else if (prefix == ShortCut)
            {
               var result = fill(shortCutAttributes, name, _value);
               if (result.IsNone)
               {
                  return fail($"Shortcut {name} not successful");
               }
            }
            else
            {
               return fail($"{name} not proceeded by {Prefix} or {ShortCut}");
            }
         }

         return tryTo(() => { methodInfo.Invoke(this, Array.Empty<object>()); });
      }

      protected Maybe<Unit> fill((PropertyInfo propertyInfo, SwitchAttribute attribute)[] switchAttributes, string name, Maybe<string> _value)
      {
         return
            from propertyInfo in switchAttributes.FirstOrNone((_, a) => a.Name == name).Select(t => t.Item1)
            from filled in fillProperty(propertyInfo, _value)
            select filled;
      }

      protected Maybe<Unit> fill((PropertyInfo propertyInfo, ShortCutAttribute attribute)[] shortCutAttributes, string name, Maybe<string> _value)
      {
         return
            from propertyInfo in shortCutAttributes.FirstOrNone((_, a) => a.Name == name).Select(t => t.Item1)
            from filled in fillProperty(propertyInfo, _value)
            select filled;
      }

      protected Maybe<Unit> fillProperty(PropertyInfo propertyInfo, Maybe<string> _value)
      {
         var type = propertyInfo.PropertyType;
         var _object = none<object>();
         if (_value.If(out var value))
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

            if (_object.If(out var objValue))
            {
               propertyInfo.SetValue(this, objValue);
               return unit;
            }
            else
            {
               return none<Unit>();
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
         else if (value.Matches("^ /s* /('true' | 'false' | '+' | '-') /b; fi").If(out var result))
         {
            return result.FirstGroup.AnySame("true", "+").Some<object>();
         }
         else
         {
            return true.Some<object>();
         }
      }

      protected static Maybe<object> getInt32(string value) => value.AsInt().Map(i => (object)i);

      protected static Maybe<object> getFloatingPoint(string value, Type type)
      {
         if (type == typeof(double))
         {
            return value.AsDouble().Map(d => (object)d);
         }
         else if (type == typeof(float))
         {
            return value.AsFloat().Map(f => (object)f);
         }
         else
         {
            return none<object>();
         }
      }

      protected static Maybe<object> getDate(string value) => value.AsDateTime().Map(d => (object)d);

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
               var _someString = value.Some();
               return ((object)_someString).Some();
            }
            else
            {
               return none<object>();
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
               var _folder = folder.Some();
               return ((object)_folder).Some();
            }
            else
            {
               return none<object>();
            }
         }
         else if (type == typeof(Maybe<FileName>))
         {
            if (value.IsNotEmpty())
            {
               FileName file = value;
               var _file = file.Some();
               return ((object)_file).Some();
            }
            else
            {
               return none<object>();
            }
         }
         else
         {
            return none<object>();
         }
      }

      protected static Maybe<object> getEnum(string value, Type type) => value.AsEnumeration(type);

      protected static Maybe<object> getStringArray(string value)
      {
         if (value.Matches("^/s* '[' /s*  /(.*) /s* ']'; f").If(out var result))
         {
            var list = result.FirstGroup;
            var array = list.Split("/s* ',' /s*; f");
            return array.Some().Map(a => (object)a);
         }
         else
         {
            return none<object>();
         }
      }

      public virtual void HandleException(Exception exception) => ExceptionWriter.WriteExceptionLine(exception);

      public virtual void HandleException(string message) => ExceptionWriter.WriteExceptionLine(message);

      public void Run() => Run(Environment.CommandLine);

      public void Dispose()
      {
      }
   }
}