using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;

namespace Core.Strings
{
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

         //matcher.Evaluate(Format, , true);
         if (Format.Matches("'{' /(/d+) '}' /('[' /(/d+) /(['lLrRcC']) ']'); fi").If(out var result))
         {
            for (var i = 0; i < result.MatchCount; i++)
            {
               maxCount = Math.Max(maxCount, result[i, 1].ToInt());
               var length = result[i, 3].AsInt();
               var item = new PadderItem { Length = length, PadType = getPadType(result[i, 4]) };
               if (item.Length.IsNone && !hasNoLength)
               {
                  hasNoLength = true;
               }

               padderItems.Add(item);
               result[i, 2] = "";
            }
         }

         Format = result.ToString();
         items = padderItems.ToArray();
         itemCount = maxCount + 1;
      }

      protected static PadType getPadType(string letter)
      {
         return letter switch
         {
            "l" or "L" => PadType.Left,
            "c" or "C" => PadType.Center,
            "r" or "R" => PadType.Right,
            _ => PadType.Left
         };
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
         return result.ToString("\r\n");
      }
   }
}