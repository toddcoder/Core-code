using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Parser
{
   public abstract class Parser
   {
      public abstract Pattern Pattern { get; }

      public abstract Matched<Unit> Parse(ParsingState state);

      public virtual Matched<Unit> Parse(string line, ParsingState state)
      {
         if (line.Matches(Pattern).If(out var result))
         {
            state.Result = result;
            return Parse(state);
         }
         else
         {
            return nil;
         }
      }
   }
}