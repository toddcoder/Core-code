using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Emojis;

public static class EmojiExtensions
{
   public static string Image(this Emoji emoji) => emoji switch
   {
      Emoji.Arrow => "⇒",
      Emoji.Check => "✔",
      Emoji.X => "✘",
      Emoji.Dot => "•",
      Emoji.Degree => "°",
      Emoji.Copyright => "©",
      Emoji.Pilcrow => "¶",
      Emoji.Diamond => "♦",
      Emoji.DoubleLeft => "«",
      Emoji.DoubleRight => "»",
      Emoji.Times => "×",
      Emoji.Divide => "÷",
      Emoji.PawsLeft => "„",
      Emoji.PawsRight => "“",
      Emoji.NotEqual => "≠",
      Emoji.Error => "ℯ",
      Emoji.Ellipsis => "…",
      Emoji.Hourglass => "⧖",
      Emoji.Empty => "Ø",
      Emoji.LeftAngle => "〈",
      Emoji.RightAngle => "〉",
      Emoji.Locked => "🔒",
      Emoji.Unlocked => "🔓",
      Emoji.Text => "🗛",
      Emoji.Format => "ƒ",
      Emoji.Copy => "❏",
      Emoji.Paste => "📋",
      Emoji.Cut => "✄",
      _ => ""
   };

   public static string EmojiSubstitutions(this string text)
   {
      if (text.Matches("-(< '//') /('//' /([/w '-']+) '.'?); f") is (true, var result))
      {
         foreach (var match in result)
         {
            Maybe<string> _replacement = match.SecondGroup switch
            {
               "arrow" => "⇒",
               "check" => "✔",
               "x" => "✘",
               "dot" => "•",
               "degree" => "°",
               "copyright" => "©",
               "pilcrow" => "¶",
               "diamond" => "♦",
               "double-left" => "«",
               "double-right" => "»",
               "times" => "×",
               "divide" => "÷",
               "paws-left" => "„",
               "paws-right" => "“",
               "not-equal" => "≠",
               "error" => "ℯ",
               "ellipsis" => "…",
               "hourglass" => "⧖",
               "empty" => "∅",
               "left-angle" => "〈",
               "right-angle" => "〉",
               "locked" => "🔒",
               "unlocked" => "🔓",
               "text" => "🗛",
               "format" => "ƒ",
               "copy"=> "❏",
               "paste"=> "📋",
               "cut"=> "✄",
               _ => nil
            };
            if (_replacement is (true, var replacement))
            {
               match.FirstGroup = replacement;
            }
         }

         return result.ToString().Replace("//", "/");
      }
      else
      {
         return text;
      }
   }
}