using System.Windows.Forms;

namespace Core.WinForms.ControlWrappers
{
   public class ProgressMessageLabel
   {
      protected ProgressBar progressBar;
      protected MessageLabel messageLabel;

      public ProgressMessageLabel(ProgressBar progressBar, Label label)
      {
         this.progressBar = progressBar;
         messageLabel = new MessageLabel(label);

         ShowMessageLabel();
      }

      public ProgressBar ProgressBar => progressBar;

      public MessageLabel MessageLabel => messageLabel;

      public void ShowProgressBar()
      {
         progressBar.Location = messageLabel.Label.Location;
         progressBar.Size = messageLabel.Label.Size;
         progressBar.Visible = true;
      }

      public void ShowMessageLabel()
      {
         progressBar.Visible = false;
      }
   }
}