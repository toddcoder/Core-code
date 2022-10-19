using Core.Applications;
using Core.Enumerables;
using Core.Markup.Rtf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;
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

         _ = document + ("Testing\n", Alignment.Left, timesFont);

         var paragraph = document + ("Test2: Character Formatting", timesFont);
         var queue = paragraph.CharFormatTemplate("^^^^^^");
         var _format = queue.Dequeue();
         if (!_format)
         {
            throw fail("Couldn't extract text");
         }

         var format = ~_format;
         format.ForegroundColor = blueColor;
         format.BackgroundColor = redColor;
         format.FontSize = 18f;

         _format = paragraph.CharFormat("Character Formatting; u");
         if (!_format)
         {
            throw fail("Couldn't extract text");
         }

         format = ~_format;
         format.FontStyle += FontStyleFlag.Bold;
         format.FontStyle += FontStyleFlag.Underline;
         format.Font = courierFont;

         paragraph = document + "Footnote";
         _ = paragraph.Footnote(7) + "Footnote details here.";

         paragraph = document.Footer + ("Test : Page: / Date: Time:", Alignment.Center, 15f);
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

         paragraph = document + "Test 7.1: Hyperlink to target (Test9)";
         _format = paragraph.CharFormatTemplate("          ^^^^^^^^^").Dequeue();
         if (_format)
         {
            format = ~_format;
            format.LocalHyperlink = "target";
            format.LocalHyperlinkTip = "Link to target";
            format.ForegroundColor = blueColor;
         }

         paragraph = document + "New page";
         paragraph.StartNewPage = true;

         paragraph = document + "Test9: Set bookmark";
         _format = paragraph.CharFormatTemplate("^^^^^^^^^^^^^^^^^^^").Dequeue();
         if (_format)
         {
            (~_format).Bookmark = "target";
         }

         document.Save(@"C:\Temp\Test.rtf");
      }

      [TestMethod]
      public void TableDataTest()
      {
         var document = new Document();
         var table = document.Table(12);
         var blueColor = document.Color("blue");

         foreach (var (prompt, hyperlink) in array(("Pull Request", "http://foobar"), ("estreamps", "http://evokeps"),
                     ("staging10ua", "http://evokeuat")))
         {
            _ = table.Row() + (prompt, Feature.Bold) + (hyperlink, blueColor, (hyperlink, ""));
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
         _ = document + ("Item 1", Feature.Bullet);
         _ = document + ("Item 2", Feature.Bullet);
         _ = document + ("Item 3", Feature.Bullet);
         _ = document + "Bottom";

         document.Save(@"C:\Temp\Test3.rtf");
      }
   }
}