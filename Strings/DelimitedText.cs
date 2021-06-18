using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Assertions;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.RegexMatching;
using static Core.Monads.MonadFunctions;

namespace Core.Strings
{
   public class DelimitedText
   {
      public static DelimitedText AsCLike()
      {
         Pattern beginPattern = Pattern.IsFriendly ? "[dquote]" : "[\"]";
         Pattern exceptPattern = Pattern.IsFriendly ? @"'\' [dquote]" : "\\[\"]";

         return new DelimitedText(beginPattern, exceptPattern);
      }

      public static DelimitedText AsSql()
      {
         Pattern beginPattern = Pattern.IsFriendly ? "[squote]" : "'";
         Pattern exceptPattern = Pattern.IsFriendly ? "[squote]2" : "''";

         return new DelimitedText(beginPattern, exceptPattern);
      }

      public static DelimitedText AsBasic()
      {
         Pattern beginPattern = Pattern.IsFriendly ? "[dquote]" : "[\"]";
         Pattern exceptPattern = Pattern.IsFriendly ? "[dquote]2" : "[\"]{2}";

         return new DelimitedText(beginPattern, exceptPattern);
      }

      public static DelimitedText BothQuotes()
      {
         Pattern beginPattern = Pattern.IsFriendly ? "[dquote squote]" : "[\"']";
         Pattern exceptPattern = Pattern.IsFriendly ? @"'\' [dquote squote]" : "\\[\"']";

         return new DelimitedText(beginPattern, exceptPattern);
      }

      protected Pattern beginPattern;
      protected IMaybe<Pattern> _endPattern;
      protected Pattern exceptPattern;
      protected IMaybe<string> _exceptReplacement;
      protected LateLazy<Slicer> slicer;
      protected Bits32<DelimitedTextStatus> status;
      protected List<string> strings;

      protected DelimitedText(Pattern beginPattern, IMaybe<string> endPattern, Pattern exceptPattern)
      {
         this.beginPattern = beginPattern;
         _endPattern = endPattern.Map(p => (Pattern)p);
         this.exceptPattern = exceptPattern;
         _exceptReplacement = none<string>();

         slicer = new LateLazy<Slicer>(true, "You must call Enumerable() before accessing this member");
         status = DelimitedTextStatus.Outside;
         TransformingMap = none<Func<string, string>>();
         strings = new List<string>();
      }

      public DelimitedText(Pattern beginPattern, Pattern endPattern, Pattern exceptPattern) : this(beginPattern, endPattern.Regex.Some(),
         exceptPattern)
      {
      }

      public DelimitedText(Pattern beginPattern, Pattern exceptPattern) : this(beginPattern, none<string>(), exceptPattern)
      {
      }

      public string BeginPattern
      {
         get => beginPattern.Regex;
         set
         {
            value.Must().Not.BeNullOrEmpty().OrThrow();
            beginPattern = value.StartsWith("^") ? value : $"^{value}";
         }
      }

      public IMaybe<string> EndPattern
      {
         get => _endPattern.Map(m => m.Regex);
         set { _endPattern = value.Map(p => (Pattern)(p.StartsWith("^") ? p : $"^{value}")); }
      }

      public string ExceptPattern
      {
         get => exceptPattern.Regex;
         set
         {
            value.Must().Not.BeNullOrEmpty().OrThrow();
            exceptPattern = value.StartsWith("^") ? value : $"^{value}";
         }
      }

      public IMaybe<string> ExceptReplacement
      {
         get => _exceptReplacement;
         set => _exceptReplacement = value;
      }

      public Bits32<DelimitedTextStatus> Status
      {
         get => status;
         set => status = value;
      }

      public IMaybe<Func<string, string>> TransformingMap { get; set; }

      protected Pattern getEndPattern(char ch)
      {
         if (Pattern.IsFriendly)
         {
            return ch switch
            {
               '\'' => "^ [squote]",
               '"' => "^ [dquote]",
               _ => $"^ '{ch}'"
            };
         }
         else
         {
            return $"^{System.Text.RegularExpressions.Regex.Escape(ch.ToString())}";
         }
      }

