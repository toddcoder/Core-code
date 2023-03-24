using System.Data.SqlClient;
using Core.Monads;

namespace Core.Data.Setups;

public interface ISetupWithInfo
{
   Optional<SqlInfoMessageEventHandler> Handler { get; }
}