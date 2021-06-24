using System;

namespace Core.Applications.CommandProcessing
{
   [AttributeUsage(AttributeTargets.Method)]
   public class CommandAttribute : Attribute
   {
      public CommandAttribute(string name)
      {
         Name = name;
      }

      public string Name { get; }
   }
}