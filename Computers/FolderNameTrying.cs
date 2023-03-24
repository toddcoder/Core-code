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

   public Optional<FolderName> CopyTo(FolderName targetFolder, bool overwrite) => tryTo(() =>
   {
      folderName.CopyTo(targetFolder, overwrite);
      return targetFolder;
   });

   public Optional<FolderName> CopyTo(FolderName targetFolder, string includePattern, string excludePattern, bool overwrite)
   {
      return tryTo(() =>
      {
         folderName.FileSuccess += FileSuccess;
         folderName.CopyTo(targetFolder, includePattern, excludePattern, overwrite);

         return targetFolder;
      }).OnFailure(_ => folderName.FileSuccess -= FileSuccess);
   }

   public Optional<FolderName> Delete() => tryTo(() =>
   {
      folderName.Delete();
      return folderName;
   });

   public Optional<FolderName> DeleteAll() => tryTo(() =>
   {
      folderName.DeleteAll();
      return folderName;
   });

   public Optional<FolderName> DeleteFiles() => tryTo(() =>
   {
      folderName.DeleteFiles();
      return folderName;
   });

   public Optional<FolderName> MoveTo(FolderName targetFolder) => tryTo(() =>
   {
      folderName.MoveTo(targetFolder);
      return targetFolder;
   });

   public Optional<FileName[]> Files => tryTo(() => folderName.Files.ToArray());

   public Optional<FolderName[]> Folders => tryTo(() => folderName.Folders.ToArray());

   public Optional<int> FileCount => tryTo(() => folderName.FileCount);

   public Optional<FolderName> this[string subfolder] => tryTo(() =>
   {
      var name = folderName[subfolder];
      return name.Must().Exist().OrFailure();
   });

   public Optional<FolderName> Subfolder(string name) => tryTo(() => folderName.Subfolder(name));

   public Optional<FolderName> CreateIfNonExistent()
   {
      return
         from unit in tryTo(() => folderName.CreateIfNonExistent())
         select folderName;
   }

   public Optional<FolderName> Guarantee() => tryTo(() => folderName.Guarantee());

   public Optional<FolderName> Parent => folderName.Parent.Result($"{folderName} doesn't have a parent");

   public Optional<FolderName> SetAsCurrent() => tryTo(() =>
   {
      FolderName.Current = folderName;
      return FolderName.Current;
   });

   public Optional<string> RelativeTo(FileName file) => tryTo(() => folderName.RelativeTo(file));

   public Optional<string> RelativeTo(FolderName folder) => tryTo(() => folderName.RelativeTo(folder));

   public Optional<FolderName> AbsoluteFolder(string relativePath) => tryTo(() => folderName.AbsoluteFolder(relativePath));

   public Optional<FileName> AbsoluteFile(string relativePath) => tryTo(() => folderName.AbsoluteFile(relativePath));

   public Optional<string> AbsoluteString(string relativePath) => tryTo(() => folderName.AbsoluteString(relativePath));

   public Optional<bool> WasCreated() => tryTo(() => folderName.WasCreated());

   public Optional<bool> Exists() => tryTo(() => folderName.Exists());

   public Optional<FolderName> Existing() => folderName.Must().Exist().OrFailure($"Folder {folderName.FullPath} doesn't exist");
}