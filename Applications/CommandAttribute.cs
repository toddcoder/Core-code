using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications
{
   [AttributeUsage(AttributeTargets.Method)]
   public class CommandAttribute : Attribute
   {
      public CommandAttribute(string name, string help)
      {
         Name = name;
         Help = help.Some();
      }

      public CommandAttribute(string name)
      {
         Name = name;

         Help = none<string>();
      }

      public string Name { get; }

      public Maybe<string> Help { get; }
   }
}