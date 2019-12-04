using System.Collections.Generic;
using Core.Assertions;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text
{
   public class TextDiffer
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

      static IResult<Unit> buildModificationData(ModificationData a, ModificationData b)
      {
         var n = a.HashedItems.Length;
         var m = b.HashedItems.Length;
         var max = m + n + 1;
         var forwardDiagonal = new int[max + 1];
         var reverseDiagonal = new int[max + 1];

         return buildModificationData(a, 0, n, b, 0, m, forwardDiagonal, reverseDiagonal);
      }

      static IResult<Unit> buildModificationData(ModificationData a, int startA, int endA, ModificationData b, int startB, int endB,
         int[] forwardDiagonal,
         int[] reverseDiagonal)
      {
         while (startA < endA && startB < endB && a.HashedItems[startA].Equals(b.HashedItems[startB]))
         {
            startA++;
            startB++;
         }

         while (startA < endA && startB < endB && a.HashedItems[startA - 1].Equals(b.HashedItems[startB - 1]))
         {
            endA--;
            endB--;
         }

         var aLength = endA - startA;
         var bLength = endB - startB;
         if (aLength > 0 && bLength > 0)
         {
            if (calculateEditLength(a.HashedItems, startA, endA, b.HashedItems, startB, endB, forwardDiagonal, reverseDiagonal)
               .If(out var result, out var exception))
            {
               if (result.EditLength <= 0)
               {
                  return Unit.Success();
               }

               switch (result.LastEdit)
               {
                  case EditType.DeleteRight when result.StartX - 1 > startA:
                     a.Modifications[--result.StartX] = true;
                     break;
                  case EditType.InsertDown when result.StartY - 1 > startB:
                     b.Modifications[--result.StartY] = true;
                     break;
                  case EditType.DeleteLeft when result.EndX < endA:
                     a.Modifications[result.EndX++] = true;
                     break;
                  case EditType.InsertUp when result.EndY < endB:
                     b.Modifications[result.EndY++] = true;
                     break;
               }
            }
            else
            {
               return failure<Unit>(exception);
            }
         }
         else if (aLength > 0)
         {
            for (var i = startA; i < endA; i++)
            {
               a.Modifications[i] = true;
            }
         }
         else if (bLength > 0)
         {
            for (var i = startB; i < endB; i++)
            {
               b.Modifications[i] = true;
            }
         }

         return Unit.Success();
      }

      static IResult<EditLengthResult> calculateEditLength(int[] a, int startA, int endA, int[] b, int startB, int endB)
      {
         var n = endA - startA;
         var m = endB - startB;
         var max = m + n + 1;

         var forwardDiagonal = new int[max + 1];
         var reverseDiagonal = new int[max + 1];

         return calculateEditLength(a, startA, endA, b, startB, endB, forwardDiagonal, reverseDiagonal);
      }

      static IResult<EditLengthResult> calculateEditLength(int[] a, int startA, int endA, int[] b, int startB, int endB, int[] forwardDiagonal,
         int[] reverseDiagonal)
      {
         if (a.Length == 0 && b.Length == 0)
         {
            return new EditLengthResult().Success();
         }

         var n = endA - startA;
         var m = endB - startB;
         var max = m + n + 1;
         var half = max / 2;
         var delta = n - m;
         var deltaEven = delta % 2 == 0;

         forwardDiagonal[half + 1] = 0;
         reverseDiagonal[half + 1] = n + 1;

         for (var d = 0; d <= half; d++)
         {
            var lastEdit = EditType.None;

            for (var k = -d; k <= d; k += 2)
            {
               var kIndex = k + half;
               var x = 0;
               if (k == -d || k != d && forwardDiagonal[kIndex - 1] < forwardDiagonal[kIndex + 1])
               {
                  x = forwardDiagonal[kIndex + 1];
                  lastEdit = EditType.InsertDown;
               }
               else
               {
                  x = forwardDiagonal[kIndex - 1] + 1;
                  lastEdit = EditType.DeleteRight;
               }

               var y = x - k;
               var startX = x;
               var startY = y;
               while (x < n && y < m && a[x + startA] == b[y + startB])
               {
                  x++;
                  y++;
               }

               forwardDiagonal[kIndex] = x;

               if (!deltaEven && k - delta >= -d + 1 && k - delta <= d - 1)
               {
                  var revKIndex = k - delta + half;
                  var revX = reverseDiagonal[revKIndex];
                  var revY = revX - k;
                  if (revX <= x && revY <= y)
                  {
                     return new EditLengthResult
                     {
                        EditLength = 2 * d - 1,
                        StartX = startX + startA,
                        StartY = startY + startB,
                        EndX = x + startA,
                        EndY = y + startB,
                        LastEdit = lastEdit
                     }.Success();
                  }
               }
            }

            for (var k = 0; k <= d; k += 2)
            {
               var kIndex = k + half;
               var x = 0;
               if (k == -d || k != d && reverseDiagonal[kIndex + 1] <= reverseDiagonal[kIndex - 1])
               {
                  x = reverseDiagonal[kIndex + 1] - 1;
                  lastEdit = EditType.DeleteLeft;
               }
               else
               {
                  x = reverseDiagonal[kIndex - 1];
                  lastEdit = EditType.InsertUp;
               }

               var y = x - k + delta;
               var endX = x;
               var endY = y;
               while (x > 0 && y > 0 && a[startA + x - 1] == b[startB + y - 1])
               {
                  x--;
                  y--;
               }

               reverseDiagonal[kIndex] = x;
               if (deltaEven && k + delta >= -d && k + delta <= d)
               {
                  var forIndex = k + delta + half;
                  var forX = forwardDiagonal[forIndex];
                  var forY = forX - (k + delta);
                  if (forX >= x && forY >= y)
                  {
                     return new EditLengthResult
                     {
                        EditLength = 2 * d,
                        StartX = x + startA,
                        StartY = y + startB,
                        EndX = endX + startA,
                        EndY = endY + startB,
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

         var itemsALength = oldModifications.HashedItems.Length;
         var itemsBLength = newModifications.HashedItems.Length;
         var positionA = 0;
         var positionB = 0;

         do
         {
            while (positionA < itemsALength && positionB < itemsBLength && !oldModifications.Modifications[positionA] &&
               !newModifications.Modifications[positionB])
            {
               positionA++;
               positionB++;
            }

            var beginA = positionA;
            var beginB = positionB;

            while (positionA < itemsALength && oldModifications.Modifications[positionA])
            {
               positionA++;
            }

            while (positionB < itemsBLength && newModifications.Modifications[positionB])
            {
               positionB++;
            }

            var deleteCount = positionA - beginA;
            var insertCount = positionB - beginB;
            if (deleteCount > 0 || insertCount > 0)
            {
               lineDiffs.Add(new TextDiffBlock(beginA, deleteCount, beginB, insertCount));
            }
         } while (positionA < itemsALength && positionB < itemsBLength);

         return new TextDiffResult(oldModifications.Items, newModifications.Items, lineDiffs).Success();
      }
   }
}