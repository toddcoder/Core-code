using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public abstract class Parser
   {
      public abstract Pattern Pattern { get; }

      public abstract Result<int> Parse(ParsingState state);
   }
}