namespace Core.Strings.Text
{
   public class TextDiffBlock
   {
      public TextDiffBlock(int deleteStartA, int deleteCountA, int insertStartB, int insertCountB)
      {
         DeleteStartA = deleteStartA;
         DeleteCountA = deleteCountA;
         InsertStartB = insertStartB;
         InsertCountB = insertCountB;
      }

      public int DeleteStartA { get; }

      public int DeleteCountA { get; }

      public int InsertStartB { get; }

      public int InsertCountB { get; }
   }
}