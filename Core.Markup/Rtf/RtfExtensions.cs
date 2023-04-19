using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public static class RtfExtensions
{
   public static Hyperlink Link(this string link, string linkTip) => new(link, linkTip);

   public static Hyperlink Link(this string link) => new(link);

   public static FirstLineIndent FirstLineIndent(this float amount) => new(amount);

   public static FirstLineIndent FirstLineIndent(this int amount) => new(amount);

   public static Bookmark Bookmark(this string name) => new(name);

   public static ColumnWidth ColumnWidth(this int index, float width) => new(index, width);

   public static RowHeight RowHeight(this int index, float height) => new(index, height);

   public static OuterBorder OuterBorder(this BorderStyle borderStyle, float width, ColorDescriptor color) => new(borderStyle, width, color);

   public static OuterBorder OuterBorder(this BorderStyle borderStyle, float width) => new(borderStyle, width, nil);

   public static InnerBorder InnerBorder(this BorderStyle borderStyle, float width, ColorDescriptor color) => new(borderStyle, width, color);

   public static InnerBorder InnerBorder(this BorderStyle borderStyle, float width) => new(borderStyle, width, nil);
}