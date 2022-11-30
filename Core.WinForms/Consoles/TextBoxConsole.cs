using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Core.Strings;

namespace Core.WinForms.Consoles;

public class TextBoxConsole
{
   public enum ConsoleColorType
   {
      Windows,
      Teletype,
      Cathode,
      Turbo,
      Quick,
      Default
   }

   protected const int WM_SET_REDRAW = 11;

   [DllImport("user32.dll")]
   protected static extern int SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

   public static void StopUpdating(TextBoxBase textBox) => SendMessage(textBox.Handle, WM_SET_REDRAW, false, 0);

   public static void ResumeUpdating(TextBoxBase textBox) => SendMessage(textBox.Handle, WM_SET_REDRAW, true, 0);

   protected Form form;
   protected RichTextBox textBox;
   protected int writeStartingIndex;
   protected int suspended;

   public TextBoxConsole(Form form, RichTextBox textBox, string fontName = "Consolas", float fontSize = 10f,
      ConsoleColorType colorType = ConsoleColorType.Windows)
   {
      var history = new CommandLineHistory();
      this.form = form;
      this.textBox = textBox;
      this.textBox.Font = new Font(fontName, fontSize);

      switch (colorType)
      {
         case ConsoleColorType.Windows:
            this.textBox.ForeColor = Color.White;
            this.textBox.BackColor = Color.Black;
            break;
         case ConsoleColorType.Teletype:
            this.textBox.ForeColor = Color.Gray;
            this.textBox.BackColor = Color.White;
            break;
         case ConsoleColorType.Cathode:
            this.textBox.ForeColor = Color.White;
            this.textBox.BackColor = Color.Green;
            break;
         case ConsoleColorType.Turbo:
            this.textBox.ForeColor = Color.Yellow;
            this.textBox.BackColor = Color.Blue;
            break;
         case ConsoleColorType.Quick:
            this.textBox.ForeColor = Color.White;
            this.textBox.BackColor = Color.Blue;
            break;
         case ConsoleColorType.Default:
            break;
      }

      this.textBox.KeyDown += (_, e) =>
      {
         switch (e.KeyCode)
         {
            case Keys.Left:
               if (!CanWrite || !inBox())
               {
                  e.Handled = true;
               }

               break;
            case Keys.Down:
            {
               var _text = history.Forward();
               if (_text)
               {
                  Text = _text;
               }

               e.Handled = true;
               break;
            }

            case Keys.Up:
            {
               var _text = history.Backward();
               if (_text)
               {
                  Text = _text;
               }

               e.Handled = true;
               break;
            }

            case Keys.Back:
               e.Handled = !inBox();
               break;
            case Keys.Escape:
               IOStatus = IOStatusType.Cancelled;
               e.Handled = true;
               break;
            case Keys.Home:
            {
               var currentIndex = textBox.SelectionStart;
               textBox.SelectionStart = writeStartingIndex;
               if (e.Shift)
               {
                  textBox.SelectionLength = currentIndex - writeStartingIndex;
               }
               else
               {
                  textBox.SelectionLength = 0;
               }

               e.Handled = true;
               break;
            }
         }
      };

      this.textBox.KeyUp += (_, e) =>
      {
         switch (e.KeyCode)
         {
            case Keys.A:
            {
               if (e.Control)
               {
                  textBox.Select(writeStartingIndex, Text.Length + 1);
                  e.Handled = true;
               }

               break;
            }

            case Keys.C:
            {
               if (e.Control)
               {
                  textBox.Copy();
                  e.Handled = true;
               }

               break;
            }

            case Keys.X:
            {
               if (e.Control)
               {
                  textBox.Cut();
                  e.Handled = true;
               }

               break;
            }

            case Keys.V:
            {
               if (e.Control)
               {
                  textBox.Paste(DataFormats.GetFormat(DataFormats.Text));
                  e.Handled = true;
               }

               break;
            }
         }
      };

      this.textBox.KeyPress += (_, e) =>
      {
         if (e.KeyChar == 13)
         {
            textBox.SelectionStart = writeStartingIndex + Text.Length;
            history.Add(Text);
            IOStatus = IOStatusType.Completed;
            e.Handled = true;
         }
      };

      this.textBox.MouseClick += (_, _) =>
      {
         if (!inBox())
         {
            textBox.SelectionStart = textBox.TextLength;
         }
      };

      writeStartingIndex = 0;
      history = new CommandLineHistory();
   }

   protected bool inBox() => textBox.SelectionStart > writeStartingIndex;

   public int Suspended => suspended;

   public bool InvokeRequired { get; set; } = true;

   public bool Buffer { get; set; }

   public void Do(Action<RichTextBox> action)
   {
      if (textBox.InvokeRequired && InvokeRequired)
      {
         textBox.Invoke(() => action(textBox));
      }
      else
      {
         action(textBox);
      }
   }

   public T Get<T>(Func<RichTextBox, T> func)
   {
      if (textBox.InvokeRequired && InvokeRequired)
      {
         return (T)textBox.Invoke(new Func<T>(() => Get(func)));
      }
      else
      {
         return func(textBox);
      }
   }

   public string Text
   {
      get => Get(tb => tb.Text.Slice(writeStartingIndex, textBox.TextLength - 2));
      set => Do(tb =>
      {
         tb.Select(writeStartingIndex, textBox.TextLength - writeStartingIndex);
         tb.SelectedText = value;
      });
   }

   public void GoToEnd() => writeStartingIndex = Get(tb => tb.TextLength);

   protected void goToEnd() => writeStartingIndex = textBox.TextLength;

   public bool CanWrite => Get(tb => tb.SelectionStart) >= writeStartingIndex;

   public TextWriter Writer() => new TextBoxWriter(this);

   public TextReader Reader() => new StreamReader(new TextBoxReader(form, this));

   public void StopUpdating()
   {
      if (suspended == 0)
      {
         StopUpdating(Get(tb => tb));
      }

      suspended++;
   }

   public void ResumeUpdating()
   {
      if (--suspended == 0)
      {
         ResumeUpdating(Get(tb => tb));
         Do(tb =>
         {
            tb.Refresh();
            tb.ScrollToCaret();
         });
      }
   }

   public void Write(char value)
   {
      Do(tb =>
      {
         if (tb.TextLength > 100000)
         {
            tb.Clear();
         }

         tb.AppendText(value.ToString());
         goToEnd();
      });
   }

   public void Clear()
   {
      Do(tb => tb.Clear());
      GoToEnd();
   }

   public IOStatusType IOStatus { get; set; }

   public bool ReadOnly
   {
      get => Get(tb => tb.ReadOnly);
      set => Do(tb => tb.ReadOnly = value);
   }

   public void Focus() => Do(tb => tb.Focus());

   public void Do(Action action)
   {
      try
      {
         StopUpdating();
         action();
      }
      finally
      {
         ResumeUpdating();
      }
   }

   public T Get<T>(Func<T> func)
   {
      try
      {
         StopUpdating();
         return func();
      }
      finally
      {
         ResumeUpdating();
      }
   }

   public void ScrollToCaret() => textBox.ScrollToCaret();
}