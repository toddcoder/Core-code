using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Controls
{
   public class ExpandCommand : UserControl
   {
      protected TextBoxBase textBox;
      protected ExpandCommandState state;
      protected Rectangle activeRectangle;

      public ExpandCommand(TextBoxBase textBox)
      {
         state = ExpandCommandState.Closed;
      }
   }
}