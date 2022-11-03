using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class SetupInfo
{
   protected string connection;
   protected Maybe<string> _command;
   protected string adapter;

   public string Connection
   {
      get => connection;
      set => connection = value;
   }

   public string Command
   {
      get => _command | adapter;
      set => _command = value;
   }

   public string Adapter
   {
      get => adapter;
      set
      {
         adapter = value;
         if (!_command)
         {
            _command = adapter;
         }
      }
   }

   public SetupInfo()
   {
      connection = string.Empty;
      _command = nil;
      adapter = string.Empty;
   }

   public SetupInfo(string connectionName, string adapterName, Maybe<string> commandName)
   {
      connection = connectionName;
      adapter = adapterName;
      _command = commandName;
      if (!_command)
      {
         _command = Adapter;
      }
   }

   public SetupInfo(string connectionName, string adapterName) : this(connectionName, adapterName, nil)
   {
   }
}