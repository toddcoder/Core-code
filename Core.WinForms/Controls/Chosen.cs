using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public class Chosen
{
   public Chosen(string text, int index, Color foreColor, Color backColor)
   {
      Text = text;
      Index = index;
      ForeColor = foreColor;
      BackColor = backColor;
   }

   public Chosen(ListViewItem item) : this(item.Text, item.Index, item.ForeColor, item.BackColor)
   {
   }

   public string Text { get; }

   public int Index { get; }

   public Color ForeColor { get; }

   public Color BackColor { get; }

   public void Deconstruct(out string text, out int index, out Color foreColor, out Color backColor)
   {
      text = Text;
      index = Index;
      foreColor = ForeColor;
      backColor = BackColor;
   }
}