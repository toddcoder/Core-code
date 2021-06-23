using System;
using System.Linq;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Computers.Synchronization
{
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
         if (copyIfNeeded(sourceFile, targetFile).If(out var file, out var _exception))
         {
            Success?.Invoke(this, new FileArgs(file, targetFile, $"{sourceFile} {(move ? "moved" : "copied")} to {targetFile}"));
         }
         else if (_exception.If(out var exception))
         {
            Failure?.Invoke(this, new FailedFileArgs(sourceFile, targetFile, exception));
         }
         else
         {
            Untouched?.Invoke(this, new FileArgs(sourceFile, targetFile, $"{sourceFile} not touched"));
         }
      }

      protected Matched<FileName> copyIfNeeded(FileName sourceFile, FileName targetFile)
      {
         try
         {
            var result =
               from targetExists in targetFile.TryTo.Exists()
               from sourceLastWriteTime in sourceFile.TryTo.LastWriteTime
               from targetLastWriteTime in targetFile.TryTo.LastWriteTime
               select !targetExists || sourceLastWriteTime > targetLastWriteTime;
            if (result.If(out var mustCopy, out var exception))
            {
               if (mustCopy)
               {
                  return copy(sourceFile, targetFile);
               }
               else
               {
                  return noMatch<FileName>();
               }
            }
            else
            {
               return failedMatch<FileName>(exception);
            }
         }
         catch (Exception exception)
         {
            return failedMatch<FileName>(exception);
         }
      }

      protected Matched<FileName> copy(FileName sourceFile, FileName targetFile)
      {
         var targetFileFolder = targetFile.Folder;
         var _wasCreated = targetFileFolder.TryTo.WasCreated();
         if (_wasCreated.If(out var wasCreated, out var exception))
         {
            if (wasCreated)
            {
               NewFolderSuccess?.Invoke(this, new FolderArgs(targetFileFolder, $"Folder {targetFileFolder} created"));
            }
         }
         else
         {
            NewFolderFailure?.Invoke(this, new FailedFolderArgs(targetFileFolder, exception));
            return failedMatch<FileName>("Folder creation failed; no action taken".Throws());
         }

         if (sourceFile.TryTo.CopyTo(targetFile, true).If(out _, out exception))
         {
            if (move)
            {
               if (sourceFile.TryTo.Delete().If(out _, out exception))
               {
                  return targetFile.Match();
               }
               else
               {
                  return failedMatch<FileName>(exception);
               }
            }
            else
            {
               return targetFile.Match();
            }
         }
         else
         {
            return failedMatch<FileName>(exception);
         }
      }

      public SynchronizerTrying TryTo => new(this);
   }
}