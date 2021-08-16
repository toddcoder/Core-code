using Core.Markup.Code;
using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Parser
{
   public class DocumentParser
   {
      protected Source source;

      public DocumentParser(string source)
      {
         this.source = new Source(source);
      }

      public Matched<Document> Parse()
      {
         var document = new Document();

         return nil;
      }
   }
}