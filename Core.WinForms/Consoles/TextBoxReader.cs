﻿using System.IO;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Consoles
{
   public class TextBoxReader : Stream
   {
      Form form;
      TextBoxConsole console;
      IMaybe<Control> anyPreviouslyFocused;

      public TextBoxReader(Form form, TextBoxConsole console)
      {
         this.form = form;
         this.form.FormClosing += (sender, e) =>
         {
            if (console.IOStatus == IOStatusType.Reading)
            {
               e.Cancel = true;
            }
         };

         this.console = console;
         this.console.IOStatus = IOStatusType.Writing;
         anyPreviouslyFocused = none<Control>();
      }

      public override void Flush() { }

      public override long Seek(long offset, SeekOrigin origin) => 0;

      public override void SetLength(long value) { }

      public override int Read(byte[] buffer, int offset, int count)
      {
         console.ReadOnly = false;
         anyPreviouslyFocused = form.ActiveControl.SomeIfNotNull();
         console.Focus();
         console.IOStatus = IOStatusType.Reading;

         Booleans.Assertions.Assert(console.Suspended == 0, "Console must be updating");

         while (console.IOStatus == IOStatusType.Reading)
         {
            Application.DoEvents();
         }

         if (console.IOStatus == IOStatusType.Completed)
         {
            var text = console.Text;
            var textIndex = 0;
            var byteIndex = offset;

            for (; byteIndex < offset + count; byteIndex++)
            {
               if (textIndex == text.Length)
               {
                  buffer[byteIndex] = (byte)'\n';
                  byteIndex++;
                  close();

                  return byteIndex - offset;
               }

               buffer[byteIndex] = (byte)text[textIndex++];
            }

            close();

            return byteIndex - offset;
         }
         else
         {
            console.Text = "";
            close();

            return 0;
         }
      }

      void close()
      {
         console.GoToEnd();
         console.ReadOnly = true;

         if (anyPreviouslyFocused.If(out var previouslyFocused))
         {
            form.ActiveControl = previouslyFocused;
         }
      }

      public override void Write(byte[] buffer, int offset, int count) { }

      public override bool CanRead => true;

      public override bool CanSeek => false;

      public override bool CanWrite => false;

      public override long Length => 0;

      public override long Position { get; set; }
   }
}