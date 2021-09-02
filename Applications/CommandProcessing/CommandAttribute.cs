using System;

namespace Core.Applications.CommandProcessing
{
   [AttributeUsage(AttributeTargets.Method)]
   public class CommandAttribute : Attribute
   {
      public CommandAttribute(string name, bool initialize = true)
      {
         Name = name;
         Initialize = initialize;
      }

      public string Name { get; }

      public bool Initialize { get; }
   }
}