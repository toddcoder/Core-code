using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;

namespace Core.WinForms.Controls;

public class AlternateWriter
{
   protected UiAction uiAction;
   protected string[] alternates;
   protected bool autoSizeText;
   protected Maybe<int> _floor;
   protected Maybe<int> _ceiling;
   protected bool deletable;
   protected Rectangle[] deletableRectangles;
   protected int selectedIndex;
   protected int disabledIndex;
   protected Lazy<Font> disabledFont;
   protected Hash<int, Color> foreColors;
   protected Hash<int, Color> backColors;

   public AlternateWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling, bool deletable)
   {
      this.uiAction = uiAction;
      this.alternates = alternates;
      this.autoSizeText = autoSizeText;
      this._floor = _floor;
      this._ceiling = _ceiling;
      this.deletable = deletable;
      deletableRectangles = deletable ? Enumerable.Range(0, alternates.Length).Select(_ => Rectangle.Empty).ToArray() : Array.Empty<Rectangle>();

      selectedIndex = 0;
      disabledIndex = -1;
      disabledFont = new Lazy<Font>(() => new Font(uiAction.Font, FontStyle.Italic));
      foreColors = new Hash<int, Color>();
      backColors = new Hash<int, Color>();
   }

   protected static (Rectangle checkRectangle, int penSize, Rectangle textRectangle, Rectangle deletableRectangle) splitRectangle(Rectangle rectangle,
      bool deletable)
   {
      var penSize = rectangle.Height / 40;
      if (penSize <= 0)
      {
         penSize = 1;
      }

      if (deletable)
      {
         var textRectangle = rectangle with { Y = rectangle.Y + 10, Height = rectangle.Height - 20 };
         var deletableRectangle = new Rectangle(rectangle.NorthEast(10), new Size(8, 8))
         {
            Y = rectangle.Y + 2
         };
         return (Rectangle.Empty, penSize, textRectangle, deletableRectangle);
      }
      else
      {
         var checkSize = rectangle.Height / 3;
         var checkSizeWithMargin = checkSize + 8;
         var checkRectangle = rectangle with
         {
            X = rectangle.X + 4, Y = (rectangle.Height - checkSize) / 2 + 2, Width = checkSize, Height = checkSize
         };
         var textRectangle = rectangle with
         {
            X = rectangle.X + checkSizeWithMargin,
            Width = rectangle.Width - checkSizeWithMargin,
            Height = rectangle.Height
         };

         return (checkRectangle, penSize, textRectangle, Rectangle.Empty);
      }
   }

   public void SetForeColor(int index, Color color) => foreColors[index] = color;

   public Maybe<Color> GetForeColor(int index) => foreColors.Maybe[index];

   public void SetBackColor(int index, Color color) => backColors[index] = color;

   public Maybe<Color> GetBackColor(int index) => backColors.Maybe[index];

   public void SetColors(int index, UiActionType type)
   {
      SetForeColor(index, uiAction.GetForeColor(type));
      SetBackColor(index, uiAction.GetBackColor(type));
   }

   public int SelectedIndex
   {
      get => selectedIndex;
      set
      {
         if (disabledIndex != value)
         {
            if (value < 0)
            {
               selectedIndex = 0;
            }
            else if (value >= alternates.Length)
            {
               selectedIndex = alternates.Length - 1;
            }
            else
            {
               selectedIndex = value;
            }
         }
      }
   }

   public int DisabledIndex
   {
      get => disabledIndex;
      set
      {
         if (value < 0 || value >= alternates.Length)
         {
            disabledIndex = -1;
         }
         else
         {
            disabledIndex = value;
         }
      }
   }

   public string Alternate => alternates[selectedIndex];

   public string[] Alternates => alternates;

   public Rectangle[] DeletableRectangles => deletableRectangles;

   protected void fillRectangle(Graphics g, Rectangle rectangle, Color color)
   {
      using var brush = new SolidBrush(color);
      g.FillRectangle(brush, rectangle);
   }

   protected virtual void drawSelected(Graphics g, Rectangle rectangle, Color foreColor, Color backColor, int penSize)
   {
      using var pen = new Pen(foreColor, penSize);
      drawUnselected(g, pen, rectangle, backColor);
      pen.StartCap = LineCap.Triangle;
      pen.EndCap = LineCap.Triangle;
      g.DrawLine(pen, rectangle.NorthWest(penSize), rectangle.SouthEast(penSize));
      g.DrawLine(pen, rectangle.NorthEast(penSize), rectangle.SouthWest(penSize));
   }

   protected virtual void drawUnselected(Graphics g, Pen pen, Rectangle rectangle, Color backColor)
   {
      using var brush = new SolidBrush(backColor);
      g.FillRectangle(brush, rectangle);
      g.DrawRectangle(pen, rectangle);
   }

   protected void drawUnselected(Graphics g, Rectangle rectangle, Color foreColor, Color backColor, int penSize)
   {
      using var pen = new Pen(foreColor, penSize);
      drawUnselected(g, pen, rectangle, backColor);
   }

   public void drawDeletable(Graphics g, Rectangle rectangle, Color foreColor, bool enabled)
   {
      if (enabled && deletable)
      {
         using var pen = new Pen(foreColor, 1);
         pen.StartCap = LineCap.Triangle;
         pen.EndCap = LineCap.Triangle;
         g.DrawLine(pen, rectangle.NorthWest(1), rectangle.SouthEast(1));
         g.DrawLine(pen, rectangle.NorthEast(1), rectangle.SouthWest(1));
      }
   }

   public Color GetAlternateForeColor(int index)
   {
      var _foreColor = foreColors.Maybe[index];
      if (index == disabledIndex)
      {
         return Color.Black;
      }
      else if (index == selectedIndex)
      {
         return _foreColor | Color.White;
      }
      else
      {
         return _foreColor | Color.Black;
      }
   }

   public Color GetAlternateBackColor(int index)
   {
      var _backColor = backColors.Maybe[index];
      if (index == disabledIndex)
      {
         return Color.LightGray;
      }
      else if (index == selectedIndex)
      {
         return _backColor | Color.Teal;
      }
      else
      {
         return _backColor | Color.Wheat;
      }
   }

   public void OnPaint(Graphics g)
   {
      var writer = new UiActionWriter(CardinalAlignment.Center, autoSizeText, _floor, _ceiling, UiActionButtonType.Normal);
      foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         var (checkRectangle, penSize, textRectangle, deletableRectangle) = splitRectangle(rectangle, deletable);
         deletableRectangles[index] = deletableRectangle;

         using var whitePen = new Pen(Color.White);
         g.DrawRectangle(whitePen, textRectangle);
         var alternate = alternates[index];

         if (deletable)
         {
            if (deletableRectangles.Length > 0)
            {
               writer.Font = uiAction.Font;
               var foreColor = GetAlternateForeColor(index);
               writer.Color = foreColor;
               var backColor = GetAlternateBackColor(index);
               fillRectangle(g, rectangle, backColor);

               writer.Rectangle = textRectangle;
               writer.Write(g, alternate);

               drawDeletable(g, deletableRectangle, foreColor, uiAction.Enabled);
            }
            else
            {
               writer.Font = disabledFont.Value;
               var foreColor = Color.Black;
               var backColor = Color.LightGray;
               fillRectangle(g, rectangle, backColor);

               if (uiAction.EmptyTextTitle is (true, var emptyTextTitle))
               {
                  writer.Color = foreColor;
                  writer.Rectangle = textRectangle;
                  writer.Write(g, emptyTextTitle);
               }
            }
         }
         else if (index == disabledIndex)
         {
            var foreColor = GetAlternateForeColor(index);
            var backColor = GetAlternateBackColor(index);
            using var brush = new SolidBrush(backColor);
            g.FillRectangle(brush, rectangle);
            writer.Rectangle = textRectangle;
            writer.Font = disabledFont.Value;
            writer.Color = foreColor;
            if (index == selectedIndex)
            {
               drawSelected(g, checkRectangle, Color.Black, Color.LightGray, penSize);
            }
            else
            {
               drawUnselected(g, checkRectangle, Color.Black, Color.LightGray, penSize);
            }
         }
         else
         {
            writer.Font = uiAction.Font;
            writer.Color = GetAlternateForeColor(index);
            var backColor = GetAlternateBackColor(index);
            fillRectangle(g, rectangle, backColor);

            if (index == selectedIndex)
            {
               drawSelected(g, checkRectangle, Color.Black, Color.White, penSize);
            }
            else
            {
               drawUnselected(g, checkRectangle, Color.Black, Color.White, penSize);
            }

            drawDeletable(g, deletableRectangle, Color.Black, uiAction.Enabled);

            writer.Rectangle = textRectangle;
         }

         writer.Write(g, alternate);
      }
   }
}