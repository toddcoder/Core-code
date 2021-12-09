﻿using System.Collections.Generic;
using Core.Enumerables;

namespace Core.Git
{
   public class FileCounter
   {
      protected bool isIndexed;
      protected int added;
      protected int modified;
      protected int deleted;
      protected int conflicted;

      public FileCounter(bool isIndexed)
      {
         this.isIndexed = isIndexed;

         added = 0;
         modified = 0;
         deleted = 0;
         conflicted = 0;
      }

      public void Increment(string code)
      {
         switch (code)
         {
            case "A":
               added++;
               break;
            case "?" when !isIndexed:
               added++;
               break;
            case "M":
               modified++;
               break;
            case "D":
               deleted++;
               break;
            case " ":
               break;
            default:
               if (!isIndexed)
               {
                  conflicted++;
               }
               break;
         }
      }

      public override string ToString()
      {
         var status = new List<string>();
         if (added > 0)
         {
            status.Add($"+{added}");
         }

         if (modified > 0)
         {
            status.Add($"~{modified}");
         }

         if (deleted > 0)
         {
            status.Add($"-{deleted}");
         }

         if (conflicted > 0)
         {
            status.Add($"!{conflicted}");
         }

         return status.ToString(" ");
      }
   }
}