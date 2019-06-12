using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Numbers;
using static System.Math;

namespace Core.Strings
{
   public class PadderList
   {
      static void allocate<T>(List<T> list, int index)
      {
         if (index >= list.Count)
            for (var i = list.Count; i <= index; i++)
               list.Add(default);
      }

      class Row
      {
         List<string> columns;

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
               yield return columns[i].Pad(padTypes[i], lengths[i]);
         }
      }

      List<Row> rows;
      List<int> lengths;
      int currentIndex;

      public PadderList()
      {
         rows = new List<Row> { new Row() };
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
            Add(i, items[i]);
         AddRow();
      }

      public void AddRow()
      {
         allocate(rows, ++currentIndex);
         rows[currentIndex] = new Row();
      }

      public IEnumerable<string> Lines(string columnSeparator, params PadType[] padTypes)
      {
         foreach (var row in rows)
            yield return row.Columns(lengths, padTypes).Stringify(columnSeparator);
      }

      static PadType[] getPadTypes(string source) => source.ToCharArray().Select(c =>
      {
         switch (c)
         {
            case 'L':
            case 'l':
               return PadType.Left;
            case 'r':
            case 'R':
               return PadType.Right;
            case 'c':
            case 'C':
               return PadType.Center;
            default:
               return PadType.Left;
         }
      }).ToArray();

      public IEnumerable<string> Lines(string columnSeparator, string padTypes)
      {
         foreach (var row in rows)
            yield return row.Columns(lengths, getPadTypes(padTypes)).Stringify(columnSeparator);
      }
   }
}