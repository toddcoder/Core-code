using System.Windows.Forms;
using Core.Strings;
using Core.WinForms.Controls;

namespace WinFormsTest;

public partial class Form1 : Form
{
   protected UiAction uiAction;

   public Form1()
   {
      InitializeComponent();

      uiAction = new UiAction(this);
      uiAction.Label("Progress").End.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
      uiAction.Maximum = 50;
      var i = 0;

      var timer = new Timer();
      timer.Tick += (_, _) =>
      {
         if (i++ < 50)
         {
            uiAction.Progress("x".Repeat(i));
         }
         else
         {
            timer.Stop();
         }
      };
      uiAction.Click += (_, _) => timer.Start();
      uiAction.ClickText = "Start timer";
   }
}