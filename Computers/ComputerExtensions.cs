using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
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

      public static IResult<FileName> LocalAndParentFiles(this IEnumerable<FolderName> folders, Predicate<FileName> predicate)
      {
         foreach (var folder in folders)
         {
            if (folder.LocalAndParentFiles.Where(f => predicate(f)).FirstOrNone().If(out var file))
            {
               return file.Success();
            }
         }

         return "File not found".Failure<FileName>();
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

      public static IResult<FolderName> LocalAndParentFolders(this IEnumerable<FolderName> folders, Predicate<FolderName> predicate)
      {
         foreach (var folder in folders)
         {
            if (folder.LocalAndParentFolders.Where(f => predicate(f)).FirstOrNone().If(out var foundFolder))
            {
               return foundFolder.Success();
            }
         }

         return "Folder not found".Failure<FolderName>();
      }
   }
}