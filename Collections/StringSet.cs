using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Enumerables;
using Core.Monads;
using static Core.Collections.CollectionFunctions;

namespace Core.Collections
{
   public class StringSet : Set<string>
   {
      protected bool ignoreCase;

      public StringSet(bool ignoreCase) : base(stringComparer(ignoreCase))
      {
         this.ignoreCase = ignoreCase;
      }

      public StringSet(bool ignoreCase, IEnumerable<string> strings) : base(strings, stringComparer(ignoreCase))
      {
         this.ignoreCase = ignoreCase;
      }

      public StringSet(bool ignoreCase, params string[] strings) : base(strings, stringComparer(ignoreCase))
      {
         this.ignoreCase = ignoreCase;
      }

      public Maybe<string> FirstStartsWith(string needle) => this.FirstOrNone(i => i.StartsWith(needle, ignoreCase, CultureInfo.CurrentCulture));

      public Maybe<string> FirstEndsWith(string needle) => this.FirstOrNone(i => i.EndsWith(needle, ignoreCase, CultureInfo.CurrentCulture));

      public Maybe<string> FirstWithin(string needle)
      {
         var comparison = stringComparison(ignoreCase);
         return this.FirstOrNone(i => i.IndexOf(needle, comparison) > -1);
      }

      public bool AnyStartsWith(string needle) => this.Any(i => i.StartsWith(needle, ignoreCase, CultureInfo.CurrentCulture));

      public bool AnyEndsWith(string needle) => this.Any(i => i.EndsWith(needle, ignoreCase, CultureInfo.CurrentCulture));

      public bool AnyWithin(string needle)
      {
         var comparison = stringComparison(ignoreCase);
         return this.Any(i => i.IndexOf(needle, comparison) > -1);
      }
   }
}