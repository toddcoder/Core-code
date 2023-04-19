namespace Core.Markup.Rtf;

public class RowHeight
{
   public RowHeight(int index, float height)
   {
      Index = index;
      Height = height;
   }

   public int Index { get; }

   public float Height { get; }
}