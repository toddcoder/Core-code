using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
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
   protected StringHash<Color> foreColors;
   protected StringHash<Color> backColors;

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
      foreColors = new StringHash<Color>(true);
      backColors = new StringHash<Color>(true);
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

   public void SetForeColor(string text, Color color) => foreColors[text] = color;

   public Maybe<Color> GetForeColor(string text) => foreColors.Maybe[text];

   public void SetForeColor(int index, Color color)
   {
      if (index.Between(0).Until(alternates.Length))
      {
         SetForeColor(alternates[index], color);
      }
   }

   public Maybe<Color> GetForeColor(int index)
   {
      return maybe<Color>() & index.Between(0).Until(alternates.Length) & (() => foreColors.Maybe[alternates[index]]);
   }

   public void SetBackColor(string text, Color color) => backColors[text] = color;

   public Maybe<Color> GetBackColor(string text) => backColors.Maybe[text];

   public void SetBackColor(int index, Color color)
   {
      if (index.Between(0).Until(alternates.Length))
      {
         SetBackColor(alternates[index], color);
      }
   }

   public Maybe<Color> GetBackColor(int index)
   {
      return maybe<Color>() & index.Between(0).Until(alternates.Length) & (() => backColors.Maybe[alternates[index]]);
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

   protected void drawSelected(Graphics g, Rectangle rectangle, Color foreColor, Color backColor)
   {
      using var pen = new Pen(foreColor, 2);
      drawUnselected(g, pen, rectangle, foreColor, backColor);
      pen.StartCap = LineCap.Triangle;
      pen.EndCap = LineCap.Triangle;
      g.DrawLine(pen, rectangle.NorthWest(4), rectangle.SouthEast(4));
      g.DrawLine(pen, rectangle.NorthEast(4), rectangle.SouthWest(4));
   }

   protected void drawUnselected(Graphics g, Pen pen, Rectangle rectangle, Color foreColor, Color backColor)
   {
      using var brush = new SolidBrush(backColor);
      g.FillRectangle(brush, rectangle);
      g.DrawRectangle(pen, rectangle);
   }

   protected void drawUnselected(Graphics g, Rectangle rectangle, Color foreColor, Color backColor)
   {
      using var pen = new Pen(foreColor, 2);
      drawUnselected(g, pen, rectangle, foreColor, backColor);
   }

   public void OnPaint(Graphics g)
   {
      var writer = new UiActionWriter(CardinalAlignment.Center, autoSizeText, _floor, _ceiling, UiActionButtonType.Normal);
      var disabledIndex = _disabledIndex | -1;
      foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         var (indicatorRectangle, textRectangle) = splitRectangle(rectangle);
         var alternate = alternates[index];

         if (index == disabledIndex)
         {
            var foreColor = foreColors.Maybe[alternate] | Color.Black;
            var backColor = backColors.Maybe[alternate] | Color.LightGray;
            using var brush = new SolidBrush(backColor);
            g.FillRectangle(brush, textRectangle);
            writer.Rectangle = textRectangle;
            writer.Font = disabledFont.Value;
            writer.Color = foreColor;
            if (index == selectedIndex)
            {
               drawSelected(g, indicatorRectangle, Color.Black, Color.LightGray);
            }
            else
            {
               drawUnselected(g, indicatorRectangle, Color.Black, Color.LightGray);
            }
         }
         else
         {
            writer.Font = uiAction.Font;

            var _foreColor = foreColors.Maybe[alternate];
            writer.Color = _foreColor | (() => index == selectedIndex ? Color.White : Color.Black);

            var _backColor = backColors.Maybe[alternate];
            var color = _backColor | (() => index == selectedIndex ? Color.Teal : Color.Wheat);
            fillRectangle(g, textRectangle, color);

            if (index == selectedIndex)
            {
               drawSelected(g, indicatorRectangle, Color.Black, Color.White);
            }
            else
            {
               drawUnselected(g, indicatorRectangle, Color.Black, Color.White);
            }

            writer.Rectangle = textRectangle;
         }

         writer.Write(g, alternate);
      }
   }

   public void OnPaintBackground(Graphics g)
   {
      fillRectangle(g, uiAction.ClientRectangle, Color.White);
   }
}