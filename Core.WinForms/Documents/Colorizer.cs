using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.RegularExpressions;
using Core.WinForms.Consoles;

namespace Core.WinForms.Documents
{
   public class Colorizer
   {
      string pattern;
      Color[] colors;

      public Colorizer(string pattern, params Color[] colors)
      {
         this.pattern = pattern;
         this.colors = colors;
      }

      public Colorizer(string pattern, string colors)
      {
         this.pattern = pattern;
         this.colors = colors.Split("/s* ',' /s*").Select(Color.FromName).ToArray();
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
            var matcher = textBox.Text.Matches(pattern, multiline: true);
            if (matcher.If(out var m))
            {
               for (var i = 0; i < m.MatchCount; i++)
               {
                  var match = m.GetMatch(i);
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

      static void colorize(RichTextBox textBox, Matcher.Group group, Color color)
      {
         textBox.Select(group.Index, group.Length);
         textBox.SelectionColor = color;
      }
   }
}