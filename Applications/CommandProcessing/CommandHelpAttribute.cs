using System;

namespace Core.Applications.CommandProcessing
{
   [AttributeUsage(AttributeTargets.Method)]
   public class CommandHelpAttribute : Attribute
   {
      public CommandHelpAttribute(string helpText, params string[] switches)
      {
         HelpText = helpText;
         Switches = switches;
      }

      public string HelpText { get; }

      public string[] Switches { get; }
   }
}