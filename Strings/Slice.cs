namespace Core.Strings;

public struct Slice
{
   public string Text;
   public int Index;
   public int Length;

   public Slice(string text, int index, int length): this()
   {
      Index = index;
      Length = length;
      Text = text;
   }

   public void Deconstruct(out string text, out int index, out int length)
   {
      text = Text;
      index = Index;
      length = Length;
   }
}