using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Applications;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ExTextBox : TextBox
{
   public class WindowExtender : NativeWindow
   {
      protected const int WM_PAINT = 15;

      protected ExTextBox baseControl;
      protected Bitmap canvas;
      protected Graphics bufferGraphics;
      protected Rectangle bufferClip;
      protected Graphics controlGraphics;
      protected bool canRender;

      public WindowExtender(ExTextBox baseControl)
      {
         this.baseControl = baseControl;

         canRender = false;
         ReinitializeCanvas();
      }

      protected override void WndProc(ref Message message)
      {
         if (message.Msg == WM_PAINT)
         {
            baseControl.Invalidate();
            base.WndProc(ref message);
            onPerformPaint();
         }
         else
         {
            base.WndProc(ref message);
         }
      }

      protected void onPerformPaint()
      {
         if (canRender)
         {
            bufferGraphics.Clear(Color.Transparent);
            baseControl.OnPaint(new PaintEventArgs(bufferGraphics, bufferClip));
            controlGraphics.DrawImageUnscaled(canvas, 0, 0);
         }
      }

      public void ReinitializeCanvas()
      {
         lock (this)
         {
            TearDown();
            canRender = baseControl.Width > 0 && baseControl.Height > 0;

            if (canRender)
            {
               canvas = new Bitmap(baseControl.Width, baseControl.Height);
               bufferGraphics = Graphics.FromImage(canvas);
               bufferClip = new Rectangle(0, 0, canvas.Width, canvas.Height);
               bufferGraphics.Clip = new Region(bufferClip);
               controlGraphics = Graphics.FromHwnd(baseControl.Handle);
            }
         }
      }

      public void TearDown()
      {
         controlGraphics?.Dispose();
         bufferGraphics?.Dispose();
         canvas?.Dispose();
      }
   }

   protected WindowExtender windowExtender;
   protected int updatingCount;

   public new event EventHandler<PaintEventArgs> Paint;

   public ExTextBox(Control control) : this()
   {
      control.Controls.Add(this);
   }

   public ExTextBox()
   {
      windowExtender = new WindowExtender(this);
      windowExtender.AssignHandle(Handle);
   }

   public bool RefreshOnTextChange { get; set; }

   public void StopUpdating()
   {
      if (updatingCount == 0)
      {
         User32.SendMessage(Handle, User32.Messages.SetRedraw, false, 0);
      }

      updatingCount++;
   }

   public void ResumeUpdating()
   {
      if (--updatingCount == 0)
      {
         User32.SendMessage(Handle, User32.Messages.SetRedraw, true, 0);
      }
   }

   public void ResetUpdating() => updatingCount = 0;

   public (int start, int length) Selection
   {
      get => (SelectionStart, SelectionLength);
      set => Select(value.start, value.length);
   }

   protected override void Dispose(bool disposing)
   {
      windowExtender.ReleaseHandle();
      windowExtender.TearDown();

      base.Dispose(disposing);
   }

   protected override void OnPaint(PaintEventArgs e) => Paint?.Invoke(this, e);

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);

      windowExtender.ReinitializeCanvas();
   }

   public void ReassignHandle() => windowExtender.AssignHandle(Handle);

   public static Size MeasureString(Graphics graphics, string text, Font font)
   {
      if (text.IsEmpty())
      {
         return new Size(0, 0);
      }
      else if (text.Contains(" "))
      {
         var size = graphics.MeasureString(text, font).ToSize();
         return size with { Width = size.Width + 8 };
      }
      else
      {
         var size = graphics.MeasureString(text, font);
         var ranges = new[] { new CharacterRange(0, text.Length) };
         var format = new StringFormat();
         format.SetMeasurableCharacterRanges(ranges);
         var regions = graphics.MeasureCharacterRanges(text, font, new RectangleF(0, 0, size.Width, size.Height), format);
         var rectangle = regions[0].GetBounds(graphics);

         return rectangle.Size.ToSize();
      }
   }

   public Rectangle RectangleFrom(Graphics graphics, int start, int length, bool expand)
   {
      var text = Text.Drop(start).Keep(length);
      var size = MeasureString(graphics, text, Font);
      if (expand)
      {
         size = size with { Width = Width };
      }

      var location = GetPositionFromCharIndex(start);
      if (expand)
      {
         location = new Point(0, location.X);
      }

      return new Rectangle(location, size);
   }

   public Rectangle RectangleFromCurrentSelection(Graphics graphics, bool expand)
   {
      return RectangleFrom(graphics, SelectionStart, SelectionLength, expand);
   }

   public IEnumerable<(Rectangle rectangle, string word)> RectangleWords(Graphics graphics)
   {
      if (Text.Matches("/w+; f").Map(out var result))
      {
         for (var i = 0; i < result.MatchCount; i++)
         {
            var (text, index, length) = result.GetMatch(i);
            var rectangle = RectangleFrom(graphics, index, length, false);
            yield return (rectangle, text);
         }
      }
   }

   public IEnumerable<(int start, int length)> Words()
   {
      if (Text.Matches("/w+; f").Map(out var result))
      {
         for (var i = 0; i < result.MatchCount; i++)
         {
            var (_, index, length) = result.GetMatch(i);
            yield return (index, length);
         }
      }
   }

   public IEnumerable<(char, Rectangle)> RectangleWhitespace(Graphics graphics)
   {
      if (Text.Matches("[' /t']; f").Map(out var result))
      {
         for (var i = 0; i < result.MatchCount; i++)
         {
            var (text, index, length) = result.GetMatch(i);
            var rectangle = RectangleFrom(graphics, index, length, false);
            yield return (text[0], rectangle);
         }
      }
   }

   public Maybe<Rectangle> WordAtSelection(Graphics graphics, int start, int length)
   {
      if (length == 0)
      {
         return WordAtSelection(graphics, start);
      }
      else if (Text.IsEmpty() || start >= Text.Length)
      {
         return nil;
      }
      else
      {
         var segment = Text.Drop(start).Keep(length);
         return segment.Matches("/w+; f").Map(result => RectangleFrom(graphics, result.Index + start, result.Length, false));
      }
   }

   public Maybe<Rectangle> WordAtSelection(Graphics graphics, int start)
   {
      var text = Text;
      if (text.IsEmpty() || start >= text.Length)
      {
         return nil;
      }
      else if (char.IsLetterOrDigit(text, start))
      {
         var i = start;
         for (; i > -1 && char.IsLetterOrDigit(text, i); i--)
         {
         }

         i++;
         return text.Drop(i).Matches("/w+; f").Map(result => RectangleFrom(graphics, result.Index + i, result.Length, false));
      }
      else
      {
         return nil;
      }
   }

   public Maybe<Rectangle> WordAtCurrentSelection(Graphics graphics)
   {
      return WordAtSelection(graphics, SelectionStart, SelectionLength);
   }

   public void DrawHighlight(Graphics graphics, Rectangle rectangle, Color foreColor, Color backColor, DashStyle dashStyle = DashStyle.Solid)
   {
      using var brush = new SolidBrush(Color.FromArgb(30, backColor));
      graphics.FillRectangle(brush, rectangle);

      using var pen = new Pen(foreColor) { DashStyle = dashStyle };
      graphics.DrawRectangle(pen, rectangle);
   }

   public void DrawWavyUnderline(Graphics graphics, Rectangle rectangle, Color color)
   {
      using var brush = new HatchBrush(HatchStyle.ZigZag, color, Color.Transparent);
      using var pen = new Pen(brush, 2f);
      graphics.DrawLine(pen, rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Bottom);
   }

   public void DrawWhitespace(Graphics graphics)
   {
      var brush = Brushes.Gray;
      var format = new StringFormat
      {
         LineAlignment = StringAlignment.Center,
         Alignment = StringAlignment.Center
      };
      foreach (var (ch, rectangle) in RectangleWhitespace(graphics))
      {
         graphics.DrawString(ch == '\t' ? "→" : "°", Font, brush, rectangle, format);
      }
   }

   public IEnumerable<Rectangle> RectanglesFromSelection(Graphics graphics, int start, int length)
   {
      if (Text.IsNotEmpty())
      {
         var text = Text.Drop(start).Keep(length);
         if (text.IsEmpty())
         {
            yield break;
         }

         if (text.Contains("\n"))
         {
            var offset = 0;

            foreach (var i in text.FindAll("\n"))
            {
               var strLength = i - offset - 2;
               yield return RectangleFrom(graphics, start + offset, strLength, false);

               offset = i + 1;
            }

            yield return RectangleFrom(graphics, start + offset, text.Length - offset - 2, false);
         }
         else
         {
            yield return RectangleFrom(graphics, start, length, false);
         }
      }
   }

   protected override void OnTextChanged(EventArgs e)
   {
      if (RefreshOnTextChange)
      {
         Refresh();
      }

      base.OnTextChanged(e);
   }

   protected override void OnMouseClick(MouseEventArgs e)
   {
      base.OnMouseClick(e);

      if (RefreshOnTextChange)
      {
         Refresh();
      }
   }

   protected override void OnKeyPress(KeyPressEventArgs e)
   {
      base.OnKeyPress(e);

      if (RefreshOnTextChange)
      {
         Refresh();
      }
   }

   protected override void OnKeyDown(KeyEventArgs e)
   {
      base.OnKeyDown(e);

      if (RefreshOnTextChange)
      {
         Refresh();
      }
   }

   protected override void OnKeyUp(KeyEventArgs e)
   {
      base.OnKeyUp(e);

      if (RefreshOnTextChange)
      {
         Refresh();
      }
   }
}