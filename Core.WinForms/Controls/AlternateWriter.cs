﻿using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Enumerables;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class AlternateWriter
{
   protected UiAction uiAction;
   protected string[] alternates;
   protected int selectedIndex;

   public AlternateWriter(UiAction uiAction, string[] alternates)
   {
      this.uiAction = uiAction;
      this.alternates = alternates;

      selectedIndex = 0;
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

   public string Alternate => alternates[selectedIndex];

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
      var writer = new UiActionWriter(CardinalAlignment.Center, true, nil, nil, UiActionButtonType.Normal)
      {
         Font = uiAction.Font
      };
      foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         var (indicatorRectangle, textRectangle) = splitRectangle(rectangle);
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
         writer.Write(g, alternates[index]);
      }
   }

   public void OnPaintBackground(Graphics g)
   {
      fillRectangle(g, uiAction.ClientRectangle, Color.AliceBlue);
   }
}