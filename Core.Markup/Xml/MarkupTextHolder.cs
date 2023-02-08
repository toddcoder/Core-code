using Core.Matching;
using Core.Strings;

namespace Core.Markup.Xml;

public class MarkupTextHolder
{
   public static string Markupify(string text)
   {
      text = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("~", "&nbsp;");
      return replaceBrackets(text);
   }

   public static string Markupify(string text, QuoteType quoteType)
   {
      text = Markupify(text);
      switch (quoteType)
      {
         case QuoteType.Double:
            text = text.Replace("\"", "&quot;");
            break;
         case QuoteType.Single:
            text = text.Replace("'", "&apos;");
            break;
      }

      return replaceBrackets(text);
   }

   protected static string replaceBrackets(string text)
   {
      return text.Substitute("'`' /([/w '-']+) ':'; f", "<$1>").Substitute("':' /([/w '-']+) '`'; f", "</$1>");
   }

   public static implicit operator MarkupTextHolder(string text) => new(text);

   protected string text;

   public MarkupTextHolder(string text) => this.text = text.IsNotEmpty() ? Markupify(text) : string.Empty;

   public virtual string Text => text;

   public override string ToString() => text;
}