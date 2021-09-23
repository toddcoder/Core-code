using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Core.Matching;

namespace Core.WinForms.Consoles
{
   public static class TextBoxExtensions
   {
      private const int WM_SET_REDRAW = 11;
      private const int EM_GET_RECT = 0x00b2;
      private const int EM_SET_RECT = 0x00b3;
      private const int EM_SCROLL = 0x00b5;
      private const int SB_LINE_UP = 0;
      private const int SB_LINE_DOWN = 1;

      [StructLayout(LayoutKind.Sequential)]
      private readonly struct Rect
      {
         public readonly int Left;
         public readonly int Top;
         public readonly int Right;
         public readonly int Bottom;

         public Rect(int left, int top, int right, int bottom) : this()
         {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
         }

         public Rect(Rectangle rectangle) : this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
         {
         }
      }

      [DllImport("user32.dll")]
      private static extern int SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

      [DllImport("user32.dll")]
      private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

      [DllImport("user32.dll")]
      private static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, ref Rect rect);

      [DllImport("user32.dll")]
      private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, ref Point point);

      public static void StopUpdating(this TextBoxBase textBox) => SendMessage(textBox.Handle, WM_SET_REDRAW, false, 0);

      public static void ResumeUpdating(this TextBoxBase textBox) => SendMessage(textBox.Handle, WM_SET_REDRAW, true, 0);

      public static string CurrentLine(this TextBoxBase textBox) => textBox.Lines[textBox.CurrentLineIndex()];

      public static int CurrentLineIndex(this TextBoxBase textBox) => textBox.GetLineFromCharIndex(textBox.SelectionStart);

      public static bool AtEnd(this TextBoxBase textBox) => textBox.SelectionStart == textBox.TextLength;

      public static bool AtLastLIne(this TextBoxBase textBox) => textBox.CurrentLineIndex() == textBox.Lines.Length - 1;

      public static void AppendAtEnd(this TextBoxBase textBox, string text, string endOfLine = "/r/n", bool moveToEndOfText = true)
      {
         if (!textBox.Text.IsMatch($"{endOfLine} $; f"))
         {
            var newEndOfLine = endOfLine.Replace("/r", "\r").Replace("/n", "\n");
            textBox.AppendText(newEndOfLine);
         }

         textBox.AppendText(text);
         if (moveToEndOfText)
         {
            textBox.Select(textBox.TextLength, 0);
         }
      }

      public static void Pad(this TextBoxBase textBox, Rectangle rectangle)
      {
         var size = textBox.ClientSize;
         var rect = new Rect(rectangle.Left, rectangle.Top, size.Width - rectangle.Left - rectangle.Right,
            size.Height - rectangle.Top - rectangle.Bottom);
         SendMessage(textBox.Handle, EM_SET_RECT, 0, ref rect);
      }

      public static void Pad(this TextBoxBase textBox, int left, int top, int right, int bottom)
      {
         textBox.Pad(new Rectangle(left, top, right, bottom));
      }

      public static (Point origin, Size size) Rectangle(this TextBoxBase textBox)
      {
         var rect = new Rect();
         SendMessage(textBox.Handle, EM_GET_RECT, 0, ref rect);

         return (new Point(rect.Left, rect.Bottom), new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
      }

      public static (int index, int length) SaveSelection(this TextBoxBase textBox)
      {
         return (textBox.SelectionStart, textBox.SelectionLength);
      }

      public static void RestoreSelection(this TextBoxBase textBox, (int index, int length) selection,
         ScrollNearCaretType scrollType = ScrollNearCaretType.NoScroll)
      {
         var (index, length) = selection;
         textBox.Select(index, length);
         textBox.ScrollNearCaret(scrollType);
      }

      public static void ScrollNearCaret(this TextBoxBase textBox, ScrollNearCaretType type)
      {
         switch (type)
         {
            case ScrollNearCaretType.NoScroll:
               return;
            case ScrollNearCaretType.ScrollNotAtLastLine:
               if (textBox.AtLastLIne())
               {
                  return;
               }

               break;
         }

         SendMessage(textBox.Handle, EM_SCROLL, SB_LINE_UP, 0);
      }
   }
}