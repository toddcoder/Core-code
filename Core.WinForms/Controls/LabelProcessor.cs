using System.Drawing;
using System.Windows.Forms;
using Core.Monads;

namespace Core.WinForms.Controls;

public class LabelProcessor
{
   protected const int LABEL_MARGIN = 8;
   protected string label;
   protected Maybe<int> _labelWidth;
   protected Font font;
   protected Maybe<string> _emptyTextTitle;
   protected Rectangle labelRectangle;

   public LabelProcessor(string label, Maybe<int> _labelWidth, Font font, Maybe<string> _emptyTextTitle, Graphics graphics, Rectangle clientRectangle)
   {
      this.label = label;
      this._labelWidth = _labelWidth;
      this.font = font;
      this._emptyTextTitle = _emptyTextTitle;

      labelRectangle = getLabelRectangle(graphics, clientRectangle);
   }

   public Rectangle Rectangle => labelRectangle;

   public static Maybe<int> LabelWidth(Maybe<string> _label, Font font)
   {
      return _label.Map(label => TextRenderer.MeasureText(label, font).Width + LABEL_MARGIN);
   }

   protected Rectangle getLabelRectangle(Graphics graphics, Rectangle clientRectangle)
   {
      if (_labelWidth)
      {
         var rectangle = clientRectangle with { Width = _labelWidth + LABEL_MARGIN };
         return rectangle;
      }
      else
      {
         var rectangle = clientRectangle with { Width = TextRenderer.MeasureText(graphics, label, font).Width + LABEL_MARGIN };
         return rectangle;
      }
   }

   public void OnPaintBackground(Graphics graphics)
   {
      using var labelBrush = new SolidBrush(Color.CadetBlue);
      graphics.FillRectangle(labelBrush, labelRectangle);
   }

   public void OnPaint(Graphics graphics)
   {
      using var labelFont = new Font(font, FontStyle.Bold);
      var writer = new UiActionWriter(CardinalAlignment.West)
      {
         Rectangle = labelRectangle,
         Font = labelFont,
         Color = Color.White,
         EmptyTextTitle = _emptyTextTitle
      };
      writer.Write(label, graphics);
   }
}