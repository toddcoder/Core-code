using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Core.Applications.Messaging;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using Core.Strings.Emojis;
using Core.WinForms;
using Core.WinForms.Controls;
using Core.WinForms.Documents;
using static Core.Monads.MonadFunctions;
using static Core.WinForms.Documents.MenuBuilderFunctions;

namespace WinFormsTest;

public partial class Form1 : Form, IMessageQueueListener
{
   protected UiAction uiAction;
   protected UiAction uiButton;
   protected UiAction uiTest;
   protected EnumerableCycle<CardinalAlignment> messageAlignments;
   protected Maybe<SubText> _subText;
   protected string test;
   protected ExTextBox textBox;

   public Form1()
   {
      InitializeComponent();

      UiAction.BusyStyle = BusyStyle.BarberPole;

      uiAction = new UiAction(this) { AutoSizeText = true };
      uiAction.SetUpInPanel(panel1);
      uiAction.Message("Progress /arrow /paws-left.end/paws-right");
      uiAction.Click += (_, _) => uiAction.Refresh();
      uiAction.ClickText = "Refresh";

      FileName sourceFile = @"C:\Temp\GoogleChromeStandaloneEnterprise_108.0.5359.125_x64_tw60560-67391.msi";
      FolderName targetFolder = @"C:\Users\tebennett\Working";

      messageAlignments = new EnumerableCycle<CardinalAlignment>(new[]
      {
         CardinalAlignment.Center, CardinalAlignment.West, CardinalAlignment.East, CardinalAlignment.North, CardinalAlignment.South,
         CardinalAlignment.NorthWest, CardinalAlignment.NorthEast, CardinalAlignment.SouthWest, CardinalAlignment.SouthEast
      });
      _subText = nil;
      test = "";

      textBox = new ExTextBox(this);
      textBox.SetUpInPanel(panel4);
      textBox.Allow = (Pattern)"^/d*$";
      textBox.RefreshOnTextChange = true;
      textBox.Paint += (_, e) =>
      {
         if (!textBox.IsAllowed)
         {
            using var pen = new Pen(Color.Red, 4);
            pen.DashStyle = DashStyle.Dot;
            var point1 = ClientRectangle.Location;
            var point2 = point1 with { X = ClientRectangle.Right };
            e.Graphics.DrawLine(pen, point1, point2);
         }
      };

      /*uiAction.Click += (_, _) =>
      {
         var _ = uiAction
            .Choose("Test")
            .Choices("f-acct-203518-intercompanycustomerreport", "f-acct-203518-intercompanycustomerreport-2r",
               "Selection dropdown for resolution branches in working window needs to be wider")
            .SizeToText(true).Choose();
      };
      uiAction.ClickText = "CopyFile";*/
      //uiAction.ClickToCancel = true;
      /*uiAction.DoWork += (_, _) =>
      {
         var _result = sourceFile.CopyToNotify(targetFolder);
         uiAction.Result(_result.Map(_ => "Copied"));
      };
      uiAction.RunWorkerCompleted += (_, _) => uiAction.ClickToCancel = false;*/

      /*uiButton = new UiAction(this);
      uiButton.SetUpInPanel(panel2);
      uiButton.Image = imageList1.Images[0];
      uiButton.CardinalAlignment = CardinalAlignment.Center;
      uiButton.Click += (_, _) => { };
      uiButton.ClickText = "Click";
      uiButton.ClickGlyph = false;

      uiTest = new UiAction(this);
      uiTest.SetUpInPanel(panel3);
      uiTest.Message("Test");*/

      MessageQueue.RegisterListener(this, "button1", "button2", "button3");

      var menus = new FreeMenus { Form = this };
      menus.Menu("File");
      _ = menus + "Alpha" + (() => uiAction.Message("Alpha")) + Keys.Control + Keys.A + menu;
      var restItem = menus + "Rest of the alphabet" + subMenu;
      _ = menus + restItem + "Bravo" + (() => uiAction.Message("Bravo")) + Keys.Alt + Keys.B + menu;
      _ = menus + ("File", "Charlie") + (() => uiAction.Message("Charlie")) + Keys.Shift + Keys.Control + Keys.C + menu;
      menus.RenderMainMenu();

      var contextMenus = new FreeMenus { Form = this };
      _ = contextMenus + "Copy" + (() => textBox1.Copy()) + Keys.Control + Keys.Alt + Keys.C + contextMenu;
      _ = contextMenus + "Paste" + (() => textBox1.Paste()) + Keys.Control + Keys.Alt + Keys.P + contextMenu;
      contextMenus.CreateContextMenu(textBox1);
   }

   protected void button1_Click(object sender, EventArgs e)
   {
      uiAction.Success("Fixed project for f-ct-remove-unneeded-report-config-grp3-2r");
      uiAction.Legend("pull request");
      uiAction.Refresh();
      uiAction.SubText("r-6.51.0-grp3 |20").Set.Alignment(CardinalAlignment.NorthEast).ForeColor(Color.White).BackColor(Color.Magenta).Outline()
         .FontSize(8);
      uiAction.Refresh();
      uiAction.SubText("1 file saved").Set.Alignment(CardinalAlignment.SouthWest).FontSize(8);
      uiAction.Refresh();
   }

   protected void button2_Click(object sender, EventArgs e)
   {
      uiAction.Success("Open Pull Request");
      uiAction.Legend("pull request");
      uiAction.SubText("http://tfs/LS/_git/Estream/pullrequest/30868?_a=overview").Set.Alignment(CardinalAlignment.SouthWest).FontSize(8);
      uiAction.Refresh();
   }

   protected void button3_Click(object sender, EventArgs e)
   {
     uiAction.Busy("Migration_202308022023_1003_Delete_Everything_That_You_Care_About.sql");
     uiAction.ClickToCancel = true;
     uiAction.Stopwatch = true;
     uiAction.StartStopwatch();
   }

   public string Listener => "form1";

   public void MessageFrom(string sender, string subject, object cargo)
   {
      switch (sender)
      {
         case "button1" when subject == "add" && cargo is string string2:
            test += string2;
            break;
         case "button2" when subject == "keep" && cargo is int count:
            test = test.Keep(count);
            break;
         case "button3" when subject == "drop" && cargo is int count:
            test = test.Drop(count);
            break;
      }

      uiAction.Message($"/left-angle.{test}/right-angle");
   }
}