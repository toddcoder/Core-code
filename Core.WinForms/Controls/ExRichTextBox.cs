using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Core.Applications;
using Core.Assertions;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Arrays.ArrayFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ExRichTextBox : RichTextBox
{
   public class WindowExtender : NativeWindow
   {
      protected const int WM_PAINT = 15;

      protected ExRichTextBox baseControl;
      protected Bitmap canvas;
      protected Graphics bufferGraphics;
      protected Rectangle bufferClip;
      protected Graphics controlGraphics;
      protected bool canRender;

      public WindowExtender(ExRichTextBox baseControl)
      {
         this.baseControl = baseControl;

         canRender = false;
         ReinitializeCanvas();
      }

      protected override void WndProc(ref Message m)
      {
         if (m.Msg == WM_PAINT)
         {
            baseControl.Invalidate();
            base.WndProc(ref m);
            onPerformPaint();
         }
         else
         {
            base.WndProc(ref m);
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
               bufferClip = baseControl.ClientRectangle;
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

   public enum SelectionVisibility
   {
      Invisible,
      PartiallyVisible,
      Visible
   }

   protected const int ANNOTATION_ALPHA = 200;

   protected WindowExtender windowExtender;
   protected bool updateLocked;
   protected int updatingCount;
   protected int leftMargin;
   protected List<ModificationState> modificationStates;

   public new event EventHandler<PaintEventArgs> Paint;

   public event EventHandler<LineChangedEventArgs> LineChanged;

   public ExRichTextBox(Control control) : this()
   {
      control.Controls.Add(control);
   }

   public ExRichTextBox()
   {
      windowExtender = new WindowExtender(this);
      windowExtender.AssignHandle(Handle);

      SelectionTabs = array(32, 64, 96, 128, 160, 192, 224);
      ModifiedGlyphColor = Color.Gold;
      SavedGlyphColor = Color.Green;
      ModificationGlyphWidth = 4f;
      ModificationGlyphLeftMargin = 2;
      AnnotationFont = new Font("Calibri", 12f, FontStyle.Bold);
      leftMargin = 0;
      modificationStates = new List<ModificationState>();

      TextChanged += (_, _) =>
      {
         if (!ModificationLocked)
         {
            var lineCount = Lines.Length;
            if (lineCount > 0)
            {
               if (modificationStates.Count < lineCount)
               {
                  while (modificationStates.Count < CurrentLineNumber)
                  {
                     modificationStates.Add(ModificationState.Unmodified);
                  }

                  while (modificationStates.Count < lineCount)
                  {
                     modificationStates.Insert(CurrentLineNumber, ModificationState.Unmodified);
                  }
               }
               else
               {
                  while (modificationStates.Count > lineCount)
                  {
                     modificationStates.RemoveAt(CurrentLineNumber);
                  }
               }

               if (CurrentLineNumber.Between(0).Until(modificationStates.Count))
               {
                  modificationStates[CurrentLineNumber] = ModificationState.Modified;
               }
            }
         }

         LineChanged?.Invoke(this, new LineChangedEventArgs(CurrentLineNumber));
      };
   }

   public Color ModifiedGlyphColor { get; set; }

   public Color SavedGlyphColor { get; set; }

   public float ModificationGlyphWidth { get; set; }

   public int ModificationGlyphLeftMargin { get; set; }

   public int LeftMargin => leftMargin;

   public void DrawModificationGlyphs(Graphics graphics)
   {
      if (TextLength > 0)
      {
         var glyphWidth = ModificationGlyphWidth;
         var glyphLeft = ModificationGlyphLeftMargin;
         foreach (var (lineNumber, line, position) in VisibleLines)
         {
            if (lineNumber.Between(0).Until(modificationStates.Count))
            {
               var draw = true;
               var color = Color.Black;
               switch (modificationStates[lineNumber])
               {
                  case ModificationState.Modified:
                     color = ModifiedGlyphColor;
                     break;
                  case ModificationState.Saved:
                     color = SavedGlyphColor;
                     break;
                  default:
                     draw = false;
                     break;
               }

               if (draw)
               {
                  var currentLine = line.Copy();
                  if (currentLine.IsEmpty())
                  {
                     currentLine = "W";
                  }

                  var size = TextRenderer.MeasureText(graphics, currentLine, Font);
                  var height = size.Height;

                  using var pen = new Pen(color, glyphWidth);
                  graphics.DrawLine(pen, glyphLeft, position.Y, glyphLeft, position.Y + height);
               }
            }
         }
      }
   }

   public void DrawModificationGlyphs()
   {
      using var graphics = Graphics.FromHwnd(Handle);
      DrawModificationGlyphs(graphics);
   }

   public void SetToSavedGlyphs(bool drawModificationGlyphs = true)
   {
      for (var i = 0; i < modificationStates.Count; i++)
      {
         if (modificationStates[i] == ModificationState.Modified)
         {
            modificationStates[i] = ModificationState.Saved;
         }
      }

      if (drawModificationGlyphs)
      {
         DrawModificationGlyphs();
      }
   }

   public void SetToUnmodifiedGlyphs(bool drawModificationGlyphs = true)
   {
      modificationStates = Lines.Select(_ => ModificationState.Unmodified).ToList();

      if (drawModificationGlyphs)
      {
         DrawModificationGlyphs();
      }
   }

   public void ClearModificationGlyphs(bool drawModificationGlyphs = true)
   {
      modificationStates.Clear();

      if (drawModificationGlyphs)
      {
         DrawModificationGlyphs();
      }
   }

   public void SetTabs(params int[] tabs) => SelectionTabs = tabs;

   public Font AnnotationFont { get; set; }

   public bool ModificationLocked { get; set; }

   public bool UpdateLocked => updateLocked;

   public bool CanBeUpdated => !updateLocked;

   public (int start, int length) StopAutoScrolling()
   {
      updateLocked = true;
      StopUpdating();
      var (start, length) = Selection;
      if (Focused)
      {
         Parent.Focus();
      }

      User32.SendMessage(Handle, User32.Messages.HideSelection, true, 0);

      return (start, length);
   }

   public void ResumeAutoScrolling((int start, int length) state)
   {
      Selection = state;
      User32.SendMessage(Handle, User32.Messages.HideSelection, false, 0);

      ResumeUpdating();
      Refresh();

      if (!Focused)
      {
         Focus();
      }

      updateLocked = false;
   }

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

   public Rectangle VisibleRectangle => ClientRectangle;

   protected int getLineNumber(int lineNumber)
   {
      lineNumber.Must().BeBetween(0).Until(Lines.Length).OrThrow();
      return lineNumber;
   }

   public Point PositionFrom(int lineNumber) => GetPositionFromCharIndex(GetFirstCharIndexFromLine(lineNumber));

   public Point PositionFromCurrentLine => GetPositionFromCharIndex(GetFirstCharIndexOfCurrentLine());

   public int FirstVisibleLine => GetLineFromCharIndex(GetCharIndexFromPosition(new Point(0, 0)));

   public int LastVisibleLine
   {
      get
      {
         var position = new Point(ClientRectangle.Right, ClientRectangle.Bottom);
         return GetLineFromCharIndex(GetCharIndexFromPosition(position));
      }
   }

   public IEnumerable<(int lineNumber, string line, Point position)> VisibleLines
   {
      get
      {
         var firstVisibleLine = FirstVisibleLine;
         var lastVisibleLine = LastVisibleLine;
         for (var i = firstVisibleLine; i <= lastVisibleLine; i++)
         {
            var position = PositionFrom(i);
            if (i.Between(0).Until(Lines.Length))
            {
               yield return (i, Lines[i], position);
            }
         }
      }
   }

   public SelectionVisibility IsSelectionVisible(int index, int length)
   {
      var rectangle = VisibleRectangle;
      var position = GetPositionFromCharIndex(index);
      var rightPosition = GetPositionFromCharIndex(index + length);
      if (rectangle.Contains(position))
      {
         return rectangle.Contains(rightPosition) ? SelectionVisibility.Visible : SelectionVisibility.PartiallyVisible;
      }
      else
      {
         return SelectionVisibility.Invisible;
      }
   }

   public SelectionVisibility IsCurrentSelectionVisible() => IsSelectionVisible(SelectionStart, SelectionLength);

   public (string[], int) SelectedLines()
   {
      var line1 = GetLineFromCharIndex(SelectionStart);
      var line2 = GetLineFromCharIndex(SelectionStart + SelectionLength);

      return (Lines.Skip(line1).Take(line2 - line1 + 1).ToArray(), line1);
   }

   public string LineFrom(int lineNumber) => Lines[getLineNumber(lineNumber)];

   public (int beginningLine, int endingLine) LinesFrom(int startIndex, int length)
   {
      var beginningLine = GetLineFromCharIndex(startIndex);
      var endingLine = GetLineFromCharIndex(startIndex + length);

      return (beginningLine, endingLine);
   }

   public int CurrentLineNumber => GetLineFromCharIndex(GetFirstCharIndexOfCurrentLine());

   public string CurrentLine => Lines[CurrentLineNumber];

   public Rectangle RectangleFrom(Graphics graphics, int lineNumber)
   {
      var location = new Point(0, PositionFrom(lineNumber).Y);
      var line = LineFrom(lineNumber);
      var height = MeasureString(graphics, line, Font).Height;
      var size = new Size(VisibleRectangle.Width, height);

      return new Rectangle(location, size);
   }

   public Rectangle RectangleFromCurrentLine(Graphics graphics) => RectangleFrom(graphics, CurrentLineNumber);

   public void DrawCurrentLineBar(Graphics graphics, Color foreColor, Color backColor, DashStyle dashStyle = DashStyle.Dot, int alpha = 30)
   {
      if (TextLength > 0)
      {
         var rectangle = RectangleFromCurrentLine(graphics);
         rectangle.Offset(rectangle.X + leftMargin, 0);
         using (var brush = new SolidBrush(Color.FromArgb(alpha, backColor)))
         {
            graphics.FillRectangle(brush, rectangle);
         }

         using var pen = new Pen(foreColor) { DashStyle = dashStyle };
         graphics.DrawRectangle(pen, rectangle);
      }
   }

   protected static void drawTabLine(Graphics graphics, Pen pen, Point location, int tabStop, int height)
   {
      var y1 = location.Y;
      var y2 = location.Y + height;

      graphics.DrawLine(pen, tabStop, y1, tabStop, y2);
   }

   public void DrawTabLines(Graphics graphics)
   {
      var height = (int)graphics.MeasureString("\t", Font).Height;
      var offset = leftMargin - 2;
      foreach (var (_, line, point) in VisibleLines)
      {
         var _count = line.Matches("^ /(/t1%7); f").Map(r => r.FirstGroup.Length);
         if (_count)
         {
            using var pen = new Pen(Color.Gray) { DashStyle = DashStyle.Dot };
            drawTabLine(graphics, pen, point, offset, height);

            for (var i = 0; i < _count; i++)
            {
               if (i.Between(0).Until(SelectionTabs.Length))
               {
                  var tabStop = SelectionTabs[i];
                  drawTabLine(graphics, pen, point, tabStop + offset, height);
               }
            }
         }
      }
   }

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
         size = size with { Width = VisibleRectangle.Width };
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

   protected void annotateAt(Graphics graphics, Point point, string annotation, Color foreColor, Color backColor, Color? outlineColor, Size size)
   {
      graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
      var rectangle = new Rectangle(point, size);
      using var backBrush = new SolidBrush(Color.FromArgb(ANNOTATION_ALPHA, backColor));
      using var foreBrush = new SolidBrush(foreColor);
      graphics.FillRectangle(backBrush, rectangle);
      graphics.DrawString(annotation, AnnotationFont, foreBrush, rectangle);
      if (outlineColor.HasValue)
      {
         using var pen = new Pen(Color.FromArgb(ANNOTATION_ALPHA, outlineColor.Value));
         graphics.DrawRectangle(pen, rectangle);
      }
   }

   public void AnnotateAt(Graphics graphics, Point point, string annotation, Color foreColor, Color backColor,
      Color? outlineColor = null)
   {
      var size = MeasureString(graphics, annotation, AnnotationFont);
      annotateAt(graphics, point, annotation, foreColor, backColor, outlineColor, size);
   }

   public void AnnotateAt(Graphics graphics, int lineNumber, string annotation, Color foreColor, Color backColor,
      Color? outlineColor = null, bool rightMost = true)
   {
      var top = RectangleFrom(graphics, lineNumber).Top;
      var size = MeasureString(graphics, annotation, AnnotationFont);
      var left = rightMost ? VisibleRectangle.Width - size.Width : 0;

      annotateAt(graphics, new Point(left, top), annotation, foreColor, backColor, outlineColor, size);
   }

   public void AnnotateAtCurrentLine(Graphics graphics, string annotation, Color foreColor, Color backColor,
      Color? outlineColor = null, bool rightMost = true)
   {
      AnnotateAt(graphics, CurrentLineNumber, annotation, foreColor, backColor, outlineColor, rightMost);
   }

   public void AnnotateAtSelection(Graphics graphics, string annotation, Color foreColor, Color backColor,
      Color? outlineColor = null)
   {
      var position = GetPositionFromCharIndex(SelectionStart);

      AnnotateAt(graphics, position, annotation, foreColor, backColor, outlineColor);
   }

   public void AnnotateBySelection(Graphics graphics, int selectionStart, int selectionLength, string annotation, Color foreColor,
      Color backColor, Color? outlineColor = null, int margin = 0)
   {
      var selectionPosition = GetPositionFromCharIndex(selectionStart);
      var selectionSize = MeasureString(graphics, Text.Drop(selectionStart).Keep(selectionLength), Font);
      var selectionRectangle = new Rectangle(selectionPosition, selectionSize);

      var annotationSize = MeasureString(graphics, annotation, AnnotationFont);
      var annotationRectangle = new Rectangle(selectionPosition, annotationSize);
      annotationRectangle.X += selectionSize.Width + margin;

      Point location;
      if (!selectionRectangle.IntersectsWith(annotationRectangle) && annotationRectangle.IntersectsWith(ClientRectangle))
      {
         location = annotationRectangle.Location;
      }
      else
      {
         annotationRectangle.X = selectionRectangle.X - selectionRectangle.Width - margin;
         location = !selectionRectangle.IntersectsWith(annotationRectangle) ? annotationRectangle.Location : new Point(0, selectionRectangle.Top);
      }

      AnnotateAt(graphics, location, annotation, foreColor, backColor, outlineColor);
   }

   public void AnnotateBySelection(Graphics graphics, string annotation, Color foreColor, Color backColor,
      Color? outlineColor = null, int margin = 0)
   {
      AnnotateBySelection(graphics, SelectionStart, SelectionLength, annotation, foreColor, backColor, outlineColor, margin);
   }

   public void AnnotateByLineAndPosition(Graphics graphics, int lineNumber, int position, string annotation, Color foreColor,
      Color backColor, Color? outlineColor = null)
   {
      if (!lineNumber.Between(0).Until(Lines.Length))
      {
         throw fail($"Line number {lineNumber} out of range");
      }

      var line = Lines[lineNumber];
      if (position < 0)
      {
         position = 0;
      }
      else if (position > line.Length)
      {
         position = line.Length;
      }

      line += " ";

      var rectangle = RectangleFrom(graphics, lineNumber);
      var width = MeasureString(graphics, line.Keep(position), Font).Width;
      var margin = GetPositionFromCharIndex(0).X;
      var location = new Point(rectangle.X + width + margin, rectangle.Y);
      var size = MeasureString(graphics, annotation, AnnotationFont);

      annotateAt(graphics, location, annotation, foreColor, backColor, outlineColor, size);
   }

   public IEnumerable<(int offset, string line)> OffsetLines
   {
      get
      {
         var offset = 0;
         foreach (var index in Text.FindAll("\n"))
         {
            yield return (offset, Text.Drop(offset).Keep(index - offset));

            offset = index + 1;
         }

         yield return (offset, Text.Drop(offset));
      }
   }

   protected bool includeVisibleOnly(bool include, Rectangle rectangle) => !include || VisibleRectangle.Contains(rectangle);

   public IEnumerable<(Rectangle rectangle, string word)> RectangleWords(Graphics graphics, bool visibleOnly = true)
   {
      var _result = Text.Matches("/w+; f");
      if (_result is (true, var result))
      {
         foreach (var (text, index, length) in result)
         {
            var rectangle = RectangleFrom(graphics, index, length, false);
            if (includeVisibleOnly(visibleOnly, rectangle))
            {
               yield return (rectangle, text);
            }
         }
      }
   }

   public IEnumerable<(int start, int length)> Words()
   {
      var _result = Text.Matches("/w+; f");
      if (_result is (true, var result))
      {
         foreach (var (_, index, length) in result)
         {
            yield return (index, length);
         }
      }
   }

   public IEnumerable<(char, Rectangle)> RectangleWhitespace(Graphics graphics, bool visibleOnly = true)
   {
      var _result = Text.Matches("[' /t']; f");
      if (_result is (true, var result))
      {
         foreach (var (text, index, length) in result)
         {
            var rectangle = RectangleFrom(graphics, index, length, false);
            if (includeVisibleOnly(visibleOnly, rectangle))
            {
               yield return (text[0], rectangle);
            }
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

   public void DrawHighlight(Graphics graphics, Rectangle rectangle, Color color, DashStyle dashStyle)
   {
      using var brush = new SolidBrush(Color.FromArgb(30, color));
      graphics.FillRectangle(brush, rectangle);

      using var pen = new Pen(Color.Black) { DashStyle = dashStyle };
      graphics.DrawRectangle(pen, rectangle);
   }

   public void DrawWavyUnderline(Graphics graphics, Rectangle rectangle, Color color)
   {
      using var brush = new HatchBrush(HatchStyle.ZigZag, color, Color.Transparent);
      using var pen = new Pen(brush, 2f);
      graphics.DrawLine(pen, rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Bottom);
   }

   public void DrawWhitespace(Graphics graphics, bool visibleOnly = false)
   {
      var brush = Brushes.Gray;
      var format = new StringFormat
      {
         LineAlignment = StringAlignment.Center,
         Alignment = StringAlignment.Center
      };
      foreach (var (ch, rectangle) in RectangleWhitespace(graphics, visibleOnly))
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

   public void Colorize(int start, int length, Color color)
   {
      Select(start, length);
      SelectionColor = color;
   }

   public void SetLeftMargin(int widthInPixels)
   {
      User32.SendMessage(Handle, User32.Messages.SetMargins, User32.LEFT_MARGIN, widthInPixels);
      leftMargin = widthInPixels;
   }

   public void DrawLineNumbers(Graphics graphics, Color foreColor, Color backColor)
   {
      var lineCount = Lines.Length;
      if (lineCount > 0)
      {
         graphics.CompositingQuality = CompositingQuality.HighQuality;
         graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
         var lineSize = lineCount.ToString().Length.MaxOf(5);

         using var brush = new SolidBrush(foreColor);
         foreach (var (lineNumber, _, position) in VisibleLines)
         {
            drawLineNumber(graphics, lineNumber, position, lineSize, brush, backColor);
         }
      }
   }

   protected void drawLineNumber(Graphics graphics, int lineNumber, Point point, int lineSize, Brush brush, Color backColor)
   {
      PointF pointF = point;
      pointF = pointF with { X = 0 };
      var str = (lineNumber + 1).ToString().RightJustify(lineSize);
      var size = MeasureString(graphics, str, Font);

      using (var backBrush = new SolidBrush(backColor))
      {
         graphics.FillRectangle(backBrush, new RectangleF(pointF, size));
      }

      var lineLeft = pointF.X + size.Width;
      var lineTop = pointF.Y;
      var lineBottom = lineTop + size.Height;

      using (var pen = new Pen(Color.Black))
      {
         graphics.DrawLine(pen, lineLeft, lineTop, lineLeft, lineBottom);
      }

      graphics.DrawString(str, Font, brush, pointF);
   }

   public void DrawLineNumber(Graphics graphics, int lineNumber, Color foreColor, Color backColor)
   {
      graphics.CompositingQuality = CompositingQuality.HighQuality;
      graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      var lineSize = Lines.Length.ToString().Length.MaxOf(5);
      var point = PositionFrom(lineNumber);

      using var brush = new SolidBrush(foreColor);
      drawLineNumber(graphics, lineNumber, point, lineSize, brush, backColor);
   }
}