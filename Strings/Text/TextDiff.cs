using System;
using System.Collections.Generic;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text
{
   public class TextDiff
   {
      string[] oldText;
      string[] newText;
      bool ignoreWhiteSpace;
      bool ignoreCase;

      delegate void ItemBuilder(string oldText, string newText, List<DiffItem> oldItems, List<DiffItem> newItems);

      public TextDiff(string[] oldText, string[] newText, bool ignoreWhiteSpace, bool ignoreCase)
      {
         this.oldText = oldText;
         this.newText = newText;
         this.ignoreWhiteSpace = ignoreWhiteSpace;
         this.ignoreCase = ignoreCase;
      }

      public IResult<Model> Build()
      {
         var differ = new TextDiffer();
         if (differ.CreateDiffs(oldText, newText, ignoreWhiteSpace, ignoreCase).If(out var result, out var exception))
         {
            var model = new Model();
            ItemBuilder itemBuilder = buildItemsNoSub;
            buildItems(result, model.OldDiffItems, model.NewDiffItems, itemBuilder.Some());
            return model.Success();
         }
         else
         {
            return failure<Model>(exception);
         }
      }

      static void buildItemsNoSub(string oldLine, string newLine, List<DiffItem> oldItems, List<DiffItem> newItems)
      {
         var oldWords = oldLine.Split("/s+");
         var newWords = newLine.Split("/s+");

         var differ = new TextDiffer();

         if (differ.CreateDiffs(oldWords, newWords, false, false).If(out var wordResult))
         {
            buildItems(wordResult, oldItems, newItems, none<ItemBuilder>());
         }
      }

      static void buildItems(TextDiffResult result, List<DiffItem> oldItems, List<DiffItem> newItems, IMaybe<ItemBuilder> anySubItemBuilder)
      {
         var aPosition = 0;
         var bPosition = 0;

         foreach (var diffBlock in result.TextDiffBlocks)
         {
            while (bPosition < diffBlock.InsertStartB && aPosition < diffBlock.DeleteStartA)
            {
               oldItems.Add(new DiffItem(result.OldItems[aPosition], DiffType.Unchanged, aPosition + 1));
               newItems.Add(new DiffItem(result.NewItems[bPosition], DiffType.Unchanged, bPosition + 1));
               aPosition++;
               bPosition++;
            }

            var i = 0;
            while (i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB))
            {
               var oldItem = new DiffItem(result.OldItems[i + diffBlock.DeleteStartA], DiffType.Deleted, aPosition + 1);
               var newItem = new DiffItem(result.NewItems[i + diffBlock.InsertStartB], DiffType.Inserted, bPosition + 1);
               if (anySubItemBuilder.If(out var subItemBuilder))
               {
                  var oldWords = result.OldItems[aPosition].Split("/s+");
                  var newWords = result.NewItems[aPosition].Split("/s+");
                  var differ = new TextDiffer();

                  if (differ.CreateDiffs(oldWords, newWords, false, false).If(out var wordResult))
                  {
                     subItemBuilder(wordResult.OldItems[aPosition], wordResult.NewItems[bPosition], oldItem.SubItems, newItem.SubItems);
                     newItem.Type = DiffType.Modified;
                     oldItem.Type = DiffType.Modified;
                  }
               }

               oldItems.Add(oldItem);
               newItems.Add(newItem);

               aPosition++;
               bPosition++;
               i++;
            }

            if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
            {
               while (i < diffBlock.DeleteCountA)
               {
                  oldItems.Add(new DiffItem(result.OldItems[i + diffBlock.DeleteStartA], DiffType.Deleted, aPosition + 1));
                  newItems.Add(new DiffItem());

                  aPosition++;
                  i++;
               }
            }
         }

         while (bPosition < result.NewItems.Length && aPosition < result.OldItems.Length)
         {
            oldItems.Add(new DiffItem(result.OldItems[aPosition], DiffType.Unchanged, aPosition + 1));
            newItems.Add(new DiffItem(result.NewItems[bPosition], DiffType.Unchanged, bPosition + 1));

            aPosition++;
            bPosition++;
         }
      }
   }
}