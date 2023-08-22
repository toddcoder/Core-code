using System.Drawing;

namespace Core.WinForms.Controls;

public class UiActionAlternateArgs : UiActionRectangleArgs
{
   public UiActionAlternateArgs(int rectangleIndex, Point location, string alternate) : base(rectangleIndex, location)
   {
      Alternate = alternate;
   }

   public string Alternate { get; }
}