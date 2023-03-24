using System;
using System.IO;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing;

public class HelpGenerator
{
   protected const string CONFIG_HELP = "View all configuration items, get or set configuration items";

   protected StringHash<(Optional<string> _helpText, Optional<string> _switchPattern, IHash<string, string> replacements)> commandHelp;
   protected StringHash<(string type, string argument, Optional<string> _shortCut)> switchHelp;
   protected string prefix;
   protected string shortCut;

   public HelpGenerator(CommandProcessor commandProcessor)
   {
      commandHelp = commandProcessor.GetCommandHelp();
      switchHelp = commandProcessor.GetSwitchHelp();
      prefix = commandProcessor.Prefix;
      shortCut = commandProcessor.ShortCut;
   }

   public string Help()
   {
      var table = new TableMaker(("Command", Justification.Left), ("Help", Justification.Left)) { Title = "Commands" };
      foreach (var (command, (_helpText, _, _)) in commandHelp)
      {
         if (_helpText)
         {
            table.Add(command, _helpText);
         }
      }

      table.Add("config", CONFIG_HELP);

      return table.ToString();
   }

   protected static Optional<string> displayConfigurationHelp()
   {
      try
      {
         using var writer = new StringWriter();

         var firstLine = $"config - {CONFIG_HELP}";
         writer.WriteLine(firstLine);
         var length = firstLine.Length.MaxOf(80);
         writer.WriteLine("=".Repeat(length));

         writer.WriteLine("config --all : Display all configuration key/values in a table");
         writer.WriteLine("       --get <key> : Display a particular value at key");
         writer.WriteLine("       --set <key> <string> : Update item at key to value");
         writer.WriteLine("       --reset : Reset configuration to default values");

         return writer.ToString();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<string> Help(string command)
   {
      if (command.Same("config"))
      {
         return displayConfigurationHelp();
      }
      else if (commandHelp.Maybe[command] is (true, var (_helpText, _switchPattern, replacements)))
      {
         if (_helpText)
         {
            if (_switchPattern)
            {
               var formatter = new SwitchHelpFormatter(command, _helpText, _switchPattern, switchHelp, prefix, shortCut, replacements);
               var _formattedHelp = formatter.Format();
               if (_formattedHelp is (true, var formattedHelp))
               {
                  return formattedHelp;
               }
               else
               {
                  return _formattedHelp.Exception;
               }
            }
            else
            {
               return $"{command} - {_helpText}";
            }
         }
         else
         {
            return $"{command} - no help text!";
         }
      }
      else
      {
         return fail($"Don't understand command '{command}'");
      }
   }
}