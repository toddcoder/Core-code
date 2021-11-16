using System;
using Core.Collections;
using static Core.Objects.ConversionFunctions;

namespace Core.Applications.Writers2
{
   public class ConsoleWriter : Writer
   {
      protected StringHash<ConsoleColor> consoleColors;

      public ConsoleWriter()
      {
         consoleColors = new StringHash<ConsoleColor>(true);
      }

      public override void WriteRaw(string text) => Console.Write(text);

      public override void SetColors(string foreColor, string backColor)
      {
         var consoleForeColor = consoleColors.Memoize(foreColor, color => Value.Enumeration<ConsoleColor>(color));
         var consoleBackColor = consoleColors.Memoize(backColor, color => Value.Enumeration<ConsoleColor>(color));
         Console.ForegroundColor = consoleForeColor;
         Console.BackgroundColor = consoleBackColor;
      }

      public override void SetLocation(int left, int top) => Console.SetCursorPosition(left, top);

      public override string DefaultForeColor => "white";

      public override string DefaultBackColor => "black";

      public override string DefaultExceptionForeColor => "red";

      public override string DefaultExceptionBackColor => "white";

      public override int CurrentLeft => Console.CursorLeft;

      public override int CurrentTop => Console.CursorTop;
   }
}