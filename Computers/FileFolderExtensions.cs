using Core.Assertions;
using Core.Monads;

namespace Core.Computers
{
   public static class FileFolderExtensions
   {
      public static IResult<FileName> File(this string fileName)
      {
         FileName file = fileName;
         return file.Must().Exist().OrFailure();
      }

      public static IResult<FolderName> Folder(this string folderName)
      {
         FolderName folder = folderName;
         return folder.Must().Exist().OrFailure();
      }
   }
}