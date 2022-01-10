using System;
using Core.Assertions;
using Core.Matching;
using static Core.Markup.Xml.MarkupTextHolder;

namespace Core.Markup.Xml
{
   public class Attribute
   {
      public static implicit operator Attribute(string source)
      {
         if (source.Matches("^ '@'? /(/w [/w '-']+) /s* '=' /s* /([quote]) /(-[quote]*) /2 $; f").If(out var result))
         {
            var (name, quote, text) = result;
            return new Attribute(name, text, quote == "'" ? QuoteType.Single : QuoteType.Double);
         }
         else
         {
            throw new ApplicationException($"Didn't understand '{source}'");
         }
      }

      protected string name;
      protected string text;
      protected QuoteType quote;

      public Attribute(string name, string text, QuoteType quote)
      {
         text.Must().Not.BeNull().OrThrow();

         this.name = name.Must().Not.BeNullOrEmpty().Force();
         this.text = Markupify(text, quote);
         this.quote = quote;
      }

      public string Name => name;

      public string Text
      {
         get => text;
         set => text = value;
      }

      public QuoteType Quote => quote;

      public override string ToString() => quote == QuoteType.Double ? $"{name}=\"{text}\"" : $"{name}='{text}'";
   }
}