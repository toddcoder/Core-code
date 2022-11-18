using Core.Markup.Rtf;
using Core.Monads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Markup.Rtf.RtfFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Tests;

[TestClass]
public class RtfTests
{
   [TestMethod]
   public void BasicTest()
   {
      var document = new Document(PaperSize.A4, PaperOrientation.Landscape);
      var timesFont = document.Font("Times New Roman");
      var consolasFont = document.Font("Consolas");
      var goldColor = document.Color("gold");
      var tableHeaderColor = document.Color(0x76923C);
      var tableRowColor = document.Color(0xD6E3BC);
      var tableRowAltColor = document.Color(0xFFFFFF);

      document.DefaultCharFormat.Style = new Style() | timesFont | 12f;

      var header = document.Header;
      _ = header | "Test Document " | none | "Test.rtf" | italic;
      _ = header | "";

      var footer = document.Footer;
      _ = footer | "Page " | page | " of " | numPages;

      _ = document | "This is test line 1";
      _ = document | "This is test line 2";
      _ = document | "This is test line 3";

      _ = document | "";

      _ = document | "Item 1" | bullet;
      _ = document | "Item 2" | bullet;
      _ = document | "Item 3" | bullet;

      _ = document | "";

      _ = document | "Test 1: " | none | "italics" | italic;
      _ = document | "Test 2: " | none | "bold" | bold;
      _ = document | "Test 3: " | none | "underline" | underline;
      _ = document | "Test 4: " | none | "gold" | goldColor.Background;

      _ = document | "";

      var headerStyle = new Style() | center | bold;
      var rowStyle = new Style() | consolasFont | italic;

      var table = document.Table(12f);
      _ = table | "Full Name" | headerStyle | "Latin Letter" | headerStyle | "Greek Letter" | headerStyle;
      _ = table | "alpha" | rowStyle | goldColor.Background | "a" | "alpha" | rowStyle;
      _ = table | "bravo" | "b" | "beta";
      _ = table | "charlie" | "c" | "kappa";
      _ = table | "delta" | rowStyle | "d" | "delta" | rowStyle;
      _ = table | "echo" | "e" | "epsilon";
      _ = table | "foxtrot" | "f" | "phi";

      table.Margins[Direction.Bottom] = 20;
      table.Margins[Direction.Left] = 20;
      table.SetInnerBorder(BorderStyle.Dotted, 1);
      table.SetOuterBorder(BorderStyle.Single, 2);
      table.HeaderBackgroundColor = tableHeaderColor;
      table.RowBackgroundColor = tableRowColor;
      table.RowAltBackgroundColor = tableRowAltColor;

      _ = document | "Test 4: " | newPage | "Character Formatting" | consolasFont | 16f | goldColor.Background;

      _ = document | "";

      var paragraph = document | "Test 5: This is a footnote";
      var footnote = paragraph.Footnote();
      _ = footnote | "Footnote information" | bold;

      var boldStyle = new Style() | timesFont | 12f | bold;
      var url = "http://tfs.eprod.com/ls/estreamuat/_releaseProgress?_a=release-environment-logs&releaseId=6024&environmentId=6024";

      _ = document | "Test 6";
      _ = document | "Please approve deployment to " | none | "staging01ua" | boldStyle | " with link " | url.Link() | " for Hotfix (Rolling) " |
         "r-6.41.10" | boldStyle;

      document.Save(@"C:\Temp\Basic.rtf");
   }

   [TestMethod]
   public void TableDataTest()
   {
      var margins = (8f, nil, nil, nil);
      var colStyle1 = new Style() | bold | margins;
      var colStyle2 = new Style() | margins;
      var document = new Document();
      var table = document.Table(12);
      _ = table | "Pull Request" | colStyle1 | "" | "http://foobar".Link() | colStyle2;
      _ = table | "estreamps" | colStyle1 | "" | "http://evokeps".Link() | colStyle2;
      _ = table | "staging10ua" | colStyle1 | "" | "http://evokeuat".Link() | colStyle2;

      table.SetOuterBorder(BorderStyle.Double, 1);
      table.SetInnerBorder(BorderStyle.Single, 1);

      document.Save(@"C:\Temp\TableData.rtf");
   }

   [TestMethod]
   public void BulletTest()
   {
      var document = new Document();
      _ = document | "Top";
      _ = document | "Item 1" | bullet;
      _ = document | "Item 2" | bullet;
      _ = document | "Item 3" | bullet;
      _ = document | "Bottom";
      document.Line();
      _ = document | "Under the line";

      document.Save(@"C:\Temp\Bullet.rtf");
   }

   [TestMethod]
   public void FullDocumentTest()
   {
      var document = new Document(paperOrientation: PaperOrientation.Landscape);

      var calibriFont = document.Font("Calibri");
      var highlightColor = document.Color("yellow");
      var standardStyle = new Style() | calibriFont | 11f;
      var emphasizedStyle = new Style() | standardStyle | bold | italic | highlightColor.Background;
      document.DefaultCharFormat.Style = standardStyle;

      _ = document | "Estream Uncleansed database (EstreamPrd replica) – TSESTMUTL10CORP “migrationtest” (non-AG) – Update" | bold | underline;
      _ = document | "";

      var indent1 = 0.5f.InchesToPoints().FirstLineIndent();
      var indent2 = 1f.InchesToPoints().FirstLineIndent();

      _ = document |
         "a.\tRun partition cleanse steps for IntervalMeasurement data older than approx 2 years (2020-10-15) -- " | indent1 | "Completed" |
         emphasizedStyle;
      _ = document | "b.\tRun Partition cleanse for log tables -- " | indent2 | "Completed" | emphasizedStyle;
      _ = document | "i.\tRun partition cleanse steps for HierarchicalLog older than 60 days (2022-08-20)" | indent2;
      _ = document | "ii.\tRun partition cleanse steps for DistributionLog older than 45 days (2022-09-04)" | indent2;

      var url = "http://tfs.eprod.com/LS/_git/Estream/pullrequest/26899";
      _ = document | "c.\tDeploy the code base from " | indent2 | "" | url.Link("PR") | " -- " | none | "In-progress" | bold | italic;
      _ = document | "d.\tRun partition reorganization steps for IntervalMeasurement to monthly. " | indent1 | "Monitor the TLOG fullness" | bold |
         14f;

      document.Save(@"C:\Temp\FullDocument.rtf");
   }

