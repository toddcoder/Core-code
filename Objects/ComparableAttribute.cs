using System;

namespace Core.Objects;

public class ComparableAttribute : Attribute
{
   public ComparableAttribute(int order)
   {
      Order = order;
   }

   public int Order { get; }
}