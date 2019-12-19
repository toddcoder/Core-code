using Core.Assertions;
using Core.Computers;
using Core.Monads;

namespace Core.Applications
{
   public class ArgumentTrying
   {
      Argument argument;

      public ArgumentTrying(Argument argument) => this.argument = argument;

      public IResult<FileName> FileName
      {
         get
         {
            var text = argument.Text;
            return
               from valid in text.MustAs(nameof(text)).BeAValidFileName().Try().Map(_ => (FileName)text)
               from exists in valid.Must().Exist().Try()
               select exists;
         }
      }

      public IResult<FolderName> FolderName
      {
         get
         {
            var text = argument.Text;
            return
               from valid in text.MustAs(nameof(text)).BeAValidFolderName().Try().Map(_ => (FolderName)text)
               from exists in valid.Must().Exist().Try()
               select exists;
         }
      }
   }
}