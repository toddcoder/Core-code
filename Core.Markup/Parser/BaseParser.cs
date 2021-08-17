using Core.Monads;

namespace Core.Markup.Parser
{
   public abstract class BaseParser<T>
   {
      public abstract Match<T> Parse(ParsingState state);
   }
}