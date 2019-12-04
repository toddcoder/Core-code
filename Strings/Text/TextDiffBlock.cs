namespace Core.Strings.Text
{
   internal class TextDiffBlock
   {
      public TextDiffBlock(int oldDeleteStart, int oldDeleteCount, int newInsertStart, int newInsertCount)
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
}