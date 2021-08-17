using Core.Monads;

namespace Core.Markup.Parser
{
   public abstract class BaseParser<T>
   {
      public abstract Matched<T> Parse(ParsingState state);
   }
}