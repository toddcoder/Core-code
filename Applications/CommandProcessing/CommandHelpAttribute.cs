using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing
{
   [AttributeUsage(AttributeTargets.Method)]
   public class CommandHelpAttribute : Attribute
   {
      public CommandHelpAttribute(string helpText, string switchPattern)
      {
         HelpText = helpText;
         SwitchPattern = switchPattern;
      }

      public CommandHelpAttribute(string helpText)
      {
         HelpText = helpText;
         SwitchPattern = nil;
      }

      public string HelpText { get; }

      public Maybe<string> SwitchPattern { get; }
   }
}