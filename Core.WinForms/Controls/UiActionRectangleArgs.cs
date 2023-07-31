using System;
using System.Drawing;

namespace Core.WinForms.Controls;

public class UiActionRectangleArgs : EventArgs
{
   public UiActionRectangleArgs(int rectangleIndex, Point location)
   {
      RectangleIndex = rectangleIndex;
      Location = location;
   }

   public int RectangleIndex { get; }

   public Point Location { get; }
}