using Core.Assertions;
using Core.Computers;
using Core.Monads;

namespace Core.Applications;

public class ArgumentTrying
{
   protected Argument argument;

   public ArgumentTrying(Argument argument) => this.argument = argument;

   public Result<FileName> FileName
   {
      get
      {
         var text = argument.Text;
         return
            from valid in text.Must().BeAValidFileName().OrFailure().Map(_ => (FileName)text)
            from exists in valid.Must().Exist().OrFailure()
            select exists;
      }
   }

   public Result<FolderName> FolderName
   {
      get
      {
         var text = argument.Text;
         return
            from valid in text.Must().BeAValidFolderName().OrFailure().Map(_ => (FolderName)text)
            from exists in valid.Must().Exist().OrFailure()
            select exists;
      }
   }
}