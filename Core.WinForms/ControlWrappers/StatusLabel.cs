using System;
using System.Windows.Forms;
using Core.WinForms.Controls;

namespace Core.WinForms.ControlWrappers
{
   [Obsolete("Use StatusProgress")]
   public class StatusLabel
   {
      protected MessageLabel message;
      protected string uninitializedText;
      protected string unselectedText;
      protected string selectedText;
      protected SelectedState state;

      public StatusLabel(Label label, string uninitializedText, string unselectedText, string selectedText,
         SelectedState state = SelectedState.Uninitialized)
      {
         message = new MessageLabel(label) { Is3D = false, Center = true };
         this.uninitializedText = uninitializedText;
         this.unselectedText = unselectedText;
         this.selectedText = selectedText;
         State = state;

         label.Click += (_, _) =>
         {
            State = this.state switch
            {
               SelectedState.Uninitialized => SelectedState.Selected,
               SelectedState.Unselected => SelectedState.Selected,
               SelectedState.Selected => SelectedState.Unselected,
               _ => SelectedState.Uninitialized
            };
         };
      }

      protected void changeState()
      {
         switch (state)
         {
            case SelectedState.Uninitialized:
               message.Uninitialized(uninitializedText);
               message.Label.BorderStyle = BorderStyle.None;
               break;
            case SelectedState.Unselected:
               message.Unselected(unselectedText);
               message.Label.BorderStyle = BorderStyle.None;
               break;
            case SelectedState.Selected:
               message.Selected(selectedText);
               message.Label.BorderStyle = BorderStyle.FixedSingle;
               break;
         }
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