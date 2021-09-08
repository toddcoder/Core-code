using System;
using System.Collections.Generic;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf
{
   public class RtfConcatenator
   {
      protected abstract class TableData
      {
         protected string source;
         protected Slice location;

         public TableData(string source)
         {
            this.source = source;
         }

         public abstract Maybe<StringHash> Table();

         public abstract IEnumerable<Slice> Instances();
      }

      protected class FontTable : TableData
      {
         public FontTable(string source) : base(source)
         {
         }

         public override Maybe<StringHash> Table()
         {
            if (source.Matches(@"'{\f' /(/d+) /s+ /(-[';']+)").If(out var result))
            {
               var table = new StringHash(true);
               foreach (var match in result)
               {
                  var (index, fontName) = match;
                  table[index] = fontName;
               }

               return table;
            }
            else
            {
               return nil;
            }
         }

         public override IEnumerable<Slice> Instances()
         {
            if (source.Matches(@"-(< '{') '\f' /(/d+)").If(out var result))
            {
               foreach (var match in result)
               {
                  yield return match.Slice;
               }
            }
         }
      }

      protected class ColorTable : TableData
      {
         public ColorTable(string source) : base(source)
         {
         }

         public override Maybe<StringHash> Table() => nil;

         public override IEnumerable<Slice> Instances()
         {
            yield break;
         }
      }

      protected class RtfData
      {
         protected TableData fontTable;
         protected TableData colorTable;

         public RtfData(string source)
         {
         }
      }
   }
}