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
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;
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
         if (commandLine.IsEmpty())
         {
            return ("help", "");
         }
         else if (commandLine.Matches("^ 'help' (/s+ /(.+))? $; f").If(out var result))
         {
            return ("help", result.FirstGroup);
         }
         else if (commandLine.IsMatch("^ /s* [/w '-']+ /s* $; f"))
         {
            return (commandLine, "");
         }
         else if (commandLine.Matches("^ /('config') /s+ ('get' | 'set') /b; f").If(out result))
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
                  generateHelp(rest);
                  break;
               case "config":
                  handleConfiguration(rest);
                  break;
               default:
               {
                  if (rest.IsEmpty())
                  {
                     if (getMethod(command).If(out var methodInfo, out var commandAttribute))
                     {
                        if (commandAttribute.Initialize)
                        {
                           Initialize();
                        }

                        var result = executeMethod(methodInfo);
                        if (result.If(out _, out var exception))
                        {
                           CleanUp();
                        }
                        else
                        {
                           HandleException(exception);
                        }
                     }
                     else if (seekCommandFile)
                     {
                        FileName file = @$"~\AppData\Local\{Application}\{command}.cli";
                        if (!file.Exists())
                        {
                           ExceptionWriter.WriteLine($"Didn't understand command {command}");
                        }

                        var text = file.Text;
                        run(text, false);
                     }
                     else
                     {
                        ExceptionWriter.WriteLine($"No switches provided for {command}");
                     }
                  }
                  else if (getMethod(command).If(out var methodInfo, out var commandAttribute))
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

      protected Maybe<(MethodInfo methodInfo, CommandAttribute CommandAttribute)> getMethod(string command)
      {
         return this.MethodsUsing<CommandAttribute>().FirstOrNone(t => t.attribute.Name.Same(command));
      }

      protected void generateHelp(string rest)
      {
         if (rest.Same("config"))
         {
            displayConfiguration();
         }
         else
         {
            var generator = new HelpGenerator(this);
            string help;
            if (rest.IsEmpty())
            {
               help = generator.Help();
            }
            else if (generator.Help(rest).IfNot(out help, out var exception))
            {
               ExceptionWriter.WriteExceptionLine(exception);
            }

            StandardWriter.WriteLine(help);
         }
      }

      protected void displayConfiguration()
      {
         var table = new TableMaker(("Key", Justification.Left), ("Value", Justification.Left)) { Title = "Configuration" };
         foreach (var (key, value) in configurationHelp)
         {
            table.Add(key, value);
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

      protected (MethodInfo methodInfo, CommandAttribute attribute)[] getCommandAttributes()
      {
         return this.MethodsUsing<CommandAttribute>().ToArray();
      }

      /*protected (MethodInfo methodInfo, CommandHelpAttribute attribute)[] getCommandHelpAttributes()
      {
         return this.MethodsUsing<CommandHelpAttribute>().ToArray();
      }

      protected (PropertyInfo propertyInfo, SwitchHelpAttribute attribute)[] getSwitchHelpAttributes()
      {
         return this.PropertiesUsing<SwitchHelpAttribute>().ToArray();
      }*/

      public StringHash<(Maybe<string> _helpText, Maybe<string> _switchPattern)> GetCommandHelp()
      {
         return getCommandAttributes()
            .Select(a => (a.attribute.Name, a.attribute.HelpText, a.attribute.SwitchPattern))
            .ToStringHash(t => t.Name, t => (t.HelpText, t.SwitchPattern), true);
      }

      public StringHash<(string type, string argument, Maybe<string> _shortCut)> GetSwitchHelp()
      {
         return getSwitchAttributes()
            .Select(a => (a.attribute.Name, a.attribute.Type, a.attribute.Argument, a.attribute.ShortCut))
            .ToStringHash(t => t.Name, t => (t.Type, t.Argument, t.ShortCut), true);
      }

      protected void handleConfiguration(string rest)
      {
         if (rest.IsMatch("^ 'all' /b; f"))
         {
            AllConfiguration();
         }
         else if (rest.IsMatch("^ 'reset' /b; f"))
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
         var tableMaker = new TableMaker(("Key", Justification.Left), ("Value", Justification.Left)) { Title = "All Configurations" };
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
               .OnFailure(ExceptionWriter.WriteExceptionLine);
         }
      }

      public virtual void SetConfiguration(string key, string value)
      {
         if (configuration.ContainsKey(key))
         {
            configuration[key] = value;
            configuration.Save().OnSuccess(_ => StandardWriter.WriteLine($"Saved {key}")).OnFailure(ExceptionWriter.WriteExceptionLine);
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

      protected Result<Unit> executeMethod(MethodInfo methodInfo)
      {
         try
         {
            methodInfo.Invoke(this, Array.Empty<object>());
            return unit;
         }
         catch (Exception exception)
         {
            return exception;
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

         return executeMethod(methodInfo);
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

      protected static Maybe<object> getEnum(string value, Type type) => Maybe.Enumeration(type, value);

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