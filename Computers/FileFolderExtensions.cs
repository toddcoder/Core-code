using Core.Assertions;
using Core.Monads;

namespace Core.Computers;

public static class FileFolderExtensions
{
   public static Result<FileName> File(this string fileName)
   {
      FileName file = fileName;
      return file.Must().Exist().OrFailure();
   }

   public static Result<FolderName> Folder(this string folderName)
   {
      FolderName folder = folderName;
      return folder.Must().Exist().OrFailure();
   }
}