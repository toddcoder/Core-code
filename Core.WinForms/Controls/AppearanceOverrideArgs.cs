using System;
using System.Drawing;

namespace Core.WinForms.Controls;

public class AppearanceOverrideArgs : EventArgs
{
   public AppearanceOverrideArgs(string text, Color foreColor, Color backColor)
   {
      Text = text;
      ForeColor = foreColor;
      BackColor = backColor;
      Override = false;

      Italic = false;
      Bold = false;
   }

   public string Text { get; set; }

   public Color ForeColor { get; set; }

   public Color BackColor { get; set; }

   public bool Italic { get; set; }

   public bool Bold { get; set; }

   public bool Override { get; set; }
}