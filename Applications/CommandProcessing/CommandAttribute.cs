using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing
{
   [AttributeUsage(AttributeTargets.Method)]
   public class CommandAttribute : Attribute
   {
      protected CommandAttribute(string name, Maybe<string> helpText, Maybe<string> switchPattern, bool initialize = true)
      {
         Name = name;
         HelpText = helpText;
         SwitchPattern = switchPattern;
         Initialize = initialize;
      }

      public CommandAttribute(string name, bool initialize = true) : this(name, nil, nil, initialize)
      {
      }

      public CommandAttribute(string name, string helpText, bool initialize = true) : this(name, helpText, nil, initialize)
      {
      }

      public CommandAttribute(string name, string helpText, string switchPattern, bool initialize = true) :
         this(name, helpText.Some(), switchPattern.Some(), initialize)
      {
      }

      public string Name { get; }

      public Maybe<string> HelpText { get; }

      public Maybe<string> SwitchPattern { get; }

      public bool Initialize { get; }
   }
}