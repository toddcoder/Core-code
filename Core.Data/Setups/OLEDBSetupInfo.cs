using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class OleDbSetupInfo : SetupInfo
{
   protected Optional<string> file;

   public OleDbSetupInfo(Optional<string> file) => this.file = file;

   public OleDbSetupInfo() : this(file: nil)
   {
   }

   public OleDbSetupInfo(string connectionName, string adapterName, Optional<string> commandName, Optional<string> file) : base(connectionName,
      adapterName, commandName)
   {
      this.file = file;
   }

   public Optional<string> File
   {
      get => file;
      set => file = value;
   }
}