   [TestMethod]
   public void HyperlinkTest()
   {
      var document = new Document();
      _ = document | "This is the url to " | "http://google.com".Link("Google") | " is here!";
      document.Save(@"C:\Temp\Hyperlink.rtf");
   }

   [TestMethod]
   public void Hyperlink2Test()
   {
      Maybe<string> _workItemId = "301905";

      var document = new Document();
      var font = document.Font("Calibri");
      var codeFont = document.Font("Consolas");
      var standardStyle = new Style() | font | 12f;
      document.DefaultCharFormat.Style = standardStyle;
      var codeStyle = new Style() | codeFont | bold;

      var workItemUrl = $"http://tfs/ls/Estream/_workitems/edit/{_workItemId}";
      var branch = "b-ops-301905-orderdeleted-failed-message";

      _ = document | "The backfill request for " | none | branch | codeStyle | " at " | workItemUrl.Link(_workItemId) | " has been completed.";
      _ = document | "Please merge master into your inflight branches. Thanks.";

      document.Save(@"C:\Temp\Hyperlink2.rtf");
   }

   [TestMethod]
   public void FormattingTest()
   {
      var document = new Document();
      _ = document | "This is " | none | "italic" | italic | " text";
      _ = document | "This is " | none | "bold" | bold | " text";
      _ = document | "This is " | none | "both" | italic | bold | "!";
      _ = document | "This is " | none | "release" | bold | " with an " | "outage" | bold | " for " | "r-6.41.0" |
         bold | ".";
      document.Save(@"C:\Temp\Formatting.rtf");
   }

   [TestMethod]
   public void CopyStyleTest()
   {
      var document = new Document();
      var font = document.Font("Times New Roman");
      var standard = new Style() | font | 12f;
      document.DefaultCharFormat.Style = standard;
      var italicStyle = ~standard | italic;
      var boldStyle = ~standard | bold;

      _ = document | "This is standard.";
      _ = document | "This is italic." | italicStyle;
      _ = document | "This is bold." | boldStyle;
      document.Save(@"C:\Temp\CopyStyle.rtf");
   }

   [TestMethod]
   public void EmbeddedUrlsTest()
   {
      var document = new Document();
      _ = document | "Embedded URL 1: (" | "http://tfs".Link("TFS") | ")";
      _ = document | "Embedded URL 2: (" | "http://google.com".Link() | ")";
      document.Save(@"C:\Temp\EmbeddedUrls.rtf");
   }

   [TestMethod]
   public void ControlWordTest()
   {
      var document = new Document();
      _ = document | "Line 1";
      _ = document | "Line 2";
      _ = document | "Line 3";

      var header = document.Header;
      _ = header | "Page " | page | " of " | numPages | right;

      document.Save(@"C:\Temp\ControlWord.rtf");
   }

   [TestMethod]
   public void Bug1Test()
   {
      var document = new Document();
      var font = document.Font("Times New Roman");
      var style = new Style() | font | 12;
      document.DefaultCharFormat.Style = style;
      var boldStyle = ~style | bold;

      var paragraph = document | "Please approve deployment to ";
      var formatter = paragraph | none;
      formatter = formatter | "staging01ua" | boldStyle;
      formatter = formatter | " with link ";
      formatter = formatter |
         "http://tfs.eprod.com/ls/estreamuat/_releaseProgress?_a=release-environment-logs&releaseId=6024&environmentId=6024".Link();
      formatter = formatter | " for Hotfix (Rolling) ";
      formatter = formatter | "r-6.41.10" | boldStyle;
      formatter = formatter | ".";

      document.Save(@"C:\Temp\Bug1.rtf");
   }

   [TestMethod]
   public void Bug2Test()
   {
      var document = new Document();
      var font = document.Font("Times New Roman");
      var style = new Style() | font | 12;
      document.DefaultCharFormat.Style = style;
      var margins = (8f, nil, 8f, nil);

      var foreColor = document.Color("white");
      var backColor = document.Color("blue");
      var style333 = new Style() | foreColor.Foreground | backColor.Background;

      var table = document.Table(12f);
      _ = table | "Column 1" | "Column 2" | "Column 3";
      _ = table | "111" | "222" | "333" | style333 | backColor.Background;
      _ = table | "222" | "333" | style333 | "444";
      _ = table | "333" | style333 | "444" | "555";
      table.DefaultCharFormat.Style = ~style | margins;
      table.SetOuterBorder(BorderStyle.Double, 1f);
      table.SetInnerBorder(BorderStyle.Single, 1f);

      document.Save(@"C:\Temp\Bug2.rtf");
   }
}