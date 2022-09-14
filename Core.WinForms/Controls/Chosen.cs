using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public class Chosen
{
   public Chosen(string value, string key, int index, Color foreColor, Color backColor)
   {
      Value = value;
      Key = key;
      Index = index;
      ForeColor = foreColor;
      BackColor = backColor;
   }

   public Chosen(string value, ListViewItem item) : this(value, item.Text, item.Index, item.ForeColor, item.BackColor)
   {
   }

   public string Key { get; }

   public string Value { get; }

   public int Index { get; }

   public Color ForeColor { get; }

   public Color BackColor { get; }

   public void Deconstruct(out string key, out string value, out int index, out Color foreColor, out Color backColor)
   {
      key = Key;
      value = Value;
      index = Index;
      foreColor = ForeColor;
      backColor = BackColor;
   }
}