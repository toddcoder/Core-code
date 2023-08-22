using System.Drawing;
using System.Windows.Forms;
using Core.Monads;
using Core.Strings.Emojis;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Drawing;

public class AutoSizingWriter
{
   public static Rectangle NarrowRectangle(Rectangle rectangle, Maybe<int> _floor, Maybe<int> _ceiling)
   {
      if (_floor is (true, var floor and >= 0))
      {
         rectangle = rectangle with { X = floor, Width = rectangle.Width - floor };
      }

      if (_ceiling is (true, var ceiling))
      {
         rectangle = rectangle with { Width = ceiling - rectangle.X };
      }

      return rectangle;
   }

   public static (Size size, Font font) TextSize(Graphics g, string text, Font font, int containerWidth, int minimumSize, int maximumSize,
      TextFormatFlags flags)
   {
      var _adjustedFont = AdjustedFont(g, text, font, containerWidth, minimumSize, maximumSize, flags);
      var adjustedFont = _adjustedFont | (() => new Font(font.Name, minimumSize, font.Style));
      if (!_adjustedFont)
      {
         flags = flags | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
      }

      return (TextRenderer.MeasureText(g, text.EmojiSubstitutions(), adjustedFont, Size.Empty, flags), adjustedFont);
   }

   protected string text;
   protected Rectangle rectangle;
   protected Color foreColor;
   protected Font font;
   protected Maybe<Color> _backColor;
   protected int minimumSize;
   protected int maximumSize;
   protected TextFormatFlags flags;
   protected TextFormatFlags failFlags;

   public AutoSizingWriter(string text, Rectangle rectangle, Color foreColor, Font font, bool isFile)
   {
      this.text = text;
      this.rectangle = rectangle;
      this.foreColor = foreColor;
      this.font = font;

      _backColor = nil;

      minimumSize = 6;
      maximumSize = 12;

      flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
      failFlags = flags | (isFile ? TextFormatFlags.PathEllipsis : TextFormatFlags.EndEllipsis) | TextFormatFlags.NoPrefix;
   }

   public Maybe<Color> BackColor
   {
      get => _backColor;
      set => _backColor = value;
   }

   public int MinimumSize
   {
      get => minimumSize;
      set => minimumSize = value;
   }

   public int MaximumSize
   {
      get => maximumSize;
      set => maximumSize = value;
   }

   public TextFormatFlags Flags
   {
      get => flags;
      set => flags = value;
   }

   protected static Font getFont(Font originalFont, int fontSize) => new(originalFont.Name, fontSize, originalFont.Style);

   public static Maybe<Font> AdjustedFont(Graphics g, string text, Font originalFont, int containerWidth, int minimumSize, int maximumSize,
      TextFormatFlags flags)
   {
      for (var size = maximumSize; size >= minimumSize; size--)
      {
         var testFont = getFont(originalFont, size);
         var textWidth = TextRenderer.MeasureText(g, text, testFont, Size.Empty, flags).Width;

         if (containerWidth > textWidth)
         {
            return testFont;
         }
      }

      return nil;
   }

   public void Write(Graphics g)
   {
      g.HighQuality();

      var _adjustedFont = AdjustedFont(g, text, font, rectangle.Width, minimumSize, maximumSize, flags);
      if (_adjustedFont is (true, var adjustedFont))
      {
         if (_backColor is (true, var backColor))
         {
            using var brush = new SolidBrush(backColor);
            g.FillRectangle(brush, rectangle);
         }

         TextRenderer.DrawText(g, text, adjustedFont, rectangle, foreColor, flags);
      }
      else
      {
         using var smallestFont = getFont(font, minimumSize);
         TextRenderer.DrawText(g, text, smallestFont, rectangle, foreColor, failFlags);
      }
   }
}