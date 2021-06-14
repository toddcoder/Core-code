using System.Text.RegularExpressions;
using Core.Strings;

namespace Core.Regex
{
   public class RegexPattern : RegularExpressions.RegexPattern
   {
      public static explicit operator RegexPattern(string source)
      {
         var matcher = new RegularExpressions.Matcher();
         if (matcher.IsMatch(source, "';' /(['icms']1%3)"))
         {
            var ignoreCase = false;
            var multiline = false;
            var options = matcher.FirstGroup;
            if (options.Contains("i"))
            {
               ignoreCase = true;
            }
            else if (options.Contains("c"))
            {
               ignoreCase = false;
            }

            if (options.Contains("m"))
            {
               multiline = true;
            }
            else if (options.Contains("s"))
            {
               multiline = false;
            }

            var pattern = source.Drop(-matcher.Length);

            return new RegexPattern(pattern.TrimEnd(), ignoreCase, multiline);
         }
         else
         {
            return new RegexPattern(source, false, false);
         }
      }

      public RegexPattern(string pattern, RegexOptions options) : base(pattern, options, false)
      {
      }

      public RegexPattern(string pattern, bool ignoreCase, bool multiline) : base(pattern, ignoreCase, multiline, false)
      {
      }

      public RegexPattern()
      {
         pattern = "";
         options = RegexOptions.None;
         friendly = false;
      }
   }
}