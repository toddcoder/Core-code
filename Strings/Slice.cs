namespace Core.Strings
{
   public struct Slice
   {
      public int Index;
      public int Length;
      public string Text;

      public Slice(int index, int length, string text): this()
      {
         Index = index;
         Length = length;
         Text = text;
      }
   }
}