      public IEnumerable<(string text, int index, DelimitedTextStatus status)> Enumerable(string source)
      {
         source.Must().Not.BeNull().OrThrow();

         slicer.ActivateWith(() => new Slicer(source));

         var builder = new StringBuilder();
         var inside = false;
         var current = source;
         var insideStart = 0;
         var outsideStart = 0;
         var _endMatcher = none<Pattern>();

         var i = 0;
         while (i < source.Length)
         {
            var ch = source[i];
            current = source.Drop(i);
            if (inside)
            {
               if (current.Matches(exceptPattern).If(out var result))
               {
                  builder.Append(_exceptReplacement.DefaultTo(() => result[0]));
                  i += result.Length;

                  continue;
               }
               else if (_endMatcher.If(out var endPattern) && current.Matches(endPattern).If(out result))
               {
                  _endMatcher = none<Pattern>();

                  yield return (builder.ToString(), insideStart, DelimitedTextStatus.Inside);
                  yield return (result[0], i, DelimitedTextStatus.EndDelimiter);

                  builder.Clear();
                  inside = false;
                  i += result.Length;
                  outsideStart = i;
                  continue;
               }
               else
               {
                  builder.Append(ch);
               }
            }
            else
            {
               if (current.Matches(beginPattern).If(out var result))
               {
                  if (_endMatcher.IsNone)
                  {
                     _endMatcher = getEndPattern(ch).Some();
                  }

                  yield return (builder.ToString(), outsideStart, DelimitedTextStatus.Outside);
                  yield return (result[0], i, DelimitedTextStatus.BeginDelimiter);

                  builder.Clear();
                  inside = true;
                  i += result.Length;
                  insideStart = i;
                  continue;
               }
               else
               {
                  builder.Append(ch);
               }
            }

            i++;
         }

         if (builder.Length > 0)
         {
            var rest = builder.ToString();
            if (inside)
            {
               yield return (rest, insideStart, DelimitedTextStatus.Inside);
            }
            else
            {
               yield return (rest, outsideStart, DelimitedTextStatus.Outside);
            }
         }
      }

      public IEnumerable<(int index, DelimitedTextStatus status)> Substrings(string source, string substring, bool ignoreCase = false)
      {
         foreach (var (text, index, inOutsideStatus) in Enumerable(source).Where(t => Status[t.status]))
         {
            foreach (var foundIndex in text.FindAll(substring, ignoreCase))
            {
               yield return (index + foundIndex, inOutsideStatus);
            }
         }
      }

      public IEnumerable<(string text, int index, DelimitedTextStatus status)> Matches(string source, Pattern pattern)
      {
         foreach (var (text, index, inOutsideStatus) in Enumerable(source).Where(t => Status[t.status]))
         {
            foreach (var (sliceText, sliceIndex, _) in text.FindAllByRegex(pattern))
            {
               yield return (sliceText, index + sliceIndex, inOutsideStatus);
            }
         }
      }

      public string this[int index, int length]
      {
         get => slicer.Value[index, length];
         set
         {
            value.Must().Not.BeNull().OrThrow();
            slicer.Value[index, length] = value;
         }
      }

      public char this[int index]
      {
         get => slicer.Value[index];
         set => slicer.Value[index] = value;
      }

      public string Drop(int index) => slicer.Value.Text.Drop(index);

      public string Keep(int index) => slicer.Value.Text.Keep(index);

      public int Length => slicer.Value.Length;

      public IEnumerable<string> Strings => strings;

      public IEnumerable<Slice> Split(string source, string pattern, bool includeDelimiter = false)
      {
         source.Must().Not.BeNull().OrThrow();
         pattern.Must().Not.BeNullOrEmpty().OrThrow();

         var lastIndex = 0;

         foreach (var (outerText, outerIndex, _) in Enumerable(source).Where(t => status[t.status]))
         {
            foreach (var (sliceText, sliceIndex, length) in outerText.FindAllByRegex(pattern))
            {
               var index = outerIndex + sliceIndex;
               var text = source.Drop(lastIndex).Keep(index - lastIndex);

               yield return new Slice(text, index, text.Length);

               if (includeDelimiter)
               {
                  yield return new Slice(sliceText, index, length);
               }

               lastIndex = index + length;
            }
         }

         var rest = source.Drop(lastIndex);
         yield return new Slice(rest, lastIndex, rest.Length);
      }

