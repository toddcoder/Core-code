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
         file.Must().Not.BeNull().Assert($"{nameof(file)} must not be null");
         file.Must().Exist().Assert($"{nameof(file)} must exist");
         folderName.Must().Not.BeNullOrEmpty().Assert($"{nameof(folderName)} must not be null or empty");

         var folder = file.Folder[folderName];
         folder.Must().Not.Exist().Assert($"{nameof(folder)} must not exist");

         ZipFile.ExtractToDirectory(file.FullPath, folder.FullPath);

         return folder;
      }

      public static FolderName Unzip(this FileName file) => file.Unzip(file.Name);

      public static IResult<FolderName> TryToUnzip(this FileName file, string folderName) => tryTo(() => file.Unzip(folderName));

      public static IResult<FolderName> TryToUnzip(this FileName file) => file.TryToUnzip(file.Name);

      public static async Task<ICompletion<FolderName>> UnzipAsync(this FileName file, string folderName, CancellationToken token)
      {
         return await runAsync(t => file.Unzip(folderName).Completed(t), token);
      }

      public static async Task<ICompletion<FolderName>> UnzipAsync(this FileName file, CancellationToken token)
      {
         return await file.UnzipAsync(file.Name, token);
      }
   }
}