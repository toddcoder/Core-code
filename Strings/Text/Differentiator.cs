using System;
using System.Collections.Generic;
using Core.Matching;
using Core.Monads;
using static Core.Arrays.ArrayFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text
{
   public class Differentiator
   {
      protected static string[] splitWords(string line) => line.Split(array(' ', '\t', '.', '(', ')', '{', '}', ',', '!'));

      protected string[] oldText;
      protected string[] newText;
      protected bool ignoreWhiteSpace;
      protected bool ignoreCase;

      protected delegate void ItemBuilder(string oldText, string newText, List<DifferenceItem> oldItems, List<DifferenceItem> newItems);

      public Differentiator(string[] oldText, string[] newText, bool ignoreWhiteSpace, bool ignoreCase)
      {
         this.oldText = oldText;
         this.newText = newText;
         this.ignoreWhiteSpace = ignoreWhiteSpace;
         this.ignoreCase = ignoreCase;
      }

      public Result<DifferenceModel> BuildModel()
      {
         var differ = new DifferenceBuilder(oldText, newText, ignoreWhiteSpace, ignoreCase);
         if (differ.Build().ValueOrCast<DifferenceModel>(out var result, out var asDifferenceModel))
         {
            var model = new DifferenceModel();
            ItemBuilder itemBuilder = buildItemsNoSub;
            buildItems(result, model.OldDifferenceItems, model.NewDifferenceItems, itemBuilder.Some());

            return model.Success();
         }
         else
         {
            return asDifferenceModel;
         }
      }

      protected static void buildItemsNoSub(string oldLine, string newLine, List<DifferenceItem> oldItems, List<DifferenceItem> newItems)
      {
         var oldWords = splitWords(oldLine);
         var newWords = splitWords(newLine);

         var differ = new DifferenceBuilder(oldWords, newWords, false, false);

         if (differ.Build().If(out var wordResult))
         {
            buildItems(wordResult, oldItems, newItems, none<ItemBuilder>());
         }
      }

      protected static void buildItems(DifferenceResult result, List<DifferenceItem> oldItems, List<DifferenceItem> newItems,
         Maybe<ItemBuilder> _subItemBuilder)
      {
         var oldPosition = 0;
         var newPosition = 0;

         foreach (var diffBlock in result.DifferenceBlocks)
         {
            while (newPosition < diffBlock.NewInsertStart && oldPosition < diffBlock.OldDeleteStart)
            {
               oldItems.Add(new DifferenceItem(result.OldItems[oldPosition], DifferenceType.Unchanged, oldPosition + 1));
               newItems.Add(new DifferenceItem(result.NewItems[newPosition], DifferenceType.Unchanged, newPosition + 1));
               oldPosition++;
               newPosition++;
            }

            var i = 0;
            while (i < Math.Min(diffBlock.OldDeleteCount, diffBlock.NewInsertCount))
            {
               var oldItem = new DifferenceItem(result.OldItems[i + diffBlock.OldDeleteStart], DifferenceType.Deleted,
                  oldPosition + 1);
               var newItem = new DifferenceItem(result.NewItems[i + diffBlock.NewInsertStart], DifferenceType.Inserted,
                  newPosition + 1);
               if (_subItemBuilder.If(out var subItemBuilder))
               {
                  var oldWords = result.OldItems[oldPosition].Split("/s+; f");
                  var newWords = result.NewItems[newPosition].Split("/s+; f");
                  var differ = new DifferenceBuilder(oldWords, newWords, false, false);

                  if (differ.Build().If(out _))
                  {
                     subItemBuilder(result.OldItems[oldPosition], result.NewItems[newPosition], oldItem.SubItems, newItem.SubItems);
                     newItem.Type = DifferenceType.Modified;
                     oldItem.Type = DifferenceType.Modified;
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
                  oldItems.Add(new DifferenceItem(result.OldItems[i + diffBlock.OldDeleteStart], DifferenceType.Deleted,
                     oldPosition + 1));
                  newItems.Add(new DifferenceItem());

                  oldPosition++;
                  i++;
               }
            }
            else
            {
               while (i < diffBlock.NewInsertCount)
               {
                  newItems.Add(new DifferenceItem(result.NewItems[i + diffBlock.NewInsertStart], DifferenceType.Inserted,
                     newPosition + 1));
                  oldItems.Add(new DifferenceItem());

                  newPosition++;
                  i++;
               }
            }
         }

         while (newPosition < result.NewItems.Length && oldPosition < result.OldItems.Length)
         {
            oldItems.Add(new DifferenceItem(result.OldItems[oldPosition], DifferenceType.Unchanged, oldPosition + 1));
            newItems.Add(new DifferenceItem(result.NewItems[newPosition], DifferenceType.Unchanged, newPosition + 1));

            oldPosition++;
            newPosition++;
         }
      }
   }
}