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

         document.Paragraph("Testing\n", Alignment.Left, timesFont);

         var paragraph = document.Paragraph("Test2: Character Formatting", timesFont);

         var _format = paragraph.CharFormat("Test2:; u");
         if (!_format)
         {
            throw fail("Couldn't extract text");
         }

         var format = ~_format; //paragraph.CharFormat(0, 5);
         format.ForegroundColor = blueColor;
         format.BackgroundColor = redColor;
         format.FontSize = 18f;

         _format = paragraph.CharFormat("Character Formatting; u");
         if (!_format)
         {
            throw fail("Couldn't extract text");
         }

         format = ~_format; //paragraph.CharFormat(7, 26);
         format.FontStyle += FontStyleFlag.Bold;
         format.FontStyle += FontStyleFlag.Underline;
         format.Font = courierFont;

         paragraph = document.Paragraph("Footnote");
         paragraph.Footnote(7).Paragraph().Text = "Footnote details here.";

         paragraph = document.Footer.Paragraph("Test : Page: / Date: Time:", Alignment.Center, 15f);
         paragraph.ControlWord(12, FieldType.Page);
         paragraph.ControlWord(13, FieldType.NumPages);
         paragraph.ControlWord(19, FieldType.Date);
         paragraph.ControlWord(25, FieldType.Time);

         document.Header.Paragraph("Header");

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
         table[4, 3].Paragraph().Text = "Table";

         paragraph = document.Paragraph("Test 7.1: Hyperlink to target (Test9)");
         format = paragraph.CharFormat(10, 18);
         format.LocalHyperlink = "target";
         format.LocalHyperlinkTip = "Link to target";
         format.ForegroundColor = blueColor;

         paragraph = document.Paragraph("New page");
         paragraph.StartNewPage = true;

         paragraph = document.Paragraph("Test9: Set bookmark");
         format = paragraph.CharFormat(0, 18);
         format.Bookmark = "target";

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
            table.Row()
               .Column(prompt, FontStyleFlag.Bold)
               .Column(hyperlink, blueColor, (hyperlink, ""));
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
   }
}