using System.Data.SqlClient;
using Core.Monads;

namespace Core.Data.Setups
{
   public interface ISetupWithInfo
   {
      IMaybe<SqlInfoMessageEventHandler> Handler { get; }
   }
}