using Core.Collections;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing
{
   public class HelpGenerator
   {
      protected StringHash<(string helpText, string switchPattern)> commandHelp;
      protected StringHash<(string type, Maybe<string> _argument, Maybe<string> _shortCut)> switchHelp;
      protected string prefix;
      protected string suffix;
      protected string shortCut;

      public HelpGenerator(CommandProcessor commandProcessor)
      {
         commandHelp = commandProcessor.GetCommandHelp();
         switchHelp = commandProcessor.GetSwitchHelp();
         prefix = commandProcessor.Prefix;
         suffix = commandProcessor.Suffix;
         shortCut = commandProcessor.ShortCut;
      }

      public string Help()
      {
         var table = new TableMaker(("Command", Justification.Left), ("Help", Justification.Left)) { Title = "Commands" };
         foreach (var (command, (helpText, _)) in commandHelp)
         {
            table.Add(command, helpText);
         }

         return table.ToString();
      }

      public Result<string> Help(string command)
      {
         if (commandHelp.If(command, out var tuple))
         {
            var (helpText, switchPattern) = tuple;
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
            return fail($"Don't understand command '{command}'");
         }
      }
   }
}