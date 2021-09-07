using System.Text;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf
{
   public class TableCell : BlockList
   {
      private float width;
      private Alignment horizontalAlignment;
      private VerticalAlignment verticalAlignmentAlignment;
      private Borders borders;
      private LateLazy<CellMergeInfo> mergeInfo;
      private int rowIndex;
      private int columnIndex;

      public TableCell(float width, int rowIndex, int columnIndex, Table parentTable) : base(true, false)
      {
         this.width = width;
         this.rowIndex = rowIndex;
         this.columnIndex = columnIndex;
         ParentTable = parentTable;

         horizontalAlignment = Alignment.None;
         verticalAlignmentAlignment = VerticalAlignment.Top;
         borders = new Borders();
         mergeInfo = new LateLazy<CellMergeInfo>(true, "Merge info has not be set through MergeInfo property");
         BackgroundColor = nil;
      }

      public bool IsBeginOfColumnSpan => mergeInfo.AnyValue.Map(mergeInfo => mergeInfo.ColumnIndex == 0).DefaultTo(() => false);

      public bool IsBeginOfRowSpan => mergeInfo.AnyValue.Map(mergeInfo => mergeInfo.RowIndex == 0).DefaultTo(() => false);

      public bool IsMerged => mergeInfo.IsActivated;

      public CellMergeInfo MergeInfo
      {
         get => mergeInfo.Value;
         set
         {
            mergeInfo.ActivateWith(() => value);
            var _ = mergeInfo.Value;
         }
      }

      public float Width
      {
         get => width;
         set => width = value;
      }

      public Borders Borders => borders;

      public Table ParentTable { get; }

      public Maybe<ColorDescriptor> BackgroundColor { get; set; }

      public Alignment Alignment
      {
         get => horizontalAlignment;
         set => horizontalAlignment = value;
      }

      public VerticalAlignment AlignmentVerticalAlignment
      {
         get => verticalAlignmentAlignment;
         set => verticalAlignmentAlignment = value;
      }

      public int RowIndex => rowIndex;

      public int ColumnIndex => columnIndex;

      public float OuterLeftBorderClearance { get; set; }

      public void SetBorderColor(ColorDescriptor color)
      {
         borders[Direction.Top].Color = color;
         borders[Direction.Bottom].Color = color;
         borders[Direction.Left].Color = color;
         borders[Direction.Right].Color = color;
      }

      public override string Render()
      {
         var result = new StringBuilder();

         var align = horizontalAlignment switch
         {
            Alignment.Left => @"\ql",
            Alignment.Right => @"\qr",
            Alignment.Center => @"\qc",
            Alignment.FullyJustify => @"\qj",
            Alignment.Distributed => @"\qd",
            _ => string.Empty
         };

         if (blocks.Count <= 0)
         {
            result.AppendLine(@"\pard\intbl");
         }
         else
         {
            for (var i = 0; i < blocks.Count; i++)
            {
               var block = blocks[i];
               block.DefaultCharFormat.CopyFrom(defaultCharFormat);

               if (block.Margins[Direction.Top] < 0)
               {
                  block.Margins[Direction.Top] = 0;
               }

               if (block.Margins[Direction.Right] < 0)
               {
                  block.Margins[Direction.Right] = 0;
               }

               if (block.Margins[Direction.Bottom] < 0)
               {
                  block.Margins[Direction.Bottom] = 0;
               }

               if (block.Margins[Direction.Left] < 0)
               {
                  block.Margins[Direction.Left] = 0;
               }

               if (i == 0)
               {
                  block.BlockHead = $@"\pard\intbl{align}";
               }
               else
               {
                  block.BlockHead = $@"\par{align}";
               }

               block.BlockTail = string.Empty;
               result.AppendLine(block.Render());
            }
         }

         result.AppendLine(@"\cell");
         return result.ToString();
      }
   }
}