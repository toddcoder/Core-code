using System.Text.RegularExpressions;
using Core.Numbers;
using Core.Strings;

namespace Core.RegularExpressions
{
   public class RegexPattern
   {
      public static implicit operator RegexPattern(string source)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(source, "';' /(['imf']1%3)"))
         {
            var ignoreCase = false;
            var multiline = false;
            var friendly = true;
            var options = matcher.FirstGroup;
            if (options.Contains("i"))
            {
               ignoreCase = true;
            }

            if (options.Contains("m"))
            {
               multiline = true;
            }

            if (options.Contains("f"))
            {
               friendly = false;
            }

            var pattern = source.Drop(-matcher.Length);

            return new RegexPattern(pattern, ignoreCase, multiline, friendly);
         }
         else
         {
            return new RegexPattern(source, false, false);
         }
      }

      protected string pattern;
      protected Bits32<RegexOptions> options;
      protected bool friendly;

      public RegexPattern(string pattern, RegexOptions options, bool friendly = true)
      {
         this.pattern = pattern;
         this.options = options;
         this.friendly = friendly;
      }

      public RegexPattern(string pattern, bool ignoreCase, bool multiline, bool friendly = true) : this(pattern, RegexOptions.None, friendly)
      {
         options[RegexOptions.IgnoreCase] = ignoreCase;
         options[RegexOptions.Multiline] = multiline;
      }

      public string Pattern => pattern;

      public RegexOptions Options => options;

      public bool IgnoreCase => options[RegexOptions.IgnoreCase];

      public bool Multiline => options[RegexOptions.Multiline];

      public bool Friendly => friendly;
   }
}