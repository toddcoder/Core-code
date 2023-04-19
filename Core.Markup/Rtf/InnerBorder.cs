using Core.Monads;

namespace Core.Markup.Rtf;

public class InnerBorder
{
   public InnerBorder(BorderStyle borderStyle, float width, Maybe<ColorDescriptor> color)
   {
      BorderStyle = borderStyle;
      Width = width;
      Color = color;
   }

   public BorderStyle BorderStyle { get; }

   public float Width { get; }

   public Maybe<ColorDescriptor> Color { get; }
}