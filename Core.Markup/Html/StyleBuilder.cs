using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Html
{
   public class StyleBuilder
   {
      // ReSharper disable once CollectionNeverUpdated.Global
      protected AutoStringHash<List<string>> styles;
      protected Maybe<string> _key;

      public StyleBuilder()
      {
         styles = new AutoStringHash<List<string>>(true, _ => new List<string>(), true);
         _key = nil;
      }

      protected void add(string key, string style, string code) => styles[key].Add($"{style}: {code}");

      public void Add(string key, string style, string code)
      {
         _key = key;
         add(key, style, code);
      }

      public void Add(string style, string code)
      {
         if (_key.If(out var key))
         {
            add(key, style, code);
         }
      }

      public override string ToString() => styles.Select(i => $"{i.Key} {{{i.Value.ToString("; ")}}}").ToString(" ");
   }
}