using System.IO;
using System.Text;
using System.Xml;
using Core.Assertions;
using Core.Matching;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Markup.Xml;

public static class MarkupExtensions
{
   private const string PATTERN_EMPTY_ELEMENT = "'<' /(-['//!'] -['>']+ -['//']) '><//' /(-['>']+) '>'; f";
   private const string TEXT_EMPTY_ELEMENT = "<$1/>";
   private const string PATTERN_HEADER = "/s* '<?' -['?']+ '?>'; mf";

   private static Result<string> fromStream(Stream stream, Encoding encoding) => tryTo(() =>
   {
      stream.Position = 0;
      using var reader = new StreamReader(stream, encoding);

      return reader.ReadToEnd();
   });

   public static string Tidy(this string markup, Encoding encoding, bool includeHeader = true, char quoteChar = '"')
   {
      markup.Must().Not.BeNullOrEmpty().OrThrow();
      encoding.Must().Not.BeNull().OrThrow();

      var document = new XmlDocument();
      document.LoadXml(markup);
      document.LoadXml(document.OuterXml.Substitute(PATTERN_EMPTY_ELEMENT, TEXT_EMPTY_ELEMENT));

      using var stream = new MemoryStream();
      using var writer = new XmlTextWriter(stream, encoding) { Formatting = Formatting.Indented, Indentation = 3, QuoteChar = quoteChar };

      document.Save(writer);

      var _text = fromStream(stream, encoding);
      if (_text)
      {
         return includeHeader ? _text : _text.Value.Substitute(PATTERN_HEADER, string.Empty).Trim();
      }
      else
      {
         return string.Empty;
      }
   }

   public static string Tidy(this string markup, bool includeHeader) => Tidy(markup, Encoding.UTF8, includeHeader);

   public static string ToMarkup(this string text)
   {
      text.Must().Not.BeNullOrEmpty().OrThrow();

      text = text.Substitute("'&' -(> ('amp' | 'lt' | 'gt' | 'quot' | 'apos') ';'); f", "&amp;");
      text = text.Substitute("'<'; f", "&lt;");
      text = text.Substitute("'>'; f", "&gt;");
      text = text.Substitute("[dquote]; f", "&quot;");
      text = text.Substitute("[squote]; f", "&apos;");

      return text;
   }

   public static string FromMarkup(this string text)
   {
      text.Must().Not.BeNullOrEmpty().OrThrow();

      text = text.Substitute("'&apos;'; f", "'");
      text = text.Substitute("'&quot;'; f", "\"");
      text = text.Substitute("'&gt;'; f", ">");
      text = text.Substitute("'&lt;'; f", "<");
      text = text.Substitute("'&amp'; f", "&");

      return text;
   }

   public static string Simplify(this string markup)
   {
      markup.Must().Not.BeNullOrEmpty().OrThrow();

      return markup
         .Substitute("/s+ /w+ ':' /w '=' [dquote] -[dquote]+ [dquote]; f", "")
         .Substitute("/s+ 'xmlns=' [dquote] -[dquote]+ [dquote]; f", "");
   }
}