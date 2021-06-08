using System;
using System.Text.RegularExpressions;
using Core.Numbers;
using Core.Strings;

namespace Core.RegularExpressions
{
   public class RegexPattern : IEquatable<RegexPattern>
   {
      public static explicit operator RegexPattern(string source)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(source, "';' /(['icmsfu']1%3)"))
         {
            var ignoreCase = false;
            var multiline = false;
            var friendly = true;
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

            if (options.Contains("f"))
            {
               friendly = true;
            }
            else if (options.Contains("u"))
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

      public static RegexPattern operator +(RegexPattern pattern, string suffix) => pattern.WithPattern(pattern.Pattern + suffix);

      public static RegexPattern operator +(string prefix, RegexPattern pattern) => pattern.WithPattern(prefix + pattern.Pattern);

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

      public RegexPattern()
      {
         pattern = "";
         options = RegexOptions.None;
         friendly = false;
      }

      public string Pattern => pattern;

      public RegexOptions Options => options;

      public bool IgnoreCase => options[RegexOptions.IgnoreCase];

      public bool Multiline => options[RegexOptions.Multiline];

      public bool Friendly => friendly;

      public RegexPattern WithPattern(string newPattern) => new(newPattern, options, friendly);

      public bool Equals(RegexPattern other)
      {
         return other is not null && (ReferenceEquals(this, other) ||
            pattern == other.pattern && Equals(options, other.options) && friendly == other.friendly);
      }

      public override bool Equals(object obj) => obj is RegexPattern other && Equals(other);

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = pattern != null ? pattern.GetHashCode() : 0;
            hashCode = hashCode * 397 ^ (options != null ? options.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ friendly.GetHashCode();

            return hashCode;
         }
      }

      public RegexResult Matches(string input)
      {
         var matcher = new Matcher(Friendly);
         return matcher.MatchOn(input, this);
      }
   }
}