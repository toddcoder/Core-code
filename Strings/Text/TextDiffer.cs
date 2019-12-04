using System.Collections.Generic;
using Core.Assertions;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text
{
   internal class TextDiffer
   {
      static void buildItemHashes(Hash<string, int> itemHash, ModificationData data, bool ignoreWhiteSpace, bool ignoreCase)
      {
         var items = data.RawData;
         data.Items = items;
         data.HashedItems = new int[items.Length];
         data.Modifications = new bool[items.Length];

         for (var i = 0; i < items.Length; i++)
         {
            var item = items[i];
            if (ignoreWhiteSpace)
            {
               item = item.Trim();
            }

            if (ignoreCase)
            {
               item = item.ToUpperInvariant();
            }

            if (itemHash.If(item, out var value))
            {
               data.HashedItems[i] = value;
            }
            else
            {
               data.HashedItems[i] = itemHash.Count;
               itemHash[item] = itemHash.Count;
            }
         }
      }

      static IResult<Unit> buildModificationData(ModificationData oldData, ModificationData newData)
      {
         var n = oldData.HashedItems.Length;
         var m = newData.HashedItems.Length;
         var max = m + n + 1;
         var forwardDiagonal = new int[max + 1];
         var reverseDiagonal = new int[max + 1];

         return buildModificationData(oldData, 0, n, newData, 0, m, forwardDiagonal, reverseDiagonal);
      }

      static IResult<Unit> buildModificationData(ModificationData oldData, int oldStart, int oldEnd, ModificationData newData, int newStart,
         int newEnd, int[] forwardDiagonal, int[] reverseDiagonal)
      {
         while (oldStart < oldEnd && newStart < newEnd && oldData.HashedItems[oldStart] == newData.HashedItems[newStart])
         {
            oldStart++;
            newStart++;
         }

         while (oldStart < oldEnd && newStart < newEnd && oldData.HashedItems[oldEnd - 1] == newData.HashedItems[newEnd - 1])
         {
            oldEnd--;
            newEnd--;
         }

         var oldLength = oldEnd - oldStart;
         var newLength = newEnd - newStart;
         if (oldLength > 0 && newLength > 0)
         {
            if (calculateEditLength(oldData.HashedItems, oldStart, oldEnd, newData.HashedItems, newStart, newEnd, forwardDiagonal, reverseDiagonal)
               .If(out var result, out var exception))
            {
               if (result.EditLength <= 0)
               {
                  return Unit.Success();
               }

               switch (result.LastEdit)
               {
                  case EditType.DeleteRight when result.OldStart - 1 > oldStart:
                     oldData.Modifications[--result.OldStart] = true;
                     break;
                  case EditType.InsertDown when result.NewStart - 1 > newStart:
                     newData.Modifications[--result.NewStart] = true;
                     break;
                  case EditType.DeleteLeft when result.OldEnd < oldEnd:
                     oldData.Modifications[result.OldEnd++] = true;
                     break;
                  case EditType.InsertUp when result.NewEnd < newEnd:
                     newData.Modifications[result.NewEnd++] = true;
                     break;
               }

               var resultAll =
                  from resultA in buildModificationData(oldData, oldStart, result.OldStart, newData, newStart, result.NewStart, forwardDiagonal,
                     reverseDiagonal)
                  from resultB in buildModificationData(oldData, result.OldEnd, oldEnd, newData, result.NewEnd, newEnd, forwardDiagonal,
                     reverseDiagonal)
                  select resultB;
               if (resultAll.IfNot(out exception))
               {
                  return failure<Unit>(exception);
               }
            }
            else
            {
               return failure<Unit>(exception);
            }
         }
         else if (oldLength > 0)
         {
            for (var i = oldStart; i < oldEnd; i++)
            {
               oldData.Modifications[i] = true;
            }
         }
         else if (newLength > 0)
         {
            for (var i = newStart; i < newEnd; i++)
            {
               newData.Modifications[i] = true;
            }
         }

         return Unit.Success();
      }

      static IResult<EditLengthResult> calculateEditLength(int[] oldItems, int oldStart, int oldEnd, int[] newItems, int newStart, int newEnd,
         int[] forwardDiagonal, int[] reverseDiagonal)
      {
         if (oldItems.Length == 0 && newItems.Length == 0)
         {
            return new EditLengthResult().Success();
         }

         var oldSize = oldEnd - oldStart;
         var newSize = newEnd - newStart;
         var maxSize = newSize + oldSize + 1;
         var half = maxSize / 2;
         var delta = oldSize - newSize;
         var deltaEven = delta % 2 == 0;

         forwardDiagonal[half + 1] = 0;
         reverseDiagonal[half + 1] = oldSize + 1;

         for (var d = 0; d <= half; d++)
         {
            var lastEdit = EditType.None;

            for (var k = -d; k <= d; k += 2)
            {
               var kIndex = k + half;
               var oldIndex = 0;
               if (k == -d || k != d && forwardDiagonal[kIndex - 1] < forwardDiagonal[kIndex + 1])
               {
                  oldIndex = forwardDiagonal[kIndex + 1];
                  lastEdit = EditType.InsertDown;
               }
               else
               {
                  oldIndex = forwardDiagonal[kIndex - 1] + 1;
                  lastEdit = EditType.DeleteRight;
               }

               var newIndex = oldIndex - k;
               var oldStartIndex = oldIndex;
               var newStartIndex = newIndex;
               while (oldIndex < oldSize && newIndex < newSize && oldItems[oldIndex + oldStart] == newItems[newIndex + newStart])
               {
                  oldIndex++;
                  newIndex++;
               }

               forwardDiagonal[kIndex] = oldIndex;

               if (!deltaEven && k - delta >= -d + 1 && k - delta <= d - 1)
               {
                  var revKIndex = k - delta + half;
                  var oldRevIndex = reverseDiagonal[revKIndex];
                  var newRevIndex = oldRevIndex - k;
                  if (oldRevIndex <= oldIndex && newRevIndex <= newIndex)
                  {
                     return new EditLengthResult
                     {
                        EditLength = 2 * d - 1,
                        OldStart = oldStartIndex + oldStart,
                        NewStart = newStartIndex + newStart,
                        OldEnd = oldIndex + oldStart,
                        NewEnd = newIndex + newStart,
                        LastEdit = lastEdit
                     }.Success();
                  }
               }
            }

            for (var k = -d; k <= d; k += 2)
            {
               var kIndex = k + half;
               var oldIndex = 0;
               if (k == -d || k != d && reverseDiagonal[kIndex + 1] <= reverseDiagonal[kIndex - 1])
               {
                  oldIndex = reverseDiagonal[kIndex + 1] - 1;
                  lastEdit = EditType.DeleteLeft;
               }
               else
               {
                  oldIndex = reverseDiagonal[kIndex - 1];
                  lastEdit = EditType.InsertUp;
               }

               var newIndex = oldIndex - (k + delta);
               var oldEndIndex = oldIndex;
               var newEndIndex = newIndex;
               while (oldIndex > 0 && newIndex > 0 && oldItems[oldStart + oldIndex - 1] == newItems[newStart + newIndex - 1])
               {
                  oldIndex--;
                  newIndex--;
               }

               reverseDiagonal[kIndex] = oldIndex;
               if (deltaEven && k + delta >= -d && k + delta <= d)
               {
                  var forIndex = k + delta + half;
                  var oldForIndex = forwardDiagonal[forIndex];
                  var newForIndex = oldForIndex - (k + delta);
                  if (oldForIndex >= oldIndex && newForIndex >= newIndex)
                  {
                     return new EditLengthResult
                     {
                        EditLength = 2 * d,
                        OldStart = oldIndex + oldStart,
                        NewStart = newIndex + newStart,
                        OldEnd = oldEndIndex + oldStart,
                        NewEnd = newEndIndex + newStart,
                        LastEdit = lastEdit
                     }.Success();
                  }
               }
            }
         }

         return "Should never get here".Failure<EditLengthResult>();
      }

      public IResult<TextDiffResult> CreateDiffs(string[] oldText, string[] newText, bool ignoreWhiteSpace, bool ignoreCase)
      {
         oldText.Must().Not.BeNull().Assert();
         newText.Must().Not.BeNull().Assert();

         var itemHash = new Hash<string, int>();
         var lineDiffs = new List<TextDiffBlock>();

         var oldModifications = new ModificationData(oldText);
         var newModifications = new ModificationData(newText);

         buildItemHashes(itemHash, oldModifications, ignoreWhiteSpace, ignoreCase);
         buildItemHashes(itemHash, newModifications, ignoreWhiteSpace, ignoreCase);

         if (buildModificationData(oldModifications, newModifications).IfNot(out var exception))
         {
            return failure<TextDiffResult>(exception);
         }

         var oldItemsLength = oldModifications.HashedItems.Length;
         var newItemsLength = newModifications.HashedItems.Length;
         var oldPosition = 0;
         var newPosition = 0;

         do
         {
            while (oldPosition < oldItemsLength && newPosition < newItemsLength && !oldModifications.Modifications[oldPosition] &&
               !newModifications.Modifications[newPosition])
            {
               oldPosition++;
               newPosition++;
            }

            var oldBegin = oldPosition;
            var newBegin = newPosition;

            while (oldPosition < oldItemsLength && oldModifications.Modifications[oldPosition])
            {
               oldPosition++;
            }

            while (newPosition < newItemsLength && newModifications.Modifications[newPosition])
            {
               newPosition++;
            }

            var deleteCount = oldPosition - oldBegin;
            var insertCount = newPosition - newBegin;
            if (deleteCount > 0 || insertCount > 0)
            {
               lineDiffs.Add(new TextDiffBlock(oldBegin, deleteCount, newBegin, insertCount));
            }
         } while (oldPosition < oldItemsLength && newPosition < newItemsLength);

         return new TextDiffResult(oldModifications.Items, newModifications.Items, lineDiffs).Success();
      }
   }
}