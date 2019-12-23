using System.IO;
using System.Text;
using System.Xml;
using Core.Assertions;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;

namespace Core.Internet.Sgml
{
   public static class SgmlExtensions
   {
      const string REGEX_EMPTY_ELEMENT = "'<' /(-['//!'] -['>']+ -['//']) '><//' /(-['>']+) '>'";
      const string TEXT_EMPTY_ELEMENT = "<$1/>";
      const string REGEX_HEADER = "/s* '<?' -['?']+ '?>'";

      static IResult<string> fromStream(Stream stream, Encoding encoding) => tryTo(() =>
      {
         stream.Position = 0;
         using (var reader = new StreamReader(stream, encoding))
         {
            return reader.ReadToEnd();
         }
      });

      public static string Tidy(this string sgml, Encoding encoding, bool includeHeader = true, char quoteChar = '"')
      {
         assert(() => sgml).Must().Not.BeNullOrEmpty().OrThrow();
         assert(() => encoding).Must().Not.BeNull().OrThrow();

         var document = new XmlDocument();
         document.LoadXml(sgml);
         document.LoadXml(document.OuterXml.Substitute(REGEX_EMPTY_ELEMENT, TEXT_EMPTY_ELEMENT));

         using (var stream = new MemoryStream())
         using (var writer = new XmlTextWriter(stream, encoding))
         {
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 3;
            writer.QuoteChar = quoteChar;

            document.Save(writer);

            if (fromStream(stream, encoding).If(out var r))
            {
               return includeHeader ? r : r.Substitute(REGEX_HEADER, "", false, true).Trim();
            }
            else
            {
               return "";
            }
         }
      }

      public static string Tidy(this string sgml, bool includeHeader) => Tidy(sgml, Encoding.UTF8, includeHeader);

      public static string Sgmlify(this string text)
      {
         assert(() => text).Must().Not.BeNullOrEmpty().OrThrow();

         text = text.Substitute("'&' -(> ('amp' | 'lt' | 'gt' | 'quot' | 'apos') ';')", "&amp;");
         text = text.Substitute("'<'", "&lt;");
         text = text.Substitute("'>'", "&gt;");
         text = text.Substitute("[dquote]", "&quot;");
         text = text.Substitute("[squote]", "&apos;");

         return text;
      }

      public static string UnSgmlify(this string text)
      {
         assert(() => text).Must().Not.BeNullOrEmpty().OrThrow();

         text = text.Substitute("'&apos;'", "'");
         text = text.Substitute("'&quot;'", "\"");
         text = text.Substitute("'&gt;'", ">");
         text = text.Substitute("'&lt;'", "<");
         text = text.Substitute("'&amp'", "&");

         return text;
      }

      public static string Simplify(this string sgml)
      {
         assert(() => sgml).Must().Not.BeNullOrEmpty().OrThrow();

         return sgml
            .Substitute("/s+ /w+ ':' /w '=' [dquote] -[dquote]+ [dquote]", "")
            .Substitute("/s+ 'xmlns=' [dquote] -[dquote]+ [dquote]", "");
      }
   }
}