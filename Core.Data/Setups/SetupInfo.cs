using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups
{
   public class SetupInfo
   {
      protected string connection;
      protected Maybe<string> command;
      protected string adapter;

      public string Connection
      {
         get => connection;
         set => connection = value;
      }

      public string Command
      {
         get => command.If(out var c) ? c : adapter;
         set => command = value.Some();
      }

      public string Adapter
      {
         get => adapter;
         set
         {
            adapter = value;
            if (command.IsNone)
            {
               command = adapter.Some();
            }
         }
      }

      public SetupInfo()
      {
         connection = "";
         command = none<string>();
         adapter = "";
      }

      public SetupInfo(string connectionName, string adapterName, Maybe<string> commandName)
      {
         connection = connectionName;
         adapter = adapterName;
         command = commandName;
         if (command.IsNone)
         {
            command = Adapter.Some();
         }
      }

      public SetupInfo(string connectionName, string adapterName) : this(connectionName, adapterName, none<string>()) { }
   }
}