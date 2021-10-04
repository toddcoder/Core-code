using System;
using System.Globalization;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing
{
   [AttributeUsage(AttributeTargets.Property)]
   public class SwitchHelpAttribute : Attribute
   {
      public SwitchHelpAttribute(string type, string argument)
      {
         Type = type;
         Argument = argument;
      }

      public SwitchHelpAttribute(string type)
      {
         Type = type;
         Argument = nil;
      }

      public string Type { get; }

      public Maybe<string> Argument { get; }
   }
}