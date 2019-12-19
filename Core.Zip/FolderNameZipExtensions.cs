using System;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Assertions;
using Core.Computers;
using Core.Monads;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Monads.AttemptFunctions;

namespace Core.Zip
{
   public static class FolderNameZipExtensions
   {
      public static FileName Zip(this FolderName folder, string zipName, Predicate<FileName> include, bool recursive = true,
         CompressionLevel compressionLevel = CompressionLevel.Optimal)
      {
         folder.Must().Not.BeNull().Assert($"{nameof(folder)} must not be null");
         zipName.Must().Not.BeNullOrEmpty().Assert($"{nameof(zipName)} must not be null or empty");
         include.Must().Not.BeNull().Assert($"{nameof(include)} must not be null");

         var zipFolder = folder.Parent.DefaultTo(() => @"C:\");
         var zipFile = zipFolder.UniqueFileName(zipName, ".zip");

         using (var archive = ZipFile.Open(zipFile.FullPath, ZipArchiveMode.Create))
         {
            zipCurrentFolder(archive, folder, include, recursive, compressionLevel, "");
         }

         return zipFile;
      }

      public static FileName Zip(this FolderName folder, string zipName, bool recursive = true,
         CompressionLevel compressionLevel = CompressionLevel.Optimal)
      {
         return folder.Zip(zipName, f => true, recursive, compressionLevel);
      }

      static void zipCurrentFolder(ZipArchive archive, FolderName folder, Predicate<FileName> include, bool recursive,
         CompressionLevel compressionLevel, string prefix)
      {
         foreach (var file in folder.Files.Where(f => include(f)))
         {
            archive.CreateEntryFromFile(file.FullPath, $"{prefix}{file.NameExtension}", compressionLevel);
         }

         if (recursive)
         {
            foreach (var subfolder in folder.Folders)
            {
               zipCurrentFolder(archive, subfolder, include, recursive, compressionLevel, $"{prefix}{subfolder.Name}\\");
            }
         }
      }

      public static IResult<FileName> TryToZip(this FolderName folder, string zipName, Predicate<FileName> include, bool recursive = true,
         CompressionLevel compressionLevel = CompressionLevel.Optimal)
      {
         return tryTo(() => folder.Zip(zipName, include, recursive, compressionLevel));
      }

      public static IResult<FileName> TryToZip(this FolderName folder, string zipName, bool recursive = true,
         CompressionLevel compressionLevel = CompressionLevel.Optimal)
      {
         return folder.TryToZip(zipName, f => true, recursive, compressionLevel);
      }

      public static async Task<ICompletion<FileName>> ZipAsync(this FolderName folder, string zipName, CancellationToken token,
         Predicate<FileName> include, bool recursive = true, CompressionLevel compressionLevel = CompressionLevel.Optimal)
      {
         return await runAsync(t => folder.Zip(zipName, include, recursive, compressionLevel).Completed(t), token);
      }

      public static async Task<ICompletion<FileName>> ZipAsync(this FolderName folder, string zipName, CancellationToken token, bool recursive = true,
         CompressionLevel compressionLevel = CompressionLevel.Optimal)
      {
         return await folder.ZipAsync(zipName, token, f => true, recursive, compressionLevel);
      }
   }
}