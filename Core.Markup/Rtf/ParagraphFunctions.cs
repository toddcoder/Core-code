using System;

namespace Core.Markup.Rtf;

public static class ParagraphFunctions
{
   public static void SetParagraphProperties(Paragraph paragraph, string text, params object[] specifiers)
   {
      paragraph.Text = text;
      var format = paragraph.DefaultCharFormat;
      var firstColor = false;

      foreach (var specifier in specifiers)
      {
         switch (specifier)
         {
            case Feature.Italic:
               format.FontStyle += FontStyleFlag.Italic;
               break;
            case Feature.Bold:
               format.FontStyle += FontStyleFlag.Bold;
               break;
            case Feature.Underline:
               format.FontStyle += FontStyleFlag.Underline;
               break;
            case Feature.Bullet:
               paragraph.Bullet = true;
               break;
            case FontData fontData:
               format.FontData = fontData;
               break;
            case FontDescriptor fontDescriptor:
               format.Font = fontDescriptor;
               break;
            case float fontSize:
               format.FontSize = fontSize;
               break;
            case Alignment alignment:
               paragraph.Alignment = alignment;
               break;
            case ForegroundColorDescriptor foregroundColor:
               format.ForegroundColor = foregroundColor;
               break;
            case BackgroundColorDescriptor backgroundColor:
               format.BackgroundColor = backgroundColor;
               break;
            case ColorDescriptor colorDescriptor when firstColor:
               format.BackgroundColor = colorDescriptor;
               break;
            case ColorDescriptor colorDescriptor:
               format.ForegroundColor = colorDescriptor;
               firstColor = true;
               break;
            case FontStyleFlag fontStyleFlag:
               format.FontStyle += fontStyleFlag;
               break;
            case LocalHyperlink link:
               format.LocalHyperlink = link.Link;
               format.LocalHyperlinkTip = link.LinkTip;
               break;
            case bool bullet:
               paragraph.Bullet = bullet;
               break;
            case Action<Paragraph> action:
               action(paragraph);
               break;
            case Action<CharFormat> action:
               action(paragraph.DefaultCharFormat);
               break;
         }
      }
   }
}