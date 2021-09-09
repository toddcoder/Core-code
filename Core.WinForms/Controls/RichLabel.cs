using System.Windows.Forms;

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

      public void Display(string message)
      {

      }
   }
}
