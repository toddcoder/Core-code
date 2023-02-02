using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Matching;
using Core.WinForms.Consoles;

namespace Core.WinForms.Documents;

public class Colorizer
{
   protected Pattern pattern;
   protected Color[] colors;

   public Colorizer(Pattern pattern, params Color[] colors)
   {
      this.pattern = pattern;
      this.colors = colors;
   }

   public Colorizer(Pattern pattern, string colors)
   {
      this.pattern = pattern;
      this.colors = colors.Unjoin("/s* ',' /s*; f").Select(Color.FromName).ToArray();
   }

   public void Colorize(RichTextBox textBox)
   {
      var index = textBox.SelectionStart;
      var length = textBox.SelectionLength;

      try
      {
         TextBoxConsole.StopUpdating(textBox);
         textBox.SelectAll();
         textBox.ForeColor = Color.Black;
         textBox.BackColor = Color.White;
         var newPattern = pattern.WithMultiline(true);
         var _matches = textBox.Text.Matches(newPattern);
         if (_matches is (true, var matches))
         {
            foreach (var match in matches)
            {
               var groups = match.Groups;

               for (var j = 0; j < groups.Length - 1; j++)
               {
                  colorize(textBox, groups[j + 1], colors[j]);
               }
            }
         }
      }
      finally
      {
         textBox.SelectionStart = index;
         textBox.SelectionLength = length;
         TextBoxConsole.ResumeUpdating(textBox);
         textBox.Refresh();
      }
   }

   protected static void colorize(RichTextBox textBox, Group group, Color color)
   {
      var (_, index, length) = group;
      textBox.Select(index, length);
      textBox.SelectionColor = color;
   }
}