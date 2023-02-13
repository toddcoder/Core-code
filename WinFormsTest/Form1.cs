using System.Windows.Forms;
using Core.Monads;
using Core.WinForms.Controls;

namespace WinFormsTest;

public partial class Form1 : Form
{
   protected UiAction uiAction;

   public Form1()
   {
      InitializeComponent();

      uiAction = new UiAction(this, true);
      uiAction.SetUp(0, 0, 300, 40, AnchorStyles.Left | AnchorStyles.Right);
      uiAction.Button("Show Floating Failure");
      uiAction.Click += (_, _) => uiAction.FloatingFailure("Failed!");
   }

   protected void button1_Click(object sender, System.EventArgs e)
   {
      if (!uiAction.HasFloatingFailureOrException)
      {
         uiAction.FloatingFailure("This has failed");
      }
      else if (uiAction.IsFloatingFailure)
      {
         uiAction.FloatingFailure();
         uiAction.FloatingException(MonadFunctions.fail("This was an exception"));
      }
      else
      {
         uiAction.FloatingException();
      }
   }
}