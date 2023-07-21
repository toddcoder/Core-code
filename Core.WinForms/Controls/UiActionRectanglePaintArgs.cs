using System;
using System.Drawing;

namespace Core.WinForms.Controls;

public class UiActionRectanglePaintArgs : EventArgs
{
   public UiActionRectanglePaintArgs(Graphics graphics, int rectangleIndex)
   {
      Graphics = graphics;
      RectangleIndex = rectangleIndex;
   }

   public Graphics Graphics { get; }

   public int RectangleIndex { get; }
}