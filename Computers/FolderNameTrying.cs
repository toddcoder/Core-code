using System;
using System.Linq;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Computers;

public class FolderNameTrying
{
   protected FolderName folderName;

   public event EventHandler<FileArgs> FileSuccess;

   public FolderNameTrying(FolderName folderName) => this.folderName = folderName;

   public FolderName FolderName => folderName;

   public Result<FolderName> CopyTo(FolderName targetFolder, bool overwrite) => tryTo(() =>
   {
      folderName.CopyTo(targetFolder, overwrite);
      return targetFolder;
   });

   public Result<FolderName> CopyTo(FolderName targetFolder, string includePattern, string excludePattern, bool overwrite)
   {
      return tryTo(() =>
      {
         folderName.FileSuccess += FileSuccess;
         folderName.CopyTo(targetFolder, includePattern, excludePattern, overwrite);

         return targetFolder;
      }).OnFailure(_ => folderName.FileSuccess -= FileSuccess);
   }

   public Result<FolderName> Delete() => tryTo(() =>
   {
      folderName.Delete();
      return folderName;
   });

   public Result<FolderName> DeleteAll() => tryTo(() =>
   {
      folderName.DeleteAll();
      return folderName;
   });

   public Result<FolderName> DeleteFiles() => tryTo(() =>
   {
      folderName.DeleteFiles();
      return folderName;
   });

   public Result<FolderName> MoveTo(FolderName targetFolder) => tryTo(() =>
   {
      folderName.MoveTo(targetFolder);
      return targetFolder;
   });

   public Result<FileName[]> Files => tryTo(() => folderName.Files.ToArray());

   public Result<FolderName[]> Folders => tryTo(() => folderName.Folders.ToArray());

   public Result<int> FileCount => tryTo(() => folderName.FileCount);

   public Result<FolderName> this[string subfolder] => tryTo(() =>
   {
      var name = folderName[subfolder];
      return name.Must().Exist().OrFailure();
   });

   public Result<FolderName> Subfolder(string name) => tryTo(() => folderName.Subfolder(name));

   public Result<FolderName> CreateIfNonExistent()
   {
      return
         from unit in tryTo(() => folderName.CreateIfNonExistent())
         select folderName;
   }

   public Result<FolderName> Guarantee() => tryTo(() => folderName.Guarantee());

   public Result<FolderName> Parent => folderName.Parent.Result($"{folderName} doesn't have a parent");

   public Result<FolderName> SetAsCurrent() => tryTo(() =>
   {
      FolderName.Current = folderName;
      return FolderName.Current;
   });

   public Result<string> RelativeTo(FileName file) => tryTo(() => folderName.RelativeTo(file));

   public Result<string> RelativeTo(FolderName folder) => tryTo(() => folderName.RelativeTo(folder));

   public Result<FolderName> AbsoluteFolder(string relativePath) => tryTo(() => folderName.AbsoluteFolder(relativePath));

   public Result<FileName> AbsoluteFile(string relativePath) => tryTo(() => folderName.AbsoluteFile(relativePath));

   public Result<string> AbsoluteString(string relativePath) => tryTo(() => folderName.AbsoluteString(relativePath));

   public Result<bool> WasCreated() => tryTo(() => folderName.WasCreated());

   public Result<bool> Exists() => tryTo(() => folderName.Exists());

   public Result<FolderName> Existing() => folderName.Must().Exist().OrFailure($"Folder {folderName.FullPath} doesn't exist");
}