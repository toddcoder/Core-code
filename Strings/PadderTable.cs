using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using Core.RegularExpressions;

namespace Core.Strings
{
   public class PadderTable
   {
      struct PadderItem
      {
         public IMaybe<int> Length;
         public PadType PadType;
      }

      static string[] newCurrentRow(int length)
      {
         var currentRow = new string[length];
         for (var i = 0; i < length; i++)
         {
            currentRow[i] = "";
         }

         return currentRow;
      }

      List<string[]> data;
      PadderItem[] items;

      int itemCount;
      int itemIndex;
      string[] currentRow;
      bool hasNoLength;
      Lazy<PadderArray> padder;

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

      void getPaddings()
      {
         hasNoLength = false;
         var maxCount = 0;
         var padderItems = new List<PadderItem>();

         var matcher = new Matcher();

         matcher.Evaluate(Format, "'{' /(/d+) '}' /('[' /(/d+) /(['lLrRcC']) ']')", true);
         for (var i = 0; i < matcher.MatchCount; i++)
         {
            maxCount = Math.Max(maxCount, matcher[i, 1].ToInt());
            var length = matcher[i, 3].AsInt();
            var item = new PadderItem { Length = length, PadType = getPadType(matcher[i, 4]) };
            if (item.Length.IsNone && !hasNoLength)
            {
               hasNoLength = true;
            }

            padderItems.Add(item);
            matcher[i, 2] = "";
         }

         Format = matcher.ToString();
         items = padderItems.ToArray();
         itemCount = maxCount + 1;
      }

      static PadType getPadType(string letter)
      {
         switch (letter)
         {
            case "l":
            case "L":
               return PadType.Left;
            case "c":
            case "C":
               return PadType.Center;
            case "r":
            case "R":
               return PadType.Right;
            default:
               return PadType.Left;
         }
      }

      public PadderTable Add(string text)
      {
         var item = items[itemIndex];
         if (item.Length.If(out var length))
         {
            currentRow[itemIndex] = text.Pad(item.PadType, length);
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
                  if (items[j].Length.IsNone)
                  {
                     datum[j] = padder.Value.Pad(j, datum[j], items[j].PadType);
                  }
               }
            }
         }

         var result = data.Select(line => string.Format(Format, line.Select(l => (object)l).ToArray()));
         return result.Stringify("\r\n");
      }
   }
}