using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Numbers;
using static System.Math;

namespace Core.Strings
{
   public class PadderList
   {
      protected static void allocate<T>(List<T> list, int index)
      {
         if (index >= list.Count)
         {
            for (var i = list.Count; i <= index; i++)
            {
               list.Add(default);
            }
         }
      }

      protected class Row
      {
         protected List<string> columns;

         public Row() => columns = new List<string>();

         public int Add(int index, string text)
         {
            allocate(columns, index);
            columns[index] = text;

            return text.Length;
         }

         public IEnumerable<string> Columns(List<int> lengths, PadType[] padTypes)
         {
            var length = Min(columns.Count, lengths.Count);
            length = Min(length, padTypes.Length);
            for (var i = 0; i < length; i++)
            {
               yield return columns[i].Pad(padTypes[i], lengths[i]);
            }
         }
      }

      protected List<Row> rows;
      protected List<int> lengths;
      protected int currentIndex;

      public PadderList()
      {
         rows = new List<Row> { new() };
         lengths = new List<int>();
         currentIndex = 0;
      }

      public void Add(int index, string text)
      {
         var length = rows[currentIndex].Add(index, text);
         allocate(lengths, index);
         lengths[index] = lengths[index].MaxOf(length);
      }

      public void AddRow(params string[] items)
      {
         for (var i = 0; i < items.Length; i++)
         {
            Add(i, items[i]);
         }

         AddRow();
      }

      public void AddRow()
      {
         allocate(rows, ++currentIndex);
         rows[currentIndex] = new Row();
      }

      public IEnumerable<string> Lines(string columnSeparator, params PadType[] padTypes)
      {
         return rows.Select(row => row.Columns(lengths, padTypes).ToString(columnSeparator));
      }

      protected static PadType[] getPadTypes(string source) => source.ToCharArray().Select(c => c switch
      {
         'L' or 'l' => PadType.Left,
         'r' or 'R' => PadType.Right,
         'c' or 'C' => PadType.Center,
         _ => PadType.Left
      }).ToArray();

      public IEnumerable<string> Lines(string columnSeparator, string padTypes)
      {
         return rows.Select(row => row.Columns(lengths, getPadTypes(padTypes)).ToString(columnSeparator));
      }
   }
}