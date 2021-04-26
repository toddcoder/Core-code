using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Assertions;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.RegularExpressions;
using static Core.Monads.MonadFunctions;

namespace Core.Strings
{
   public class DelimitedText
   {
      public static DelimitedText AsCLike(bool ignoreCase = false, bool multiline = false, bool friendly = true)
      {
         var beginPattern = friendly ? "[dquote]" : "[\"]";
         var exceptPattern = friendly ? @"'\' [dquote]" : "\\[\"]";

         return new DelimitedText(beginPattern, exceptPattern, ignoreCase, multiline, friendly);
      }

      public static DelimitedText AsSql(bool ignoreCase = false, bool multiline = false, bool friendly = true)
      {
         var beginPattern = friendly ? "[squote]" : "'";
         var exceptPattern = friendly ? "[squote]2" : "''";

         return new DelimitedText(beginPattern, exceptPattern, ignoreCase, multiline, friendly);
      }

      public static DelimitedText AsBasic(bool ignoreCase = false, bool multiline = false, bool friendly = true)
      {
         var beginPattern = friendly ? "[dquote]" : "[\"]";
         var exceptPattern = friendly ? "[dquote]2" : "[\"]{2}";

         return new DelimitedText(beginPattern, exceptPattern, ignoreCase, multiline, friendly);
      }

      public static DelimitedText BothQuotes(bool ignoreCase = false, bool multiline = false, bool friendly = true)
      {
         var beginPattern = friendly ? "[dquote squote]" : "[\"']";
         var exceptPattern = friendly ? @"'\' [dquote squote]" : "\\[\"']";

         return new DelimitedText(beginPattern, exceptPattern, ignoreCase, multiline, friendly);
      }

      protected string beginPattern;
      protected IMaybe<string> _endPattern;
      protected string exceptPattern;
      protected IMaybe<string> _exceptReplacement;
      protected bool ignoreCase;
      protected bool multiline;
      protected bool friendly;
      protected LateLazy<Slicer> slicer;
      protected Bits32<DelimitedTextStatus> status;
      protected List<string> strings;

      protected DelimitedText(string beginPattern, IMaybe<string> endPattern, string exceptPattern, bool ignoreCase = false, bool multiline = false,
         bool friendly = true)
      {
         BeginPattern = beginPattern;
         EndPattern = endPattern;
         ExceptPattern = exceptPattern;
         ExceptReplacement = none<string>();
         this.ignoreCase = ignoreCase;
         this.multiline = multiline;
         this.friendly = friendly;

         slicer = new LateLazy<Slicer>(true, "You must call Enumerable() before accessing this member");
         status = DelimitedTextStatus.Outside;
         TransformingMap = none<Func<string, string>>();
         strings = new List<string>();
      }

      public DelimitedText(string beginPattern, string endPattern, string exceptPattern, bool ignoreCase = false, bool multiline = false,
         bool friendly = true) : this(beginPattern, endPattern.Some(), exceptPattern, ignoreCase, multiline, friendly)
      {
      }

      public DelimitedText(string beginPattern, string exceptPattern, bool ignoreCase = false, bool multiline = false,
         bool friendly = true) : this(beginPattern, none<string>(), exceptPattern, ignoreCase, multiline, friendly)
      {
      }

      public string BeginPattern
      {
         get => beginPattern;
         set
         {
            value.Must().Not.BeNullOrEmpty().OrThrow();
            beginPattern = value.StartsWith("^") ? value : $"^{value}";
         }
      }

      public IMaybe<string> EndPattern
      {
         get => _endPattern;
         set
         {
            if (value.If(out var newEndPattern))
            {
               newEndPattern.Must().Not.BeNullOrEmpty().OrThrow();
               _endPattern = (newEndPattern.StartsWith("^") ? newEndPattern : $"^{newEndPattern}").Some();
            }
            else
            {
               _endPattern = value;
            }
         }
      }

      public string ExceptPattern
      {
         get => exceptPattern;
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

      public bool IgnoreCase
      {
         get => ignoreCase;
         set => ignoreCase = value;
      }

      public bool Multiline
      {
         get => multiline;
         set => multiline = value;
      }

      public bool Friendly
      {
         get => friendly;
         set => friendly = value;
      }

      public Bits32<DelimitedTextStatus> Status
      {
         get => status;
         set => status = value;
      }

      public IMaybe<Func<string, string>> TransformingMap { get; set; }

      protected string getEndPattern(char ch)
      {
         if (friendly)
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
         var matcher = new Matcher(friendly);
         var endPattern = "";

         var i = 0;
         while (i < source.Length)
         {
            var ch = source[i];
            current = source.Drop(i);
            if (inside)
            {
               if (matcher.IsMatch(current, exceptPattern, ignoreCase, multiline))
               {
                  builder.Append(_exceptReplacement.DefaultTo(() => matcher[0]));
                  i += matcher.Length;

                  continue;
               }
               else if (matcher.IsMatch(current, endPattern, ignoreCase, multiline))
               {
                  endPattern = "";

                  yield return (builder.ToString(), insideStart, DelimitedTextStatus.Inside);
                  yield return (matcher[0], i, DelimitedTextStatus.EndDelimiter);

                  builder.Clear();
                  inside = false;
                  i += matcher.Length;
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
               if (matcher.IsMatch(current, beginPattern, ignoreCase, multiline))
               {
                  endPattern = _endPattern.DefaultTo(() => getEndPattern(ch));

                  yield return (builder.ToString(), outsideStart, DelimitedTextStatus.Outside);
                  yield return (matcher[0], i, DelimitedTextStatus.BeginDelimiter);

                  builder.Clear();
                  inside = true;
                  i += matcher.Length;
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

      public IEnumerable<(string text, int index, DelimitedTextStatus status)> Matches(string source, string pattern, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         foreach (var (text, index, inOutsideStatus) in Enumerable(source).Where(t => Status[t.status]))
         {
            foreach (var (sliceText, sliceIndex, _) in text.FindAllByRegex(pattern, ignoreCase, multiline, friendly))
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
         pattern.Must().Not.BeNullOrEmpty().OrThrow();

         var lastIndex = 0;

         foreach (var (outerText, outerIndex, _) in Enumerable(source).Where(t => status[t.status]))
         {
            foreach (var (sliceText, sliceIndex, length) in outerText.FindAllByRegex(pattern, ignoreCase, multiline, friendly))
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
         replacement.Must().Not.BeNull().OrThrow();

         Replace(source, pattern, _ => replacement, count);
      }

      public void Replace(string source, string pattern, Func<Slice, string> map, int count = 0)
      {
         pattern.Must().Not.BeNullOrEmpty().OrThrow();
         map.Must().Not.BeNull().OrThrow();
         count.Must().BeGreaterThan(-1).OrThrow();

         var limited = count > 0;
         var replaced = 0;

         foreach (var (text, outerIndex, _) in Enumerable(source).Where(t => status[t.status]))
         {
            foreach (var slice in text.FindAllByRegex(pattern, ignoreCase, multiline, friendly))
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
         pattern.Must().Not.BeNullOrEmpty().OrThrow();

         var startIndex = 0;
         Status = DelimitedTextStatus.Outside;
         var values = new List<string>();

         foreach (var (text, sliceIndex, length) in pattern.SliceSplit("'$' /d+").Where(s => s.Length > 0))
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