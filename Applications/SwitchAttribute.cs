using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications
{
   [AttributeUsage(AttributeTargets.Property)]
   public class SwitchAttribute : Attribute
   {
      protected SwitchAttribute(string name, Maybe<string> shortCut, Maybe<string> help)
      {
         Name = name;
         ShortCut = shortCut;
         Help = help;
      }

      public SwitchAttribute(string name) : this(name, none<string>(), none<string>())
      {
      }

      public SwitchAttribute(string name, string shortCut) : this(name, shortCut.Some(), none<string>())
      {
      }

      public SwitchAttribute(string name, string shortCut, string help) : this(name, shortCut.Some(), help.Some())
      {
      }

      public string Name { get; }

      public Maybe<string> ShortCut { get; }

      public Maybe<string> Help { get; }
   }
}