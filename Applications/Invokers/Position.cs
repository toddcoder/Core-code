using static System.Console;

namespace Core.Applications.Invokers;

public struct Position
{
   public static Position Save() => new() { Left = CursorLeft, Top = CursorTop };

   public int Left;

   public int Top;

   public void Retrieve() => SetCursorPosition(Left, Top);
}