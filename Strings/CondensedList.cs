﻿using System.Collections.Generic;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;

namespace Core.Strings;

public class CondensedList : IHash<string, int>
{
   protected string separator;
   protected StringHash<int> counts;

   public CondensedList(string separator = ", ", bool ignoreCase = true)
   {
      this.separator = separator;
      counts = new StringHash<int>(ignoreCase);
   }

   public void Add(string text)
   {
      var _count = counts.Maybe[text];
      if (_count)
      {
         counts[text] = _count + 1;
      }
      else
      {
         counts[text] = 1;
      }
   }

   public void AddRange(IEnumerable<string> texts)
   {
      foreach (var text in texts)
      {
         Add(text);
      }
   }

   protected IEnumerable<string> enumerable()
   {
      foreach (var (text, count) in counts)
      {
         if (count == 1)
         {
            yield return text;
         }
         else
         {
            yield return $"{text} x {count}";
         }
      }
   }

   public override string ToString() => enumerable().ToString(separator);

   public int this[string key] => counts[key];

   public bool ContainsKey(string key) => counts.ContainsKey(key);

   public Result<Hash<string, int>> AnyHash() => counts;

   public HashInterfaceMaybe<string, int> Items => new(this);
}