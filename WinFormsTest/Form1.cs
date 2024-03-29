﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Applications.Messaging;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.Documents;
using static Core.Lambdas.LambdaFunctions;
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
   protected StringSet set;

   public Form1()
   {
      set = new StringSet(true)
      {
         "Foundation",
         "Foundation",
         "Estream.Migrations",
         "Foundation.Testing",
         "Estream.ReportProxy.Contracts",
         "Estream.Distributions.L10N",
         "Foundation.Legacy",
         "Estream.Common",
         "Estream.Common",
         "Estream.Measurements.Common",
         "Estream.Common.Legacy",
         "Estream.Administration",
         "Estream.Migrations.ManualMigrationRunner",
         "Estream.Web.UITests",
         "Estream.Administration",
         "Estream.Web.PerformanceTests",
         "Estream.Accounting",
         "Estream.Accounting",
         "Estream.Distributions.L10N",
         "Estream.Measurements.Common",
         "Estream.AlertAndNotification.Testing",
         "Estream.Accounting.TestCommon",
         "Estream.Accounting.Backend.Processing",
         "Estream.Solution.UnitTests",
         "Estream.Common.Orleans",
         "Estream.Public.MasterData",
         "EStream.MasterData",
         "Estream.Interfaces",
         "Estream.Common.Orleans",
         "Estream.Contracts",
         "Estream.FastSearch.Foundation",
         "Foundation.UnitTests",
         "Estream.Nominations",
         "Foundation.IntegrationTests",
         "Estream.Accounting.TestCommon",
         "Estream.Accounting.Backend",
         "Estream.Accounting.Backend.Processing",
         "Estream.Contracts",
         "Estream.FastSearch.Foundation",
         "Estream.FastSearch.TestSupport",
         "Estream.Pricing",
         "Estream.Measurements.FastSearch",
         "Estream.Security.Testing",
         "Estream.AlertAndNotification",
         "Estream.Security.Testing.UnitTests",
         "Estream.Measurements",
         "Estream.Terminals",
         "Estream.Public.MasterData",
         "Estream.Accounting.Service",
         "Estream.Accounting.Backend",
         "EStream.MasterData",
         "Foundation.Testing",
         "Estream.Interfaces.Backend",
         "Estream.MigrationTests",
         "Estream.Common.Timer",
         "Estream.AlertAndNotification",
         "Estream.Common.SiloService",
         "Estream.Interfaces",
         "Estream.MasterData.Orleans",
         "Estream.Nominations",
         "Estream.Accounting.Denormalization.Service",
         "Estream.Operations",
         "Estream.Operations.Test.Common",
         "Estream.Accounting.Orleans",
         "Estream.Testing",
         "Estream.MasterData.Orleans",
         "Estream.Interfaces.Oracle",
         "Estream.NominationService",
         "Estream.Accounting.Orleans",
         "Estream.Contracts.Service",
         "Estream.Terminals.AuditService",
         "Estream.Solution.UnitTests",
         "Foundation.IntegrationTests",
         "Estream.Accounting.Reprocessing.Service",
         "Estream.Interfaces.Backend",
         "Estream.Terminals.TestCommon",
         "Estream.Accounting.Service",
         "Estream.Terminals.AllocationService",
         "Estream.Accounting.Denormalization.Service",
         "Foundation.UnitTests",
         "Estream.MasterData.Test.Common",
         "Estream.Interfaces.Harness.OracleAdvancedQueue",
         "Estream.Contracts.Denormalization.Service",
         "Estream.Common.Timer",
         "Estream.Contracts.Service",
         "Estream.Measurements.FastSearch",
         "Estream.FastSearch.TestSupport",
         "Estream.FastSearch.DataLoader",
         "Estream.Pricing",
         "Estream.ReportProxy.Contracts",
         "Estream.Measurements.FastSearch.DailyMeasurementLoaderService",
         "Estream.Contracts.Denormalization.Service",
         "Estream.NominationsLite",
         "Estream.Measurements.FastSearch.BatchMeasurementLoaderService",
         "Estream.Measurements.FastSearch.IntervalMeasurementLoaderService",
         "Estream.Audit",
         "Estream.FastSearch.Migrator",
         "Estream.Audit",
         "Estream.Distributions",
         "Estream.AuditResult",
         "Estream.FastSearch.DataLoader.IntegrationTests",
         "Estream.Distributions.DispatcherService",
         "Estream.Audit.UnitTests",
         "Estream.Contracts.TestCommon",
         "Estream.FastSearch.ManualTestRunner",
         "Estream.Accounting.JobScheduler.Service",
         "Estream.Distributions",
         "Estream.FastSearch.Migrator.IntegrationTests",
         "Estream.Nominations.Test.Common",
         "Estream.Measurements",
         "Estream.Interfaces.EccrpService",
         "Estream.Audit.Service",
         "Estream.NominationsLite",
         "Estream.AlertAndNotification.Orleans",
         "Estream.Distributions.Orleans",
         "Estream.Operations",
         "Estream.Interfaces.Test.Common",
         "Estream.Interfaces.Harness",
         "Estream.Common.UnitTests",
         "Estream.MasterData.Test.Common",
         "Estream.Operations.Orleans",
         "Estream.Administration.Test.Common",
         "Estream.Interfaces.Orleans",
         "Estream.Terminals.Service",
         "Estream.Interfaces.OracleAdapterService",
         "Estream.Distributions.DistributionAuditService",
         "Estream.MaterialBalance",
         "Estream.Interfaces.Orleans",
         "Estream.Interfaces.Harness",
         "Estream.Interfaces.EccrpService",
         "Estream.Measurements.Services.Domain",
         "Estream.Interfaces.Test.Common",
         "Estream.Measurements.Services.Domain",
         "Estream.Measurements.Test.Common",
         "Estream.NominationsLite.UnitTests",
         "Estream.Accounting.JobScheduler.Service",
         "Estream.Testing",
         "Estream.Operations.Orleans",
         "Estream.Measurements.Service",
         "Estream.Terminals.AvailabilityService",
         "Estream.MaterialBalance.Orleans",
         "Estream.Administration.Service",
         "Estream.MaterialBalance.Test.Common",
         "Estream.MaterialBalance.LabAnalysisService",
         "Estream.Operations.Test.Common",
         "Estream.MaterialBalance.Priority.Domain.Service",
         "Estream.MaterialBalance.Denormalization.Service",
         "Estream.Common.IntegrationTests",
         "Estream.Distributions.Orleans",
         "Estream.Pricing.Service",
         "Estream.Interfaces.TicketImportService",
         "Estream.Interfaces.TopHatAdapterService",
         "Estream.Distributions.DistributionSummaryService",
         "Estream.Distributions.ManualDistributionService",
         "Estream.Distributions.DistributionAuditService",
         "Estream.Distributions.DistributionBeginningBalanceService",
         "Estream.Pricing.UnitTests",
         "Estream.Measurements.UnitTests",
         "Estream.FastSearch.Foundation.UnitTests",
         "Estream.FastSearch.DataLoader.UnitTests",
         "Estream.Distributions.DistributionEventsService",
         "Estream.Measurements.IntegrationTests",
         "Estream.MasterData.UnitTests",
         "Foundation.Testing.Legacy",
         "Estream.Interfaces.SiloService",
         "Estream.Distributions.DispatcherService",
         "Estream.Operations.Service",
         "Estream.Operations.BatchTrackingService",
         "Estream.Operations.Service",
         "Estream.Operations.BatchTrackingService",
         "Estream.Interfaces.PiSystemAdapterService",
         "Estream.NominationService",
         "Estream.Distributions.DistributionProcessingService",
         "Estream.Interfaces.LimsAdapterService",
         "Estream.Distributions.DistributionAvailabilityService",
         "Estream.Interfaces.FciAdapterService",
         "Estream.Interfaces.EvttsAdapterService",
         "Estream.FastSearch.Migrator.UnitTests",
         "Estream.Interfaces.PiSystemAdapterService",
         "Estream.Interfaces.LimsAdapterService",
         "Estream.Interfaces.FciAdapterService",
         "Estream.Interfaces.EvttsAdapterService",
         "Estream.Accounting.Reprocessing.Service",
         "Estream.Interfaces.SiloService",
         "Estream.Interfaces.TicketImportService",
         "Estream.Interfaces.TopHatAdapterService",
         "Estream.Administration.IntegrationTests",
         "Estream.Terminals",
         "Estream.Operations.UnitTests",
         "Estream.Distributions.Legacy",
         "Estream.Operations.IntegrationTests",
         "Estream.Common.Legacy.UnitTests",
         "Estream.FastSearch.Foundation.UnitTests",
         "Estream.MaterialBalance",
         "Estream.Measurements.Service",
         "Estream.Distributions.DistributionBeginningBalanceService",
         "Estream.Interfaces.ScheduleImportService",
         "Estream.Interfaces.ScheduleImportService",
         "Estream.MaterialBalance.Legacy",
         "Estream.MaterialBalance.Services.OverShort",
         "Estream.MaterialBalance.Services.Domain",
         "Estream.MaterialBalance.TicketingSubscriptionService",
         "Estream.AuditResult",
         "Estream.Distributions.ManualDistributionService",
         "Estream.Distributions.DistributionSummaryService",
         "Estream.Distributions.Service",
         "Estream.Terminals.Service",
         "Estream.Distributions.DistributionEventsService",
         "Estream.Distributions.DistributionAvailabilityService",
         "Estream.MaterialBalance.Test.Common",
         "Estream.Audit.Service",
         "Estream.Distributions.DistributionProcessingService",
         "Estream.Audit.UnitTests",
         "Estream.AlertAndNotification.Service",
         "Estream.MaterialBalance.Denormalization.Service",
         "Estream.MaterialBalance.Priority.Domain.Service",
         "Estream.MaterialBalance.Services.OverShort",
         "Estream.Common.JobService",
         "Estream.Terminals.AuditService",
         "Estream.ReportProxy.Web",
         "Estream.Pricing.IntegrationTests",
         "Estream.Terminals.AvailabilityService",
         "Estream.Interfaces.IntegrationTests",
         "Estream.MaterialBalance.UnitTests",
         "Estream.Accounting.IntegrationTests",
         "Estream.Nominations.IntegrationTests",
         "Estream.MaterialBalance.PersistAndPublishApi",
         "Estream.Contracts.UnitTests",
         "Estream.MaterialBalance.Orleans",
         "Estream.MaterialBalance.IntegrationTests",
         "Estream.MaterialBalance.LabAnalysisService",
         "Estream.Terminals.TestCommon",
         "Estream.Terminals.AllocationService",
         "Estream.Interfaces.UnitTests",
         "Estream.Common.UnitTests",
         "Estream.Web",
         "Estream.Contracts.TestCommon",
         "Estream.Pricing.IntegrationTests",
         "Estream.Administration.Test.Common",
         "Estream.Measurements.Test.Common",
         "Estream.Distributions.UnitTests",
         "Estream.Pricing.Service",
         "Estream.Common.IntegrationTests",
         "Estream.Measurements.FastSearch.IntegrationTests",
         "Estream.Interfaces.UnitTests",
         "Estream.MasterData.UnitTests",
         "Estream.Contracts.IntegrationTests",
         "Estream.Accounting.IntegrationTests",
         "Estream.Pricing.UnitTests",
         "Estream.Operations.IntegrationTests",
         "Estream.MaterialBalance.TicketingSubscriptionService",
         "Estream.Administration.IntegrationTests",
         "Estream.Interfaces.Legacy.UnitTests",
         "Estream.MaterialBalance.Services.Domain",
         "Estream.Terminals.UnitTests",
         "Estream.Accounting.UnitTests",
         "Estream.Nominations.UnitTests",
         "Estream.Interfaces.IntegrationTests",
         "Estream.NominationsLite.UnitTests",
         "Estream.Administration.UnitTests",
         "Estream.Nominations.Test.Common",
         "Estream.MasterData.IntegrationTests",
         "Estream.Distributions.Service",
         "Estream.MasterData.IntegrationTests",
         "Estream.AlertAndNotification.IntegrationTests",
         "Estream.AlertAndNotification.UnitTests",
         "Foundation.Legacy.UnitTests",
         "Foundation.Legacy.IntegrationTests",
         "Estream.Operations.UnitTests",
         "Estream.Contracts.IntegrationTests",
         "Estream.Terminals.IntegrationTests",
         "Estream.Distributions.IntegrationTests",
         "Estream.Measurements.UnitTests",
         "Estream.AlertAndNotification.Orleans",
         "Estream.Interfaces.Security.IntegrationTests",
         "Estream.Interfaces.Security.IntegrationTests",
         "Estream.Measurements.IntegrationTests",
         "Estream.Audit.IntegrationTests",
         "Estream.Distributions.UnitTests",
         "Estream.Distributions.IntegrationTests",
         "Estream.Terminals.UnitTests",
         "Estream.Nominations.IntegrationTests",
         "Estream.Accounting.UnitTests",
         "Estream.Nominations.UnitTests",
         "Estream.Terminals.IntegrationTests",
         "Estream.Audit.IntegrationTests",
         "Estream.Web.UnitTests",
         "Estream.MaterialBalance.UnitTests",
         "Estream.Contracts.UnitTests",
         "Estream.MaterialBalance.IntegrationTests",
         "Estream.Web.IntegrationTests"
      };

      InitializeComponent();

      UiAction.BusyStyle = BusyStyle.BarberPole;

      uiAction = new UiAction(this) { AutoSizeText = true };
      uiAction.SetUpInPanel(panel1);
      uiAction.Message("Progress /arrow /paws-left.end/paws-right");
      uiAction.Click += (_, _) => uiAction.Refresh();
      //uiAction.ClickText = "Refresh";

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
      textBox.SetUpInPanel(panel4, dockStyle: DockStyle.Fill);
      //textBox.Allow = func<string, bool>(text => text.IsMatch(@"^ 'r-' /(/d+) '.' /(/d+) '.' /(/d+) $; f") || text == "master");
      textBox.AllowMessage = "matching";
      textBox.TrendMessage = "matching 'master'";
      textBox.DenyMessage = "not matching";
      textBox.Validate = func<string, AllowanceStatus>(text =>
      {
         if (text.IsEmpty())
         {
            return AllowanceStatus.Denied;
         }

         if (text.IsMatch(@"^ 'r-' (/d) '.' (/d2) '.' (/d) $; f") || text == "master")
         {
            return AllowanceStatus.Allowed;
         }
         else if ("master".StartsWith(text))
         {
            return AllowanceStatus.Trending;
         }
         else
         {
            return AllowanceStatus.Denied;
         }
      });
      textBox.Trending += (_, e) => textBox.ShadowText = "master".StartsWith(e.Text) ? "master" : "r-#.##.# ";
      textBox.Denied += (_, _) => textBox.ShadowText = "r-#.##.# ";
      textBox.RefreshOnTextChange = true;
      /*textBox.Paint += (_, e) =>
      {
         textBox.ClearSubTexts();
         if (textBox.IsAllowed)
         {
            textBox.SubText("allowed", Color.White, Color.Green).Set.FontSize(8).Alignment(CardinalAlignment.East);
         }
         else
         {
            textBox.SubText("disallowed", Color.White, Color.Red).Set.FontSize(8).Alignment(CardinalAlignment.East);
         }
         /*if (!textBox.IsAllowed)
         {
            using var pen = new Pen(Color.Red, 4);
            pen.DashStyle = DashStyle.Dot;
            var point1 = ClientRectangle.Location;
            var point2 = point1 with { X = ClientRectangle.Right };
            e.Graphics.DrawLine(pen, point1, point2);
         }#1#
      };*/
      textBox.ValidateMessages = true;
      /*textBox.Allowed += (_, _) =>
      {
         textBox.ClearSubTexts();
         textBox.SubText("allowed", Color.White, Color.Green).Set.FontSize(8).Alignment(CardinalAlignment.NorthEast);
      };
      textBox.NotAllowed += (_, e) =>
      {
         if ("master".StartsWith(e.Text))
         {
            textBox.ShadowText = "master";
         }
         else
         {
            textBox.ShadowText = "r-#.##.#";
         }
         textBox.ClearSubTexts();
         textBox.SubText("disallowed", Color.White, Color.Red).Set.FontSize(8).Alignment(CardinalAlignment.NorthEast);
         //textBox.Refresh();
      };*/

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
      uiAction.EmptyTextTitle = "Click to add new items";
      uiAction.AutoSizeText = true;
      uiAction.AlternateDeletable("Merge Request Received", "Merge Request Rejected", "Merged to r-6.51.0-grp1", "Merged to r-6.51.0-grp7a",
         "Add status");
      uiAction.ClickOnAlternate += (_, e) => Text = e.Alternate;
      uiAction.DeleteOnAlternate += (_, e) => uiAction.RemoveAlternate(e.RectangleIndex);
      uiAction.DynamicToolTip += (_, e) =>
      {
         if (e.RectangleIndex is (true, var index))
         {
            e.ToolTipText = index.ToString();
         }

         if (!e.ToolTipText)
         {
            e.ToolTipText = "not there";
         }
      };
   }

   protected void button2_Click(object sender, EventArgs e)
   {
      uiAction.CheckStyle = CheckStyle.Checked;
      uiAction.Legend("action");
      uiAction.ClickToCancel = true;
      //uiAction.Busy("with check");
      uiAction.Maximum = 100;
      uiAction.Stopwatch = true;
      uiAction.StartStopwatch();
      for (var i = 0; i < 100; i++)
      {
         uiAction.Progress(i.ToWords());
      }

      uiAction.Busy(true);
      uiAction.StopStopwatch();
   }

   protected void button3_Click(object sender, EventArgs e)
   {
      uiAction.SetForeColor(0, Color.White);
      uiAction.SetBackColor(0, Color.Blue);
      uiAction.SetForeColor(1, Color.Black);
      uiAction.SetBackColor(1, Color.Gold);
      uiAction.SetForeColor(2, Color.White);
      uiAction.SetBackColor(2, Color.Green);
      uiAction.SetForeColor(3, Color.White);
      uiAction.SetBackColor(3, Color.Green);
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

   protected void button4_Click(object sender, EventArgs e)
   {
      /*textBox.Legend("release");
      textBox.ShadowText = "r-#.##.#";
      textBox.ValidateMessages = true;*/
      textBox.Paint += (_, e) =>
      {
         foreach (var (rectangle, _) in textBox.RectangleWords(e.Graphics))
         {
            textBox.BoxRectangle(e.Graphics, rectangle, Color.White, Color.Green);
         }
      };
   }
}