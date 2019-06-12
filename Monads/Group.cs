namespace Core.Monads
{
   public struct Group
   {
      public Group(string text, int index, int length)
      {
         Text = text;
         Index = index;
         Length = length;
      }

      public string Text { get; }

      public int Index { get; }

      public int Length { get; }
   }
}