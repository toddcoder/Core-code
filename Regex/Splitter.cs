using System;

namespace Core.Regex
{
   [Obsolete]
   public class Splitter : RegularExpressions.Splitter
   {
      public Splitter() : base(false)
      {
      }
   }
}