using System.Drawing;

namespace Core.WinForms.Controls;

public class UiActionAlternateArgs : UiActionRectangleArgs
{
   public UiActionAlternateArgs(int rectangleIndex, Point location, string alternate, bool fromClick) : base(rectangleIndex, location)
   {
      Alternate = alternate;
      FromClick = fromClick;
   }

   public string Alternate { get; }

   public bool FromClick { get; }
}