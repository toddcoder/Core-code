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
               from valid in Computers.FileName.IsValidFileName(text).Must().Be().Try(() => $"{text} invalid file name").Map(_ => (FileName)text)
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
               from valid in Computers.FolderName.IsValidFolderName(text).Must().Be().Try(() => $"{text} invalid Folder name")
                  .Map(_ => (FolderName)text)
               from exists in valid.Must().Exist().Try()
               select exists;
         }
      }
   }
}