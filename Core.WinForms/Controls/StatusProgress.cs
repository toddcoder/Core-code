using System.Windows.Forms;

namespace Core.WinForms.Controls
{
   public class StatusProgress : MessageProgress
   {
      protected string uninitializedText;
      protected string unselectedText;
      protected string selectedText;
      protected SelectedState state;

      public StatusProgress(Control control, string uninitializedText, string unselectedText, string selectedText,
         SelectedState state = SelectedState.Uninitialized) : base(control)
      {
         this.uninitializedText = uninitializedText;
         this.unselectedText = unselectedText;
         this.selectedText = selectedText;
         State = state;

         Is3D = false;
         Center = true;

         Click += (_, _) =>
         {
            this.state = this.state switch
            {
               SelectedState.Uninitialized => SelectedState.Selected,
               SelectedState.Unselected => SelectedState.Selected,
               SelectedState.Selected => SelectedState.Unselected,
               _ => SelectedState.Uninitialized
            };
            changeState();
         };

         ClickText = "Click to change status";
      }

      protected void changeState()
      {
         type = state switch
         {
            SelectedState.Uninitialized => MessageProgressType.Uninitialized,
            SelectedState.Unselected => MessageProgressType.Unselected,
            SelectedState.Selected => MessageProgressType.Selected,
            _ => MessageProgressType.Uninitialized
         };

         text = state switch
         {
            SelectedState.Uninitialized => uninitializedText,
            SelectedState.Unselected => unselectedText,
            SelectedState.Selected => selectedText,
            _ => uninitializedText
         };

         this.Do(refresh);
      }

      public SelectedState State
      {
         get => state;
         set
         {
            state = value;
            changeState();
         }
      }

      public void Uninitialized() => State = SelectedState.Uninitialized;

      public void Unselected() => State = SelectedState.Unselected;

      public void Selected() => State = SelectedState.Selected;
   }
}