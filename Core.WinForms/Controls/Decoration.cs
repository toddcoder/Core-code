using System.Drawing;
using System.Windows.Forms;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;
using DrawingColor = System.Drawing.Color;

namespace Core.WinForms.Controls
{
   public abstract class Decoration
   {
   }

   public sealed class Undecorated : Decoration
   {
   }

   public sealed class TextColor : Decoration
   {
      public TextColor(DrawingColor color)
      {
         Color = color;
      }

      public DrawingColor Color { get; }
   }

   public sealed class TextFont : Decoration
   {
      public TextFont(string fontName, float fontSize)
      {
         FontName = fontName;
         FontSize = fontSize;
      }

      public string FontName { get; }

      public float FontSize { get; }
   }

   public abstract class TextStyle : Decoration
   {
      protected TextStyle(bool enabled)
      {
         Enabled = enabled;
      }

      public bool Enabled { get; }
   }

   public sealed class Bold : TextStyle
   {
      public Bold(bool enabled) : base(enabled)
      {
      }
   }

   public sealed class Italic: TextStyle
   {
      public Italic(bool enabled) : base(enabled)
      {
      }
   }

   public sealed class Underline : TextStyle
   {
      public Underline(bool enabled) : base(enabled)
      {
      }
   }

   public class CombinedDecorations
   {
      public CombinedDecorations(int index, int length)
      {
         Index = index;
         Length = length;

         TextColor = nil;
         TextFont = nil;
         Bold = nil;
         Italic = nil;
         Underline = nil;
      }

      public CombinedDecorations With(Decoration decoration, int index, int length)
      {
         var newDecoration = new CombinedDecorations(index, length)
         {
            TextColor = TextColor,
            TextFont = TextFont,
            Bold = Bold,
            Italic = Italic,
            Underline = Underline
         };

         switch (decoration)
         {
            case TextColor textColor:
               newDecoration.TextColor = textColor;
               break;
            case TextFont textFont:
               newDecoration.TextFont = textFont;
               break;
            case Bold bold:
               newDecoration.Bold = bold;
               break;
            case Italic italic:
               newDecoration.Italic = italic;
               break;
            case Underline underline:
               newDecoration.Underline = underline;
               break;
         }

         return newDecoration;
      }

      public int Index { get; }

      public int Length { get; }

      public int NextIndex => Index + Length;

      public Maybe<TextColor> TextColor { get; set; }

      public Maybe<TextFont> TextFont { get; set; }

      public Maybe<Bold> Bold { get; set; }

      public Maybe<Italic> Italic { get; set; }

      public Maybe<Underline> Underline { get; set; }

      public void Format(RichTextBox richTextBox)
      {
         richTextBox.Select(Index, Length);

         if (TextColor.If(out var textColor))
         {
            richTextBox.SelectionColor = textColor.Color;
         }

         if (TextFont.If(out var textFont))
         {
            Bits32<FontStyle> fontStyle = FontStyle.Regular;
            fontStyle[FontStyle.Bold] = Bold.Map(b => b.Enabled).DefaultTo(() => false);
            fontStyle[FontStyle.Italic] = Italic.Map(i => i.Enabled).DefaultTo(() => false);
            fontStyle[FontStyle.Underline] = Underline.Map(u => u.Enabled).DefaultTo(() => false);

            richTextBox.SelectionFont = new Font(textFont.FontName, textFont.FontSize, fontStyle);
         }
      }
   }
}