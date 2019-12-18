using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Assertions;
using Core.Computers;
using Core.Monads;
using static Core.Applications.Async.AsyncFunctions;

namespace Core.Zip
{
   public class ZipperAsync
   {
      protected Zipper zipper;

      public ZipperAsync(Zipper zipper)
      {
         zipper.Must().Not.BeNull().Assert();
         this.zipper = zipper;
      }

      public async Task<ICompletion<FileName>> ZipFolder(FolderName folder, string name, Predicate<FileName> include, CancellationToken token)
      {
         return await
            from result in runAsync(() => zipper.ZipFolder(folder, name, include), token)
            from exists in zipper.ZipFile.Must().Exist().TryAsync(token)
            select exists;
      }

      public async Task<ICompletion<FileName>> ZipFolder(FolderName folder, string name, CancellationToken token)
      {
         return await ZipFolder(folder, name, f => true, token);
      }

      public async Task<ICompletion<FileName>> Replace(FolderName folder, CancellationToken token)
      {
         return await
            from result in runAsync(() => zipper.Replace(folder), token)
            from exists in zipper.ZipFile.Must().Exist().TryAsync(token)
            select exists;
      }

      public async Task<ICompletion<Unit>> Unzip(FileName file, FolderName targetFolder, CancellationToken token)
      {
         return await runAsync(() => zipper.Unzip(file, targetFolder), token);
      }
   }
}