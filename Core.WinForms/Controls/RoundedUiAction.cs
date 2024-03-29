﻿using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public class RoundedUiAction : UiAction
{
   public int CornerRadius { get; set; }

   public RoundedUiAction(Control control, bool center = false) : base(control, false)
   {
      CornerRadius = 8;
   }

   protected override void drawRectangle(Graphics graphics, Pen pen, Rectangle rectangle)
   {
      using var path = rectangle.Rounded(CornerRadius);
      graphics.HighQuality();
      graphics.DrawPath(pen, path);
   }

   protected override void fillRectangle(Graphics graphics, Brush brush, Rectangle rectangle)
   {
      using var path = rectangle.Rounded(CornerRadius);
      graphics.HighQuality();
      graphics.FillPath(brush, path);
   }
}