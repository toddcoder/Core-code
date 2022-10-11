using System;

namespace Core.Markup.Rtf;

public class TableCellArgs : EventArgs
{
   public TableCellArgs(int rowIndex, int columnIndex, string text, TableCell tableCell)
   {
      RowIndex = rowIndex;
      ColumnIndex = columnIndex;
      Text = text ?? "";
      TableCell = tableCell;
   }

   public int RowIndex { get; }

   public int ColumnIndex { get; }

   public string Text { get; }

   public TableCell TableCell { get; }
}