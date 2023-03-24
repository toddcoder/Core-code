using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing;

[AttributeUsage(AttributeTargets.Property)]
public class SwitchAttribute : Attribute
{
   protected SwitchAttribute(string name, string type, string argument, Optional<string> shortCut)
   {
      Name = name;
      Type = type;
      Argument = argument;
      ShortCut = shortCut;
   }

   public SwitchAttribute(string name, string type, string argument) : this(name, type, argument, nil)
   {
   }

   public SwitchAttribute(string name, string type, string argument, string shortCut) : this(name, type, argument, shortCut.Some())
   {
   }

   public string Name { get; }

   public string Type { get; }

   public string Argument { get; }

   public Optional<string> ShortCut { get; }
}