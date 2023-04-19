namespace Core.Markup.Rtf;

public class ColumnWidth
{
   public ColumnWidth(int index, float width)
   {
      Index = index;
      Width = width;
   }

   public int Index { get; }

   public float Width { get; }
}