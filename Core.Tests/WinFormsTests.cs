using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Strings;
using Core.WinForms.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Monads.MonadFunctions;

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
         message.AutomaticMessage += (_, e) => { e.Text = stopwatch.Elapsed.ToString(); };
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
         responder.ButtonClick += (_, e) => { form.Text = e.Key; };
         form.ShowDialog();
      }

      [TestMethod]
      public void EnabledTest1()
      {
         var form = new Form();
         var message = new MessageProgress(form);
         message.SetUp(0, 0, 300, 30);
         message.Checked = true;
         message.Busy("working...");

         var checkBox = new CheckBox
         {
            Text = "Enabled",
            Checked = true
         };
         checkBox.Click += (_, _) => message.Enabled = checkBox.Checked;
         checkBox.Location = new Point(0, 35);
         form.Controls.Add(checkBox);

         form.ShowDialog();
      }

      [TestMethod]
      public void EnabledTest2()
      {
         var form = new Form();
         var responder = new Responder(form, "!OK|ok", "?Cancel|cancel");
         responder.SetUp(0, 0, 300, 30, 30);
         responder.ButtonClick += (_, e) => form.Text = e.Key;
         responder["ok"].Checked = true;
         responder["cancel"].Checked = true;

         var checkBox = new CheckBox
         {
            Text = "Enabled",
            Checked = true
         };
         checkBox.Click += (_, _) => responder.Enabled = checkBox.Checked;
         checkBox.Location = new Point(0, 35);
         form.Controls.Add(checkBox);

         form.ShowDialog();
      }

      [TestMethod]
      public void RoundedMessageTest()
      {
         var form = new Form();
         var message = new RoundedMessage(form, true) { CornerRadius = 8 };
         message.SetUp(4, 4, form.ClientSize.Width - 20, 27, AnchorStyles.Left);
         message.Message("Round");
         form.ShowDialog();
      }

      [TestMethod]
      public void ClickableRoundedMessageTest()
      {
         var form = new Form();
         var message = new RoundedMessage(form, true) { CornerRadius = 8 };
         message.SetUp(4, 4, form.ClientSize.Width - 20, 27, AnchorStyles.Left);
         message.Message("Round");
         message.Click += (_, _) => message.Message("Clicked");
         message.ClickText = "Click me!";
         form.ShowDialog();
      }

      [TestMethod]
      public void StatusesTest()
      {
         var form = new Form();
         var messages = Enumerable.Range(0, 4).Select(_ => new MessageProgress(form, true)).ToArray();
         var width = form.ClientSize.Width - 20;
         for (var i = 0; i < 4; i++)
         {
            messages[i].SetUp(4, 4 + i * 27, width, 27, AnchorStyles.Left);
         }

         messages[0].Success("Success");
         messages[1].Caution("Caution");
         messages[2].Failure("Failure");
         messages[3].Exception(fail("Exception"));

         form.ShowDialog();
      }

      [TestMethod]
      public void ImageTest()
      {
         var form = new Form();
         var message = new MessageProgress(form, true)
         {
            Dock = DockStyle.Fill
         };

         var image = Image.FromFile(@"..\..\TestData\build.jpg");
         message.Image = image;
         message.Message("Build");
         form.ShowDialog();
      }

      [TestMethod]
      public void StretchImageTest()
      {
         var form = new Form();
         var message = new MessageProgress(form, true)
         {
            Dock = DockStyle.Fill
         };

         var image = Image.FromFile(@"..\..\TestData\build.jpg");
         message.Image = image;
         message.StretchImage = true;
         message.Message("Build");
         form.ShowDialog();
      }

      [TestMethod]
      public void SubTextTest()
      {
         var form = new Form();
         var message = new MessageProgress(form, true)
         {
            Dock = DockStyle.Fill
         };
         message.SubText("now", 10, 10)
            .SetForeColor(Color.White)
            .SetBackColor(Color.Green)
            .SetFont("Verdana", 8, FontStyle.Regular)
            .SetOutline(true);
         message.Message("Message");
         form.ShowDialog();
      }
   }
}