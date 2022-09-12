using System.Drawing;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class LabelProcessor
{
   protected const int LABEL_MARGIN = 8;
   protected string label;
   protected Maybe<int> _labelWidth;
   protected Font font;
   protected Maybe<string> _emptyTextTitle;
   protected Maybe<Rectangle> _labelRectangle;

   public LabelProcessor(string label, Maybe<int> _labelWidth, Font font, Maybe<string> _emptyTextTitle)
   {
      this.label = label;
      this._labelWidth = _labelWidth;
      this.font = font;
      this._emptyTextTitle = _emptyTextTitle;

      _labelRectangle = nil;
   }

   public Rectangle LabelRectangle(Graphics graphics, Rectangle clientRectangle)
   {
      if (!_labelRectangle)
      {
         _labelRectangle = getLabelRectangle(graphics, clientRectangle);
      }

      return _labelRectangle;
   }

   protected Rectangle getLabelRectangle(Graphics graphics, Rectangle clientRectangle)
   {
      if (_labelWidth.Map(out var labelWidth))
      {
         var rectangle = clientRectangle with { Width = labelWidth + LABEL_MARGIN };
         _labelRectangle = rectangle;
         return rectangle;
      }
      else
      {
         var rectangle = clientRectangle with { Width = TextRenderer.MeasureText(graphics, label, font).Width + LABEL_MARGIN };
         _labelRectangle = rectangle;
         return rectangle;
      }
   }

   public void OnPaintBackground(Graphics graphics)
   {
      if (_labelRectangle)
      {
         using var labelBrush = new SolidBrush(Color.CadetBlue);
         graphics.FillRectangle(labelBrush, _labelRectangle);
      }
   }

   public void OnPaint(Graphics graphics)
   {
      if (_labelRectangle)
      {
         using var labelFont = new Font(font, FontStyle.Bold);
         var writer = new UiActionWriter(false, CheckStyle.None, _emptyTextTitle)
         {
            Rectangle = _labelRectangle,
            Font = labelFont,
            Color = Color.White
         };
         writer.Write(label, graphics);
      }
   }
}