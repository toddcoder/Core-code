using System.Windows.Forms;
using Core.WinForms.Controls;

namespace WinFormsTest;

public partial class Form1 : Form
{
   protected UiAction uiAction;

   public Form1()
   {
      InitializeComponent();

      uiAction = new UiAction(this);
      uiAction.Label("Busy").End.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
      uiAction.Uninitialized("ready");
   }

   protected void button1_Click(object sender, System.EventArgs e)
   {
      uiAction.Busy(true);
   }
}