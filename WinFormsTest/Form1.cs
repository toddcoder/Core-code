using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Applications.Messaging;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
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
      uiAction.SubText("1 file saved").Set.Alignment(CardinalAlignment.SouthWest).FontSize(8).Invert();
      uiAction.Refresh();
   }

   protected void button2_Click(object sender, EventArgs e)
   {
      uiAction.Success("Open Pull Request");
      uiAction.Legend("pull request");
      uiAction.SubText("http://tfs/LS/_git/Estream/pullrequest/30868?_a=overview").Set.Alignment(CardinalAlignment.SouthWest).FontSize(8)
         .IncludeFloor(false);
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

   protected void button4_Click(object sender, EventArgs e)
   {
      uiAction.KeyMatch("down", "up");
      uiAction.Click += (_, _) =>
      {
         if (uiAction.IsKeyDown)
         {
            uiAction.Message("Down");
         }
         else
         {
            uiAction.Message("Up");
         }
      };
      uiAction.ClickText = "Up or down";
   }
}