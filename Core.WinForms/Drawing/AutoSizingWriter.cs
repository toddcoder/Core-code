using System.Drawing;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Core.Monads.Monads;

namespace Core.WinForms.Drawing;

public class AutoSizingWriter
{
   protected string text;
   protected Rectangle rectangle;
   protected Color foreColor;
   protected Font font;
   protected Maybe<Color> _backColor;
   protected int minimumSize;
   protected int maximumSize;
   protected bool smallestOnFail;
   protected TextFormatFlags flags;

   public AutoSizingWriter(string text, Rectangle rectangle, Color foreColor, Font font)
   {
      this.text = text;
      this.rectangle = rectangle;
      this.foreColor = foreColor;
      this.font = font;

      _backColor = nil;

      minimumSize = 8;
      maximumSize = 12;
      smallestOnFail = true;

      flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
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

   public bool SmallestOnFail
   {
      get => smallestOnFail;
      set => smallestOnFail = value;
   }

   public TextFormatFlags Flags
   {
      get => flags;
      set => flags = value;
   }

   public static Maybe<Font> AdjustedFont(Graphics g, string text, Font originalFont, int containerWidth, int minimumSize, int maximumSize,
      bool smallestOnFail, TextFormatFlags flags)
   {
      var _testFont = monads.maybe<Font>();
      for (var size = maximumSize; size >= minimumSize; size--)
      {
         var testFont = new Font(originalFont.Name, size, originalFont.Style);
         _testFont = testFont;

         var textWidth = TextRenderer.MeasureText(g, text, testFont, Size.Empty, flags).Width;

         if (containerWidth > textWidth)
         {
            return testFont;
         }
      }

      if (smallestOnFail && _testFont is (true, var font))
      {
         return font;
      }
      else
      {
         return originalFont;
      }
   }

   public void Write(Graphics g)
   {
      g.HighQuality();

      var _adjustedFont = AdjustedFont(g, text, font, rectangle.Width, minimumSize, maximumSize, smallestOnFail, flags);
      if (_adjustedFont is (true, var adjustedFont))
      {
         if (_backColor is (true, var backColor))
         {
            using var brush = new SolidBrush(backColor);
            g.FillRectangle(brush, rectangle);
         }

         TextRenderer.DrawText(g, text, adjustedFont, rectangle, foreColor, flags);
      }
   }
}