using System;
using System.Windows.Forms;
using Core.Applications.Messaging;
using Core.Computers;
using Core.Dates;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.Documents;
using static Core.Monads.MonadFunctions;
using static Core.WinForms.Documents.MenuBuilderFunctions;

namespace WinFormsTest;

public partial class Form1 : Form, IMessageQueueListener
{
   protected UiAction uiAction;
   protected UiAction uiButton;
   protected EnumerableCycle<CardinalAlignment> messageAlignments;
   protected Maybe<SubText> _subText;
   protected string test;

   public Form1()
   {
      InitializeComponent();

      UiAction.BusyStyle = BusyStyle.Sine;

      uiAction = new UiAction(this);
      uiAction.SetUpInPanel(panel1);
      uiAction.Message("Progress /arrow /paws-left.end/paws-right");

      FileName sourceFile = @"C:\Temp\GoogleChromeStandaloneEnterprise_108.0.5359.125_x64_tw60560-67391.msi";
      FolderName targetFolder = @"C:\Users\tebennett\Working";

      messageAlignments = new EnumerableCycle<CardinalAlignment>(new[]
      {
         CardinalAlignment.Center, CardinalAlignment.West, CardinalAlignment.East, CardinalAlignment.North, CardinalAlignment.South,
         CardinalAlignment.NorthWest, CardinalAlignment.NorthEast, CardinalAlignment.SouthWest, CardinalAlignment.SouthEast
      });
      _subText = nil;
      test = "";

      uiAction.Click += (_, _) =>
      {
         uiAction.ClickToCancel = true;
         sourceFile.Percentage += (_, e) => uiAction.Progress((int)e.Percentage);
         sourceFile.Finished += (_, e) =>
         {
            uiAction.SubText(e.BytesTransferred.ByteSize()).Set.GoToUpperLeft(8);
            uiAction.SubText(e.ElapsedTime.ToLongString(true)).Set.GoToUpperRight(8);
         };
         uiAction.RunWorkerAsync();
      };
      uiAction.ClickText = "CopyFile";
      //uiAction.ClickToCancel = true;
      uiAction.DoWork += (_, _) =>
      {
         var _result = sourceFile.CopyToNotify(targetFolder);
         uiAction.Result(_result.Map(_ => "Copied"));
      };
      uiAction.RunWorkerCompleted += (_, _) => uiAction.ClickToCancel = false;

      uiButton = new UiAction(this);
      uiButton.SetUpInPanel(panel2);
      uiButton.Image = imageList1.Images[0];
      uiButton.CardinalAlignment = CardinalAlignment.Center;
      uiButton.Click += (_, _) => { };
      uiButton.ClickText = "Click";
      uiButton.ClickGlyph = false;

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
      uiAction.Message("Test");
      uiAction.Maximum = 20;
      uiAction.ProgressStripe = true;
   }

   protected void button2_Click(object sender, EventArgs e)
   {
      uiAction.Progress();
   }

   protected void button3_Click(object sender, EventArgs e)
   {
      uiAction.Enabled = !uiAction.Enabled;
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