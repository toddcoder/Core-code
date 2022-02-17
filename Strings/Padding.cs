using System;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using static Core.Objects.ConversionFunctions;

namespace Core.Strings
{
   public class Padding
   {
      protected const string DEFAULT_SPLIT_PATTERN = "/s* ',' /s*; f";
      protected const string LINE_SPLIT_PATTERN = "/r /n | /r | /n; f";

      protected PadType[] padTypes;
      protected string splitPattern;
      protected string columnSeparator;
      protected string text;
      protected int[] sizes;

      public Padding(params PadType[] padTypes)
      {
         this.padTypes = padTypes;
         splitPattern = DEFAULT_SPLIT_PATTERN;
         columnSeparator = " ";
         text = "";
         sizes = Array.Empty<int>();
      }

      public Padding(string padTypes)
      {
         this.padTypes = padTypes.Unjoin(DEFAULT_SPLIT_PATTERN)
            .Select(s => Maybe.Enumeration<PadType>(s))
            .SomeValue()
            .ToArray();
         splitPattern = DEFAULT_SPLIT_PATTERN;
         columnSeparator = " ";
         text = "";
         sizes = Array.Empty<int>();
      }

      public Padding(int count, PadType padType)
      {
         padTypes = Enumerable.Repeat(padType, count).ToArray();
         splitPattern = DEFAULT_SPLIT_PATTERN;
         columnSeparator = " ";
         text = "";
         sizes = Array.Empty<int>();
      }

      public string SplitPattern
      {
         get => splitPattern;
         set => splitPattern = value;
      }

      public string ColumnSeparator
      {
         get => columnSeparator;
         set => columnSeparator = value;
      }

      public string Text
      {
         get
         {
            if (text.IsNotEmpty())
            {
               var lines = text.Unjoin(LINE_SPLIT_PATTERN).Select(line => line.Unjoin(splitPattern)).ToArray();
               return getText(lines);
            }
            else
            {
               return "";
            }
         }
         set => text = value;
      }

      protected string getText(string[][] lines)
      {
         if (lines.Length != 0)
         {
            var enumerable = lines.Select(line => line.Select(column => column.Length).ToArray()).Pivot(() => 0).ToArray();
            sizes = enumerable.Select(columns => columns.Max()).ToArray();
            var sizesLength = sizes.Length;
            var paddedPadTypes = padTypes.Pad(sizesLength, PadType.Right);
            lines = lines.Select(columns => columns.Pad(sizesLength, "")).ToArray();

            return lines
               .Select(columns => columns
                  .Select((column, i) => column.Pad(paddedPadTypes[i], sizes[i]))
                  .ToString(columnSeparator))
               .ToString("\r\n");
         }
         else
         {
            return "";
         }
      }

      public string ToString(IEnumerable<IEnumerable<string>> source) => getText(source.Select(columns => columns.ToArray()).ToArray());

      public override string ToString() => Text;

      public string Header(params string[] headers)
      {
         if (sizes.Length == 0)
         {
            return getText(new[] { headers });
         }
         else
         {
            var newHeaders = headers.LimitTo(sizes.Length, "");
            return newHeaders.Zip(sizes, (header, size) => header.PadCenter(size)).ToString(columnSeparator);
         }
      }
   }
}