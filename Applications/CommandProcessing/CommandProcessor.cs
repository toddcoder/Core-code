using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Applications.Writers;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;

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

      protected string application;

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

         Console.CancelKeyPress += CancelKeyPress;
      }

      public abstract void Initialize();

      public IWriter StandardWriter { get; set; }

      public IWriter ExceptionWriter { get; set; }

      public bool Test { get; set; }

      public bool Running { get; set; }

      public string Application => application;

      public int ErrorCode { get; set; }

      public string Prefix { get; set; }

      public string ShortCut { get; set; }

      public string Suffix { get; set; }

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
            return ("help", "").Some();
         }
         else
         {
            return commandLine.Matches("^ /([/w '-']+) /s+ /(.+) $; f").Map(result => (result.FirstGroup, result.SecondGroup));
         }
      }

      public void Run(string commandLine)
      {
         try
         {
            Initialize();
            commandLine = removeExecutableFromCommandLine(commandLine);
            run(commandLine, true);
         }
         catch (Exception exception)
         {
            HandleException(exception);
         }
      }

      private void run(string commandLine, bool seekCommandFile)
      {
         if (splitCommandFromRest(commandLine).If(out var command, out var rest))
         {
            if (command == "help")
            {
               generateHelp();
            }
            else if (rest.IsEmpty())
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
            else if (this.MethodsUsing<CommandAttribute>().FirstOrNone(t => command == t.attribute.Name).If(out var methodInfo, out _))
            {
               executeMethod(methodInfo, rest);
            }
         }
         else
         {
            HandleException("Command couldn't be determined");
         }
      }

      protected void generateHelp()
      {
         var commandHelps = getCommandHelps().ToStringHash(t => t.methodName, t => (t.command, t.attribute), true);
         var switches = getSwitchAttributes()
            .Select(t => (switchName: t.attribute.Name, propertyName: t.propertyInfo.Name))
            .ToStringHash(t => t.switchName, t => t.propertyName, true);
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

      protected void executeMethod(MethodInfo methodInfo, string rest)
      {
         var switchAttributes = getSwitchAttributes();
         var shortCutAttributes = getShortCutAttributes();
         while (rest.IsNotEmpty() && rest.Matches($"^ /s* /('{Prefix}' | '{ShortCut}') /([/w '-']+) '{Suffix}' /(.*) $").If(out var result))
         {
            var (prefix, name, value) = result;
            if (fillFromPrefix(switchAttributes, prefix, name, value).If(out var remainder))
            {
               rest = remainder;
            }
            else if (fillFromShortCut(shortCutAttributes, prefix, name, value).If(out remainder))
            {
               rest = remainder;
            }
            else
            {
               break;
            }
         }

         methodInfo.Invoke(this, new object[0]);
      }

      protected Maybe<string> fillFromPrefix((PropertyInfo propertyInfo, SwitchAttribute attribute)[] switchAttributes, string prefix, string name,
         string value)
      {
         if (prefix == Prefix)
         {
            return
               from tuple in switchAttributes.FirstOrNone((_, a) => a.Name == name)
               from remainder in fillProperty(tuple.Item1, value)
               select remainder;
         }
         else
         {
            return none<string>();
         }
      }

      protected Maybe<string> fillFromShortCut((PropertyInfo propertyInfo, ShortCutAttribute attribute)[] switchAttributes, string prefix,
         string name,
         string value)
      {
         if (prefix == ShortCut)
         {
            return
               from tuple in switchAttributes.FirstOrNone((_, a) => a.Name == name)
               from remainder in fillProperty(tuple.Item1, value)
               select remainder;
         }
         else
         {
            return none<string>();
         }
      }

      protected Maybe<string> fillProperty(PropertyInfo propertyInfo, string value)
      {
         var type = propertyInfo.PropertyType;
         var _object = none<object>();
         var stringToReturn = string.Empty;
         if (type == typeof(bool))
         {
            var (obj, remainder) = getBoolean(value);
            _object = obj.Some();
            stringToReturn = remainder;
         }
         else if ((type == typeof(string) || type == typeof(FileName) || type == typeof(FolderName) || type == typeof(Maybe<FileName>) ||
            type == typeof(Maybe<FolderName>)) && getString(value, type).If(out var valueRemainder))
         {
            var (obj, remainder) = valueRemainder;
            _object = obj.Some();
            stringToReturn = remainder;
         }
         else if (type == typeof(int) && getInt32(value).If(out valueRemainder))
         {
            var (obj, remainder) = valueRemainder;
            _object = obj.Some();
            stringToReturn = remainder;
         }
         else if ((type == typeof(double) || type == typeof(float)) && getFloatingPoint(value, type).If(out valueRemainder))
         {
            var (obj, remainder) = valueRemainder;
            _object = obj.Some();
            stringToReturn = remainder;
         }
         else if (type.IsEnum && getEnum(value, type).If(out valueRemainder))
         {
            var (obj, remainder) = valueRemainder;
            _object = obj.Some();
            stringToReturn = remainder;
         }
         else if (type == typeof(string[]) && getStringArray(value).If(out valueRemainder))
         {
            var (obj, remainder) = valueRemainder;
            _object = obj.Some();
            stringToReturn = remainder;
         }

         if (_object.If(out var objValue))
         {
            propertyInfo.SetValue(this, objValue);
            return stringToReturn.Some();
         }
         else
         {
            return none<string>();
         }
      }

      protected static ValueRemainder getBoolean(string rest)
      {
         if (rest.IsMatch("^ /s* $; f"))
         {
            return new ValueRemainder(true, string.Empty);
         }
         else if (rest.Matches("^ /s* /('true' | 'false' | '+' | '-') /b; fi").If(out var result))
         {
            return new ValueRemainder(result.FirstGroup.AnySame("true", "+"), rest.Drop(result.Length));
         }
         else
         {
            return new ValueRemainder(true, rest);
         }
      }

      protected static Maybe<ValueRemainder> getInt32(string rest)
      {
         if (rest.Matches("^ /s* /('-'? [/d '_']+); f").If(out var result) && result.FirstGroup.Replace("_", "").AsInt().If(out var value))
         {
            var remainder = rest.Drop(result.Length);
            return new ValueRemainder(value, remainder).Some();
         }
         else
         {
            return none<ValueRemainder>();
         }
      }

      protected static Maybe<ValueRemainder> getFloatingPoint(string rest, Type type)
      {
         if (rest.Matches("^ /s* /('-'? [/d '_']+ '.' [/d '_']+); f").If(out var result))
         {
            var source = result.FirstGroup.Replace("_", "");
            var remainder = rest.Drop(result.Length);
            if (type == typeof(double) && source.AsDouble().If(out var dValue))
            {
               return new ValueRemainder(dValue, remainder).Some();
            }
            else if (type == typeof(float) && source.AsFloat().If(out var fValue))
            {
               return new ValueRemainder(fValue, remainder).Some();
            }
         }

         return none<ValueRemainder>();
      }

      protected static Maybe<ValueRemainder> getString(string rest, Type type)
      {
         string source;
         string remainder;
         if (rest.Matches("^ /s* /(`quote) /(.*?) /1; f").If(out var result))
         {
            source = result.SecondGroup;
            remainder = rest.Drop(result.Length);
         }
         else if (rest.Matches("^ /s* /(-/s+); f").If(out result))
         {
            source = result.FirstGroup;
            remainder = rest.Drop(result.Length);
         }
         else
         {
            source = rest;
            remainder = string.Empty;
         }

         if (type == typeof(string))
         {
            return new ValueRemainder(source, remainder).Some();
         }
         else if (type == typeof(Maybe<string>))
         {
            return new ValueRemainder(maybe(source.IsNotEmpty(), () => source), remainder).Some();
         }
         else if (type == typeof(FolderName))
         {
            return new ValueRemainder((FolderName)source, remainder).Some();
         }
         else if (type == typeof(FileName))
         {
            return new ValueRemainder((FileName)source, remainder).Some();
         }
         else if (type == typeof(Maybe<FolderName>))
         {
            return new ValueRemainder(maybe(source.IsNotEmpty(), () => (FolderName)source), remainder).Some();
         }
         else if (type == typeof(Maybe<FileName>))
         {
            return new ValueRemainder(maybe(source.IsNotEmpty(), () => (FileName)source), remainder).Some();
         }
         else
         {
            return none<ValueRemainder>();
         }
      }

      protected static Maybe<ValueRemainder> getEnum(string rest, Type type)
      {
         if (rest.Matches("^ /s* /(-/s+); f").If(out var result))
         {
            var source = result.FirstGroup;
            var remainder = rest.Drop(result.Length);

            return source.AsEnumeration(type).Map(e => new ValueRemainder(e, remainder));
         }
         else
         {
            return none<ValueRemainder>();
         }
      }

      protected static Maybe<ValueRemainder> getStringArray(string rest)
      {
         if (rest.Matches("^/s* '[' /s*  /(.*) /s* ']'; f").If(out var result))
         {
            var list = result.FirstGroup;
            var array = list.Split("/s* ',' /s*; f");
            var remainder = rest.Drop(result.Length);
            return new ValueRemainder(array, remainder).Some();
         }
         else
         {
            return none<ValueRemainder>();
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