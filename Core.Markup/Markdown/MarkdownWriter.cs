using System.IO;
using System.Linq;
using Core.Enumerables;
using Core.Matching;
using Core.Strings;

namespace Core.Markup.Markdown;

public class MarkdownWriter
{
   protected StringWriter writer;

   public MarkdownWriter()
   {
      writer = new StringWriter();
   }

   protected static string flattenString(string text) => text.ToNonNullString().Substitute("'/r/n' | '/r' | '/n'; f", " ");

   public static string FixString(string text)
   {
      var flatText = flattenString(text);
      var linkText = flatText.Substitute("-(< ['[(']) /b /('https'? ':////' -/s+); f", "[$1]($1)");
      var escapedPipes = linkText.Substitute(@"-(< '\') '\|'; f", @"\|");

      return escapedPipes;
   }

   public static string FixNumberUrl(string url)
   {
      var flatText = flattenString(url);
      var _description = flatText.Matches("/(/d+) -/d* $; f").Map(r => r.FirstGroup);

      return $"[{_description | url}]({url})";
   }

   protected void writeLine(string text)
   {
      writer.WriteLine(FixString(text));
      writer.WriteLine();
   }

   public void WriteHeader(string text, int level) => writeLine($"{"#".Repeat(level)} {text}");

   public void WriteTableBegin(params string[] text)
   {
      var columnHeaders = text.Select(i => $"|{FixString(i)}").ToString("");
      writer.WriteLine();
      writer.WriteLine(columnHeaders);
      writer.WriteLine("|");
   }

   public void WriteRow(params string[] text)
   {
      if (text.Length > 1 && text[1].IsNotEmpty())
      {
         writer.WriteLine(text.Select(i => $"|{FixString(i)}").ToString(""));
      }
   }

   public void WriteColumn(string text, bool end = false)
   {
      writer.Write($"| {FixString(text)}");
      if (end)
      {
         writer.WriteLine();
      }
   }

   public void WriteColumn(string text, string link, bool end = false, bool isImage = false)
   {
      WriteColumn(isImage ? ImageLink(text, link) : Link(text, link), end);
   }

   public void WriteColumnAs(string text, string @class, bool end = false) => WriteColumn($"<span class='{@class}'>{text}</span>", end);

   public void WriteMultilineRow(string column1, string[] lines)
   {
      switch (lines.Length)
      {
         case 0:
            break;
         case 1 when lines[0].IsNotEmpty():
            WriteRow(column1, lines[0]);
            break;
         default:
            WriteRow(column1, lines[0]);
            foreach (var line in lines.Skip(1))
            {
               WriteRow("", line);
            }

            break;
      }
   }

   public void WriteTableEnd() => writer.WriteLine();

   public void WriteLine(string line) => writeLine(line);

   public void WriteLine(string text, string link, bool isImage = false)
   {
      writer.WriteLine(isImage ? ImageLink(text, link) : Link(text, link));
      writer.WriteLine();
   }

   public void WriteLineAs(string text, string @class) => writeLine($"<p class='{@class}'>{text}</p>");

   public void WriteLine() => writeLine("");

   public void WriteHtml(string html) => writer.WriteLine(html);

   public static string Link(string text, string link) => $"[{text}]({link})";

   public static string ImageLink(string text, string link) => $"![{text}]({link})";

   public void WriteRuler()
   {
      writer.WriteLine();
      writeLine("---");
   }

   public static string HtmlWrapper(string rawHtml)
   {
      using var stringWriter = new StringWriter();

      stringWriter.WriteLine("<html>");
      stringWriter.WriteLine("<head>");
      stringWriter.WriteLine("<meta http-equiv='X-UA-Compatible' content='IE=edge' />");
      stringWriter.WriteLine("<style>");
      stringWriter.WriteLine("h1 {font-family: arial; font-size: 28pt}");
      stringWriter.WriteLine("h2 {font-family: arial; font-size: 16pt}");
      stringWriter.WriteLine("h3 {font-family: arial; font-size: 12pt}");
      stringWriter.WriteLine("table {font-family: Times New Roman; font-size: 12pt}");
      stringWriter.WriteLine("td {padding: 4px; margin: 0; border: 0; border-bottom: 1px solid black; border-collapse: collapse}");
      stringWriter.WriteLine("th {border: 0}");
      stringWriter.WriteLine("tbody tr:nth-child(even) { background: #f0f0f2}");
      stringWriter.WriteLine("p {font-family: Times New Roman; font-size: 12pt");
      stringWriter.WriteLine("</style>");
      stringWriter.WriteLine("</head>");
      stringWriter.WriteLine("<body>");
      stringWriter.WriteLine(rawHtml);
      stringWriter.WriteLine("</body>");
      stringWriter.WriteLine("</html>");

      return stringWriter.ToString();
   }

   public void WriteList(string text, int indent = 0) => writer.WriteLine($"{"    ".Repeat(indent)}- {FixString(text)}");

   public void WriteList(string text, string link, int indent = 0, bool isImage = false)
   {
      WriteList(isImage ? ImageLink(text, link) : Link(text, link), indent);
   }

   public void WritePageBreak()
   {
      writer.WriteLine("<div style='page-break-after: always'></div>");
      writer.WriteLine();
   }

   public override string ToString() => writer.ToString();
}