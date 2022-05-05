using System.Windows.Forms;
using Core.WinForms.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class WinFormsTests
   {
      [TestMethod]
      public void ConsoleScrollingTest()
      {
         var form = new Form();
         var richTextBox = new RichTextBox { Dock = DockStyle.Fill };
         form.Controls.Add(richTextBox);
         var console = new WinForms.Consoles.TextBoxConsole(form, richTextBox);
         using var writer = console.Writer();

         form.Show();

         for (var i = 0; i < 300; i++)
         {
            writer.WriteLine(i);
         }
      }

      [TestMethod]
      public void MessageProgressBusyTest()
      {
         var form = new Form();
         //form.Size = new Size(800, 100);
         var message = new MessageProgress(form);
         message.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
         message.Busy("This message is in no way clickable!");
         form.ShowDialog();
      }
   }
}