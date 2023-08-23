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
   protected Hash<int, Color> foreColors;
   protected Hash<int, Color> backColors;

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
      foreColors = new Hash<int, Color>();
      backColors = new Hash<int, Color>();
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

   public Color GetAlternateForeColor(int index)
   {
      var _foreColor = foreColors.Maybe[index];
      if (_disabledIndex is (true, var disabledIndex))
      {
         if (index == disabledIndex)
         {
            return _foreColor | Color.Black;
         }
         else if (index == selectedIndex)
         {
            return _foreColor | Color.Black;
         }
         else
         {
            return _foreColor | Color.LightGray;
         }
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
      if (_disabledIndex is (true, var disabledIndex))
      {
         if (index == disabledIndex)
         {
            return _backColor | Color.LightGray;
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
      var disabledIndex = _disabledIndex | -1;
      foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         var (indicatorRectangle, textRectangle) = splitRectangle(rectangle);
         var alternate = alternates[index];

         if (index == disabledIndex)
         {
            var foreColor = GetAlternateForeColor(index);
            var backColor = GetAlternateBackColor(index);
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

            writer.Color = GetAlternateForeColor(index);

            var backColor = GetAlternateBackColor(index);
            fillRectangle(g, textRectangle, backColor);

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