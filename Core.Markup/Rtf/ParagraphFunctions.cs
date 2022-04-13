namespace Core.Markup.Rtf
{
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
               case FontDescriptor fontDescriptor:
                  format.Font = fontDescriptor;
                  break;
               case float fontSize:
                  format.FontSize = fontSize;
                  break;
               case Alignment alignment:
                  paragraph.Alignment = alignment;
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
               case (string hyperlink, string hyperlinkTip):
                  format.LocalHyperlink = hyperlink;
                  format.LocalHyperlinkTip = hyperlinkTip;
                  break;
            }
         }
      }
   }
}