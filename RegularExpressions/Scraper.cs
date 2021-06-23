using System;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Collections;
using Core.DataStructures;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions
{
   public class Scraper : IHash<string, string>
   {
      protected static MaybeStack<Scraper> scraperStack;

      static Scraper()
      {
         scraperStack = new MaybeStack<Scraper>();
      }

      protected Source source;
      protected bool friendly;
      protected Matcher matcher;
      protected StringHash<string> variables;

      public Scraper(string source, bool friendly = true)
      {
         this.source = new Source(source);
         this.friendly = friendly;

         matcher = new Matcher(this.friendly);

         variables = new StringHash<string>(true);
      }

      protected IMatched<Scraper> match(string pattern, bool ignoreCase, bool multiline, string[] names)
      {
         try
         {
            if (!source.More)
            {
               return noMatch<Scraper>();
            }

            if (matcher.IsMatch(source.Current, pattern, ignoreCase, multiline))
            {
               var groups = matcher.Groups(0).Skip(1).ToArray();
               var minLength = groups.Length.MinOf(names.Length);
               for (var i = 0; i < minLength; i++)
               {
                  variables[names[i]] = groups[i];
               }

               source.Advance(matcher.Length);
               return this.Match();
            }
            else
            {
               return noMatch<Scraper>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<Scraper>(exception);
         }
      }

      public IMatched<Scraper> Match(RegexPattern pattern, params string[] names)
      {
         return match(pattern.Pattern, pattern.IgnoreCase, pattern.Multiline, names);
      }

      public IMatched<Scraper> Match(string pattern, params string[] names)
      {
         return Match((RegexPattern)pattern, names);
      }

      public IMatched<Scraper> Match(string pattern, bool ignoreCase, bool multiline, params string[] names)
      {
         return match(pattern, ignoreCase, multiline, names);
      }

      public IMatched<Scraper> Match(string pattern, Bits32<RegexOptions> options, params string[] names)
      {
         return match(pattern, options[RegexOptions.IgnoreCase], options[RegexOptions.Multiline], names);
      }

      protected IMatched<Scraper> match(string pattern, bool ignoreCase, bool multiline, Func<string, string> nameFunc)
      {
         try
         {
            if (!source.More)
            {
               return noMatch<Scraper>();
            }

            if (matcher.IsMatch(source.Current, pattern, ignoreCase, multiline))
            {
               foreach (var group in matcher.Groups(0).Skip(1))
               {
                  var name = nameFunc(group);
                  variables[name] = group;
               }

               source.Advance(matcher.Length);
               return this.Match();
            }
            else
            {
               return noMatch<Scraper>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<Scraper>(exception);
         }
      }

      public IMatched<Scraper> Match(RegexPattern pattern, Func<string, string> nameFunc)
      {
         return match(pattern.Pattern, pattern.IgnoreCase, pattern.Multiline, nameFunc);
      }

      public IMatched<Scraper> Match(string pattern, Func<string, string> nameFunc)
      {
         return Match((RegexPattern)pattern, nameFunc);
      }

      public IMatched<Scraper> Match(string pattern, bool ignoreCase, bool multiline, Func<string, string> nameFunc)
      {
         return match(pattern, ignoreCase, multiline, nameFunc);
      }

      public IMatched<Scraper> Match(string pattern, Bits32<RegexOptions> options, Func<string, string> nameFunc)
      {
         return match(pattern, options[RegexOptions.IgnoreCase], options[RegexOptions.Multiline], nameFunc);
      }

      protected IMatched<Scraper> split(string pattern, RegexOptions options, Func<string, string> nameFunc)
      {
         try
         {
            if (!source.More)
            {
               return noMatch<Scraper>();
            }

            var split = source.Current.Split(pattern, options, friendly);
            foreach (var value in split)
            {
               variables[nameFunc(value)] = value;
            }

            source.Advance(source.Current.Length);
            return this.Match();
         }
         catch (Exception exception)
         {
            return failedMatch<Scraper>(exception);
         }
      }

      public IMatched<Scraper> Split(RegexPattern pattern, Func<string, string> nameFunc)
      {
         return split(pattern.Pattern, pattern.Options, nameFunc);
      }

      public IMatched<Scraper> Split(string pattern, Func<string, string> nameFunc)
      {
         return Split((RegexPattern)pattern, nameFunc);
      }

      public IMatched<Scraper> Split(string pattern, bool ignoreCase, bool multiline, Func<string, string> nameFunc)
      {
         Bits32<RegexOptions> options = RegexOptions.None;
         options[RegexOptions.IgnoreCase] = ignoreCase;
         options[RegexOptions.Multiline] = multiline;

         return split(pattern, options, nameFunc);
      }

      public IMatched<Scraper> Split(string pattern, RegexOptions options, Func<string, string> nameFunc)
      {
         return split(pattern, options, nameFunc);
      }

      protected IMatched<Scraper> skip(string pattern, bool ignoreCase, bool multiline)
      {
         try
         {
            if (!source.More)
            {
               return noMatch<Scraper>();
            }

            if (matcher.IsMatch(source.Current, pattern, ignoreCase, multiline))
            {
               source.Advance(matcher.Length);
               return this.Match();
            }
            else
            {
               return noMatch<Scraper>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<Scraper>(exception);
         }
      }

      public IMatched<Scraper> Skip(RegexPattern pattern) => skip(pattern.Pattern, pattern.IgnoreCase, pattern.Multiline);

      public IMatched<Scraper> Skip(string pattern) => Skip((RegexPattern)pattern);

      public IMatched<Scraper> Skip(string pattern, bool ignoreCase, bool multiline) => skip(pattern, ignoreCase, multiline);

      public IMatched<Scraper> Skip(string pattern, Bits32<RegexOptions> options)
      {
         return skip(pattern, options[RegexOptions.IgnoreCase], options[RegexOptions.Multiline]);
      }

      public IMatched<Scraper> Skip(int count)
      {
         try
         {
            if (!source.More)
            {
               return noMatch<Scraper>();
            }

            source.Advance(count);
            return this.Match();
         }
         catch (Exception exception)
         {
            return failedMatch<Scraper>(exception);
         }
      }

      public string this[string key] => variables[key];

      public bool ContainsKey(string key) => variables.ContainsKey(key);

      public Result<Hash<string, string>> AnyHash() => variables.Success<Hash<string, string>>();

      protected IMatched<Scraper> push(string pattern, bool ignoreCase, bool multiline)
      {
         try
         {
            if (matcher.IsMatch(source.Current, pattern, ignoreCase, multiline))
            {
               var scraper = new Scraper(source.Current.Keep(matcher.Length));
               source.Advance(matcher.Length);
               scraperStack.Push(this);

               return scraper.Match();
            }
            else
            {
               return noMatch<Scraper>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<Scraper>(exception);
         }
      }

      public IMatched<Scraper> Push(RegexPattern regexPattern)
      {
         return push(regexPattern.Pattern, regexPattern.IgnoreCase, regexPattern.Multiline);
      }

      public IMatched<Scraper> Push(string pattern) => Push((RegexPattern)pattern);

      public IMatched<Scraper> Push(string pattern, bool ignoreCase, bool multiline) => push(pattern, ignoreCase, multiline);

      public IMatched<Scraper> Push(string pattern, Bits32<RegexOptions> options)
      {
         return push(pattern, options[RegexOptions.IgnoreCase], options[RegexOptions.Multiline]);
      }

      public IMatched<Scraper> Pop()
      {
         if (scraperStack.Pop().If(out var scraper))
         {
            if (scraper.AnyHash().If(out var poppedVariables, out var exception))
            {
               foreach (var (variable, value) in variables)
               {
                  poppedVariables[variable] = value;
               }

               return scraper.Match();
            }
            else
            {
               return failedMatch<Scraper>(exception);
            }
         }
         else
         {
            return "Scraper stack empty".FailedMatch<Scraper>();
         }
      }
   }
}