using System;
using System.Linq;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Computers
{
   public class FolderNameTrying
   {
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

      public IResult<FolderName> this[string subfolder] => tryTo(() =>
      {
         var name = folderName[subfolder];
         return name.Must().Exist().Try();
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
               return parent.Map(p => p.Success()).DefaultTo(() => $"{folderName} doesn't have a parent".Failure<FolderName>());
            });
         }
      }

      public IResult<FolderName> SetAsCurrent() => tryTo(() =>
      {
         FolderName.Current = folderName;
         return FolderName.Current;
      });

      public IResult<string> RelativeTo(FileName file) => tryTo(() => folderName.RelativeTo(file));

      public IResult<string> RelativeTo(FolderName folder) => tryTo(() => folderName.RelativeTo(folder));

      public IResult<FolderName> AbsoluteFolder(string relativePath) => tryTo(() => folderName.AbsoluteFolder(relativePath));

      public IResult<FileName> AbsoluteFile(string relativePath) => tryTo(() => folderName.AbsoluteFile(relativePath));

      public IResult<string> AbsoluteString(string relativePath) => tryTo(() => folderName.AbsoluteString(relativePath));

      public IResult<bool> WasCreated() => tryTo(() => folderName.WasCreated());
   }
}