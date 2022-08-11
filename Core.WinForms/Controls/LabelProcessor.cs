using System.Drawing;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class LabelProcessor
{
   protected string label;
   protected string text;
   protected bool line;
   protected Maybe<int> _labelWidth;
   protected Font font;
   protected Maybe<string> _emptyTextTitle;
   protected Maybe<Rectangle> _labelRectangle;
   protected Maybe<Rectangle> _textRectangle;

   public LabelProcessor(string label, string text, bool line, Maybe<int> _labelWidth, Font font, Maybe<string> _emptyTextTitle)
   {
      this.label = label;
      this.text = text;
      this.line = line;
      this._labelWidth = _labelWidth;
      this.font = font;
      this._emptyTextTitle = _emptyTextTitle;

      _labelRectangle = nil;
      _textRectangle = nil;
   }

   protected Rectangle getLabelRectangle(Graphics graphics, Rectangle clientRectangle)
   {
      if (_labelRectangle.Map(out var labelRectangle))
      {
         return labelRectangle;
      }
      else if (_labelWidth.Map(out var labelWidth))
      {
         var rectangle = clientRectangle with { Width = labelWidth + 20 };
         _labelRectangle = rectangle;
         return rectangle;
      }
      else
      {
         var rectangle = clientRectangle with { Width = TextRenderer.MeasureText(graphics, label, font).Width + 20 };
         _labelRectangle = rectangle;
         return rectangle;
      }
   }

   protected Rectangle getTextRectangle(Graphics graphics, Rectangle clientRectangle)
   {
      if (_textRectangle.Map(out var textRectangle))
      {
         return textRectangle;
      }
      else if (_labelWidth.Map(out var labelWidth))
      {
         var rectangle = clientRectangle with { X = labelWidth };
         _textRectangle = rectangle;
         return rectangle;
      }
      else
      {
         var labelRectangle = getLabelRectangle(graphics, clientRectangle);
         var rectangle = clientRectangle with { X = clientRectangle.X + labelRectangle.Width };
         return rectangle;
      }
   }

   public void OnPaintBackground(Graphics graphics, Rectangle clientRectangle)
   {
      if (line)
      {
         using var brush = new SolidBrush(Color.Gray);
         graphics.FillRectangle(brush, clientRectangle);
      }
      else
      {
         var labelRectangle = getLabelRectangle(graphics, clientRectangle);
         using var labelBrush = new SolidBrush(Color.DarkSlateGray);
         graphics.FillRectangle(labelBrush, labelRectangle);

         var textRectangle = getTextRectangle(graphics, clientRectangle);
         using var textBrush = new SolidBrush(Color.Gray);
         graphics.FillRectangle(textBrush, textRectangle);
      }
   }

   public void OnPaint(Graphics graphics, Rectangle clientRectangle)
   {
      var labelRectangle = getLabelRectangle(graphics, clientRectangle);
      using var labelFont = new Font(font, FontStyle.Bold);
      var writer = new UiActionWriter(true, CheckStyle.None, _emptyTextTitle)
      {
         Rectangle = labelRectangle,
         Font = labelFont,
         Color = Color.White
      };
      writer.Write(label, graphics);

      writer.Rectangle = getTextRectangle(graphics, clientRectangle);
      writer.Center(false);
      writer.Font = font;
      writer.Write(text, graphics);

      if (line)
      {
         using var pen = new Pen(Color.White);
         graphics.DrawLine(pen, labelRectangle.Width, 0, labelRectangle.Width, clientRectangle.Height);
      }
   }
}