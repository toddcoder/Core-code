using System;
using System.Linq;
using System.Reflection;
using Core.Applications.Writers;
using Core.Assertions;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications
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

      public CommandProcessor(string application, IWriter standardWriter, IWriter exceptionWriter)
      {
         application.Must().Not.BeNullOrEmpty().OrThrow();
         standardWriter.Must().Not.BeNull().OrThrow();
         exceptionWriter.Must().Not.BeNull().OrThrow();

         this.application = application;
         StandardWriter = standardWriter;
         ExceptionWriter = exceptionWriter;

         Prefix = "--";
         ShortCut = "-";
         Suffix = " ";

         Console.CancelKeyPress += CancelKeyPress;
      }

      public CommandProcessor(string application, IWriter standardWriter) : this(application, standardWriter, getExceptionWriter())
      {
      }

      public CommandProcessor(string application) : this(application, getStandardWriter())
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

      protected static string removeExecutableFromCommandLine(string commandLine)
      {
         return commandLine.Matches("^ (.+? ('.exe' | '.dll') /s* [quote]? /s*) /s* /(.*) $; f")
            .Map(result => result.FirstGroup)
            .DefaultTo(() => commandLine);
      }

      protected static Maybe<(string command, string rest)> splitCommandFromRest(string commandLine)
      {
         return commandLine.Matches("^ /([/w '-']+) /s+ /(.+) $").Map(result => (result.FirstGroup, result.SecondGroup));
      }

      public void Run(string commandLine)
      {
         try
         {
            commandLine = removeExecutableFromCommandLine(commandLine);
            if (splitCommandFromRest(commandLine).If(out var command, out var rest))
            {
               if (this.MethodsUsing<CommandAttribute>().FirstOrNone(t => command == t.attribute.Name).If(out var methodInfo, out var attribute))
               {
                  executeMethod(methodInfo, attribute, rest);
               }
            }
            else
            {
               HandleException("Command couldn't be determined");
            }
         }
         catch (Exception exception)
         {
            HandleException(exception);
         }
      }

      protected (PropertyInfo propertyInfo, SwitchAttribute attribute)[] getSwitchAttributes()
      {
         return this.PropertiesUsing<SwitchAttribute>().ToArray();
      }

      protected void executeMethod(MethodInfo methodInfo, CommandAttribute attribute, string rest)
      {
         var switchAttributes = getSwitchAttributes();
         while (rest.IsNotEmpty() && rest.Matches($"^ /s* /('{Prefix}' | '{ShortCut}') /([/w '-']+) '{Suffix}' /(.*) $").If(out var result))
         {
            var (prefix, name, value) = result;
            name = name.ToPascal();
            if (fillFromPrefix(switchAttributes, prefix, name, value).If(out var remainder))
            {
               rest = remainder;
            }
            else if (fillFromShortCut(switchAttributes, prefix, name, value).If(out remainder))
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
               from tuple in switchAttributes.FirstOrNone(t => t.propertyInfo.Name == name)
               from remainder in fillProperty(tuple.propertyInfo, value)
               select remainder;
         }
         else
         {
            return none<string>();
         }
      }

      protected Maybe<string> fillFromShortCut((PropertyInfo propertyInfo, SwitchAttribute attribute)[] switchAttributes, string prefix, string name,
         string value)
      {
         if (prefix == ShortCut)
         {
            return
               from tuple in switchAttributes.FirstOrNone(t => t.propertyInfo.Name == name)
               from remainder in fillProperty(tuple.propertyInfo, value)
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
         else if (type == typeof(string) || type == typeof(FileName) || type == typeof(FolderName) || type == typeof(Maybe<FileName>) ||
            type == typeof(Maybe<FolderName>))
         {
            var (obj, remainder) = getString(value, type);
            _object = obj.Some();
            stringToReturn = remainder;
         }
         else if (type == typeof(int) && getInt32(value).If(out var iValue, out var remainder))
         {
            _object = iValue.Some<object>();
            stringToReturn = remainder;
         }
         else if ((type == typeof(double) || type == typeof(float)) && getFloatingPoint(value, type).If(out var oValue, out remainder))
         {
            _object = oValue.Some();
            stringToReturn = remainder;
         }
         else if (type.IsEnum && getEnum(value, type).If(out var eValue, out remainder))
         {
            _object = eValue.Some();
            stringToReturn = remainder;
         }
         else if (type == typeof(string[]) && getStringArray(value).If(out var array, out remainder))
         {
            _object = array.Some<object>();
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

      protected static (object value, string remainder) getBoolean(string rest)
      {
         if (rest.IsMatch("^ /s* $; f"))
         {
            return (true, string.Empty);
         }
         else if (rest.Matches("^ /s* /('true' | 'false' | '+' | '-') /b; fi").If(out var result))
         {
            return (result.FirstGroup.AnySame("true", "+"), rest.Drop(result.Length));
         }
         else
         {
            return (true, rest);
         }
      }

      protected static Maybe<(int value, string remainder)> getInt32(string rest)
      {
         if (rest.Matches("^ /s* /('-'? [/d '_']+); f").If(out var result) && result.FirstGroup.Replace("_", "").AsInt().If(out var value))
         {
            var remainder = rest.Drop(result.Length);
            return (value, remainder).Some();
         }
         else
         {
            return none<(int, string)>();
         }
      }

      protected static Maybe<(object value, string remainder)> getFloatingPoint(string rest, Type type)
      {
         if (rest.Matches("^ /s* /('-'? [/d '_']+ '.' [/d '_']+); f").If(out var result))
         {
            var source = result.FirstGroup.Replace("_", "");
            var remainder = rest.Drop(result.Length);
            if (type == typeof(double) && source.AsDouble().If(out var dValue))
            {
               return (dValue, remainder).Some<(object, string)>();
            }
            else if (type == typeof(float) && source.AsFloat().If(out var fValue))
            {
               return (fValue, remainder).Some<(object, string)>();
            }
         }

         return none<(object, string)>();
      }

      protected static (object value, string remainder) getString(string rest, Type type)
      {
         var source = rest.IsMatch("^ /s* `quote; f") ? rest.Keep("^ /s* /(`quote) .*? /1; f").TrimStart().Drop(1).Drop(-1)
            : rest.Keep("^ /s* -/s+; f").TrimStart();
         var remainder = rest.Drop(source.Length);
         if (type == typeof(string))
         {
            return (source, remainder);
         }
         else if (type == typeof(Maybe<string>))
         {
            return (maybe(source.IsNotEmpty(), () => source), remainder);
         }
         else if (type == typeof(FolderName))
         {
            return ((FolderName)source, remainder);
         }
         else if (type == typeof(FileName))
         {
            return ((FileName)source, remainder);
         }
         else if (type == typeof(Maybe<FolderName>))
         {
            return (maybe(source.IsNotEmpty(), () => (FolderName)source), remainder);
         }
         else if (type == typeof(Maybe<FileName>))
         {
            return (maybe(source.IsNotEmpty(), () => (FileName)source), remainder);
         }
         else
         {
            return (rest, string.Empty);
         }
      }

      protected static Maybe<(object value, string remainder)> getEnum(string rest, Type type)
      {
         if (rest.Matches("^ /s* /(-/s+); f").If(out var result))
         {
            var source = result.FirstGroup;
            var remainder = rest.Drop(result.Length);

            return source.AsEnumeration(type).Map(e => (e, remainder));
         }
         else
         {
            return none<(object, string)>();
         }
      }

      protected static Maybe<(string[] array, string remainder)> getStringArray(string rest)
      {
         if (rest.Matches("^/s* '[' /s*  /(.*) /s* ']'; f").If(out var result))
         {
            var list = result.FirstGroup;
            var array = list.Split("/s* ',' /s*; f");
            var remainder = rest.Drop(result.Length);
            return (array, remainder).Some();
         }
         else
         {
            return none<(string[], string)>();
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