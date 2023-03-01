using System;
using System.Windows.Forms;
using Core.Computers;
using Core.Dates;
using Core.Numbers;
using Core.WinForms.Controls;
using Core.WinForms.Documents;
using static Core.Monads.MonadFunctions;

namespace WinFormsTest;

public partial class Form1 : Form
{
   protected UiAction uiAction;

   public Form1()
   {
      InitializeComponent();

      uiAction = new UiAction(this, true);
      uiAction.SetUpInPanel(panel1);
      uiAction.Message("Progress /arrow /paws-left.end/paws-right");

      FileName sourceFile = @"C:\Temp\GoogleChromeStandaloneEnterprise_108.0.5359.125_x64_tw60560-67391.msi";
      FolderName targetFolder = @"C:\Users\tebennett\Working";

      uiAction.Click += (_, _) =>
      {
         sourceFile.Percentage += (_, e) => uiAction.Progress((int)e.Percentage);
         sourceFile.Finished += (_, e) =>
         {
            uiAction.SubText(e.BytesTransferred.ByteSize()).Set.GoToUpperLeft(8);
            uiAction.SubText(e.ElapsedTime.ToLongString(true)).Set.GoToUpperRight(8);
         };
         uiAction.RunWorkerAsync();
      };
      uiAction.ClickText = "CopyFile";
      uiAction.DoWork += (_, _) =>
      {
         var _result = sourceFile.CopyToNotify(targetFolder);
         uiAction.Result(_result.Map(_ => "Copied"));
      };

      var menus = new FreeMenus { Form = this };
      menus.Menu("File");
      var item = menus.Menu("Alpha", (_, _) => uiAction.Message("Alpha"));
      menus.Menu(item, "Bravo", (_, _) => uiAction.Message("Bravo"));
      menus.Menu("Charlie", (_, _) => uiAction.Message("Charlie"));
      menus.RenderMainMenu();
   }

   protected void button1_Click(object sender, EventArgs e)
   {
      uiAction.Exception(fail("Testing exception"));
   }

   protected void button2_Click(object sender, EventArgs e)
   {
      uiAction.ToolTipTitle = "Title";
   }

   protected void button3_Click(object sender, EventArgs e)
   {
      uiAction.Success("Success!");
   }
}