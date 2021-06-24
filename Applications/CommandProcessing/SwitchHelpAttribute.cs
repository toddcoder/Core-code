using System;

namespace Core.Applications.CommandProcessing
{
   [AttributeUsage(AttributeTargets.Property)]
   public class SwitchHelpAttribute : Attribute
   {
      public SwitchHelpAttribute(string helpText)
      {
         HelpText = helpText;
      }

      public string HelpText { get; }
   }
}