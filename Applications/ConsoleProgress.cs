using System;
using Core.Numbers;
using Core.Strings;

namespace Core.Applications
{
   public class ConsoleProgress
   {
      protected int count;
      protected int length;
      protected string progressCharacter;
      protected ConsoleColor foregroundColor;
      protected ConsoleColor backgroundColor;
      protected int index;
      protected int currentLeft;
      protected int currentRight;
      protected int width;
      protected ConsoleColor originalForegroundColor;
      protected ConsoleColor originalBackgroundColor;
      protected int extent;

      public ConsoleProgress(int count, int length, char progressCharacter = '#', ConsoleColor foregroundColor = ConsoleColor.Black,
         ConsoleColor backgroundColor = ConsoleColor.DarkCyan)
      {
         this.count = count;
         this.length = length;
         this.progressCharacter = progressCharacter.ToString();
         this.foregroundColor = foregroundColor;
         this.backgroundColor = backgroundColor;

         index = 1;

         currentLeft = Console.CursorLeft + 1;
         width = length - 2;
         Console.Write($"[{" ".Repeat(width)}] - ");
         currentRight = Console.CursorLeft;
         originalForegroundColor = Console.ForegroundColor;
         originalBackgroundColor = Console.BackgroundColor;
         extent = 0;
      }

      protected char getSpinner()
      {
         switch (index % 4)
         {
            case 0:
               return '-';
            case 1:
               return '\\';
            case 2:
               return '|';
            case 3:
               return '/';
            default:
               return '?';
         }
      }

      public void Progress(string label, bool increment)
      {
         var backGroundCount = (int)((double)index / count * width) + 1;
         backGroundCount = backGroundCount.MinOf(width);

         Console.CursorLeft = currentLeft;

         Console.ForegroundColor = foregroundColor;
         Console.BackgroundColor = backgroundColor;

         Console.Write(progressCharacter.Repeat(backGroundCount));

         Console.ForegroundColor = originalForegroundColor;
         Console.BackgroundColor = originalBackgroundColor;

         Console.CursorLeft = currentRight - 2;
         var spinner = getSpinner();
         Console.Write($"{spinner} ");
         extent = extent.MaxOf(label.Length);
         Console.Write(label.LeftJustify(extent));

         if (increment)
         {
            index++;
         }
      }

      public void Clear()
      {
         Console.CursorLeft = currentLeft - 1;
         Console.Write(" ".Repeat(length + 3 + extent));
         Console.CursorLeft = currentLeft - 1;
      }
   }
}