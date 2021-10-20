using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

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
               select unit;
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

      public static Responding<string> readConsole(string prompt, string suggestion)
      {
         try
         {
            Console.Write(prompt);
            var left = Console.CursorLeft;
            Console.Write(suggestion);

            Responding<string> _input = suggestion;
            var looping = true;
            while (looping)
            {
               var key = Console.ReadKey(true);
               switch (key.Key)
               {
                  case ConsoleKey.Enter:
                     Console.WriteLine();
                     looping = false;
                     break;
                  case ConsoleKey.Backspace when key.Modifiers.HasFlag(ConsoleModifiers.Control):
                  {
                     if (_input.Map(i => i.Length).If(out var length) && length > 0)
                     {
                        var backspaces = '\b'.Repeat(length);
                        Console.Write(backspaces);
                        Console.Write(" ".Repeat(length));
                        Console.Write(backspaces);
                        _input = "";
                     }

                     break;
                  }
                  case ConsoleKey.Backspace:
                  {
                     if (Console.CursorLeft != left)
                     {
                        Console.Write('\b');
                        Console.Write(" ");
                        Console.Write('\b');
                        _input = _input.Map(i => i.Drop(-1));
                     }

                     break;
                  }
                  case ConsoleKey.Escape:
                     _input = nil;
                     looping = false;
                     Console.WriteLine();
                     break;
                  default:
                  {
                     if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsWhiteSpace(key.KeyChar))
                     {
                        _input = _input.Map(i => i + key.KeyChar);
                        Console.Write(key.KeyChar);
                     }

                     break;
                  }
               }
            }

            return _input;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
   }
}