using System;

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

      public string HelpText { get; }

      public string SwitchPattern { get; }
   }
}