using Core.Numbers;

namespace Core.Markup.Rtf;

public class FontStyle
{
   public static FontStyle operator +(FontStyle fontStyle, FontStyleFlag fontStyleFlag)
   {
      fontStyle.styleAdd[fontStyleFlag] = true;
      fontStyle.styleRemove[fontStyleFlag] = false;

      return fontStyle;
   }

   public static FontStyle operator -(FontStyle fontStyle, FontStyleFlag fontStyleFlag)
   {
      fontStyle.styleAdd[fontStyleFlag] = false;
      fontStyle.styleRemove[fontStyleFlag] = true;

      return fontStyle;
   }

   protected Bits32<FontStyleFlag> styleAdd;
   protected Bits32<FontStyleFlag> styleRemove;

   public FontStyle()
   {
      styleAdd = FontStyleFlag.None;
      styleRemove = FontStyleFlag.None;
   }

   public FontStyle(FontStyle src)
   {
      styleAdd = src.styleAdd;
      styleRemove = src.styleRemove;
   }

   public bool ContainsStyleAdd(FontStyleFlag styleFlag) => styleAdd[styleFlag];

   public bool ContainsStyleRemove(FontStyleFlag styleFlag) => styleRemove[styleFlag];

   public bool IsEmpty => styleAdd == FontStyleFlag.None && styleRemove == FontStyleFlag.None;
}