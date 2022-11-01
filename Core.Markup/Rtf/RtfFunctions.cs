using System;
using System.Collections.Generic;
using Core.DataStructures;
using Core.Matching;

namespace Core.Markup.Rtf;

public static class RtfFunctions
{
   public static Func<Paragraph, Formatter> format(int begin, int end) => p => p.Format(begin, end);

   public static Func<Paragraph, Formatter> format(Pattern pattern, int groupIndex = 0) => p => p.Format(pattern, groupIndex);

   public static Func<Paragraph, Formatter> formatFind(string substring, bool ignoreCase = false) => p => p.FormatFind(substring, ignoreCase);

   public static Func<Paragraph, IEnumerable<Formatter>> formatFindAll(string substring, bool ignoreCase = false)
   {
      return p => p.FormatFindAll(substring, ignoreCase);
   }

   public static Func<Paragraph, Formatter> format(MatchResult result, int groupIndex = 0) => p => p.Format(result, groupIndex);

   public static Func<Paragraph, Formatter> format() => p => p.Format();

   public static Func<Paragraph, Formatter> formatUrl(string placeholder, bool ignoreCase = false) => p => p.FormatUrl(placeholder, ignoreCase);

   public static Func<Paragraph, MaybeQueue<Formatter>> formatTemplate(string formatTemplate) => p => p.FormatTemplate(formatTemplate);

   public static readonly Paragraph para = Paragraph.Empty;

   public static readonly Feature none = Feature.None;

   public static readonly Feature bold = Feature.Bold;

   public static readonly Feature italic = Feature.Italic;

   public static readonly Feature underline = Feature.Underline;

   public static readonly Feature bullet = Feature.Bullet;

   public static readonly Feature newPage = Feature.NewPage;

   public static readonly Feature newPageAfter = Feature.NewPageAfter;

   public static readonly Alignment noAlignment = Alignment.None;

   public static readonly Alignment left = Alignment.Left;

   public static readonly Alignment right = Alignment.Right;

   public static readonly Alignment center = Alignment.Center;

   public static readonly Alignment fullyJustify = Alignment.FullyJustify;

   public static readonly Alignment distributed = Alignment.Distributed;
}