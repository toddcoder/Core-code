using Core.Computers;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications;

public class Argument
{
   protected string text;
   protected int index;
   protected Maybe<FileName> fileName;
   protected Maybe<FolderName> folderName;

   public Argument(string text, int index)
   {
      this.text = text;
      this.index = index;
      fileName = nil;
      folderName = nil;
   }

   public string Text => text;

   public Maybe<FileName> FileName
   {
      get
      {
         if (Computers.FileName.IsValidFileName(text))
         {
            FileName file = text;
            fileName = maybe(file.Exists(), () => file);
         }
         else
         {
            fileName = nil;
         }

         return fileName;
      }
   }

   public Maybe<FolderName> FolderName
   {
      get
      {
         if (Computers.FolderName.IsValidFolderName(text))
         {
            FolderName folder = text;
            folderName = maybe(folder.Exists(), () => folder);
         }
         else
         {
            folderName = nil;
         }

         return folderName;
      }
   }

   public int Index => index;

   public ArgumentTrying TryTo => new(this);
}