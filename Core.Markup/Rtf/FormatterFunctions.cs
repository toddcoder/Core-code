using System;
using System.Collections.Generic;
using Core.DataStructures;
using Core.Matching;
using Core.Monads;

namespace Core.Markup.Rtf;

public static class FormatterFunctions
{
   public static Func<Paragraph, Formatter> format(int begin, int end) => p => p.Format(begin, end);

   public static Func<Paragraph, Maybe<Formatter>> format(Pattern pattern, int groupIndex = 0) => p => p.Format(pattern, groupIndex);

   public static Func<Paragraph, Maybe<Formatter>> formatFind(string substring, bool ignoreCase = false) => p => p.FormatFind(substring, ignoreCase);

   public static Func<Paragraph, IEnumerable<Formatter>> formatFindAll(string substring, bool ignoreCase = false)
   {
      return p => p.FormatFindAll(substring, ignoreCase);
   }

   public static Func<Paragraph, Maybe<Formatter>> format(MatchResult result, int groupIndex = 0) => p => p.Format(result, groupIndex);

   public static Func<Paragraph, Formatter> format() => p => p.Format();

   public static Func<Paragraph, MaybeQueue<Formatter>> formatTemplate(string formatTemplate) => p => p.FormatTemplate(formatTemplate);
}