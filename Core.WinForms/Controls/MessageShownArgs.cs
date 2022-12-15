using System;

namespace Core.WinForms.Controls;

public class MessageShownArgs : EventArgs
{
   public MessageShownArgs(string text, UiActionType type)
   {
      Text = text;
      Type = type;
   }

   public string Text { get; }

   public UiActionType Type { get; }
}