using System.Collections.Generic;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text
{
   internal class DifferenceBuilder
   {
      static void buildItemHashes(Hash<string, int> itemHash, Modification modification, bool ignoreWhiteSpace, bool ignoreCase)
      {
         var items = modification.RawData;
         modification.Items = items;
         modification.HashedItems = new int[items.Length];
         modification.Modifications = new bool[items.Length];

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
               modification.HashedItems[i] = value;
            }
            else
            {
               modification.HashedItems[i] = itemHash.Count;
               itemHash[item] = itemHash.Count;
            }
         }
      }

      IResult<Unit> buildModifications()
      {
         var oldSize = oldModification.HashedItems.Length;
         var newSize = newModification.HashedItems.Length;
         var maxSize = newSize + oldSize + 1;
         var forwardDiagonal = new int[maxSize + 1];
         var reverseDiagonal = new int[maxSize + 1];

         return buildModifications(0, oldSize, 0, newSize, forwardDiagonal, reverseDiagonal);
      }

      IResult<Unit> buildModifications(int oldStart, int oldEnd, int newStart, int newEnd, int[] forwardDiagonal, int[] reverseDiagonal)
      {
         while (oldStart < oldEnd && newStart < newEnd && oldModification.HashedItems[oldStart] == newModification.HashedItems[newStart])
         {
            oldStart++;
            newStart++;
         }

         while (oldStart < oldEnd && newStart < newEnd && oldModification.HashedItems[oldEnd - 1] == newModification.HashedItems[newEnd - 1])
         {
            oldEnd--;
            newEnd--;
         }

         var oldLength = oldEnd - oldStart;
         var newLength = newEnd - newStart;
         if (oldLength > 0 && newLength > 0)
         {
            if (calculateEditLength(oldModification.HashedItems, oldStart, oldEnd, newModification.HashedItems, newStart, newEnd, forwardDiagonal,
                  reverseDiagonal)
               .If(out var result, out var exception))
            {
               if (result.EditLength <= 0)
               {
                  return Unit.Success();
               }

               switch (result.LastEdit)
               {
                  case EditType.DeleteRight when result.OldStart - 1 > oldStart:
                     oldModification.Modifications[--result.OldStart] = true;
                     break;
                  case EditType.InsertDown when result.NewStart - 1 > newStart:
                     newModification.Modifications[--result.NewStart] = true;
                     break;
                  case EditType.DeleteLeft when result.OldEnd < oldEnd:
                     oldModification.Modifications[result.OldEnd++] = true;
                     break;
                  case EditType.InsertUp when result.NewEnd < newEnd:
                     newModification.Modifications[result.NewEnd++] = true;
                     break;
               }

               var resultAll =
                  from resultA in buildModifications(oldStart, result.OldStart, newStart, result.NewStart, forwardDiagonal, reverseDiagonal)
                  from resultB in buildModifications(result.OldEnd, oldEnd, result.NewEnd, newEnd, forwardDiagonal, reverseDiagonal)
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
               oldModification.Modifications[i] = true;
            }
         }
         else if (newLength > 0)
         {
            for (var i = newStart; i < newEnd; i++)
            {
               newModification.Modifications[i] = true;
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

      bool ignoreWhiteSpace;
      bool ignoreCase;
      Modification oldModification;
      Modification newModification;

      public DifferenceBuilder(string[] oldText, string[] newText, bool ignoreWhiteSpace, bool ignoreCase)
      {
         this.ignoreWhiteSpace = ignoreWhiteSpace;
         this.ignoreCase = ignoreCase;

         oldModification = new Modification(oldText);
         newModification = new Modification(newText);
      }

      public IResult<DifferenceResult> Build()
      {
         var itemHash = new Hash<string, int>();
         var lineDiffs = new List<DifferenceBlock>();

         buildItemHashes(itemHash, oldModification, ignoreWhiteSpace, ignoreCase);
         buildItemHashes(itemHash, newModification, ignoreWhiteSpace, ignoreCase);

         if (buildModifications().IfNot(out var exception))
         {
            return failure<DifferenceResult>(exception);
         }

         var oldItemsLength = oldModification.HashedItems.Length;
         var newItemsLength = newModification.HashedItems.Length;
         var oldPosition = 0;
         var newPosition = 0;

         do
         {
            while (oldPosition < oldItemsLength && newPosition < newItemsLength && !oldModification.Modifications[oldPosition] &&
               !newModification.Modifications[newPosition])
            {
               oldPosition++;
               newPosition++;
            }

            var oldBegin = oldPosition;
            var newBegin = newPosition;

            while (oldPosition < oldItemsLength && oldModification.Modifications[oldPosition])
            {
               oldPosition++;
            }

            while (newPosition < newItemsLength && newModification.Modifications[newPosition])
            {
               newPosition++;
            }

            var deleteCount = oldPosition - oldBegin;
            var insertCount = newPosition - newBegin;
            if (deleteCount > 0 || insertCount > 0)
            {
               lineDiffs.Add(new DifferenceBlock(oldBegin, deleteCount, newBegin, insertCount));
            }
         } while (oldPosition < oldItemsLength && newPosition < newItemsLength);

         return new DifferenceResult(oldModification.Items, newModification.Items, lineDiffs).Success();
      }
   }
}