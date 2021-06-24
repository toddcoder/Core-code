using System;

namespace Core.Applications.CommandProcessing
{
   [AttributeUsage(AttributeTargets.Property)]
   public class SwitchAttribute : Attribute
   {
      public SwitchAttribute(string name)
      {
         Name = name;
      }

      public string Name { get; }
   }
}