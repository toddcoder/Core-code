using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Core.Strings;
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

      [TestMethod]
      public void ProgressDefiniteTest()
      {
         var form = new Form();
         var message = new MessageProgress(form);
         message.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
         message.Maximum = 50;
         var i = 0;

         var timer = new Timer();
         timer.Tick += (_, _) =>
         {
            if (i++ < 50)
            {
               message.Progress("x".Repeat(i));
            }
            else
            {
               timer.Stop();
            }
         };
         timer.Start();
         form.ShowDialog();
      }

      [TestMethod]
      public void AutomaticMessageTest()
      {
         var form = new Form();
         var message = new MessageProgress(form);
         message.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
         var stopwatch = new Stopwatch();
         message.AutomaticMessage += (_, e) =>
         {
            e.Text = stopwatch.Elapsed.ToString();
         };
         message.StartAutomatic();
         stopwatch.Start();
         form.ShowDialog();
      }

      [TestMethod]
      public void ResponderTest()
      {
         var form = new Form
         {
            BackColor = Color.Aqua
         };
         var responder = new Responder(form, "!Yes|yes", "?No|no", "$Stop|stop", ".Maybe|maybe", "???")
         {
            BackColor = Color.White
         };
         responder.SetUp(0, 0, 500, 100, 24);
         responder.ButtonClick += (_, e) =>
         {
            form.Text = e.Key;
         };
         form.ShowDialog();
      }
   }
}