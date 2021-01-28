using Core.Monads;

namespace Core.Data.Setups
{
   public class OleDbSetupInfo : SetupInfo
   {
      protected IMaybe<string> file;

      public OleDbSetupInfo(IMaybe<string> file) => this.file = file;

      public OleDbSetupInfo() : this(file: MonadFunctions.none<string>())
      {
      }

      public OleDbSetupInfo(string connectionName, string adapterName, IMaybe<string> commandName, IMaybe<string> file) : base(connectionName,
         adapterName, commandName)
      {
         this.file = file;
      }

      public IMaybe<string> File
      {
         get => file;
         set => file = value;
      }
   }
}