using System;

namespace Core.Applications.CommandProcessing
{
   [AttributeUsage(AttributeTargets.Property)]
   public class SwitchAttribute : Attribute
   {
      public SwitchAttribute(string name, bool flag = false)
      {
         Name = name;
         Flag = flag;
      }

      public string Name { get; }

      public bool Flag { get; }
   }
}