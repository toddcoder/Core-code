using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Monads;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Computers;

public static class ComputerExtensions
{
   public static Maybe<FileName> AsFileName(this string file) => (FileName)file;

   public static Maybe<FolderName> AsFolderName(this string folder) => (FolderName)folder;

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

   public static Result<FileName> LocalAndParentFiles(this IEnumerable<FolderName> folders, Predicate<FileName> predicate)
   {
      foreach (var folder in folders)
      {
         var _file = folder.LocalAndParentFiles.Where(f => predicate(f)).FirstOrNone();
         if (_file is (true, var file))
         {
            return file;
         }
      }

      return fail("File not found");
   }

   public static async Task<Completion<FileName>> LocalAndParentFilesAsync(this IEnumerable<FolderName> folders, Predicate<FileName> predicate)
   {
      return await runFromResultAsync(() => folders.LocalAndParentFiles(predicate));
   }

   public static async Task<Completion<FileName>> LocalAndParentFilesAsync(this IEnumerable<FolderName> folders, Predicate<FileName> predicate,
      CancellationToken token)
   {
      return await runFromResultAsync(_ => folders.LocalAndParentFiles(predicate), token);
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

   public static Result<FolderName> LocalAndParentFolders(this IEnumerable<FolderName> folders, Predicate<FolderName> predicate)
   {
      foreach (var subFolder in folders)
      {
         var _folder = subFolder.LocalAndParentFolders.Where(f => predicate(f)).FirstOrNone();
         if (_folder is (true, var folder))
         {
            return folder;
         }
      }

      return fail("Folder not found");
   }

   public static async Task<Completion<FolderName>> LocalAndParentFoldersAsync(this IEnumerable<FolderName> folders,
      Predicate<FolderName> predicate)
   {
      return await runFromResultAsync(() => folders.LocalAndParentFolders(predicate));
   }

   public static async Task<Completion<FolderName>> LocalAndParentFoldersAsync(this IEnumerable<FolderName> folders,
      Predicate<FolderName> predicate, CancellationToken token)
   {
      return await runFromResultAsync(_ => folders.LocalAndParentFolders(predicate), token);
   }
}