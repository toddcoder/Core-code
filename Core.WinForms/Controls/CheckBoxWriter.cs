using System.Drawing;
using Core.Monads;

namespace Core.WinForms.Controls;

public class CheckBoxWriter : AlternateWriter
{
   public CheckBoxWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling) :
      base(uiAction, alternates, autoSizeText, _floor, _ceiling, false)
   {
   }

   public bool BoxChecked
   {
      get => selectedIndex != -1;
      set => selectedIndex = value ? 0 : -1;
   }

   protected override void drawSelected(Graphics g, Rectangle rectangle, Color foreColor, Color backColor, int penSize)
   {
      g.HighQuality();
      penSize = 2;
      using var pen = new Pen(foreColor, penSize);
      drawUnselected(g, pen, rectangle, backColor);
      using var greenPen = new Pen(Color.Green, penSize);
      g.DrawLine(greenPen, rectangle.West(penSize), rectangle.South(penSize));
      g.DrawLine(greenPen, rectangle.South(penSize), rectangle.NorthEast(penSize));
   }
}