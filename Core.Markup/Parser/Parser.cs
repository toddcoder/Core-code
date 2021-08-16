using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public abstract class Parser<T>
   {
      public abstract Either<T, string> Parse(string line, MatchResult result);
   }
}