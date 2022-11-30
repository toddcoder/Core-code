namespace Core.Strings.Text;

public class DifferenceBlock
{
   public DifferenceBlock(int oldDeleteStart, int oldDeleteCount, int newInsertStart, int newInsertCount)
   {
      OldDeleteStart = oldDeleteStart;
      OldDeleteCount = oldDeleteCount;
      NewInsertStart = newInsertStart;
      NewInsertCount = newInsertCount;
   }

   public int OldDeleteStart { get; }

   public int OldDeleteCount { get; }

   public int NewInsertStart { get; }

   public int NewInsertCount { get; }
}