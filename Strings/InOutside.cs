using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Assertions;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.RegularExpressions;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Strings
{
   public class InOutside
   {
      protected string beginPattern;
      protected string endPattern;
      protected string exceptPattern;
      protected IMaybe<string> exceptReplacement;
      protected bool ignoreCase;
      protected bool multiline;
      protected bool friendly;
      protected LateLazy<Slicer> slicer;
      protected Bits32<InOutsideStatus> status;

      protected InOutside(string beginPattern, string endPattern, string exceptPattern, IMaybe<string> exceptReplacement, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         BeginPattern = beginPattern;
         EndPattern = endPattern;
         ExceptPattern = exceptPattern;
         ExceptReplacement = exceptReplacement;
         this.ignoreCase = ignoreCase;
         this.multiline = multiline;
         this.friendly = friendly;

         slicer = new LateLazy<Slicer>(true, "You must call Enumerable() before accessing this member");
         status = InOutsideStatus.Outside;
      }

      public InOutside(string beginPattern, string endPattern, string exceptPattern, string exceptReplacement, bool ignoreCase = false,
         bool multiline = false, bool friendly = true) :
         this(beginPattern, endPattern, exceptPattern, exceptReplacement.Some(), ignoreCase, multiline, friendly) { }

      public InOutside(string beginPattern, string endPattern, string exceptPattern, bool ignoreCase = false, bool multiline = false,
         bool friendly = true) : this(beginPattern, endPattern, exceptPattern, none<string>(), ignoreCase, multiline, friendly) { }

      public string BeginPattern
      {
         get => beginPattern;
         set
         {
            assert(() => value).Must().Not.BeNullOrEmpty().OrThrow();
            beginPattern = value.StartsWith("^") ? value : $"^{value}";
         }
      }

      public string EndPattern
      {
         get => endPattern;
         set
         {
            assert(() => value).Must().Not.BeNullOrEmpty().OrThrow();
            endPattern = value.StartsWith("^") ? value : $"^{value}";
         }
      }

      public string ExceptPattern
      {
         get => exceptPattern;
         set
         {
            assert(() => value).Must().Not.BeNullOrEmpty().OrThrow();
            exceptPattern = value.StartsWith("^") ? value : $"^{value}";
         }
      }

      public IMaybe<string> ExceptReplacement
      {
         get => exceptReplacement;
         set => exceptReplacement = value;
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

      public Bits32<InOutsideStatus> Status
      {
         get => status;
         set => status = value;
      }

      public IEnumerable<(string text, int index, InOutsideStatus status)> Enumerable(string source)
      {
         assert(() => source).Must().Not.BeNullOrEmpty().OrThrow();

         slicer.ActivateWith(() => new Slicer(source));

         var builder = new StringBuilder();
         var inside = false;
         var current = source;
         var insideStart = 0;
         var outsideStart = 0;
         var matcher = new Matcher(friendly);

         var i = 0;
         while (i < source.Length)
         {
            var ch = source[i];
            current = source.Drop(i);
            if (inside)
            {
               if (matcher.IsMatch(current, exceptPattern, ignoreCase, multiline))
               {
                  builder.Append(exceptReplacement.DefaultTo(() => matcher[0]));
                  i += matcher.Length;
                  continue;
               }
               else if (matcher.IsMatch(current, endPattern, ignoreCase, multiline))
               {
                  yield return (builder.ToString(), insideStart, InOutsideStatus.Inside);
                  yield return (matcher[0], i, InOutsideStatus.EndDelimiter);

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
                  yield return (builder.ToString(), outsideStart, InOutsideStatus.Outside);
                  yield return (matcher[0], i, InOutsideStatus.BeginDelimiter);

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
               yield return (rest, insideStart, InOutsideStatus.Inside);
            }
            else
            {
               yield return (rest, outsideStart, InOutsideStatus.Outside);
            }
         }
      }

      public string this[int index, int length]
      {
         get => slicer.Value[index, length];
         set
         {
            assert(() => value).Must().Not.BeNull().OrThrow();
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

      public IEnumerable<Slice> Split(string source, string pattern, bool includeDelimiter = false)
      {
         assert(() => pattern).Must().Not.BeNullOrEmpty().OrThrow();

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
         assert(() => replacement).Must().Not.BeNull().OrThrow();

         Replace(source, pattern, _ => replacement, count);
      }

      public void Replace(string source, string pattern, Func<Slice, string> map, int count = 0)
      {
         assert(() => pattern).Must().Not.BeNullOrEmpty().OrThrow();
         assert(() => (object)map).Must().Not.BeNull().OrThrow();
         assert(() => count).Must().BeGreaterThan(-1).OrThrow();

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

      public override string ToString() => slicer.Value.ToString();
   }
}