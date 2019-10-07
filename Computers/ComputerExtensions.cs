using Core.Monads;

namespace Core.Computers
{
   public static class ComputerExtensions
   {
      public static IMaybe<FileName> AsFileName(this string file) => ((FileName)file).Some();

      public static IMaybe<FolderName> AsFolderName(this string folder) => ((FolderName)folder).Some();
   }
}