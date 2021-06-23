using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Core.Monads;

namespace Core.Applications
{
   public static class ConsoleFunctions
   {
      private const uint ERROR_ACCESS_DENIED = 5;
      private const char BLOCK = '■';
      private const string BACK = "\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b";
      private const string TWIRL = "-\\|/";

      private static string errorMessage() => new Win32Exception(Marshal.GetLastWin32Error()).Message;

      public static Result<Unit> consoleNew(bool alwaysCreateNewConsole = true)
      {
         if (alwaysCreateNewConsole ? !Kernel32.consoleAllocate() : !Kernel32.consoleAttach())
         {
            return
               from outStream in Kernel32.initializeOutStream()
               from inStream in Kernel32.initializeInStream()
               select Unit.Value;
         }
         else
         {
            return errorMessage().Failure<Unit>();
         }
      }

      public static void writeProgressBar(int percent, bool update = false)
      {
         if (update)
         {
            Console.Write(BACK);
         }

         Console.Write("[");

         var percentAsInt = (int)(percent / 10f + 0.5f);

         for (var i = 0; i < 10; i++)
         {
            if (i >= percentAsInt)
            {
               Console.Write(' ');
            }
            else
            {
               Console.Write(BLOCK);
            }
         }

         Console.Write($"] {percent:##0}%");
      }

      public static void writeProgress(int progress, bool update = false)
      {
         if (update)
         {
            Console.Write("\b");
         }

         Console.Write(TWIRL[progress % TWIRL.Length]);
      }
   }
}