using System;

namespace Core.Strings;

public class Padder
{
   public static void Evaluate(string source, ref int maximumLength)
   {
      if (source.IsNotEmpty())
      {
         maximumLength = Math.Max(source.Length, maximumLength);
      }
   }

   public static string PadLeft(string source, int maximumLength, char padding = ' ') => source.PadLeft(maximumLength, padding);

   public static string PadRight(string source, int maximumLength, char padding = ' ') => source.PadRight(maximumLength, padding);

   public static string PadCenter(string source, int maximumLength, char padding = ' ') => source.PadCenter(maximumLength, padding);

   public static string Pad(string text, PadType type, int maximumLength, char paddingCharacter = ' ') => type switch
   {
      PadType.Left => PadLeft(text, maximumLength, paddingCharacter),
      PadType.Right => PadRight(text, maximumLength, paddingCharacter),
      PadType.Center => PadCenter(text, maximumLength, paddingCharacter),
      _ => text
   };

   public static string Repeat(string text, int maximumLength) => text.Repeat(maximumLength);

   protected int maximumLength;

   public Padder() => maximumLength = 0;

   public int MaximumLength => maximumLength;

   public void Evaluate(string source) => Evaluate(source, ref maximumLength);

   public void Evaluate(params string[] text)
   {
      foreach (var item in text)
      {
         Evaluate(item);
      }
   }

   public string PadLeft(string source, char paddingCharacter = ' ') => PadLeft(source, maximumLength, paddingCharacter);

   public string PadRight(string source, char paddingCharacter = ' ') => PadRight(source, maximumLength, paddingCharacter);

   public string PadCenter(string source, char paddingCharacter = ' ') => PadCenter(source, maximumLength, paddingCharacter);

   public string Pad(string text, PadType type, char paddingCharacter = ' ') => Pad(text, type, maximumLength, paddingCharacter);

   public void Reset() => maximumLength = 0;

   public string Repeat(string text) => Repeat(text, maximumLength);
}