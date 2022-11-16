using System;
using System.Linq;
using Core.Collections;
using Core.DataStructures;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Matching;

public class Scraper : IHash<string, string>
{
   protected static MaybeStack<Scraper> scraperStack;

   static Scraper()
   {
      scraperStack = new MaybeStack<Scraper>();
   }

   protected Source source;
   protected StringHash variables;

   public Scraper(string source)
   {
      this.source = new Source(source);

      variables = new StringHash(true);
   }

   public Responding<Scraper> Match(Pattern pattern, params string[] names)
   {
      if (!source.More)
      {
         return nil;
      }

      var _result = pattern.MatchedBy(source.Current);
      if (_result)
      {
         var groups = _result.Value.Groups(0).Skip(1).ToArray();
         var minLength = groups.Length.MinOf(names.Length);
         for (var i = 0; i < minLength; i++)
         {
            variables[names[i]] = groups[i];
         }

         source.Advance(_result.Value.Length);
         return this;
      }
      else if (_result.AnyException)
      {
         return _result.Exception;
      }
      else
      {
         return nil;
      }
   }

   public Responding<Scraper> Match(Pattern pattern, Func<string, string> nameFunc)
   {
      if (!source.More)
      {
         return nil;
      }

      var _result = pattern.MatchedBy(source.Current);
      if (_result)
      {
         foreach (var group in _result.Value.Groups(0).Skip(1))
         {
            var name = nameFunc(group);
            variables[name] = group;
         }

         source.Advance(_result.Value.Length);
         return this;
      }
      else if (_result.AnyException)
      {
         return _result.Exception;
      }
      else
      {
         return nil;
      }
   }

   public Responding<Scraper> Split(Pattern pattern, Func<string, string> nameFunc)
   {
      if (!source.More)
      {
         return nil;
      }

      var split = source.Current.Unjoin(pattern);
      foreach (var value in split)
      {
         variables[nameFunc(value)] = value;
      }

      source.Advance(source.Current.Length);
      return this;
   }

   public Responding<Scraper> Skip(Pattern pattern)
   {
      if (!source.More)
      {
         return nil;
      }

      var _result = pattern.MatchedBy(source.Current);
      if (_result)
      {
         source.Advance(_result.Value.Length);
         return this;
      }
      else if (_result.AnyException)
      {
         return _result.Exception;
      }
      else
      {
         return nil;
      }
   }

   public Responding<Scraper> Skip(int count)
   {
      if (!source.More)
      {
         return nil;
      }

      source.Advance(count);
      return this;
   }

   public string this[string key] => variables[key];

   public bool ContainsKey(string key) => variables.ContainsKey(key);

   public Result<Hash<string, string>> AnyHash() => variables;

   public Responding<Scraper> Push(Pattern pattern)
   {
      var _length = pattern.MatchedBy(source.Current).Map(r => r.Length);
      if (_length)
      {
         var scraper = new Scraper(source.Current.Keep(_length));
         source.Advance(_length);
         scraperStack.Push(this);

         return scraper;
      }
      else if (_length.AnyException)
      {
         return _length.Exception;
      }
      else
      {
         return nil;
      }
   }

   public Responding<Scraper> Pop()
   {
      var _scraper = scraperStack.Pop();
      if (_scraper)
      {
         var _variables = _scraper.Value.AnyHash();
         if (_variables)
         {
            variables = _variables.Map(h => h.ToStringHash(true));
            foreach (var (variable, value) in variables)
            {
               _variables.Value[variable] = value;
            }

            return _scraper.Responding();
         }
         else
         {
            return _variables.Exception;
         }
      }
      else
      {
         return fail("Scraper stack empty");
      }
   }
}