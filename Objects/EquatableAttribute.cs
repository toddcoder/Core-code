using System;

namespace Core.Objects
{
   [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
   public class EquatableAttribute : Attribute { }
}