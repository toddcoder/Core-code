using System;
using System.Linq;
using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Computers.Synchronization;

public class Synchronizer
{
   protected FolderName sourceFolder;
   protected FolderName targetFolder;
   protected Pattern pattern;
   protected bool move;
   protected bool recursive;

   public event EventHandler<FileArgs> Success;
   public event EventHandler<FailedFileArgs> Failure;
   public event EventHandler<FileArgs> Untouched;
   public event EventHandler<FolderArgs> NewFolderSuccess;
   public event EventHandler<FailedFolderArgs> NewFolderFailure;

   public Synchronizer(FolderName sourceFolder, FolderName targetFolder, Pattern pattern, bool move = false, bool recursive = true)
   {
      this.sourceFolder = sourceFolder;
      this.targetFolder = targetFolder;
      this.pattern = pattern;
      this.move = move;
      this.recursive = recursive;
   }

   public Synchronizer(FolderName sourceFolder, FolderName targetFolder, bool move = false, bool recursive = true) :
      this(sourceFolder, targetFolder, "^ .* $; f", move, recursive)
   {
   }

   public void Synchronize() => handleFolder(sourceFolder, targetFolder);

   public void Synchronize(params string[] fileNames)
   {
      foreach (var fileName in fileNames)
      {
         handleFile(sourceFolder + fileName, targetFolder);
      }
   }

   protected void handleFolder(FolderName currentSourceFolder, FolderName currentTargetFolder)
   {
      if (recursive)
      {
         foreach (var subFolder in currentSourceFolder.Folders)
         {
            handleFolder(subFolder, currentTargetFolder[subFolder.Name]);
         }
      }

      foreach (var sourceFile in currentSourceFolder.Files.Where(f => f.NameExtension.IsMatch(pattern)))
      {
         handleFile(sourceFile, currentTargetFolder);
      }
   }

   protected void handleFile(FileName sourceFile, FolderName currentTargetFolder)
   {
      var targetFile = currentTargetFolder + sourceFile;
      var _file = copyIfNeeded(sourceFile, targetFile);
      if (_file)
      {
         Success?.Invoke(this, new FileArgs(_file, targetFile, $"{sourceFile} {(move ? "moved" : "copied")} to {targetFile}"));
      }
      else if (_file.AnyException)
      {
         Failure?.Invoke(this, new FailedFileArgs(sourceFile, targetFile, _file.Exception));
      }
      else
      {
         Untouched?.Invoke(this, new FileArgs(sourceFile, targetFile, $"{sourceFile} not touched"));
      }
   }

   protected Responding<FileName> copyIfNeeded(FileName sourceFile, FileName targetFile)
   {
      try
      {
         var _mustCopy =
            from targetExists in targetFile.TryTo.Exists()
            from sourceLastWriteTime in sourceFile.TryTo.LastWriteTime
            from targetLastWriteTime in targetFile.TryTo.LastWriteTime
            select !targetExists || sourceLastWriteTime > targetLastWriteTime;
         if (_mustCopy.IsSuccess())
         {
            if (~_mustCopy)
            {
               return copy(sourceFile, targetFile);
            }
            else
            {
               return nil;
            }
         }
         else
         {
            return _mustCopy.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Responding<FileName> copy(FileName sourceFile, FileName targetFile)
   {
      var targetFileFolder = targetFile.Folder;
      var _wasCreated = targetFileFolder.TryTo.WasCreated();
      if (_wasCreated.IsSuccess())
      {
         if (~_wasCreated)
         {
            NewFolderSuccess?.Invoke(this, new FolderArgs(targetFileFolder, $"Folder {targetFileFolder} created"));
         }
      }
      else
      {
         NewFolderFailure?.Invoke(this, new FailedFolderArgs(targetFileFolder, _wasCreated.Exception));
         return fail("Folder creation failed; no action taken");
      }

      var _copied = sourceFile.TryTo.CopyTo(targetFile, true);
      if (_copied)
      {
         if (move)
         {
            var _deleted = sourceFile.TryTo.Delete();
            if (_deleted)
            {
               return targetFile;
            }
            else
            {
               return _deleted.Exception;
            }
         }
         else
         {
            return targetFile;
         }
      }
      else
      {
         return _copied.Exception;
      }
   }

   public SynchronizerTrying TryTo => new(this);
}