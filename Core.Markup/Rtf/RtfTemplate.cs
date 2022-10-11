using System;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Markup.Rtf;

public class RtfTemplate
{
   protected const string PATTERN_NAME = "['a-zA-Z_'] ['a-zA-Z_-']*";
   protected const string PATTERN_IN_SLASH = "-['//']+";
   protected const string PATTERN_IN_BANG = "-['!']+";

   protected Document document;
   protected StringHash<Descriptor> descriptors;
   protected Hash<string, Pattern> patterns;

   public RtfTemplate(PaperSize paperSize = PaperSize.Letter, PaperOrientation paperOrientation = PaperOrientation.Portrait, Lcid lcid = Lcid.English)
   {
      document = new Document(paperSize, paperOrientation, lcid);

      descriptors = new StringHash<Descriptor>(true);

      patterns = new Hash<string, Pattern>
      {
         ["create-font"] = $"^ '//font' /s+ /({PATTERN_NAME}) /s* '=' /s* /(.+) $; f",
         ["create-color"] = $"^ '//color' /s+ /({PATTERN_NAME}) /s* '=' /s* /(.+) $; f",
         ["use-descriptor"] = $"^ '//' /({PATTERN_IN_SLASH}) '//; f",
         ["use-descriptor-format"] = $"'!' /({PATTERN_IN_BANG}) '!' /({PATTERN_IN_BANG}) '!'; f",
         ["font-size"] = $"^ '//font-size//' /({PATTERN_IN_SLASH}) '//; f",
         ["font-size-format"]=$"'!font-size!' /({PATTERN_IN_BANG}) '!' /({PATTERN_IN_BANG}) '!'; f",
         ["style"] = $"^ '//style//' /({PATTERN_IN_SLASH}) '//'; f",
         ["style-format"]=$" '!style!' /({PATTERN_IN_BANG}) '!' /({PATTERN_IN_BANG}) '!'; f",
         ["alignment"] = $"^ '//alignment//' /({PATTERN_IN_SLASH}) '//'; f"
      };
   }

   protected void createFont(MatchResult result)
   {
      var (fontName, specifier) = result;
      var fontDescriptor = document.Font(specifier);
      descriptors[fontName] = fontDescriptor;
   }

   protected void createColor(MatchResult result)
   {
      var (colorName, specifier) = result;
      var colorDescriptor = document.Color(specifier);
      descriptors[colorName] = colorDescriptor;
   }

   protected void useDescriptor(CharFormat charFormat, MatchResult result)
   {
      var name = result.FirstGroup;
      var _result = name.Matches("^ /(-[';']*) ';' /(.*) $");
      if (_result)
      {
         var (foreground, background) = ~_result;
         foreground = foreground.Trim();
         background = background.Trim();

         if (foreground.IsNotEmpty())
         {
            var _descriptor = descriptors.Of(foreground);
            if (_descriptor && ~_descriptor is ColorDescriptor foregroundDescriptor)
            {
               charFormat.ForegroundColor = foregroundDescriptor;
            }
         }

         if (background.IsNotEmpty())
         {
            var _descriptor = descriptors.Of(background);
            if (_descriptor && ~_descriptor is ColorDescriptor backgroundDescriptor)
            {
               charFormat.BackgroundColor = backgroundDescriptor;
            }
         }
      }
      else
      {
         var _descriptor = descriptors.Of(name);
         if (_descriptor && ~_descriptor is FontDescriptor fontDescriptor)
         {
            charFormat.Font = fontDescriptor;
         }
      }
   }

   protected void useDescriptor(Paragraph paragraph, MatchResult result) => useDescriptor(paragraph.DefaultCharFormat, result);

   protected string useDescriptor(CharFormat charFormat, MatchResult result, string line)
   {

   }

   protected void setFontSize(CharFormat charFormat, MatchResult result)
   {
      var _fontSize = Maybe.Single(result.FirstGroup);
      if (_fontSize)
      {
         charFormat.FontSize = _fontSize;
      }
   }

   protected void setFontSize(Paragraph paragraph, MatchResult result) => setFontSize(paragraph.DefaultCharFormat, result);

   protected void setStyle(CharFormat charFormat, MatchResult result)
   {
      var styles = result.FirstGroup.Unjoin("/s* '+' /s*; f");
      foreach (var style in styles)
      {
         Maybe<FontStyleFlag> _flag = style.ToLower() switch
         {
            "bold" => FontStyleFlag.Bold,
            "italic" => FontStyleFlag.Italic,
            "underline" => FontStyleFlag.Underline,
            "super" => FontStyleFlag.Super,
            "sub" => FontStyleFlag.Sub,
            "scaps" => FontStyleFlag.Scaps,
            "strike" => FontStyleFlag.Strike,
            _ => nil
         };
         if (_flag)
         {
            charFormat.FontStyle += _flag;
         }
      }
   }

   protected void setStyle(Paragraph paragraph, MatchResult result) => setStyle(paragraph.DefaultCharFormat, result);

   protected void setAlignment(Paragraph paragraph, MatchResult result)
   {
      var alignment = result.FirstGroup;
      Maybe<Alignment> _alignment = alignment.ToLower() switch
      {
         "left" => Alignment.Left,
         "right" => Alignment.Right,
         "center" => Alignment.Center,
         "full-justify" => Alignment.FullyJustify,
         "distributed" => Alignment.Distributed,
         _ => nil
      };
      if (_alignment)
      {
         paragraph.Alignment = _alignment;
      }
   }

   public Result<Document> Render(string template)
   {
      try
      {
         var source = new Source(template);

         while (source.More)
         {
            var _line = source.NextLine();
            if (!_line)
            {
               break;
            }

            var line = ~_line;
            var paragraph = document.Paragraph();
            while (true)
            {
               var _tagged = patterns.Matches(line);
               if (_tagged)
               {
                  var (tag, result) = ~_tagged;
                  switch (tag)
                  {
                     case "create-font":
                        createFont(result);
                        break;
                     case "create-color":
                        createColor(result);
                        break;
                     case "use-descriptor":
                        useDescriptor(paragraph, result);
                        break;
                     case "use-descriptor-format":
                        break;
                     case "font-size":
                        setFontSize(paragraph, result);
                        break;
                     case "font-size-format":
                        break;
                     case "style":
                        setStyle(paragraph, result);
                        break;
                     case "style-format":
                        break;
                     case "alignment":
                        setAlignment(paragraph, result);
                        break;
                  }
               }
               else
               {
                  paragraph.Text = line;
               }
            }
         }

         return document;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}