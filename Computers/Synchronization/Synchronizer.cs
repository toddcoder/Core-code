using System;
using System.Linq;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Monads.MonadFunctions;

namespace Core.Computers.Synchronization
{
   public class Synchronizer
   {
      FolderName sourceFolder;
      FolderName targetFolder;
      string pattern;
      bool move;

      public event EventHandler<FileArgs> Success;
      public event EventHandler<FailedFileArgs> Failure;
      public event EventHandler<FileArgs> Untouched;

      public Synchronizer(FolderName sourceFolder, FolderName targetFolder, string pattern, bool move)
      {
         this.sourceFolder = sourceFolder;
         this.targetFolder = targetFolder;
         this.pattern = pattern;
         this.move = move;
      }

      public void Synchronize()
      {
         foreach (var sourceFile in sourceFolder.Files.Where(f => f.NameExtension.IsMatch(pattern)))
         {
            var targetFile = targetFolder + sourceFile;
            if (copyIfNeeded(sourceFile, targetFile).If(out var file, out var anyException))
            {
               Success?.Invoke(this, new FileArgs(file, targetFile, $"{sourceFile} {(move ? "moved" : "copied")} to {targetFile}"));
            }
            else if (anyException.If(out var exception))
            {
               Failure?.Invoke(this, new FailedFileArgs(sourceFile, targetFile, exception));
            }
            else
            {
               Untouched?.Invoke(this, new FileArgs(sourceFile, targetFile, $"{sourceFile} not touched"));
            }
         }
      }

      IMatched<FileName> copyIfNeeded(FileName sourceFile, FileName targetFile)
      {
         try
         {
            var result =
               from targetExists in targetFile.TryTo.Exists()
               from sourceLastWriteTime in sourceFile.TryTo.LastWriteTime
               from targetLastWriteTime in targetFile.TryTo.LastWriteTime
               select !targetExists || sourceLastWriteTime != targetLastWriteTime;
            if (result.If(out var mustCopy, out var exception))
            {
               if (mustCopy)
               {
                  return copy(sourceFile, targetFile);
               }
               else
               {
                  return notMatched<FileName>();
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

      IMatched<FileName> copy(FileName sourceFile, FileName targetFile)
      {
         if (sourceFile.TryTo.CopyTo(targetFile, true).If(out _, out var exception))
         {
            if (move)
            {
               if (sourceFile.TryTo.Delete().If(out _, out exception))
               {
                  return targetFile.Matched();
               }
               else
               {
                  return failedMatch<FileName>(exception);
               }
            }
            else
            {
               return targetFile.Matched();
            }
         }
         else
         {
            return failedMatch<FileName>(exception);
         }
      }
   }
}