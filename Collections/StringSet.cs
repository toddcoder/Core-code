using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public class StringSet : Set<string>
   {
      protected static StringComparison getStringComparison(bool ignoreCase)
      {
         return ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
      }

      protected static Func<string, List<string>, IMaybe<int>> getFinder(bool ignoreCase)
      {
         return ignoreCase ? (Func<string, List<string>, IMaybe<int>>)caseInsensitiveFinder : caseSensitiveFinder;
      }

      protected static IMaybe<int> caseSensitiveFinder(string item, List<string> list)
      {
         var index = list.IndexOf(item);
         return maybe(index > -1, () => index);
      }

      protected static IMaybe<int> caseInsensitiveFinder(string item, List<string> list)
      {
         for (var i = 0; i < list.Count; i++)
         {
            if (list[i].Same(item))
            {
               return i.Some();
            }
         }

         return none<int>();
      }

      protected StringComparison stringComparison;
      protected Func<string, List<string>, IMaybe<int>> finder;

      public StringSet() { }

      public StringSet(bool ignoreCase)
      {
         stringComparison = getStringComparison(ignoreCase);
         finder = getFinder(ignoreCase);
      }

      public StringSet(IEnumerable<string> strings) : base(strings) { }

      public StringSet(IEnumerable<string> strings, bool ignoreCase) : base(strings)
      {
         stringComparison = getStringComparison(ignoreCase);
         finder = getFinder(ignoreCase);
      }

      public StringSet(params string[] items) : base(items) { }

      public bool IgnoreCase
      {
         get => stringComparison == StringComparison.OrdinalIgnoreCase;
         set
         {
            stringComparison = getStringComparison(value);
            finder = getFinder(value);
         }
      }

      public override bool Contains(string item)
      {
         if (item != null)
         {
            return content.Any(i => string.Compare(item, i, stringComparison) == 0);
         }
         else
         {
            return content.Any(i => i == null);
         }
      }

      public IMaybe<bool> IsExactlyEqualTo(string target)
      {
         return content.FirstOrNone(i => i.IsExactlyEqualTo(target).HasValue).Map(r => r.IsExactlyEqualTo(target));
      }

      public override void Remove(string item)
      {
         if (finder(item, content).If(out var index))
         {
            content.RemoveAt(index);
         }
      }
   }
}