using System;

namespace Core.Regex
{
   public class Matcher : RegularExpressions.Matcher
   {
      [Obsolete("Use RegexMatching.Matcher")]
      public Matcher() : base(false)
      {
      }
   }
}