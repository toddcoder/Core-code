using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls
{
   public partial class RichLabel : RichTextBox
   {
      protected const int WM_R_BUTTON_DOWN = 0x204;
      protected const int WM_R_BUTTON_UP = 0x205;

      public RichLabel()
      {
         InitializeComponent();

         ReadOnly = true;
         BorderStyle = BorderStyle.None;
         TabStop = false;
         SetStyle(ControlStyles.Selectable, false);
         SetStyle(ControlStyles.UserMouse, true);
         SetStyle(ControlStyles.SupportsTransparentBackColor, true);

         MouseEnter += (_, _) => Cursor = Cursors.Default;
      }

      protected override void WndProc(ref Message m)
      {
         if (m.Msg is WM_R_BUTTON_DOWN or WM_R_BUTTON_UP)
         {
            return;
         }

         base.WndProc(ref m);
      }

      protected IEnumerable<CombinedDecorations> decorations(string message)
      {
         var combinedDecorations = new CombinedDecorations(0, 0);

         if (message.Matches("/('//color:' -[';']+ ';' | '//font:' -[';']+ ';' | '//' ['biu']); f").If(out var result))
         {
            foreach (var match in result)
            {
               var (text, index, length) = match;
               var nextIndex = combinedDecorations.NextIndex;
               if (nextIndex < index)
               {
                  combinedDecorations = combinedDecorations.With(new Undecorated(), nextIndex, index - nextIndex);
                  yield return combinedDecorations;
               }
               else
               {
                  if (text.Matches("^ '//color:' /(-[';']+) ';'; f").If(out var subResult))
                  {
                     Maybe<Color> _color = nil;
                     var argument = subResult.FirstGroup;
                     if (argument.Matches("^ 'r' /s* /(/d1%3) /s* 'b' /s* /(/d1%3) /s* 'g' /s* /(/d1%3)").If(out subResult))
                     {
                        var (redString, greenString, blueString) = subResult;
                        var _rgb =
                           from redByte in redString.AsByte()
                           from greenByte in greenString.AsByte()
                           from blueByte in blueString.AsByte()
                           select (redByte, greenByte, blueByte);
                        if (_rgb.If(out var red, out var green, out var blue))
                        {
                           _color = Color.FromArgb(red, green, blue);
                        }
                     }
                     else
                     {
                        _color = argument.AsEnumeration<Color>();
                     }

                     if (_color.If(out var color))
                     {
                        //combinedDecorations=combinedDecorations.With(new TextColor(color), index)
                     }
                  }
               }
            }
         }
      }

      public void Display(string message)
      {

      }
   }
}
