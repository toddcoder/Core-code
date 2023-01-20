using System.Collections.Generic;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text;

internal class DifferenceBuilder
{
   protected static void buildItemHashes(StringHash<int> itemHash, Modification modification, bool ignoreWhiteSpace, bool ignoreCase)
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

         var _value = itemHash.Maybe[item];
         if (_value)
         {
            modification.HashedItems[i] = _value;
         }
         else
         {
            modification.HashedItems[i] = itemHash.Count;
            itemHash[item] = itemHash.Count;
         }
      }
   }

   protected Result<Unit> buildModifications()
   {
      var oldSize = oldModification.HashedItems.Length;
      var newSize = newModification.HashedItems.Length;
      var maxSize = newSize + oldSize + 1;
      var forwardDiagonal = new int[maxSize + 1];
      var reverseDiagonal = new int[maxSize + 1];

      return buildModifications(0, oldSize, 0, newSize, forwardDiagonal, reverseDiagonal);
   }

   protected Result<Unit> buildModifications(int oldStart, int oldEnd, int newStart, int newEnd, int[] forwardDiagonal, int[] reverseDiagonal)
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
      switch (oldLength)
      {
         case > 0 when newLength > 0:
         {
            var _result = calculateEditLength(oldModification.HashedItems, oldStart, oldEnd, newModification.HashedItems, newStart, newEnd, forwardDiagonal,
               reverseDiagonal);
            if (_result)
            {
               var result = _result.Value;
               if (result.EditLength <= 0)
               {
                  return unit;
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

               var _resultAll =
                  from resultA in buildModifications(oldStart, result.OldStart, newStart, result.NewStart, forwardDiagonal, reverseDiagonal)
                  from resultB in buildModifications(result.OldEnd, oldEnd, result.NewEnd, newEnd, forwardDiagonal, reverseDiagonal)
                  select resultB;
               if (!_resultAll)
               {
                  return _resultAll.Exception;
               }
            }
            else
            {
               return _result.Exception;
            }

            break;
         }
         case > 0:
         {
            for (var i = oldStart; i < oldEnd; i++)
            {
               oldModification.Modifications[i] = true;
            }

            break;
         }
         default:
         {
            if (newLength > 0)
            {
               for (var i = newStart; i < newEnd; i++)
               {
                  newModification.Modifications[i] = true;
               }
            }

            break;
         }
      }

      return unit;
   }

   protected static Result<EditLengthResult> calculateEditLength(int[] oldItems, int oldStart, int oldEnd, int[] newItems, int newStart,
      int newEnd, int[] forwardDiagonal, int[] reverseDiagonal)
   {
      if (oldItems.Length == 0 && newItems.Length == 0)
      {
         return new EditLengthResult();
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
         EditType lastEdit;

         for (var k = -d; k <= d; k += 2)
         {
            var kIndex = k + half;
            int oldIndex;
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
                  };
               }
            }
         }

         for (var k = -d; k <= d; k += 2)
         {
            var kIndex = k + half;
            int oldIndex;
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
                  };
               }
            }
         }
      }

      return fail("Should never get here");
   }

   protected bool ignoreWhiteSpace;
   protected bool ignoreCase;
   protected Modification oldModification;
   protected Modification newModification;

   public DifferenceBuilder(string[] oldText, string[] newText, bool ignoreWhiteSpace, bool ignoreCase)
   {
      this.ignoreWhiteSpace = ignoreWhiteSpace;
      this.ignoreCase = ignoreCase;

      oldModification = new Modification(oldText);
      newModification = new Modification(newText);
   }

   public Result<DifferenceResult> Build()
   {
      var itemHash = new StringHash<int>(ignoreCase);
      var lineDiffs = new List<DifferenceBlock>();

      buildItemHashes(itemHash, oldModification, ignoreWhiteSpace, ignoreCase);
      buildItemHashes(itemHash, newModification, ignoreWhiteSpace, ignoreCase);

      var _result = buildModifications();
      if (!_result)
      {
         return _result.Exception;
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

      return new DifferenceResult(oldModification.Items, newModification.Items, lineDiffs);
   }
}