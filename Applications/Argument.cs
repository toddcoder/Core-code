using Core.Computers;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications
{
   public class Argument
   {
      protected string text;
      protected int index;
      protected IMaybe<FileName> fileName;
      protected IMaybe<FolderName> folderName;

      public Argument(string text, int index)
      {
         this.text = text;
         this.index = index;
         fileName = none<FileName>();
         folderName = none<FolderName>();
      }

      public string Text => text;

      public IMaybe<FileName> FileName
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
               fileName = none<FileName>();
            }

            return fileName;
         }
      }

      public IMaybe<FolderName> FolderName
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
               folderName = none<FolderName>();
            }

            return folderName;
         }
      }

      public int Index => index;

      public ArgumentTrying TryTo => new ArgumentTrying(this);
   }
}