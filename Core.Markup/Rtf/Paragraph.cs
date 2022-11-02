using System;
using System.Collections.Generic;
using System.Text;
using Core.Assertions;
using Core.Collections;
using Core.DataStructures;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class Paragraph : Block
{
   public static Paragraph Empty => new();

   public static Formatter operator |(Paragraph paragraph, Func<Paragraph, Formatter> func) => func(paragraph);

   public static Paragraph operator |(Paragraph paragraph, Paragraph _) => paragraph;

   public static IEnumerable<Formatter> operator |(Paragraph paragraph, Func<Paragraph, IEnumerable<Formatter>> func) => func(paragraph);

   public static MaybeQueue<Formatter> operator |(Paragraph paragraph, Func<Paragraph, MaybeQueue<Formatter>> func) => func(paragraph);

   public static Formatter operator |(Paragraph paragraph, Alignment alignment)
   {
      return new Formatter(paragraph, paragraph.DefaultCharFormat).Alignment(alignment);
   }

   public static Formatter operator |(Paragraph paragraph, ForegroundColorDescriptor foregroundColor)
   {
      return new Formatter(paragraph, paragraph.DefaultCharFormat).ForegroundColor(foregroundColor);
   }

   public static Formatter operator |(Paragraph paragraph, BackgroundColorDescriptor backgroundColor)
   {
      return new Formatter(paragraph, paragraph.DefaultCharFormat).BackgroundColor(backgroundColor);
   }

   public static Formatter operator |(Paragraph paragraph, FontDescriptor font)
   {
      return new Formatter(paragraph, paragraph.DefaultCharFormat).Font(font);
   }

   public static Formatter operator |(Paragraph paragraph, float fontSize)
   {
      return new Formatter(paragraph, paragraph.DefaultCharFormat).FontSize(fontSize);
   }

   public static Formatter operator |(Paragraph paragraph, FirstLineIndent firstLineIndent)
   {
      return new Formatter(paragraph, paragraph.DefaultCharFormat).FirstLineIndent(firstLineIndent.Amount);
   }

   public static Formatter operator |(Paragraph paragraph, Feature feature) => paragraph.Format() | feature;

   public static Formatter operator |(Paragraph paragraph, (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      return new Formatter(paragraph, paragraph.DefaultCharFormat).Margins(margins);
   }

   public static Formatter operator |(Paragraph paragraph, Style style) => style.Formatter(paragraph);

   public static Paragraph operator |(Paragraph paragraph, Hyperlink hyperlink) => paragraph.AddPendingHyperlink(hyperlink);

   public static Formatter operator |(Paragraph paragraph, Bookmark bookmark) => paragraph.Bookmark(bookmark);

   public static Formatter operator |(Paragraph paragraph, FieldType fieldType)
   {
      return new Formatter(paragraph, paragraph.DefaultCharFormat).ControlWord(fieldType);
   }

   public static Paragraph operator |(Paragraph paragraph, string text)
   {
      paragraph.text.Append(text);
      return paragraph;
   }

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
   protected bool bullet;
   protected CharFormat defaultCharFormat;
   protected List<(int, int, FontStyleFlag)> pendingCharFormats;
   protected StringHash<(int, int, Hyperlink)> pendingHyperlinks;

   protected struct Token
   {
      public string Text;
      public bool IsControl;
   }

   protected class DisjointRange
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

   protected Paragraph() : this(false, false)
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
      pendingCharFormats = new List<(int, int, FontStyleFlag)>();
      pendingHyperlinks = new StringHash<(int, int, Hyperlink)>(true);
   }

   public Style Style
   {
      set => value.SetParagraph(this);
   }

   protected void setText(string newText)
   {
      var _result = newText.Matches("/(['*^%']) /(-['*^%']+) /(/1); f");
      if (_result)
      {
         var result = ~_result;
         var begin = result.Index;
         var end = begin + result.GetGroup(0, 2).Length - 1;
         Bits32<FontStyleFlag> flags = FontStyleFlag.None;
         switch (result.FirstGroup)
         {
            case "*":
               flags[FontStyleFlag.Italic] = true;
               break;
            case "^":
               flags[FontStyleFlag.Bold] = true;
               break;
            case "%":
               flags[FontStyleFlag.Italic] = true;
               flags[FontStyleFlag.Bold] = true;
               break;
         }

         pendingCharFormats.Add((begin, end, flags));
         result.FirstGroup = "";
         result.ThirdGroup = "";
         setText(setControlWords(result.Text));
      }
      else
      {
         text = new StringBuilder(setControlWords(newText));

         foreach (var (begin, end, flag) in pendingCharFormats)
         {
            var format = CharFormat(begin, end);
            format.FontStyle += flag;
         }
      }
   }

   protected string setControlWords(string text)
   {
      var _result = text.Matches(@"['@#?!']");
      if (_result)
      {
         var offset = 1;
         foreach (var match in ~_result)
         {
            Maybe<FieldType> _fieldType = match.Text switch
            {
               "@" => FieldType.Page,
               "#" => FieldType.NumPages,
               "?" => FieldType.Date,
               "!" => FieldType.Time,
               _ => nil
            };
            if (_fieldType)
            {
               ControlWord(match.Index - offset++, _fieldType);
               match.Text = "";
            }
         }

         return (~_result).Text;
      }
      else
      {
         return text;
      }
   }

   public string Text
   {
      get => text.ToString();
      set
      {
         if (value.StartsWith("/"))
         {
            setText(value.Drop(1));
         }
         else
         {
            text = new StringBuilder(value);
         }
      }
   }

   public Paragraph AddPendingHyperlink(Hyperlink hyperlink)
   {
      var key = $"/url{pendingHyperlinks.Count}";
      var begin = text.Length;
      var hyperlinkText = hyperlink.LinkTip | hyperlink.Link;
      var end = begin + hyperlinkText.Length - 1;
      text.Append("");
      pendingHyperlinks[key] = (begin, end, hyperlink);

      return this;
   }

   public Formatter Bookmark(Bookmark bookmark)
   {
      var formatter = new Formatter(this, DefaultCharFormat);
      return formatter.Bookmark(bookmark.Name);
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

   public bool Bullet
   {
      get => bullet;
      set => bullet = value;
   }

   public CharFormat CharFormat(int begin, int end, bool checkRange = true)
   {
      var format = new CharFormat(begin, end, text.Length, checkRange);
      charFormats.Add(format);

      return format;
   }

   public Maybe<CharFormat> CharFormat(Pattern pattern, int groupIndex = 0)
   {
      var _result = text.ToString().Matches(pattern);
      if (_result)
      {
         var result = ~_result;
         return CharFormat(result, groupIndex);
      }
      else
      {
         return nil;
      }
   }

   public Maybe<CharFormat> CharFormatFind(string substring, bool ignoreCase = false)
   {
      var _index = text.ToString().Find(substring, ignoreCase: ignoreCase);
      if (_index)
      {
         return CharFormat(_index, _index + substring.Length - 1);
      }
      else
      {
         return nil;
      }
   }

   public IEnumerable<CharFormat> CharFormatFindAll(string substring, bool ignoreCase = false)
   {
      foreach (var index in text.ToString().FindAll(substring, ignoreCase))
      {
         yield return CharFormat(index, index + substring.Length - 1);
      }
   }

   public Maybe<CharFormat> CharFormat(MatchResult result, int groupIndex = 0)
   {
      if (groupIndex < result.GroupCount(0))
      {
         var (_, begin, length) = result.GetGroup(0, groupIndex);
         var end = length + begin - 1;

         return CharFormat(begin, end);
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
         foreach (var charFormat in CharFormats(_result, groupIndex))
         {
            yield return charFormat;
         }
      }
   }

   public IEnumerable<CharFormat> CharFormats(MatchResult result, int groupIndex = 0)
   {
      var tested = false;
      foreach (var match in result)
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

         var (_, begin, length) = match.Groups[groupIndex];
         var end = length + begin - 1;

         yield return CharFormat(begin, end);
      }
   }

   public CharFormat CharFormat()
   {
      var format = new CharFormat();
      charFormats.Add(format);

      return format;
   }

   public MaybeQueue<CharFormat> CharFormatTemplate(string charFormatTemplate)
   {
      var queue = new MaybeQueue<CharFormat>();
      var _result = charFormatTemplate.Matches("'^'+; f");
      if (_result)
      {
         foreach (var match in ~_result)
         {
            var begin = match.Index;
            var end = begin + match.Length - 1;
            queue.Enqueue(CharFormat(begin, end));
         }
      }

      return queue;
   }

   public Formatter Format(int begin, int end) => new(this, CharFormat(begin, end));

   public Formatter Format(Pattern pattern, int groupIndex = 0)
   {
      var _format = CharFormat(pattern, groupIndex);
      return _format.Map(f => new Formatter(this, f)) | (() => new NullFormatter(this));
   }

   public Formatter FormatFind(string substring, bool ignoreCase = false)
   {
      var _format = CharFormatFind(substring, ignoreCase);
      return _format.Map(f => new Formatter(this, f)) | (() => new NullFormatter(this));
   }

   public IEnumerable<Formatter> FormatFindAll(string substring, bool ignoreCase = false)
   {
      foreach (var format in CharFormatFindAll(substring, ignoreCase))
      {
         yield return new Formatter(this, format);
      }
   }

   public Formatter Format(MatchResult result, int groupIndex = 0)
   {
      var _format = CharFormat(result, groupIndex);
      return _format.Map(f => new Formatter(this, f)) | (() => new NullFormatter(this));
   }

   public Formatter Format() => new(this, DefaultCharFormat);

   public Formatter FormatUrl(string placeholder, bool ignoreCase = false) => FormatFind($"/url({placeholder})", ignoreCase);

   public MaybeQueue<Formatter> FormatTemplate(string formatTemplate)
   {
      var queue = new MaybeQueue<Formatter>();
      var _result = formatTemplate.Matches("'^'+; f");
      if (_result)
      {
         foreach (var match in ~_result)
         {
            var begin = match.Index;
            var end = begin + match.Length - 1;
            queue.Enqueue(Format(begin, end));
         }
      }

      return queue;
   }

   public void ControlWorlds(string controlWorldTemplate)
   {
      var _result = controlWorldTemplate.Matches(@"['@#?!']");
      if (_result)
      {
         var offset = 1;
         foreach (var match in ~_result)
         {
            Maybe<FieldType> _fieldType = match.Text switch
            {
               "@" => FieldType.Page,
               "#" => FieldType.NumPages,
               "?" => FieldType.Date,
               "!" => FieldType.Time,
               _ => nil
            };
            if (_fieldType)
            {
               ControlWord(match.Index - offset++, _fieldType);
            }
         }
      }
   }

   public Footnote Footnote(int position)
   {
      allowFootnote.Must().BeTrue().OrThrow("Footnote is not allowed.");

      var footnote = new Footnote(position, text.Length);
      footnotes.Add(footnote);

      return footnote;
   }

   public Footnote Footnote() => Footnote(text.Length - 1);

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

   protected System.Collections.Generic.LinkedList<Token> buildTokenList()
   {
      int count;
      var tokens = new System.Collections.Generic.LinkedList<Token>();
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
                     while (true)
                     {
                        var _next = node.Next.NotNull();
                        if (!_next || !(~_next).Value.IsControl)
                        {
                           break;
                        }

                        node = _next;
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

   protected string extractTokenList(System.Collections.Generic.LinkedList<Token> tokList)
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
            var nodeText = node.Value.Text;
            var _keyResult = nodeText.Matches("'//url' (/d+); f");
            if (_keyResult)
            {
               var keyResult = ~_keyResult;
               foreach (var match in keyResult)
               {
                  match.Text = "";
               }

               nodeText = keyResult.Text;
               if (nodeText.IsNotEmpty())
               {
                  result.Append(nodeText.UnicodeEncode());
               }
            }
            else
            {
               var _result = nodeText.Matches("'//url(' -[')']+ ')'; fi");
               if (_result)
               {
                  var matchResult = ~_result;
                  nodeText = nodeText.Keep(matchResult.Index) + nodeText.Drop(matchResult.Index + matchResult.Length);
               }

               if (nodeText.IsNotEmpty())
               {
                  result.Append(nodeText.UnicodeEncode());
               }
            }
         }

         node = node.Next;
      }

      return result.ToString();
   }

   public override string Render()
   {
      foreach (var (begin, end, hyperlink) in pendingHyperlinks.Values)
      {
         var format = CharFormat(begin, end, false);
         format.Hyperlink = hyperlink.Link;
         format.HyperlinkTip = hyperlink.LinkTip;
      }

      var tokens = buildTokenList();
      var result = new StringBuilder(blockHead);

      if (startNewPage)
      {
         result.Append(@"\pagebb");
      }

      if (_lineSpacing)
      {
         result.Append($@"\sl-{(~_lineSpacing).PointsToTwips()}\slmult0");
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

      if (bullet)
      {
         if (margins[Direction.Left] == 0)
         {
            result.Append(@"\li500");
         }

         result.Append(@"\pntext\pn\pnlvlblt\bullet\tab");
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