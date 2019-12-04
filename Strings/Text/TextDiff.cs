using System;
using System.Collections.Generic;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Arrays.ArrayFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text
{
   public class TextDiff
   {
      static string[] splitWords(string line) => line.Split(array(' ', '\t', '.', '(', ')', '{', '}', ',', '!'));

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
         var oldWords = splitWords(oldLine);
         var newWords = splitWords(newLine);

         var differ = new TextDiffer();

         if (differ.CreateDiffs(oldWords, newWords, false, false).If(out var wordResult))
         {
            buildItems(wordResult, oldItems, newItems, none<ItemBuilder>());
         }
      }

      static void buildItems(TextDiffResult result, List<DiffItem> oldItems, List<DiffItem> newItems, IMaybe<ItemBuilder> anySubItemBuilder)
      {
         var oldPosition = 0;
         var newPosition = 0;

         foreach (var diffBlock in result.TextDiffBlocks)
         {
            while (newPosition < diffBlock.NewInsertStart && oldPosition < diffBlock.OldDeleteStart)
            {
               oldItems.Add(new DiffItem(result.OldItems[oldPosition], DiffType.Unchanged, oldPosition + 1));
               newItems.Add(new DiffItem(result.NewItems[newPosition], DiffType.Unchanged, newPosition + 1));
               oldPosition++;
               newPosition++;
            }

            var i = 0;
            while (i < Math.Min(diffBlock.OldDeleteCount, diffBlock.NewInsertCount))
            {
               var oldItem = new DiffItem(result.OldItems[i + diffBlock.OldDeleteStart], DiffType.Deleted, oldPosition + 1);
               var newItem = new DiffItem(result.NewItems[i + diffBlock.NewInsertStart], DiffType.Inserted, newPosition + 1);
               if (anySubItemBuilder.If(out var subItemBuilder))
               {
                  var oldWords = result.OldItems[oldPosition].Split("/s+");
                  var newWords = result.NewItems[oldPosition].Split("/s+");
                  var differ = new TextDiffer();

                  if (differ.CreateDiffs(oldWords, newWords, false, false).If(out _))
                  {
                     subItemBuilder(result.OldItems[oldPosition], result.NewItems[newPosition], oldItem.SubItems, newItem.SubItems);
                     newItem.Type = DiffType.Modified;
                     oldItem.Type = DiffType.Modified;
                  }
               }

               oldItems.Add(oldItem);
               newItems.Add(newItem);

               oldPosition++;
               newPosition++;
               i++;
            }

            if (diffBlock.OldDeleteCount > diffBlock.NewInsertCount)
            {
               while (i < diffBlock.OldDeleteCount)
               {
                  oldItems.Add(new DiffItem(result.OldItems[i + diffBlock.OldDeleteStart], DiffType.Deleted, oldPosition + 1));
                  newItems.Add(new DiffItem());

                  oldPosition++;
                  i++;
               }
            }
         }

         while (newPosition < result.NewItems.Length && oldPosition < result.OldItems.Length)
         {
            oldItems.Add(new DiffItem(result.OldItems[oldPosition], DiffType.Unchanged, oldPosition + 1));
            newItems.Add(new DiffItem(result.NewItems[newPosition], DiffType.Unchanged, newPosition + 1));

            oldPosition++;
            newPosition++;
         }
      }
   }
}