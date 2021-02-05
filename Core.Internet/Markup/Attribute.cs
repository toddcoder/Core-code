using Core.Assertions;
using static Core.Assertions.AssertionFunctions;
using static Core.Internet.Markup.MarkupTextHolder;

namespace Core.Internet.Markup
{
   public class Attribute
   {
      protected string name;
      protected string text;
      protected QuoteType quote;

      public Attribute(string name, string text, QuoteType quote)
      {
         assert(() => text).Must().Not.BeNull().OrThrow();

         this.name = assert(() => name).Must().Not.BeNullOrEmpty().Force();
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