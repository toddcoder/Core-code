using System;

namespace Core.Monads
{
   [Obsolete("Maybe be removed")]
   public interface IHasValue
   {
      bool HasValue { get; }
   }
}