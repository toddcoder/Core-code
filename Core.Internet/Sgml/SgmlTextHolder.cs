using Core.RegularExpressions;
using Core.Strings;

namespace Core.Internet.Sgml
{
   public class SgmlTextHolder
   {
      public static string Sgmlify(string text, bool html)
      {
         text = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
         if (!html)
         {
            text = text.Replace("~", "&nbsp;");
         }

         return replaceBrackets(text);
      }

      public static string Sgmlify(string text, QuoteType quoteType)
      {
         text = Sgmlify(text, false);
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

      static string replaceBrackets(string text)
      {
         return text.Substitute("'`' /([/w '-']+) ':'", "<$1>").Substitute("':' /([/w '-']+) '`'", "</$1>");
      }

      public static implicit operator SgmlTextHolder(string text) => new SgmlTextHolder(text);

      protected string text;

      public SgmlTextHolder(string text) => this.text = text.IsNotEmpty() ? Sgmlify(text, false) : "";

      public virtual string Text => text;

      public override string ToString() => text;
   }
}