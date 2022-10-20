using Core.Applications;
using Core.Enumerables;
using Core.Markup.Rtf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;
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

         _ = document + "Testing\n" + format() + Alignment.Left + timesFont;

         var queue = document + "Test2: Character Formatting" + format() + timesFont + para + formatTemplate("^^^^^^");
         var _formatter = queue.Dequeue();
         if (!_formatter)
         {
            throw fail("Couldn't extract text");
         }

         _ = ~_formatter + blueColor.Foreground + redColor.Background + 18f + para + format("Character Formatting; u") + Feature.Bold +
            Feature.Underline + courierFont;

         var paragraph = document + "Footnote";
         _ = paragraph.Footnote(7) + "Footnote details here.";

         paragraph = document.Footer + "Test : Page: / Date: Time:" + format() + Alignment.Center + 15f + para;
         paragraph.ControlWorlds("Test : Page: @/# Date:? Time:!");

         _ = document.Header + "Header";

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

         queue = document + "Test 7.1: Hyperlink to target (Test9)" + formatTemplate("          ^^^^^^^^^");
         _formatter = queue.Dequeue();
         if (_formatter)
         {
            _ = ~_formatter + "target".Link("Link to target") + blueColor.Foreground;
         }

         _ = document + "New page" + format() + Feature.NewPage;

         queue = document + "Test9: Set bookmark" + formatTemplate("^^^^^^^^^^^^^^^^^^^");
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
         var document = new Document();
         var table = document.Table(12);
         var blueColor = document.Color("blue");

         foreach (var (link, linkTip) in array("http://foobar".Link("Pull Request"), "http://evokeps".Link("estreamps"),
                     "http://evokeuat".Link("staging10ua")))
         {
            _ = table.Row() + (linkTip, Feature.Bold) + (link, blueColor, link.Link());
         }

         document.Save(@"C:\Temp\Test2.rtf");
      }

      [TestMethod]
      public void RtfTemplateTest()
      {
         var resources = new Resources<RtfTests>();
         var source = resources.String("rtf-template.txt");

         var template = new RtfTemplate();
         var _document = template.Render(source);
         if (_document)
         {
            (~_document).Save(@"C:\Temp\Test3.rtf");
         }
         else
         {
            throw _document.Exception;
         }
      }

      [TestMethod]
      public void BulletTest()
      {
         var document = new Document();
         _ = document + "Top";
         _ = document + "Item 1" + format() + Feature.Bullet;
         _ = document + "Item 2" + format() + Feature.Bullet;
         _ = document + "Item 3" + format() + Feature.Bullet;
         _ = document + "Bottom";
         document.Line();
         _ = document + "Under the line";

         document.Save(@"C:\Temp\Test3.rtf");
      }

      [TestMethod]
      public void FullDocument()
      {
         var document = new Document(paperOrientation: PaperOrientation.Landscape);

         var calibriFont = document.Font("Calibri");
         var highlightColor = document.Color("yellow");
         var urlColor = document.Color("blue");
         var standardFontData = new FontData(calibriFont) { FontSize = 11f };
         var emphasizedFontData = new FontData(standardFontData) { Bold = true, Italic = true, BackgroundColor = highlightColor };
         var urlFontData = new FontData(standardFontData) { Underline = true, ForegroundColor = urlColor };
         document.DefaultCharFormat.FontData = standardFontData;

         _ = document + "Estream Uncleansed database (EstreamPrd replica) – TSESTMUTL10CORP “migrationtest” (non-AG) – Update" + format() +
            Feature.Bold + Feature.Underline;
         _ = document + "";

         var indent1 = 0.5f.InchesToPoints().FirstLineIndent();
         var indent2 = 1f.InchesToPoints().FirstLineIndent();

         _ = document +
            "a.\tRun partition cleanse steps for IntervalMeasurement data older than approx 2 years (2020-10-15) -- Completed" + format() + indent1 +
            para + formatFind("Completed") + emphasizedFontData;
         _ = document + "b.\tRun Partition cleanse for log tables -- Completed" + format() + indent1 +
            para + formatFind("Completed") + emphasizedFontData;
         _ = document + "i.\tRun partition cleanse steps for HierarchicalLog older than 60 days (2022-08-20)" + format() + indent2;
         _ = document + "ii.\tRun partition cleanse steps for DistributionLog older than 45 days (2022-09-04)" + format() + indent2;

         var url = "http://tfs.eprod.com/LS/_git/Estream/pullrequest/26899";
         _ = document + $"c.\tDeploy the code base from {url} -- In-progress" + format() + indent1 +
            para + formatFind(url) + url.Link() + urlFontData +
            para + formatFind("In-progress") + Feature.Bold + Feature.Italic;

         _ = document + "d.\tRun partition reorganization steps for IntervalMeasurement to monthly. Monitor the TLOG fullness" + format() + indent1 +
            para + formatFind("Monitor the TLOG fullness") + Feature.Bold + 14f;

         document.Save(@"C:\Temp\Test4.rtf");
      }
   }
}