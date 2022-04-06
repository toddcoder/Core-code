using System;
using System.Collections.Generic;

namespace Core.Markup.Rtf
{
   public class TableData
   {
      protected Document document;
      protected List<List<string>> rows;
      protected int maxColumnCount;

      public event EventHandler<TableCellArgs> TableCell;

      public TableData(Document document)
      {
         this.document = document;

         rows = new List<List<string>>();
         maxColumnCount = 0;
      }

      public int RowCount => rows.Count;

      public int MaxColumnCount => maxColumnCount;

      public void AddRow(params string[] columns)
      {
         var columnList = new List<string>();
         columnList.AddRange(columns);
         rows.Add(columnList);
         if (columns.Length > maxColumnCount)
         {
            maxColumnCount = columns.Length;
         }
      }

      public Table Table(float fontSize)
      {
         var table = document.Table(rows.Count, maxColumnCount, fontSize);
         return getTable(table);
      }

      public Table Table(float horizontalWidth, float fontSize)
      {
         var table = document.Table(rows.Count, maxColumnCount, horizontalWidth, fontSize);
         return getTable(table);
      }

      protected Table getTable(Table table)
      {
         var rowIndex = 0;

         foreach (var row in rows)
         {
            var columnIndex = 0;
            foreach (var column in row)
            {
               if (columnIndex < maxColumnCount)
               {
                  var tableCell = table[rowIndex, columnIndex];
                  TableCell?.Invoke(this, new TableCellArgs(rowIndex, columnIndex, column ?? "", tableCell));
               }
               else
               {
                  break;
               }

               columnIndex++;
            }

            rowIndex++;
         }

         return table;
      }
   }
}