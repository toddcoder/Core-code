using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using static Core.Objects.ConversionFunctions;

namespace Core.Strings;

public class PadderTable
{
   protected struct PadderItem
   {
      public Maybe<int> Length;
      public PadType PadType;
   }

   protected static string[] newCurrentRow(int length)
   {
      var currentRow = new string[length];
      for (var i = 0; i < length; i++)
      {
         currentRow[i] = "";
      }

      return currentRow;
   }

   protected List<string[]> data;
   protected PadderItem[] items;
   protected int itemCount;
   protected int itemIndex;
   protected string[] currentRow;
   protected bool hasNoLength;
   protected Lazy<PadderArray> padder;

   public PadderTable(string format)
   {
      Format = format;
      data = new List<string[]>();
      itemIndex = 0;
      padder = new Lazy<PadderArray>(() => new PadderArray(itemCount));
      getPaddings();
      currentRow = newCurrentRow(itemCount);
   }

   public string Format { get; set; }

   public int ItemCount => itemCount;

   public int ItemIndex => itemIndex;

   protected void getPaddings()
   {
      hasNoLength = false;
      var maxCount = 0;
      var padderItems = new List<PadderItem>();

      var _matches = Format.Matches("'{' /(/d+) '}' /('[' /(/d+) /(['lLrRcC']) ']'); fi");
      if (_matches)
      {
         foreach (var match in ~_matches)
         {
            maxCount = Math.Max(maxCount, Value.Int32(match.FirstGroup));
            var _length = Maybe.Int32(match.ThirdGroup);
            var item = new PadderItem { Length = _length, PadType = getPadType(match.FourthGroup) };
            if (!item.Length && !hasNoLength)
            {
               hasNoLength = true;
            }

            padderItems.Add(item);
            match.SecondGroup = "";
         }

         Format = (~_matches).ToString();
         items = padderItems.ToArray();
         itemCount = maxCount + 1;
      }
   }

   protected static PadType getPadType(string letter) => letter switch
   {
      "l" or "L" => PadType.Left,
      "c" or "C" => PadType.Center,
      "r" or "R" => PadType.Right,
      _ => PadType.Left
   };

   public PadderTable Add(string text)
   {
      var item = items[itemIndex];
      if (item.Length)
      {
         currentRow[itemIndex] = text.Pad(item.PadType, item.Length);
      }
      else
      {
         currentRow[itemIndex] = text;
      }

      if (hasNoLength)
      {
         padder.Value.Evaluate(itemIndex, text);
      }

      if (++itemIndex >= itemCount)
      {
         data.Add(currentRow);
         currentRow = newCurrentRow(itemCount);
         itemIndex = 0;
      }

      return this;
   }

   public PadderTable AddItems(params object[] texts)
   {
      foreach (var text in texts)
      {
         Add(text.ToNonNullString());
      }

      return this;
   }

   public void AddObject(object obj, params string[] signatures)
   {
      var evaluator = new PropertyEvaluator(obj);
      foreach (var signature in signatures)
      {
         Add(evaluator[signature].ToNonNullString());
      }
   }

   public override string ToString()
   {
      if (hasNoLength)
      {
         foreach (var datum in data)
         {
            for (var j = 0; j < itemCount; j++)
            {
               if (!items[j].Length)
               {
                  datum[j] = padder.Value.Pad(j, datum[j], items[j].PadType);
               }
            }
         }
      }

      var result = data.Select(line => string.Format(Format, line.Select(l => (object)l).ToArray()));
      return result.ToString("\r\n");
   }
}