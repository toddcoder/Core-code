using System;
using System.Linq;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Computers
{
   public class FolderNameTrying
   {
      public static IResult<FolderName> SetCurrent(FolderName newCurrent) => tryTo(() =>
      {
         FolderName.Current = newCurrent;
         return FolderName.Current;
      });

      FolderName folderName;

      public event EventHandler<FileArgs> FileSuccess;

      public FolderNameTrying(FolderName folderName) => this.folderName = folderName;

      public FolderName FolderName => folderName;

      public IResult<FolderName> CopyTo(FolderName targetFolder, bool overwrite) => tryTo(() =>
      {
         folderName.CopyTo(targetFolder, overwrite);
         return targetFolder;
      });

      public IResult<FolderName> CopyTo(FolderName targetFolder, string includePattern, string excludePattern, bool overwrite)
      {
         return tryTo(() =>
         {
            folderName.FileSuccess += FileSuccess;
            folderName.CopyTo(targetFolder, includePattern, excludePattern, overwrite);

            return targetFolder;
         }).OnFailure(e => folderName.FileSuccess -= FileSuccess);
      }

      public IResult<FolderName> Delete() => tryTo(() =>
      {
         folderName.Delete();
         return folderName;
      });

      public IResult<FolderName> DeleteAll() => tryTo(() =>
      {
         folderName.DeleteAll();
         return folderName;
      });

      public IResult<FolderName> DeleteFiles() => tryTo(() =>
      {
         folderName.DeleteFiles();
         return folderName;
      });

      public IResult<FolderName> MoveTo(FolderName targetFolder) => tryTo(() =>
      {
         folderName.MoveTo(targetFolder);
         return targetFolder;
      });

      public IResult<FileName[]> Files => tryTo(() => folderName.Files.ToArray());

      public IResult<FolderName[]> Folders => tryTo(() => folderName.Folders.ToArray());

      public IResult<int> FileCount => tryTo(() => folderName.FileCount);

      public IResult<FolderName> Exists()
      {
         return assert(folderName.Exists(), () => folderName, $"Folder {folderName} doesnt exist");
      }

      public IResult<FolderName> this[string subfolder] => tryTo(() =>
      {
         var name = folderName[subfolder];
         return assert(() => name.Exists(), () => name, $"Folder {name} doesn't exist");
      });

      public IResult<FolderName> Subfolder(string name) => tryTo(() => folderName.Subfolder(name));

      public IResult<FolderName> CreateIfNonExistent()
      {
         return
            from unit in tryTo(() => folderName.CreateIfNonExistent())
            select folderName;
      }

      public IResult<FolderName> Guarantee() => tryTo(() => folderName.Guarantee());

      public IResult<FolderName> Parent
      {
         get
         {
            return tryTo(() =>
            {
               var parent = folderName.Parent;
               return parent.FlatMap(p => p.Success(), () => $"{folderName} doesn't have a parent".Failure<FolderName>());
            });
         }
      }
   }
}