using System;

namespace Core.Applications.CommandProcessing
{
   public class ShortCutAttribute : Attribute
   {
      public ShortCutAttribute(string name)
      {
         Name = name;
      }

      public string Name { get; }
   }
}