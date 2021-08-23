using Core.Collections;
using Core.Markup.Code;
using Core.Markup.Code.Extents;
using Core.Matching;

namespace Core.Markup.Parser
{
   public class ParsingState
   {
      protected Source source;
      protected Document document;
      protected StringHash<Format> formats;

      public ParsingState(string source)
      {
         this.source = new Source(source);

         document = new Document();
         formats = new StringHash<Format>(true);
      }

      public Source Source => source;

      public Document Document => document;

      public StringHash<Format> Formats => formats;
   }
}