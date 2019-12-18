using System;
using Core.Assertions;
using Core.Computers;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Zip
{
   public class ZipperTrying
   {
      protected Zipper zipper;

      public ZipperTrying(Zipper zipper)
      {
         zipper.Must().Not.BeNull().Assert();
         this.zipper = zipper;
      }

      public IResult<FileName> ZipFolder(FolderName folder, string name, Predicate<FileName> include)
      {
         return
            from result in tryTo(() => zipper.ZipFolder(folder, name, include))
            from exists in zipper.ZipFile.Must().Exist().Try()
            select exists;
      }

      public IResult<FileName> ZipFolder(FolderName folder, string name) => ZipFolder(folder, name, f => true);

      public IResult<FileName> Replace(FolderName folder)
      {
         return
            from result in tryTo(() => zipper.Replace(folder))
            from exists in zipper.ZipFile.Must().Exist().Try()
            select exists;
      }

      public IResult<Unit> Unzip(FileName file, FolderName targetFolder) => tryTo(() => zipper.Unzip(file, targetFolder));
   }
}