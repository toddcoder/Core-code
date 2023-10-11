using System;

namespace Core.WinForms.Controls;

public class TextEventArgs : EventArgs
{
   public TextEventArgs(string text)
   {
      Text = text;
   }

   public string Text { get; }
}