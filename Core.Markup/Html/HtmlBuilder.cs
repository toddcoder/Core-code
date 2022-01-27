using Core.Collections;
using Core.Enumerables;
using Core.Markup.Xml;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Html
{
   public class HtmlBuilder : MarkupBuilder
   {
      public static Selector operator +(HtmlBuilder builder, Selector selector)
      {
         builder.selectors.Add(selector);
         return selector;
      }

      public static HtmlBuilder operator *(HtmlBuilder builder, Selector selector)
      {
         builder.selectors.Add(selector);
         return builder;
      }

      protected Element head;
      protected Element meta;
      protected Maybe<Element> _style;
      protected Element body;
      protected Set<Selector> selectors;

      public HtmlBuilder() : base("html")
      {
         head = root + "head>";

         meta = head + "meta>";
         meta *= "@http-equiv=X-UA-Compatible";
         meta *= "@content=IE=edge";

         _style = nil;

         body = root + "body>";

         selectors = new Set<Selector>();
      }

      public Element Html => root;

      public Element Head => head;

      public Element Meta => meta;

      public Element Style
      {
         get
         {
            _style = _style.Initialize(() => head + "style>");
            return _style.Required("Style not set");
         }
      }

      public Element Body => body;

      public override string ToString()
      {
         if (selectors.Count > 0)
         {
            Style.Text = selectors.ToString(" ");
         }
         return base.ToString();
      }
   }
}