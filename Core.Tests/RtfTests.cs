using System.Runtime.InteropServices;
using Core.Enumerables;
using Core.Markup.Rtf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

         var paragraph = document.Paragraph();
         paragraph.Alignment = Alignment.Left;
         paragraph.DefaultCharFormat.Font = timesFont;
         paragraph.DefaultCharFormat.AnsiFont = courierFont;
         paragraph.Text = "Testing\n";

         paragraph = document.Paragraph();
         paragraph.DefaultCharFormat.Font = timesFont;
         paragraph.Text = "Test2: Character Formatting";

         var format = paragraph.CharFormat(0, 5);
         format.ForegroundColor = blueColor;
         format.BackgroundColor = redColor;
         format.FontSize = 18f;

         format = paragraph.CharFormat(7, 26);
         format.FontStyle += FontStyleFlag.Bold;
         format.FontStyle += FontStyleFlag.Underline;
         format.Font = courierFont;

         paragraph = document.Paragraph();
         paragraph.Text = "Footnote";
         paragraph.Footnote(7).Paragraph().Text = "Footnote details here.";

         paragraph = document.Footer.Paragraph();
         paragraph.Text = "Test : Page: / Date: Time:";
         paragraph.Alignment = Alignment.Center;
         paragraph.DefaultCharFormat.FontSize = 15f;
         paragraph.ControlWord(12, FieldType.Page);
         paragraph.ControlWord(13, FieldType.NumPages);
         paragraph.ControlWord(19, FieldType.Date);
         paragraph.ControlWord(25, FieldType.Time);

         paragraph = document.Header.Paragraph();
         paragraph.Text = "Header";

         var image = document.Image(@"C:\Temp\rabbit-mq.jpg");
         image.Width = 130;
         image.StartNewParagraph = true;

         var table = document.Table(5, 4, 415.2f, 12);
         table.Margins[Direction.Bottom] = 20;
         table.SetInnerBorder(BorderStyle.Dotted, 1);
         table.SetOuterBorder(BorderStyle.Single, 2);

         table.HeaderBackgroundColor = tableHeaderColor;
         table.RowBackgroundColor = tableRowColor;
         table.RowAltBackgroundColor = tableRowAltColor;

         foreach (var row in table.RowCount.Times())
         {
            foreach (var column in table.ColumnCount.Times())
            {
               table[row, column].Paragraph().Text = $"CELL {row}, {column}";
            }
         }

         table.Merge(1, 0, 3, 1);
         table[4, 3].BackgroundColor = redColor;
         table[4, 3].Paragraph().Text = "Table";

         paragraph = document.Paragraph();
         paragraph.Text = "Test 7.1: Hyperlink to target (Test9)";
         format = paragraph.CharFormat(10, 18);
         format.LocalHyperlink = "target";
         format.LocalHyperlinkTip = "Link to target";
         format.ForegroundColor = blueColor;

         paragraph = document.Paragraph();
         paragraph.StartNewPage = true;
         paragraph.Text = "New page";

         paragraph = document.Paragraph();
         paragraph.Text = "Test9: Set bookmark";
         format = paragraph.CharFormat(0, 18);
         format.Bookmark = "target";

         document.Save(@"C:\Temp\Test.rtf");
      }

      [TestMethod]
      public void TableDataTest()
      {
         var document = new Document();
         var tableData = new TableData(document);
         var blueColor = document.Color("blue");
         tableData.TableCell += (_, e) =>
         {
            var paragraph = e.TableCell.Paragraph();
            paragraph.Text = e.Text;

            switch (e.ColumnIndex)
            {
               case 0:
                  paragraph.DefaultCharFormat.FontStyle += FontStyleFlag.Bold;
                  break;
               case 1:
                  paragraph.DefaultCharFormat.LocalHyperlink = e.Text;
                  paragraph.DefaultCharFormat.ForegroundColor = blueColor;
                  break;
            }
         };

         tableData.AddRow("Pull Request", "http://foobar");
         tableData.AddRow("estreamps", "http://evokeps");
         tableData.AddRow("staging10ua", "http://evokeuat");
         _ = tableData.Table(12);
         document.Save(@"C:\Temp\Test2.rtf");
      }
   }
}