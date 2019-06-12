using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.RegularExpressions;
using Core.Strings;

namespace Core.Computers
{
   public class FolderIterator : IEnumerable<FolderName>
   {
      string predecessor;
      string[] folderParts;
      bool endOnly;

      public FolderIterator(string pattern, bool endOnly)
      {
         predecessor = "";
         var prefix = "";
         if (pattern.StartsWith(@"\\"))
         {
            prefix = pattern.Keep(2);
            pattern = pattern.Drop(2);
         }
         else if (pattern.IsMatch(@"^ ['A-Za-z'] ':\'"))
         {
            prefix = pattern.Keep(3);
            pattern = pattern.Drop(3);
         }

         folderParts = pattern.Split('\\');
         if (folderParts.Length > 0 && prefix.IsNotEmpty())
            folderParts[0] = $"{prefix}{folderParts[0]}";

         this.endOnly = endOnly;
      }

      public FolderIterator(string predecessor, string[] folderParts, bool endOnly)
      {
         this.predecessor = predecessor;
         this.folderParts = folderParts;
         this.endOnly = endOnly;
      }

      public IEnumerator<FolderName> GetEnumerator()
      {
         var current = predecessor;

         for (var index = 0; index < folderParts.Length; index++)
         {
            var part = folderParts[index];
            if (part.StartsWith("~"))
            {
               part = part.Drop(1);
               foreach (var folder in ((FolderName)current).Folders.Where(f => f.Name.IsMatch(part)))
               {
                  if (!endOnly || index == folderParts.Length - 1)
                     yield return folder;

                  var iterator = new FolderIterator(folder.FullPath, folderParts.Skip(index + 1).ToArray(), endOnly);
                  foreach (var subfolder in iterator)
                     yield return subfolder;
               }
            }
            else if (current.IsEmpty())
               current = part;
            else
               current = $"{current}\\{part}";
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}