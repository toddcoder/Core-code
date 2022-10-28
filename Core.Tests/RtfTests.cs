using Core.Applications;
using Core.Enumerables;
using Core.Markup.Rtf;
using Core.Monads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Markup.Rtf.FormatterFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Tests
{
   [TestClass]
   public class RtfTests
   {
      [TestMethod]
      public void BasicTest()
      {
         var document = new Document(PaperSize.A4, PaperOrientation.Landscape);

         var timesFont = document.Font("Times New Roman");
         var courierFont = document.Font("Courier New");

         var redColor = document.Color(0xff0000);
         var blueColor = document.Color("Blue");

         var tableHeaderColor = document.Color(0x76923C);
         var tableRowColor = document.Color(0xD6E3BC);
         var tableRowAltColor = document.Color(0xFFFFFF);

         _ = document | "Testing\n" | Alignment.Left | timesFont;

         var queue = document | "Test2: Character Formatting" | timesFont | para | formatTemplate("^^^^^^");
         var _formatter = queue.Dequeue();
         if (!_formatter)
         {
            throw fail("Couldn't extract text");
         }

         _ = ~_formatter | blueColor.Foreground | redColor.Background | 18f | para | format("Character Formatting; u") | Feature.Bold |
            Feature.Underline | courierFont;

         var paragraph = document | "Footnote";
         _ = paragraph.Footnote(7) | "Footnote details here.";

         _ = document.Footer | "/Test : Page: @/# Date:? Time:!" | Alignment.Center | 15f;
         //paragraph.ControlWorlds("Test : Page: @/# Date:? Time:!");

         _ = document.Header | "Header";

         var image = document.Image(@"C:\Temp\rabbit-mq.jpg");
         image.Width = 130;
         image.StartNewParagraph = true;

         var table = document.Table(415.2f, 12);

         foreach (var row in 5.Times())
         {
            table.Row();
            foreach (var column in 4.Times())
            {
               table.Column($"CELL {row}, {column}");
            }
         }

         table.Margins[Direction.Bottom] = 20;
         table.SetInnerBorder(BorderStyle.Dotted, 1);
         table.SetOuterBorder(BorderStyle.Single, 2);

         table.HeaderBackgroundColor = tableHeaderColor;
         table.RowBackgroundColor = tableRowColor;
         table.RowAltBackgroundColor = tableRowAltColor;

         table.Merge(1, 0, 3, 1);
         table[4, 3].BackgroundColor = redColor;
         table[4, 3].Text = "Table";

         queue = document | "Test 7.1: Hyperlink to target (Test9)" | formatTemplate("          ^^^^^^^^^");
         _formatter = queue.Dequeue();
         if (_formatter)
         {
            _ = ~_formatter | "target".Link("Link to target") | blueColor.Foreground;
         }

         _ = document | "New page" | Feature.NewPage;

         queue = document | "Test9: Set bookmark" | formatTemplate("^^^^^^^^^^^^^^^^^^^");
         _formatter = queue.Dequeue();
         if (_formatter)
         {
            (~_formatter).Bookmark("target");
         }

         document.Save(@"C:\Temp\Test.rtf");
      }

      [TestMethod]
      public void TableDataTest()
      {
         var margins = (8f, nil, nil, nil);
         var col1 = new Style() | Feature.Bold | margins;
         var col2 = new Style() | margins;
         var document = new Document();
         var table = document.Table(12);
         _ = table | "Pull Request" | col1 | "" | "http://foobar".Link() | col2;
         _ = table | "estreamps" | col1 | "" | "http://evokeps".Link() | col2;
         _ = table | "staging10ua" | col1 | "" | "http://evokeuat".Link() | col2;

         table.SetOuterBorder(BorderStyle.Double, 1);
         table.SetInnerBorder(BorderStyle.Single, 1);

         document.Save(@"C:\Temp\Test2.rtf");
      }

      [TestMethod]
      public void BulletTest()
      {
         var document = new Document();
         _ = document | "Top";
         _ = document | "Item 1" | Feature.Bullet;
         _ = document | "Item 2" | Feature.Bullet;
         _ = document | "Item 3" | Feature.Bullet;
         _ = document | "Bottom";
         document.Line();
         _ = document | "Under the line";

         document.Save(@"C:\Temp\Test3.rtf");
      }

      [TestMethod]
      public void FullDocument()
      {
         var document = new Document(paperOrientation: PaperOrientation.Landscape);

         var calibriFont = document.Font("Calibri");
         var highlightColor = document.Color("yellow");
         var standardStyle = new Style() | calibriFont | 11f;
         var emphasizedStyle = new Style() | standardStyle | Feature.Bold | Feature.Italic | highlightColor.Background;
         document.DefaultCharFormat.Style = standardStyle;

         _ = document | "Estream Uncleansed database (EstreamPrd replica) – TSESTMUTL10CORP “migrationtest” (non-AG) – Update" | Feature.Bold |
            Feature.Underline;
         _ = document | "";

         var indent1 = 0.5f.InchesToPoints().FirstLineIndent();
         var indent2 = 1f.InchesToPoints().FirstLineIndent();

         _ = document |
            "a.\tRun partition cleanse steps for IntervalMeasurement data older than approx 2 years (2020-10-15) -- Completed" | indent1 |
            para | formatFind("Completed") | emphasizedStyle;
         _ = document | "b.\tRun Partition cleanse for log tables -- Completed" | indent1 |
            para | formatFind("Completed") | emphasizedStyle;
         _ = document | "i.\tRun partition cleanse steps for HierarchicalLog older than 60 days (2022-08-20)" | indent2;
         _ = document | "ii.\tRun partition cleanse steps for DistributionLog older than 45 days (2022-09-04)" | indent2;

         var url = "http://tfs.eprod.com/LS/_git/Estream/pullrequest/26899";
         _ = document | "c.\tDeploy the code base from /url(pr) -- In-progress" | indent1 |
            para | formatUrl("pr") | url.Link("PR") |
            para | formatFind("In-progress") | Feature.Bold | Feature.Italic;

         _ = document | "d.\tRun partition reorganization steps for IntervalMeasurement to monthly. Monitor the TLOG fullness" | indent1 |
            para | formatFind("Monitor the TLOG fullness") | Feature.Bold | 14f;

         document.Save(@"C:\Temp\Test4.rtf");
      }

      [TestMethod]
      public void HyperlinkTest()
      {
         var document = new Document();
         _ = document | "This is the url to /url(google) is here!" | formatUrl("google") | "http://google.com".Link("Google");
         document.Save(@"C:\Temp\Test5.rtf");
      }

      [TestMethod]
      public void HyperlinkTest2()
      {
         Maybe<string> _workItemId = "301905";

         var document = new Document();
         var font = document.Font("Calibri");
         var codeFont = document.Font("Consolas");
         var standardFont = new Style() | font | 12f;
         document.DefaultCharFormat.Style = standardFont;

         var workItemUrl = ~_workItemId;
         var branch = "b-ops-301905-orderdeleted-failed-message";

         _ = document | $"/The backfill request for ^{branch}^ (/url(workItemId)) has been completed." |
            para | formatUrl("workItemId") | codeFont | Feature.Bold | workItemUrl.Link(_workItemId);

         _ = document | "Please merge master into your inflight branches. Thanks.";

         document.Save(@"C:\Temp\Test5a.rtf");
      }

      [TestMethod]
      public void FormattingTest()
      {
         var document = new Document();
         _ = document | "/This is *italic* text";
         _ = document | "/This is ^bold^ text";
         _ = document | "/This is %both%!";
         _ = document | "/This is ^release^ with an ^outage^ for ^r-6.41.0^.";
         document.Save(@"C:\Temp\Test6.rtf");
      }

      [TestMethod]
      public void CopyStyleTest()
      {
         var document = new Document();
         var font = document.Font("Times New Roman");
         var standard = new Style() | font | 12f;
         document.DefaultCharFormat.Style = standard;
         var italic = ~standard | Feature.Italic;
         var bold = ~standard | Feature.Bold;

         _ = document | "This is standard.";
         _ = document | "This is italic." | italic;
         _ = document | "This is bold." | bold;
         document.Save(@"C:\Temp\Test7.rtf");
      }
   }
}