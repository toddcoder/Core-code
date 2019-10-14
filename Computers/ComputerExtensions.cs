using System.Collections.Generic;
using Core.Monads;

namespace Core.Computers
{
   public static class ComputerExtensions
   {
      public static IMaybe<FileName> AsFileName(this string file) => ((FileName)file).Some();

      public static IMaybe<FolderName> AsFolderName(this string folder) => ((FolderName)folder).Some();

      public static IEnumerable<FileName> LocalAndParentFiles(this IEnumerable<FolderName> folders)
      {
         foreach (var folder in folders)
         {
            foreach (var file in folder.LocalAndParentFiles)
            {
               yield return file;
            }
         }
      }

      public static IEnumerable<FolderName> LocalAndParentFolders(this IEnumerable<FolderName> folders)
      {
         foreach (var folder in folders)
         {
            foreach (var subfolder in folder.LocalAndParentFolders)
            {
               yield return subfolder;
            }
         }
      }
   }
}