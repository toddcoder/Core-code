using System;
using System.Linq;
using Core.Collections;
using Core.DataStructures;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Matching
{
   public class Scraper : IHash<string, string>
   {
      protected static MaybeStack<Scraper> scraperStack;

      static Scraper()
      {
         scraperStack = new MaybeStack<Scraper>();
      }

      protected Source source;
      protected StringHash<string> variables;

      public Scraper(string source)
      {
         this.source = new Source(source);

         variables = new StringHash<string>(true);
      }

      public IMatched<Scraper> Match(Pattern pattern, params string[] names)
      {
         if (!source.More)
         {
            return notMatched<Scraper>();
         }

         if (pattern.MatchedBy(source.Current).If(out var result, out var _exception))
         {
            var groups = result.Groups(0).Skip(1).ToArray();
            var minLength = groups.Length.MinOf(names.Length);
            for (var i = 0; i < minLength; i++)
            {
               variables[names[i]] = groups[i];
            }

            source.Advance(result.Length);
            return this.Matched();
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<Scraper>(exception);
         }
         else
         {
            return notMatched<Scraper>();
         }
      }

      public IMatched<Scraper> Match(Pattern pattern, Func<string, string> nameFunc)
      {
         if (!source.More)
         {
            return notMatched<Scraper>();
         }

         if (pattern.MatchedBy(source.Current).If(out var result, out var _exception))
         {
            foreach (var group in result.Groups(0).Skip(1))
            {
               var name = nameFunc(group);
               variables[name] = group;
            }

            source.Advance(result.Length);
            return this.Matched();
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<Scraper>(exception);
         }
         else
         {
            return notMatched<Scraper>();
         }
      }

      public IMatched<Scraper> Split(Pattern pattern, Func<string, string> nameFunc)
      {
         if (!source.More)
         {
            return notMatched<Scraper>();
         }

         var split = source.Current.Split(pattern);
         foreach (var value in split)
         {
            variables[nameFunc(value)] = value;
         }

         source.Advance(source.Current.Length);
         return this.Matched();
      }

      public IMatched<Scraper> Skip(Pattern pattern)
      {
         if (!source.More)
         {
            return notMatched<Scraper>();
         }

         if (pattern.MatchedBy(source.Current).If(out var result, out var _exception))
         {
            source.Advance(result.Length);
            return this.Matched();
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<Scraper>(exception);
         }
         else
         {
            return notMatched<Scraper>();
         }
      }

      public IMatched<Scraper> Skip(int count)
      {
         if (!source.More)
         {
            return notMatched<Scraper>();
         }

         source.Advance(count);
         return this.Matched();
      }

      public string this[string key] => variables[key];

      public bool ContainsKey(string key) => variables.ContainsKey(key);

      public IResult<Hash<string, string>> AnyHash() => variables.Success<Hash<string, string>>();

      public IMatched<Scraper> Push(Pattern pattern)
      {
         if (pattern.MatchedBy(source.Current).If(out var result, out var _exception))
         {
            var scraper = new Scraper(source.Current.Keep(result.Length));
            source.Advance(result.Length);
            scraperStack.Push(this);

            return scraper.Matched();
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<Scraper>(exception);
         }
         else
         {
            return notMatched<Scraper>();
         }
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

               return scraper.Matched();
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