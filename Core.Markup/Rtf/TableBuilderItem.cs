using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class TableBuilderItem
{
   public static TableBuilderItem operator |(TableBuilderItem tableBuilderItem, int index)
   {
      tableBuilderItem.Index = index;
      return tableBuilderItem;
   }

   public static TableBuilderItem operator |(TableBuilderItem tableBuilderItem, float size)
   {
      tableBuilderItem.Size = size;
      return tableBuilderItem;
   }

   public static TableBuilderItem operator |(TableBuilderItem tableBuilderItem, BorderStyle borderStyle)
   {
      tableBuilderItem.BorderStyle = borderStyle;
      return tableBuilderItem;
   }

   public static TableBuilderItem operator |(TableBuilderItem tableBuilderItem, ColorDescriptor color)
   {
      tableBuilderItem.Color = color;
      return tableBuilderItem;
   }

   public TableBuilderItem()
   {
      Type = TableBuilderType.None;
      Index = nil;
      BorderStyle = nil;
      Size = nil;
      Color = nil;
   }

   public TableBuilderType Type { get; set; }

   public Maybe<int> Index { get; set; }

   public Maybe<BorderStyle> BorderStyle { get; set; }

   public Maybe<float> Size { get; set; }

   public Maybe<ColorDescriptor> Color { get; set; }

   public void SetItem(Table table)
   {
      switch (Type)
      {
         case TableBuilderType.ColumnWidth when Index && Size:
            table.SetColumnWidth(Index, Size);
            break;
         case TableBuilderType.RowHeight when Index && Size:
            table.SetRowHeight(Index, Size);
            break;
         case TableBuilderType.OuterBorder when BorderStyle && Size && Color:
            table.SetOuterBorder(BorderStyle, Size, Color);
            break;
         case TableBuilderType.OuterBorder when BorderStyle && Size:
            table.SetOuterBorder(BorderStyle, Size);
            break;
         case TableBuilderType.InnerBorder when BorderStyle && Size && Color:
            table.SetInnerBorder(BorderStyle, Size, Color);
            break;
         case TableBuilderType.InnerBorder when BorderStyle && Size:
            table.SetInnerBorder(BorderStyle, Size);
            break;
         case TableBuilderType.HeaderBackgroundColor when Color:
            table.HeaderBackgroundColor = Color;
            break;
         case TableBuilderType.RowBackgroundColor when Color:
            table.RowBackgroundColor = Color;
            break;
         case TableBuilderType.RowAltBackgroundColor when Color:
            table.RowAltBackgroundColor = Color;
            break;
         case TableBuilderType.MarginTop when Size:
            table.Margins[Direction.Top] = Size;
            break;
         case TableBuilderType.MarginBottom when Size:
            table.Margins[Direction.Bottom] = Size;
            break;
         case TableBuilderType.MarginRight when Size:
            table.Margins[Direction.Right] = Size;
            break;
         case TableBuilderType.MarginLeft when Size:
            table.Margins[Direction.Left] = Size;
            break;
      }
   }
}