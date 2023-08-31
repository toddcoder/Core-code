﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
   protected int selectedIndex;
   protected int disabledIndex;
   protected Lazy<Font> disabledFont;
   protected Hash<int, Color> foreColors;
   protected Hash<int, Color> backColors;
   protected int checkSize;
   protected int checkMargin;

   public AlternateWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling)
   {
      this.uiAction = uiAction;
      this.alternates = alternates;
      this.autoSizeText = autoSizeText;
      this._floor = _floor;
      this._ceiling = _ceiling;

      selectedIndex = 0;
      disabledIndex = -1;
      disabledFont = new Lazy<Font>(() => new Font(uiAction.Font, FontStyle.Italic));
      foreColors = new Hash<int, Color>();
      backColors = new Hash<int, Color>();
      checkSize = 3;
      checkMargin = 4;
   }

   protected static (Rectangle checkRectangle, int penSize, Rectangle textRectangle) splitRectangle(Rectangle rectangle)
   {
      var checkSize = rectangle.Height / 3;
      var checkSizeWithMargin = checkSize + 8;
      var checkRectangle = rectangle with { X = rectangle.X + 4, Y = (rectangle.Height - checkSize) / 2 + 2, Width = checkSize, Height = checkSize };
      var textRectangle = rectangle with
      {
         X = rectangle.X + checkSizeWithMargin, Width = rectangle.Width - checkSizeWithMargin, Height = rectangle.Height
      };

      var penSize = rectangle.Height / 40;

      return (checkRectangle, penSize, textRectangle);
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

   protected void fillRectangle(Graphics g, Rectangle rectangle, Color color)
   {
      using var brush = new SolidBrush(color);
      g.FillRectangle(brush, rectangle);
   }

   protected void drawSelected(Graphics g, Rectangle rectangle, Color foreColor, Color backColor, int penSize)
   {
      using var pen = new Pen(foreColor, penSize);
      drawUnselected(g, pen, rectangle, backColor);
      pen.StartCap = LineCap.Triangle;
      pen.EndCap = LineCap.Triangle;
      g.DrawLine(pen, rectangle.NorthWest(checkMargin), rectangle.SouthEast(checkMargin));
      g.DrawLine(pen, rectangle.NorthEast(checkMargin), rectangle.SouthWest(checkMargin));
   }

   protected void drawUnselected(Graphics g, Pen pen, Rectangle rectangle, Color backColor)
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
         var (checkRectangle, penSize, textRectangle) = splitRectangle(rectangle);

         using var whitePen = new Pen(Color.White);
         g.DrawRectangle(whitePen, textRectangle);
         var alternate = alternates[index];

         if (index == disabledIndex)
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

            writer.Rectangle = textRectangle;
         }

         writer.Write(g, alternate);
      }
   }

   public void OnPaintBackground(Graphics g)
   {
      /*foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         var backColor = GetAlternateBackColor(index);
         g.FillRectangle(backColor, rectangle);
      }*/
   }
}