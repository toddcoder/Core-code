using Core.Markup.Code.Extents;
using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public class BoldItalicParser : Parser
   {
      public override Pattern Pattern => "^ /('*'1%3); f";

      public override Matched<Unit> Parse(ParsingState state)
      {
         var stars = state.Result.FirstGroup;
         switch (stars)
         {
            case "*":
               state.IsItalic = !state.IsItalic;
               state.Document.CurrentBlock.Add(new Italic(state.IsItalic));
               break;
            case "**":
               state.IsBold = !state.IsBold;
               state.Document.CurrentBlock.Add(new Bold(state.IsBold));
               break;
            case "***":
               state.IsItalic = !state.IsItalic;
               state.IsBold = !state.IsBold;
               state.Document.CurrentBlock.Add(new Italic(state.IsItalic));
               state.Document.CurrentBlock.Add(new Bold(state.IsBold));
               break;
         }

         state.Source.Advance(stars.Length);
         return Unit.Value;
      }
   }
}