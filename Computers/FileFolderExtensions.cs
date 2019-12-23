using Core.Assertions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Computers
{
   public static class FileFolderExtensions
   {
      public static IResult<FileName> File(this string fileName)
      {
         FileName file = fileName;
         return assert(() => file).Must().Exist().OrFailure();
      }

      public static IResult<FolderName> Folder(this string folderName)
      {
         FolderName folder = folderName;
         return assert(() => folder).Must().Exist().OrFailure();
      }
   }
}