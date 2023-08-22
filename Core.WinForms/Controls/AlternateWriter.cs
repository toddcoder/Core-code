using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class AlternateWriter
{
   protected UiAction uiAction;
   protected string[] alternates;
   protected bool autoSizeText;
   protected Maybe<int> _floor;
   protected Maybe<int> _ceiling;
   protected int selectedIndex;
   protected Maybe<int> _disabledIndex;
   protected Lazy<Font> disabledFont;

   public AlternateWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling)
   {
      this.uiAction = uiAction;
      this.alternates = alternates;
      this.autoSizeText = autoSizeText;
      this._floor = _floor;
      this._ceiling = _ceiling;

      selectedIndex = 0;
      _disabledIndex = nil;
      disabledFont = new Lazy<Font>(() => new Font(uiAction.Font, FontStyle.Italic));
   }

   protected (Rectangle indicatorRectangle, Rectangle textRectangle) splitRectangle(Rectangle rectangle)
   {
      var height = rectangle.Height;

      var indicatorRectangle = rectangle.Reposition(2, 2);
      indicatorRectangle.Width = height - 4;
      indicatorRectangle.Height = height - 4;

      var textRectangle = rectangle;
      textRectangle.X += height;
      textRectangle.Width -= height;

      return (indicatorRectangle, textRectangle);
   }

   public int SelectedIndex
   {
      get => selectedIndex;
      set
      {
         if (_disabledIndex is (true, var disabledIndex) && disabledIndex != value || !_disabledIndex)
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
      get => _disabledIndex | -1;
      set
      {
         if (value < 0)
         {
            _disabledIndex = 0;
         }
         else if (value >= alternates.Length)
         {
            _disabledIndex = alternates.Length - 1;
         }
         else
         {
            _disabledIndex = value;
         }
      }
   }

   public string Alternate => alternates[selectedIndex];

   public string[] Alternates => alternates;

   protected void fillRectangle(Graphics g, Rectangle rectangle, Color color)
   {
      using var brush = new SolidBrush(color);
      g.FillRectangle(brush, rectangle);
   }

   protected void drawSelected(Graphics g, Rectangle rectangle)
   {
      using var pen = new Pen(Color.Black, 2);
      drawUnselected(g, pen, rectangle);
      pen.StartCap = LineCap.Triangle;
      pen.EndCap = LineCap.Triangle;
      g.DrawLine(pen, rectangle.NorthWest(4), rectangle.SouthEast(4));
      g.DrawLine(pen, rectangle.NorthEast(4), rectangle.SouthWest(4));
   }

   protected void drawUnselected(Graphics g, Pen pen, Rectangle rectangle)
   {
      using var brush = new SolidBrush(Color.White);
      g.FillRectangle(brush, rectangle);
      g.DrawRectangle(pen, rectangle);
   }

   protected void drawUnselected(Graphics g, Rectangle rectangle)
   {
      using var pen = new Pen(Color.Black, 2);
      drawUnselected(g, pen, rectangle);
   }

   public void OnPaint(Graphics g)
   {
      var writer = new UiActionWriter(CardinalAlignment.Center, autoSizeText, _floor, _ceiling, UiActionButtonType.Normal);
      var disabledIndex = _disabledIndex | -1;
      foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         var (indicatorRectangle, textRectangle) = splitRectangle(rectangle);
         if (index == disabledIndex)
         {
            using var disabledBrush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Black, Color.Gold);
            g.FillRectangle(disabledBrush, rectangle);
            var filledRectangle = writer.TextRectangle(alternates[index], g, rectangle);
            g.FillRectangle(Brushes.Gold, filledRectangle);
            g.DrawRectangle(Pens.Black, filledRectangle);
            writer.Rectangle = rectangle;
            writer.Font = disabledFont.Value;
            writer.Color = Color.Black;
         }
         else
         {
            writer.Font = uiAction.Font;
            var color = index == selectedIndex ? Color.Teal : Color.Wheat;
            writer.Color = index == selectedIndex ? Color.White : Color.Black;
            fillRectangle(g, textRectangle, color);
            if (index == selectedIndex)
            {
               drawSelected(g, indicatorRectangle);
            }
            else
            {
               drawUnselected(g, indicatorRectangle);
            }

            writer.Rectangle = textRectangle;
         }

         writer.Write(g, alternates[index]);
      }
   }

   public void OnPaintBackground(Graphics g)
   {
      fillRectangle(g, uiAction.ClientRectangle, Color.AliceBlue);
   }
}