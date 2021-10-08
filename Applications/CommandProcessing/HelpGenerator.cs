﻿using Core.Collections;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing
{
   public class HelpGenerator
   {
      protected StringHash<(Maybe<string> _helpText, Maybe<string> _switchPattern)> commandHelp;
      protected StringHash<(string type, string argument, Maybe<string> _shortCut)> switchHelp;
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
         foreach (var (command, (_helpText, _)) in commandHelp)
         {
            if (_helpText.If(out var helpText))
            {
               table.Add(command, helpText);
            }
         }

         return table.ToString();
      }

      public Result<string> Help(string command)
      {
         if (commandHelp.If(command, out var tuple))
         {
            var (_helpText, _switchPattern) = tuple;
            if (_helpText.If(out var helpText) && _switchPattern.If(out var switchPattern))
            {
               var formatter = new SwitchHelpFormatter(command, helpText, switchPattern, switchHelp, prefix, shortCut);
               if (formatter.Format().If(out var formattedHelp, out var exception))
               {
                  return formattedHelp;
               }
               else
               {
                  return exception;
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
}