      public void Replace(string source, string pattern, string replacement, int count = 0)
      {
         source.Must().Not.BeNull().OrThrow();
         replacement.Must().Not.BeNull().OrThrow();
         Replace(source, pattern, _ => replacement, count);
      }

      public void Replace(string source, string pattern, Func<Slice, string> map, int count = 0)
      {
         source.Must().Not.BeNull().OrThrow();
         pattern.Must().Not.BeNullOrEmpty().OrThrow();
         map.Must().Not.BeNull().OrThrow();
         count.Must().BeGreaterThan(-1).OrThrow();

         var limited = count > 0;
         var replaced = 0;

         foreach (var (text, outerIndex, _) in Enumerable(source).Where(t => status[t.status]))
         {
            foreach (var slice in text.FindAllByRegex(pattern))
            {
               var (_, sliceIndex, length) = slice;
               var index = outerIndex + sliceIndex;
               slicer.Value[index, length] = map(slice);
               if (limited && ++replaced == count)
               {
                  break;
               }
            }
         }
      }

      public string Transform(string source, string pattern, string replacement, bool ignoreCase = false)
      {
         source.Must().Not.BeNull().OrThrow();
         pattern.Must().Not.BeNull().OrThrow();
         replacement.Must().Not.BeNull().OrThrow();

         var startIndex = 0;
         Status = DelimitedTextStatus.Outside;
         var values = new List<string>();

         foreach (var (text, sliceIndex, length) in pattern.SplitIntoSlices("'$' /d+").Where(s => s.Length > 0))
         {
            if (sliceIndex == 0)
            {
               startIndex = sliceIndex + length;
            }
            else
            {
               foreach (var (index, _) in Substrings(source, text, ignoreCase))
               {
                  var item = source.Drop(startIndex).Keep(index - startIndex);
                  item = TransformingMap.Map(m => m(item)).DefaultTo(() => item);
                  values.Add(item);
                  startIndex = index + text.Length;
               }
            }
         }

         var rest = source.Drop(startIndex);
         if (rest.Length > 0)
         {
            rest = TransformingMap.Map(m => m(rest)).DefaultTo(() => rest);
            values.Add(rest);
         }

         var builder = new StringBuilder(replacement);
         for (var i = 0; i < values.Count; i++)
         {
            builder.Replace($"${i}", values[i]);
         }

         return builder.ToString();
      }

      public IEnumerable<(string text, int index)> StringsOnly(string source)
      {
         return Enumerable(source).Where(t => t.status == DelimitedTextStatus.Inside).Select(t => (t.text, t.index));
      }

      public string Destringify(string source, bool includeDelimiters = false)
      {
         var builder = new StringBuilder();
         strings.Clear();

         foreach (var (text, _, delimitedTextStatus) in Enumerable(source))
         {
            switch (delimitedTextStatus)
            {
               case DelimitedTextStatus.Outside:
                  builder.Append(text);
                  break;
               case DelimitedTextStatus.Inside:
                  builder.Append($"/({strings.Count})");
                  strings.Add(text);
                  break;
               case DelimitedTextStatus.BeginDelimiter:
               case DelimitedTextStatus.EndDelimiter:
                  if (includeDelimiters)
                  {
                     builder.Append(text);
                  }

                  break;
            }
         }

         return builder.ToString();
      }

      public string Restringify(string source, RestringifyQuotes restringifyQuotes)
      {
         source.Must().Not.BeNull().OrThrow();

         var quote = restringifyQuotes switch
         {
            RestringifyQuotes.None => "",
            RestringifyQuotes.DoubleQuote => "\"",
            RestringifyQuotes.SingleQuote => "'",
            _ => throw new ArgumentOutOfRangeException(nameof(restringifyQuotes), restringifyQuotes, null)
         };

         Slicer restringified = source;
         for (var i = 0; i < strings.Count; i++)
         {
            var substring = $"/({i})";
            var replacement = $"{quote}{strings[i]}{quote}";
            foreach (var index in source.FindAll(substring))
            {
               restringified[index, substring.Length] = replacement;
            }
         }

         return restringified.ToString();
      }

      public override string ToString() => slicer.Value.ToString();
   }
}