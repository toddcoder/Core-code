using System.Windows.Forms;
using Core.WinForms.Controls;

namespace WinFormsTest;

public partial class Form1 : Form
{
   protected UiAction uiAction;

   public Form1()
   {
      InitializeComponent();

      uiAction = new UiAction(this, true);
      uiAction.SetUp(0, 0, 400, 40);
      uiAction.Label("Busy").End.Busy(true);
   }
}