using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core.Enumerables;

namespace Core.WinForms.Consoles;

public class ListBoxWriter : TextWriter
{
   protected ListBox listBox;
   protected int index;
   protected StringBuilder builder;
   protected bool nextLineLastTime;

   public ListBoxWriter(ListBox listBox)
   {
      this.listBox = listBox;
      index = -1;
      builder = new StringBuilder();
      nextLineLastTime = false;
   }

   public override Encoding Encoding => Encoding.UTF8;

   public override void Flush()
   {
      listBox.Items.Clear();
      index = -1;
      nextLineLastTime = false;
   }

   public override void Write(char value)
   {
      if (index == -1)
      {
         index = 0;
         listBox.Items.Add(builder);
      }

      if (value is '\r' or '\n')
      {
         if (nextLineLastTime)
         {
            nextLineLastTime = false;
         }
         else
         {
            listBox.Items[index++] = builder.ToString();
            builder.Clear();
            listBox.Items.Add(builder);
            nextLineLastTime = true;
         }
      }
      else
      {
         builder.Append(value);
         nextLineLastTime = false;
      }
   }

   public override string ToString()
   {
      var array = new object[listBox.Items.Count];
      listBox.Items.CopyTo(array, 0);

      return array.Select(o => o?.ToString() ?? "").ToString("\r\n");
   }
}