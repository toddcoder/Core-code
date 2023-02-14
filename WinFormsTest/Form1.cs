using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Dates.DateIncrements;
using Core.WinForms.Controls;
using static Core.Monads.MonadFunctions;

namespace WinFormsTest;

public partial class Form1 : Form
{
   protected UiAction uiAction;

   public Form1()
   {
      InitializeComponent();

      var index = 0;
      uiAction = new UiAction(this, true);
      uiAction.SetUp(0, 0, 300, 40, AnchorStyles.Left | AnchorStyles.Right);
      uiAction.Message("Progress");
      uiAction.Tick += (_, _) =>
      {
         if (++index <= 100)
         {
            var sin = Math.Sin(index);
            uiAction.Progress(sin.ToString("##.000"));
            if (index == 10 && !uiAction.ProgressSubText)
            {
               uiAction.ProgressSubText = uiAction.SubText("error").Set.GoToMiddleLeft(100).ForeColor(Color.White).BackColor(Color.Red).End;
            }
         }
         else
         {
            uiAction.Success("Done");
            uiAction.StopTimer();
         }
      };
      uiAction.Click += (_, _) =>
      {
         uiAction.Maximum = 100;
         uiAction.StartTimer(1.Second());
      };
      uiAction.ClickText = "Remove label";
   }

   protected void button1_Click(object sender, EventArgs e)
   {
      if (!uiAction.HasFloatingFailureOrException)
      {
         uiAction.FloatingFailure("This has failed\r\nOne more line");
      }
      else if (uiAction.FailureToolTip)
      {
         uiAction.FloatingFailure();
         uiAction.FloatingException(fail("This was an exception"));
      }
      else
      {
         uiAction.FloatingException();
      }
   }

   protected void button2_Click(object sender, EventArgs e)
   {
      uiAction.ToolTipTitle = "Title";
   }

   protected void button3_Click(object sender, EventArgs e)
   {
      uiAction.Success("Success!");
   }
}