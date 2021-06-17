using System;

namespace Core.Regex
{
   [Obsolete("Use RegexMatching.Scraper")]
   public class Scraper : RegularExpressions.Scraper
   {
      public Scraper(string source) : base(source, false)
      {
      }
   }
}