using System;

namespace Core.Applications
{
   public class EntryPointAttribute : Attribute
   {
      public EntryPointAttribute(bool usesObject = false)
      {
         UsesObject = usesObject;
      }

      public bool UsesObject { get; }
   }
}