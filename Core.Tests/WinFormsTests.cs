using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Dates.DateIncrements;
using Core.Strings;
using Core.WinForms;
using Core.WinForms.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Monads.MonadFunctions;
using Timer = System.Windows.Forms.Timer;

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
         var uiAction = new UiAction(form);
         uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
         uiAction.Busy("This message is in no way clickable!");
         form.ShowDialog();
      }

      [TestMethod]
      public void MessageProgressBusyIndefiniteTest()
      {
         var form = new Form();
         var uiAction = new UiAction(form);
         uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
         uiAction.Busy(true);
         form.ShowDialog();
      }

      [TestMethod]
      public void ProgressDefiniteTest()
      {
         var form = new Form();
         var uiAction = new UiAction(form);
         uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
         uiAction.Maximum = 50;
         var i = 0;

         var timer = new Timer();
         timer.Tick += (_, _) =>
         {
            if (i++ < 50)
            {
               uiAction.Progress("x".Repeat(i));
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
         var uiAction = new UiAction(form);
         uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
         var stopwatch = new Stopwatch();
         uiAction.AutomaticMessage += (_, e) => { e.Text = stopwatch.Elapsed.ToString(); };
         uiAction.StartAutomatic();
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
         responder.Dock = DockStyle.Fill;
         responder.ButtonClick += (_, e) => { form.Text = e.Key; };
         form.ShowDialog();
      }

      [TestMethod]
      public void EnabledTest1()
      {
         var form = new Form();
         var uiAction = new UiAction(form);
         uiAction.SetUp(0, 0, 300, 30);
         uiAction.CheckStyle = CheckStyle.Checked;
         uiAction.Busy("working...");

         var checkBox = new CheckBox
         {
            Text = "Enabled",
            Checked = true
         };
         checkBox.Click += (_, _) => uiAction.Enabled = checkBox.Checked;
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
         responder["ok"].CheckStyle = CheckStyle.Checked;
         responder["cancel"].CheckStyle = CheckStyle.Unchecked;

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
         var message = new RoundedUiAction(form, true) { CornerRadius = 8 };
         message.SetUp(4, 4, form.ClientSize.Width - 20, 27, AnchorStyles.Left);
         message.Message("Round");
         form.ShowDialog();
      }

      [TestMethod]
      public void ClickableRoundedMessageTest()
      {
         var form = new Form();
         var message = new RoundedUiAction(form, true) { CornerRadius = 8 };
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
         var uiActions = Enumerable.Range(0, 4).Select(_ => new UiAction(form, true)).ToArray();
         var width = form.ClientSize.Width - 20;
         for (var i = 0; i < 4; i++)
         {
            uiActions[i].SetUp(4, 4 + i * 27, width, 27, AnchorStyles.Left);
         }

         uiActions[0].Success("Success");
         uiActions[1].Caution("Caution");
         uiActions[2].Failure("Failure");
         uiActions[3].Exception(fail("Exception"));

         form.ShowDialog();
      }

      [TestMethod]
      public void ImageTest()
      {
         var form = new Form();
         var uiAction = new UiAction(form, true)
         {
            Dock = DockStyle.Fill
         };

         var image = Image.FromFile(@"..\..\TestData\build.jpg");
         uiAction.Image = image;
         uiAction.Message("Build");
         form.ShowDialog();
      }

      [TestMethod]
      public void StretchImageTest()
      {
         var form = new Form();
         var uiAction = new UiAction(form, true)
         {
            Dock = DockStyle.Fill
         };

         var image = Image.FromFile(@"..\..\TestData\build.jpg");
         uiAction.Image = image;
         uiAction.StretchImage = true;
         uiAction.Message("Build");
         form.ShowDialog();
      }

      [TestMethod]
      public void SubTextTest()
      {
         var form = new Form();
         var uiAction = new UiAction(form, true)
         {
            Dock = DockStyle.Fill
         };
         uiAction.SubText("now", 10, 10).Set
            .ForeColor(Color.White)
            .BackColor(Color.Green)
            .Font("Verdana", 8)
            .Outline(true);
         uiAction.Message("Message");
         form.ShowDialog();
      }

      [TestMethod]
      public void StopwatchTest()
      {
         var form = new Form();
         var uiAction = new UiAction(form) { Stopwatch = true };
         uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
         uiAction.Busy("Timing");
         form.ShowDialog();
      }

      [TestMethod]
      public void ExTextBoxTest()
      {
         var form = new Form();
         var textBox = new ExTextBox(form) { RefreshOnTextChange = true };
         textBox.Font = new Font("Consolas", 12);
         textBox.Width = form.ClientSize.Width;
         textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
         textBox.Location = new Point(0, 0);
         textBox.Paint += (_, e) =>
         {
            var pen = new Pen(Color.Green, 2);
            e.Graphics.DrawRectangle(pen, textBox.Bounds);
            foreach (var (rectangle, word) in textBox.RectangleWords(e.Graphics))
            {
               Console.WriteLine(word);
               textBox.DrawHighlight(e.Graphics, rectangle, Color.White, Color.Red, DashStyle.Dot);
            }
         };
         form.ShowDialog();
      }

      [TestMethod]
      public void ExTextBoxTest2()
      {
         var form = new Form();
         var textBox = new ExTextBox(form) { RefreshOnTextChange = true };
         textBox.Font = new Font("Consolas", 12);
         textBox.Width = form.ClientSize.Width;
         textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
         textBox.Location = new Point(0, 0);
         textBox.Paint += (_, e) =>
         {
            var rectangle = new Rectangle(textBox.Bounds.Width - 16, 0, 16, textBox.Bounds.Height);
            using var brush = new HatchBrush(HatchStyle.DiagonalBrick, Color.White, Color.Green);
            e.Graphics.FillRectangle(brush, rectangle);
         };
         form.ShowDialog();
      }

      [TestMethod]
      public void ExTextBoxTest3()
      {
         var form = new Form();
         var textBox = new ExTextBox(form)
         {
            RefreshOnTextChange = true,
            Font = new Font("Consolas", 12),
            Width = form.ClientSize.Width,
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            Location = new Point(0, 0),
            BackColor = SystemColors.Control
         };
         textBox.Paint += (_, e) =>
         {
            foreach (var (rectangle, word) in textBox.RectangleWords(e.Graphics))
            {
               Console.WriteLine(word);
               textBox.DrawHighlight(e.Graphics, rectangle, Color.Black, Color.CadetBlue);
            }
         };
         form.ShowDialog();
      }

      [TestMethod]
      public void ExTextBoxTest4()
      {
         var form = new Form();
         var textBox = new ExTextBox(form)
         {
            RefreshOnTextChange = true,
            ForeColor = Color.White,
            BackColor = Color.Blue
         };
         textBox.SetUp(0, 0, form.ClientSize.Width, 30);
         textBox.Paint += (_, e) =>
         {
            foreach (var (rectangle, _) in textBox.RectangleWords(e.Graphics))
            {
               textBox.DrawHighlight(e.Graphics, rectangle, Color.White, Color.White, DashStyle.Dot);
            }
         };
         form.ShowDialog();
      }

      [TestMethod]
      public void BackgroundTest()
      {
         var running = false;
         var text = "a";

         var form = new Form();
         var uiAction = new UiAction(form, true);
         uiAction.SetUp(0, 0, 400, 40, AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
         uiAction.Message("Ready");
         uiAction.Click += (_, _) =>
         {
            if (running)
            {
               running = false;
               uiAction.Success("Done");
            }
            else
            {
               uiAction.ClickText = "Stop";
               running = true;
               uiAction.RunWorkerAsync();
            }
         };
         uiAction.ClickText = "Start";
         uiAction.DoWork += (_, _) =>
         {
            while (running)
            {
               uiAction.Do(() => uiAction.Busy(text));
               Application.DoEvents();
               text = text.Succ();
               Thread.Sleep(500.Milliseconds());
            }
         };

         form.ShowDialog();
      }

      [TestMethod]
      public void LabeledTest()
      {
         var form = new Form();
         var uiAction = new UiAction(form, true);
         form.Controls.Add(uiAction);
         uiAction.SetUp(0, 0, 400, 40);
         uiAction.LabeledText("Name", "July Maintenance Window");
         form.ShowDialog();
      }

      [TestMethod]
      public void LabeledLineTest()
      {
         var form = new Form();
         var uiAction = new UiAction(form, true);
         uiAction.SetUp(0, 0, 400, 40);
         uiAction.LabeledText("Name", "July Maintenance Window", line: true);
         form.ShowDialog();
      }

      [TestMethod]
      public void TextBoxLabelTest()
      {
         var form = new Form();
         var textBox = new TextBox();
         form.Controls.Add(textBox);
         textBox.Location = new Point(0, 30);
         textBox.Width = form.ClientSize.Width;

         var uiAction = new UiAction(form, true);
         uiAction.AttachTo("Name", textBox);

         form.ShowDialog();
      }

      [TestMethod]
      public void UiActionButtonTest()
      {
         var form = new Form();
         var uiButton = new UiAction(form, true);
         uiButton.SetUp(0, 0, 200, 40);
         uiButton.Button("Push me");
         uiButton.Click += (_, _) => form.Text = "Clicked";
         uiButton.ClickText = "Click me";
         form.ShowDialog();
      }

      [TestMethod]
      public void UiActionWorkingTest()
      {
         var form = new Form();
         var uiButton = new UiAction(form, true);
         uiButton.SetUp(0, 0, 200, 40);
         uiButton.Message("Not Working");
         uiButton.Click += (_, _) =>
         {
            uiButton.Working = !uiButton.Working;
            uiButton.Message(uiButton.Working ? "Working" : "Not Working");
         };
         uiButton.ClickText = "Toggle working";
         form.ShowDialog();
      }
   }
}