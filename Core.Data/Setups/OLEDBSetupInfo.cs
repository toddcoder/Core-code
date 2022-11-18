using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class OleDbSetupInfo : SetupInfo
{
   protected Maybe<string> file;

   public OleDbSetupInfo(Maybe<string> file) => this.file = file;

   public OleDbSetupInfo() : this(file: nil)
   {
   }

   public OleDbSetupInfo(string connectionName, string adapterName, Maybe<string> commandName, Maybe<string> file) : base(connectionName,
      adapterName, commandName)
   {
      this.file = file;
   }

   public Maybe<string> File
   {
      get => file;
      set => file = value;
   }
}