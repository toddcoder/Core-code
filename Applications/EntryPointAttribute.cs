using System;

namespace Core.Applications;

public class EntryPointAttribute : Attribute
{
   public EntryPointAttribute(EntryPointType type)
   {
      Type = type;
   }

   public EntryPointType Type { get; }
}