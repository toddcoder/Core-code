using System;

namespace Core.Regex
{
   [Obsolete("Use RegexMatching.Matcher")]
   public class Matcher : RegularExpressions.Matcher
   {
      public Matcher() : base(false)
      {
      }
   }
}