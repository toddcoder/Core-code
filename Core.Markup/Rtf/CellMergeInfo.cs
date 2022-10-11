namespace Core.Markup.Rtf;

public class CellMergeInfo
{
   protected int rowSpan;
   protected int columnSpan;
   protected int rowIndex;
   protected int columnIndex;
   protected TableCell representative;

   public CellMergeInfo(TableCell representative, int rowSpan, int columnSpan, int rowIndex, int columnIndex)
   {
      this.representative = representative;
      this.rowSpan = rowSpan;
      this.columnSpan = columnSpan;
      this.rowIndex = rowIndex;
      this.columnIndex = columnIndex;
   }

   public int RowSpan => rowSpan;

   public int ColumnSpan => columnSpan;

   public int RowIndex => rowIndex;

   public int ColumnIndex => columnIndex;

   public TableCell Representative => representative;
}