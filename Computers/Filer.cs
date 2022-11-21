using System;

namespace Core.Computers;

public class Filer
{
   public static FolderName CurrentFolder
   {
      get => FolderName.Current;
      set => FolderName.Current = value;
   }

   public void ForEach(Action<FileName> action, bool recursive) => forEach(action, CurrentFolder, recursive);

   protected static void forEach(Action<FileName> action, FolderName folder, bool recursive)
   {
      foreach (var file in folder.Files)
      {
         action(file);
      }

      if (recursive) { }
   }
}