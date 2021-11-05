using System;
using System.Collections.Generic;
using System.Text;
using Core.Arrays;
using Core.Collections;
using Core.Enumerables;
using Core.Markup.Rtf;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Markup.Builder
{
   public class RtfBuilder
   {
      protected Document document;
      protected Source source;
      protected StringHash<FontDescriptor> fontDescriptors;
      protected StringHash<ColorDescriptor> colorDescriptors;

      public RtfBuilder(PaperSize paperSize = PaperSize.Letter, PaperOrientation paperOrientation = PaperOrientation.Portrait,
         Lcid lcid = Lcid.English)
      {
         document = new Document(paperSize, paperOrientation, lcid);
         fontDescriptors = new StringHash<FontDescriptor>(true);
         colorDescriptors = new StringHash<ColorDescriptor>(true);
      }

      public Result<Document> Build(string markupSource)
      {
         source = new Source(markupSource);

         defineDescriptors();

         while (source.NextLine().If(out var line))
         {
            var result = createParagraph(line) | createHeader(line) | createFooter(line);
            if (result.If(out _, out var _exception))
            {
            }
            else if (_exception.If(out var exception))
            {
               return exception;
            }
            else
            {
               var paragraph = document.Paragraph();
               paragraph.Text = line;
            }
         }

         return document;
      }

      protected void defineDescriptors()
      {
         while (source.NextLine("^ ('font' | 'color') ':'; f").If(out var line))
         {
            if (line.Matches("^ 'font:' /s* /(.+) $; f").If(out var result))
            {
               var specifiers = result.FirstGroup.Divide();
               foreach (var specifier in specifiers)
               {
                  if (specifier.Matches("^ /(/w [/w '-']*) /s* '=' /s* /(.+) $; f").If(out result))
                  {
                     fontDescriptors[result.FirstGroup] = document.Font(result.SecondGroup);
                  }
               }
            }
            else if (line.Matches("^ 'color:' /s* /(.+) $; f").If(out result))
            {
               var specifiers = result.FirstGroup.Divide();
               foreach (var specifier in specifiers)
               {
                  var _hex =
                     from specifierResult in specifier.Matches("^ /(/w [/w '-']*) /s* '=' /s* /('0x' ['0-9a-z']6) $; fi")
                     from hexInt in specifierResult.SecondGroup.FromHex()
                     select (specifierResult.FirstGroup, hexInt);
                  if (_hex.If(out var key, out var hex))
                  {
                     colorDescriptors[key] = document.Color(hex);
                  }
                  else if (specifier.Matches("^ /(/w [/w '-']*) /s* '=' /s* /(.+) $; f").If(out result))
                  {
                     colorDescriptors[result.FirstGroup] = document.Color(result.SecondGroup);
                  }
               }
            }
         }
      }

      protected static Maybe<FontStyle> addFontStyle(string specifier, FontStyle fontStyle)
      {
         if (specifier.IsMatch("'none' | 'bold' | 'italic' | 'underline' | 'super' | 'sub' | 'scaps' | 'strike'; f"))
         {
            if (Maybe.Enumeration<FontStyleFlag>(specifier).If(out var flag))
            {
               fontStyle += flag;
               return fontStyle;
            }
         }

         return nil;
      }

      protected Result<string> formatCharFormat(string specifierList, CharFormat format)
      {
         try
         {
            if (specifierList.IsEmpty())
            {
               return specifierList;
            }

            var missing = new List<string>();
            var specifiers = specifierList.Divide(DividerType.Slash);
            foreach (var specifier in specifiers)
            {
               if (addFontStyle(specifier, format.FontStyle).If(out var fontStyle))
               {
                  format.FontStyle = fontStyle;
                  continue;
               }

               var (key, value) = specifier.Divide(DividerType.Colon);
               switch (key.ToLower())
               {
                  case "fore-color":
                     format.ForegroundColor = colorDescriptors.Map(value);
                     break;
                  case "back-color":
                     format.BackgroundColor = colorDescriptors.Map(value);
                     break;
                  case "font":
                     format.Font = fontDescriptors.Map(value);
                     break;
                  case "ansi-font":
                     format.AnsiFont = fontDescriptors.Map(value);
                     break;
                  case "font-size":
                     format.FontSize = Maybe.Single(value);
                     break;
                  case "link":
                     format.LocalHyperlink = value;
                     break;
                  case "link-tip":
                     format.LocalHyperlinkTip = value;
                     break;
                  case "bookmark":
                     format.Bookmark = value;
                     break;
                  default:
                     missing.Add(specifier);
                     break;
               }
            }

            return missing.ToString("/");
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      protected static Result<string> formatParagraph(string specifierList, Paragraph paragraph)
      {
         try
         {
            if (specifierList.IsEmpty())
            {
               return specifierList;
            }

            var missing = new List<string>();
            var specifiers = specifierList.Divide(DividerType.Slash);
            foreach (var specifier in specifiers)
            {
               if (specifier.Same("new-page"))
               {
                  paragraph.StartNewPage = true;
                  continue;
               }

               var (key, value) = specifier.Divide(DividerType.Colon);
               switch (key.ToLower())
               {
                  case "alignment":
                  {
                     if (Maybe.Enumeration<Alignment>(value).If(out var alignment))
                     {
                        paragraph.Alignment = alignment;
                     }

                     break;
                  }
                  case "first-line-indent":
                  {
                     if (Maybe.Single(value).If(out var floatValue))
                     {
                        paragraph.FirstLineIndent = floatValue;
                     }

                     break;
                  }
                  case "margin-left":
                  {
                     if (Maybe.Single(value).If(out var floatValue))
                     {
                        paragraph.Margins[Direction.Left] = floatValue;
                     }

                     break;
                  }
                  case "margin-top":
                  {
                     if (Maybe.Single(value).If(out var floatValue))
                     {
                        paragraph.Margins[Direction.Top] = floatValue;
                     }

                     break;
                  }
                  case "margin-right":
                  {
                     if (Maybe.Single(value).If(out var floatValue))
                     {
                        paragraph.Margins[Direction.Right] = floatValue;
                     }

                     break;
                  }
                  case "margin-bottom":
                  {
                     if (Maybe.Single(value).If(out var floatValue))
                     {
                        paragraph.Margins[Direction.Bottom] = floatValue;
                     }

                     break;
                  }
                  default:
                     missing.Add(key);
                     break;
               }
            }

            return missing.ToString("/");
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      protected static Maybe<(IEnumerable<Span>, string)> getSpecifiers(string text)
      {
         var spans = new List<Span>();

         while (true)
         {
            if (text.Matches(" -(< '//') '//' /(.+?) '//(' /(-[')']+?) ')'; f").If(out var result))
            {
               var (specifiers, index, _) = result.GetGroup(0, 1);
               var (affectedText, _, length) = result.GetGroup(0, 2);
               index--;
               length--;
               var slice = new Span(specifiers, index, index + length);
               spans.Add(slice);

               result.FirstMatch = affectedText;
               text = result.ToString();
            }
            else
            {
               break;
            }
         }

         if (spans.Count > 0)
         {
            return (spans, text);
         }
         else
         {
            return nil;
         }
      }

      protected Result<string> format(string line, Paragraph paragraph)
      {
         if (line.Matches("^ '[' /(-[']']+) ']'").If(out var result))
         {
            var specifiers = result.FirstGroup;
            var _result =
               from restFromCharFormat in formatCharFormat(specifiers, paragraph.DefaultCharFormat)
               from restFromParagraph in formatParagraph(restFromCharFormat, paragraph)
               select restFromParagraph;
            if (_result.If(out var remaining, out var exception))
            {
               if (remaining.IsEmpty())
               {
                  return line.Drop(result.Length);
               }
               else
               {
                  return fail($"Didn't understand specification {remaining}");
               }
            }
            else
            {
               return exception;
            }
         }
         else
         {
            return line;
         }
      }

      protected Result<Unit> formatCharacters(string paragraphText, Paragraph paragraph)
      {
         if (getSpecifiers(paragraphText).If(out var slices, out var newParagraphText))
         {
            paragraph.Text = newParagraphText;
            foreach (var (text, start, stop) in slices)
            {
               var format = paragraph.CharFormat(start, stop);
               if (formatCharFormat(text, format).If(out var remaining))
               {
                  if (remaining.IsNotEmpty())
                  {
                     return fail($"Didn't understand specification {remaining}");
                  }
               }
            }
         }
         else
         {
            paragraph.Text = paragraphText;
         }

         return unit;
      }

      protected Result<Unit> formatBlockList(Paragraph paragraph, string line)
      {
         return
            from paragraphText in format(line, paragraph)
            from result in formatCharacters(paragraphText, paragraph)
            select result;
      }

      protected Responding<Unit> createParagraph(string line)
      {
         if (line.Matches("^ '>' /s*").If(out var result))
         {
            line = line.Drop(result.Length);
            var paragraph = document.Paragraph();
            if (formatBlockList(paragraph, line).IfNot(out var exception))
            {
               return exception;
            }
            else
            {
               return unit;
            }
         }
         else
         {
            return nil;
         }
      }

      protected Result<string> addControlWords(Paragraph paragraph, string paragraphText)
      {
         if (paragraphText.Matches("'#' /('page' | 'num-pages' | 'date' | 'time') /b; f").If(out var result))
         {
            var builder = new StringBuilder();
            var lastIndex = 0;
            foreach (var match in result)
            {
               var fieldTypeName = match.FirstGroup;
               var fieldType = fieldTypeName switch
               {
                  "page" => FieldType.Page,
                  "num-pages" => FieldType.NumPages,
                  "date" => FieldType.Date,
                  "time" => FieldType.Time,
                  _ => FieldType.None
               };

               builder.Append(paragraphText.Drop(lastIndex));
               lastIndex += match.Length;
               paragraph.ControlWord(lastIndex);
            }
         }
      }

      protected Responding<Unit> createHeader(string line)
      {
         if (line.Matches("^ '^' /s*").If(out var result))
         {
            line = line.Drop(result.Length);
            var paragraph = document.Header.Paragraph();
            if (formatBlockList(paragraph, line).IfNot(out var exception))
            {
               return exception;
            }
            else
            {
               return unit;
            }
         }
         else
         {
            return nil;
         }
      }

      protected Responding<Unit> createFooter(string line)
      {
         if (line.Matches("^ '$' /s*").If(out var result))
         {
            line = line.Drop(result.Length);
            var paragraph = document.Footer.Paragraph();
            if (formatBlockList(paragraph, line).IfNot(out var exception))
            {
               return exception;
            }
            else
            {
               return unit;
            }
         }
         else
         {
            return nil;
         }
      }
   }
}