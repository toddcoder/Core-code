using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Core.Assertions;
using Core.Computers;
using Core.Monads;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Monads.AttemptFunctions;

namespace Core.Zip
{
   public static class FileNameZipExtensions
   {
      public static FolderName Unzip(this FileName file, string folderName)
      {
         file.Must().Not.BeNull().OrThrow();
         file.Must().Exist().OrThrow();
         folderName.Must().Not.BeNullOrEmpty().OrThrow();

         var folder = file.Folder[folderName];
         folder.Must().Not.Exist().OrThrow();

         ZipFile.ExtractToDirectory(file.FullPath, folder.FullPath);

         return folder;
      }

      public static FolderName Unzip(this FileName file) => file.Unzip(file.Name);

      public static Result<FolderName> TryToUnzip(this FileName file, string folderName) => tryTo(() => file.Unzip(folderName));

      public static Result<FolderName> TryToUnzip(this FileName file) => file.TryToUnzip(file.Name);

      public static async Task<Completion<FolderName>> UnzipAsync(this FileName file, string folderName, CancellationToken token)
      {
         return await runAsync(t => file.Unzip(folderName).Completed(t), token);
      }

      public static async Task<Completion<FolderName>> UnzipAsync(this FileName file, CancellationToken token)
      {
         return await file.UnzipAsync(file.Name, token);
      }
   }
}