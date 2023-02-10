using System;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public partial class UiFailure : Form
{
   public static void Failure(string message, UiAction parentUiAction)
   {
      var uiFailure = new UiFailure(message, UiActionType.Failure);
      display(uiFailure, parentUiAction);
   }

   public static void Exception(Exception exception, UiAction parentUiAction)
   {
      var uiFailure = new UiFailure(exception.Message, UiActionType.Exception);
      display(uiFailure, parentUiAction);
   }

   protected static void display(UiFailure uiFailure, UiAction parentUiAction)
   {
      uiFailure.Location = parentUiAction.PointToScreen(parentUiAction.Location with { Y = parentUiAction.Location.Y - 40 });
      if (uiFailure.Left < 0)
      {
         uiFailure.Left = 0;
      }

      if (uiFailure.Top < 0)
      {
         uiFailure.Top = 0;
      }

      uiFailure.Width = parentUiAction.Width;
      var position = uiFailure.Location;
      position.Offset(-8, -8);
      Cursor.Position = position;

      uiFailure.Show();
   }

   protected UiAction uiMessage;

   public UiFailure(string message, UiActionType type)
   {
      InitializeComponent();

      uiMessage = new UiAction(this, true) { Dock = DockStyle.Fill };
      uiMessage.ShowMessage(message, type);
   }

   protected void UiFailure_MouseMove(object sender, MouseEventArgs e)
   {
      Capture = true;
      if (!ClientRectangle.Contains(Cursor.Position))
      {
         Close();
      }
      else
      {
         Capture = false;
      }
   }

   protected void UiFailure_MouseDown(object sender, MouseEventArgs e) => Close();
}