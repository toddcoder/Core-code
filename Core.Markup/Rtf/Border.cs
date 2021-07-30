namespace Core.Markup.Rtf
{
   public class Border
   {
      private BorderStyle style;
      private float width;
      private ColorDescriptor colorDescriptor;

      public Border()
      {
         style = BorderStyle.None;
         width = 0.5F;
         colorDescriptor = new ColorDescriptor(0);
      }

      public override bool Equals(object obj) => obj is Border border && Style == border.Style && Width == border.Width;

      public override int GetHashCode() => width.GetHashCode() * 1000 + (int)style;

      public BorderStyle Style
      {
         get => style;
         set => style = value;
      }

      public float Width
      {
         get => width;
         set => width = value;
      }

      public ColorDescriptor Color
      {
         get => colorDescriptor;
         set => colorDescriptor = value;
      }
   }
}