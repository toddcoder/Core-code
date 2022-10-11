using System.Collections.Generic;
using System.Text;
using Core.Assertions;
using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf
{
   public class Paragraph : Block
   {
      protected StringBuilder text;
      protected Maybe<float> _lineSpacing;
      protected Margins margins;
      protected Alignment alignment;
      protected List<CharFormat> charFormats;
      protected bool allowFootnote;
      protected bool allowControlWord;
      protected List<Footnote> footnotes;
      protected List<FieldControlWord> controlWords;
      protected string blockHead;
      protected string blockTail;
      protected bool startNewPage;
      protected bool startNewPageAfter;
      protected float firstLineIndent;
      protected CharFormat defaultCharFormat;

      protected struct Token
      {
         public string Text;
         public bool IsControl;
      }

      private class DisjointRange
      {
         public DisjointRange()
         {
            Head = -1;
            Tail = -1;
            Format = null;
         }

         public int Head;
         public int Tail;
         public CharFormat Format;
      }

      public Paragraph() : this(false, false)
      {
      }

      public Paragraph(bool allowFootnote, bool allowControlWord)
      {
         text = new StringBuilder();
         _lineSpacing = nil;
         margins = new Margins();
         alignment = Alignment.Left;
         charFormats = new List<CharFormat>();
         this.allowFootnote = allowFootnote;
         this.allowControlWord = allowControlWord;
         footnotes = new List<Footnote>();
         controlWords = new List<FieldControlWord>();
         blockHead = @"{\pard";
         blockTail = @"\par}";
         startNewPage = false;
         firstLineIndent = 0;
         defaultCharFormat = new CharFormat();
      }

      public string Text
      {
         get => text.ToString();
         set => text = new StringBuilder(value);
      }

      public Maybe<float> LineSpacing
      {
         get => _lineSpacing;
         set => _lineSpacing = value;
      }

      public float FirstLineIndent
      {
         get => firstLineIndent;
         set => firstLineIndent = value;
      }

      public override CharFormat DefaultCharFormat => defaultCharFormat;

      public override bool StartNewPage
      {
         get => startNewPage;
         set => startNewPage = value;
      }

      public bool StartNewPageAfter
      {
         get => startNewPageAfter;
         set => startNewPageAfter = value;
      }

      public override Alignment Alignment
      {
         get => alignment;
         set => alignment = value;
      }

      public override Margins Margins => margins;

      public override string BlockHead
      {
         set => blockHead = value;
      }

      public override string BlockTail
      {
         set => blockTail = value;
      }

      public CharFormat CharFormat(int begin, int end)
      {
         var format = new CharFormat(begin, end, text.Length);
         charFormats.Add(format);

         return format;
      }

      public Maybe<CharFormat> CharFormat(Pattern pattern, int groupIndex = 0)
      {
         var _result = text.ToString().Matches(pattern);
         if (_result)
         {
            var result = ~_result;
            if (groupIndex < result.GroupCount(0))
            {
               var group = result.GetGroup(0, groupIndex);
               var begin = group.Index;
               var end = group.Length + begin - 1;
               return CharFormat(begin, end);
            }
            else
            {
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }

      public IEnumerable<CharFormat> CharFormats(Pattern pattern, int groupIndex = 0)
      {
         var _result = text.ToString().Matches(pattern);
         if (_result)
         {
            var tested = false;
            foreach (var match in ~_result)
            {
               if (!tested)
               {
                  if (match.Groups.Length >= groupIndex)
                  {
                     yield break;
                  }
                  else
                  {
                     tested = true;
                  }
               }

               var group = match.Groups[groupIndex];
               var begin = group.Index;
               var end = group.Length + begin - 1;
               yield return CharFormat(begin, end);
            }
         }
      }

      public CharFormat CharFormat()
      {
         var format = new CharFormat();
         charFormats.Add(format);

         return format;
      }

      public Footnote Footnote(int position)
      {
         allowFootnote.Must().BeTrue().OrThrow("Footnote is not allowed.");

         var footnote = new Footnote(position, text.Length);
         footnotes.Add(footnote);

         return footnote;
      }

      public void ControlWord(int position, FieldType type)
      {
         allowControlWord.Must().BeTrue().OrThrow("ControlWord is not allowed.");

         var controlWord = new FieldControlWord(position, type);
         for (var i = 0; i < controlWords.Count; i++)
         {
            if (controlWords[i].Position == controlWord.Position)
            {
               controlWords[i] = controlWord;
               return;
            }
         }

         controlWords.Add(controlWord);
      }

      protected LinkedList<Token> buildTokenList()
      {
         int count;
         var tokens = new LinkedList<Token>();
         LinkedListNode<Token> node;
         var disjointRanges = new List<DisjointRange>();

         foreach (var format in charFormats)
         {
            DisjointRange range;

            if (format.Begin && format.End)
            {
               var begin = ~format.Begin;
               var end = ~format.End;
               if (begin <= end)
               {
                  range = new DisjointRange { Head = begin, Tail = end, Format = format };
               }
               else
               {
                  continue;
               }
            }
            else
            {
               range = new DisjointRange { Head = 0, Tail = text.Length - 1, Format = format };
            }

            if (range.Tail >= text.Length)
            {
               range.Tail = text.Length - 1;
               if (range.Head > range.Tail)
               {
                  continue;
               }
            }

            var deletedRanges = new List<DisjointRange>();
            var addedRanges = new List<DisjointRange>();
            var anchorRanges = new List<DisjointRange>();
            foreach (var disjointRange in disjointRanges)
            {
               if (range.Head <= disjointRange.Head && range.Tail >= disjointRange.Tail)
               {
                  deletedRanges.Add(disjointRange);
               }
               else if (range.Head <= disjointRange.Head && range.Tail >= disjointRange.Head && range.Tail < disjointRange.Tail)
               {
                  disjointRange.Head = range.Tail + 1;
               }
               else if (range.Head > disjointRange.Head && range.Head <= disjointRange.Tail && range.Tail >= disjointRange.Tail)
               {
                  disjointRange.Tail = range.Head - 1;
               }
               else if (range.Head > disjointRange.Head && range.Tail < disjointRange.Tail)
               {
                  var newRange = new DisjointRange { Head = range.Tail + 1, Tail = disjointRange.Tail, Format = disjointRange.Format };
                  disjointRange.Tail = range.Head - 1;
                  addedRanges.Add(newRange);
                  anchorRanges.Add(disjointRange);
               }
            }

            disjointRanges.Add(range);
            foreach (var deletedRange in deletedRanges)
            {
               disjointRanges.Remove(deletedRange);
            }

            for (var i = 0; i < addedRanges.Count; i++)
            {
               var index = disjointRanges.IndexOf(anchorRanges[i]);
               if (index >= 0)
               {
                  disjointRanges.Insert(index, addedRanges[i]);
               }
            }
         }

         var token = new Token { Text = text.ToString(), IsControl = false };
         tokens.AddLast(token);

         foreach (var disjointRange in disjointRanges)
         {
            count = 0;
            if (disjointRange.Head == 0)
            {
               var newToken = new Token { IsControl = true, Text = disjointRange.Format.RenderHead() };
               tokens.AddFirst(newToken);
            }
            else
            {
               node = tokens.First;
               while (node != null)
               {
                  var nodeValue = node.Value;

                  if (!nodeValue.IsControl)
                  {
                     count += nodeValue.Text.Length;
                     if (count == disjointRange.Head)
                     {
                        var newToken = new Token { IsControl = true, Text = disjointRange.Format.RenderHead() };
                        while (node.Next.NotNull(out var next) && next.Value.IsControl)
                        {
                           node = next;
                        }

                        tokens.AddAfter(node, newToken);
                        break;
                     }
                     else if (count > disjointRange.Head)
                     {
                        var newToken1 = new Token
                        {
                           IsControl = false, Text = nodeValue.Text.Substring(0, nodeValue.Text.Length - (count - disjointRange.Head))
                        };
                        var newNode = tokens.AddAfter(node, newToken1);
                        var newToken2 = new Token { IsControl = true, Text = disjointRange.Format.RenderHead() };
                        newNode = tokens.AddAfter(newNode, newToken2);
                        var newToken3 = new Token
                        {
                           IsControl = false, Text = nodeValue.Text.Substring(nodeValue.Text.Length - (count - disjointRange.Head))
                        };
                        newNode = tokens.AddAfter(newNode, newToken3);
                        tokens.Remove(node);
                        break;
                     }
                  }

                  node = node.Next;
               }
            }

            count = 0;
            node = tokens.First;
            while (node != null)
            {
               var tokenValue = node.Value;

               if (!tokenValue.IsControl)
               {
                  count += tokenValue.Text.Length;
                  if (count - 1 == disjointRange.Tail)
                  {
                     var newToken = new Token { IsControl = true, Text = disjointRange.Format.RenderTail() };
                     tokens.AddAfter(node, newToken);
                     break;
                  }
                  else if (count - 1 > disjointRange.Tail)
                  {
                     var newToken1 = new Token
                     {
                        IsControl = false, Text = tokenValue.Text.Substring(0, tokenValue.Text.Length - (count - disjointRange.Tail) + 1)
                     };
                     var newNode = tokens.AddAfter(node, newToken1);
                     var newToken2 = new Token { IsControl = true, Text = disjointRange.Format.RenderTail() };
                     newNode = tokens.AddAfter(newNode, newToken2);
                     var newToken3 = new Token
                     {
                        IsControl = false, Text = tokenValue.Text.Substring(tokenValue.Text.Length - (count - disjointRange.Tail) + 1)
                     };
                     _ = tokens.AddAfter(newNode, newToken3);
                     tokens.Remove(node);
                     break;
                  }
               }

               node = node.Next;
            }
         }

         foreach (var footnote in footnotes)
         {
            var pos = footnote.Position;
            if (pos >= text.Length)
            {
               continue;
            }

            count = 0;
            node = tokens.First;
            while (node != null)
            {
               var nodeValue = node.Value;

               if (!nodeValue.IsControl)
               {
                  count += nodeValue.Text.Length;
                  if (count - 1 == pos)
                  {
                     var newToken = new Token { IsControl = true, Text = footnote.Render() };
                     tokens.AddAfter(node, newToken);
                     break;
                  }
                  else if (count - 1 > pos)
                  {
                     var newToken1 = new Token { IsControl = false, Text = nodeValue.Text.Substring(0, nodeValue.Text.Length - (count - pos) + 1) };
                     var newNode = tokens.AddAfter(node, newToken1);

                     var newToken2 = new Token { IsControl = true, Text = footnote.Render() };
                     newNode = tokens.AddAfter(newNode, newToken2);

                     var newToken3 = new Token { IsControl = false, Text = nodeValue.Text.Substring(nodeValue.Text.Length - (count - pos) + 1) };
                     newNode = tokens.AddAfter(newNode, newToken3);
                     tokens.Remove(node);
                     break;
                  }
               }

               node = node.Next;
            }
         }

         foreach (var controlWord in controlWords)
         {
            var pos = controlWord.Position;
            if (pos >= text.Length)
            {
               continue;
            }

            count = 0;
            node = tokens.First;
            while (node != null)
            {
               var nodeValue = node.Value;

               if (!nodeValue.IsControl)
               {
                  count += nodeValue.Text.Length;
                  if (count - 1 == pos)
                  {
                     var newToken = new Token { IsControl = true, Text = controlWord.Render() };
                     tokens.AddAfter(node, newToken);
                     break;
                  }
                  else if (count - 1 > pos)
                  {
                     var newToken1 = new Token { IsControl = false, Text = nodeValue.Text.Substring(0, nodeValue.Text.Length - (count - pos) + 1) };
                     var newNode = tokens.AddAfter(node, newToken1);

                     var newToken2 = new Token { IsControl = true, Text = controlWord.Render() };
                     newNode = tokens.AddAfter(newNode, newToken2);

                     var newToken3 = new Token { IsControl = false, Text = nodeValue.Text.Substring(nodeValue.Text.Length - (count - pos) + 1) };
                     newNode = tokens.AddAfter(newNode, newToken3);
                     tokens.Remove(node);
                     break;
                  }
               }

               node = node.Next;
            }
         }

         return tokens;
      }

      protected static string extractTokenList(LinkedList<Token> tokList)
      {
         var result = new StringBuilder();
         var node = tokList.First;

         while (node != null)
         {
            if (node.Value.IsControl)
            {
               result.Append(node.Value.Text);
            }
            else
            {
               result.Append(node.Value.Text.UnicodeEncode());
            }

            node = node.Next;
         }

         return result.ToString();
      }

      public override string Render()
      {
         var tokens = buildTokenList();
         var result = new StringBuilder(blockHead);

         if (startNewPage)
         {
            result.Append(@"\pagebb");
         }

         if (_lineSpacing.Map(out var lineSpacing))
         {
            result.Append($@"\sl-{lineSpacing.PointsToTwips()}\slmult0");
         }

         if (margins[Direction.Top] > 0)
         {
            result.Append($@"\sb{margins[Direction.Top].PointsToTwips()}");
         }

         if (margins[Direction.Bottom] > 0)
         {
            result.Append($@"\sa{margins[Direction.Bottom].PointsToTwips()}");
         }

         if (margins[Direction.Left] > 0)
         {
            result.Append($@"\li{margins[Direction.Left].PointsToTwips()}");
         }

         if (margins[Direction.Right] > 0)
         {
            result.Append($@"\ri{margins[Direction.Right].PointsToTwips()}");
         }

         result.Append($@"\fi{firstLineIndent.PointsToTwips()}");
         result.Append(AlignmentCode());
         result.AppendLine();

         result.AppendLine(defaultCharFormat.RenderHead());

         result.AppendLine(extractTokenList(tokens));
         result.Append(defaultCharFormat.RenderTail());

         result.AppendLine(blockTail);

         if (startNewPageAfter)
         {
            result.Append(@"\pagebb");
         }

         return result.ToString();
      }
   }
}