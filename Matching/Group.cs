using Core.Strings;

namespace Core.Matching
{
   public class Group
   {
      public static Group Empty => new() { Index = -1, Length = 0, Text = "", Which = -1 };

      public int Index { get; set; }

      public int Length { get; set; }

      public string Text { get; set; }

      public int Which { get; set; }

      internal string GetSlice(Slicer slicer) => slicer[Index, Length];

      internal void SetSlice(Slicer slicer, string text) => slicer[Index, Length] = text;

      public void Deconstruct(out string text, out int index, out int length)
      {
         text = Text;
         index = Index;
         length = Length;
      }

      public Slice Slice => new(Text, Index, Length);

      public override string ToString() => Text;
   }
}