using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Monads;
using static Core.Applications.Async.AsyncFunctions;

namespace Core.Computers
{
   public static class ComputerExtensions
   {
      public static Maybe<FileName> AsFileName(this string file) => ((FileName)file).Some();

      public static Maybe<FolderName> AsFolderName(this string folder) => ((FolderName)folder).Some();

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

      public static async Task<ICompletion<FileName>> LocalAndParentFilesAsync(this IEnumerable<FolderName> folders, Predicate<FileName> predicate)
      {
         return await runFromResultAsync(() => folders.LocalAndParentFiles(predicate));
      }

      public static async Task<ICompletion<FileName>> LocalAndParentFilesAsync(this IEnumerable<FolderName> folders, Predicate<FileName> predicate,
         CancellationToken token)
      {
         return await runFromResultAsync(t => folders.LocalAndParentFiles(predicate), token);
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

      public static async Task<ICompletion<FolderName>> LocalAndParentFoldersAsync(this IEnumerable<FolderName> folders,
         Predicate<FolderName> predicate)
      {
         return await runFromResultAsync(() => folders.LocalAndParentFolders(predicate));
      }
      public static async Task<ICompletion<FolderName>> LocalAndParentFoldersAsync(this IEnumerable<FolderName> folders,
         Predicate<FolderName> predicate, CancellationToken token)
      {
         return await runFromResultAsync(t => folders.LocalAndParentFolders(predicate), token);
      }
   }
}