using System;

namespace Core.WinForms.Controls;

public class UiActionRectangleArgs : EventArgs
{
   public UiActionRectangleArgs(int rectangleIndex)
   {
      RectangleIndex = rectangleIndex;
   }

   public int RectangleIndex { get; }
}