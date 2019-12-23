using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Core.Assertions;
using Core.Computers;
using Core.Monads;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;

namespace Core.Zip
{
   public static class FileNameZipExtensions
   {
      public static FolderName Unzip(this FileName file, string folderName)
      {
         assert(() => file).Must().Not.BeNull().OrThrow();
         assert(() => file).Must().Exist().OrThrow();
         assert(() => folderName).Must().Not.BeNullOrEmpty().OrThrow();

         var folder = file.Folder[folderName];
         assert(()=> folder).Must().Not.Exist().OrThrow();

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