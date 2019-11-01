using Core.Monads;

namespace Core.Data.Setups
{
   public class OLEDBSetupInfo : SetupInfo
   {
      protected IMaybe<string> file;

      public OLEDBSetupInfo(IMaybe<string> file) => this.file = file;

      public OLEDBSetupInfo() : this(file: MonadFunctions.none<string>()) { }

      public OLEDBSetupInfo(string connectionName, string adapterName, IMaybe<string> commandName, IMaybe<string> file)
         : base(connectionName, adapterName, commandName) => this.file = file;

      public IMaybe<string> File
      {
         get => file;
         set => file = value;
      }
   }
}