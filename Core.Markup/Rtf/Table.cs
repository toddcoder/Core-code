using System;
using System.Collections.Generic;
using System.Text;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.DataStructures;
using Core.Exceptions;
using Core.Monads;
using static Core.Markup.Rtf.ParagraphFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf
{
   public class Table : Block
   {
      protected class CellData
      {
         public CellData()
         {
            Text = string.Empty;
            Specifiers = Array.Empty<object>();
            ImageFile = nil;
            ImageFileType = nil;
         }

         public string Text { get; set; }

         public object[] Specifiers { get; set; }

         public Maybe<FileName> ImageFile { get; set; }

         public Maybe<ImageFileType> ImageFileType { get; set; }
      }

      protected Alignment alignment;
      protected Margins margins;
      protected int rowCount;
      protected int columnCount;
      protected TableCell[][] cells;
      protected List<TableCell> representatives;
      protected bool startNewPage;
      protected float[] rowHeights;
      protected bool[] rowKeepInSamePage;
      protected int titleRowCount;
      protected readonly float fontSize;
      protected CharFormat defaultCharFormat;
      protected Margins[] cellPadding;
      protected List<List<CellData>> rows;
      protected int maxColumnCount;
      protected float horizontalWidth;
      protected int rowIndex;
      protected bool arrayCreated;
      protected MaybeQueue<(int topRow, int leftColumn, int rowSpan, int colSpan)> pendingMerges;

      public Table(float horizontalWidth, float fontSize)
      {
         this.horizontalWidth = horizontalWidth;
         this.fontSize = fontSize;

         alignment = Alignment.None;
         margins = new Margins();
         representatives = new List<TableCell>();
         startNewPage = false;
         titleRowCount = 0;

         HeaderBackgroundColor = nil;
         RowBackgroundColor = nil;
         RowAltBackgroundColor = nil;
         defaultCharFormat = new CharFormat();

         rows = new List<List<CellData>>();
         maxColumnCount = 0;
         rowIndex = -1;
         arrayCreated = false;
         pendingMerges = new MaybeQueue<(int, int, int, int)>();
      }

      public void AddRow()
      {
         rows.Add(new List<CellData>());
         rowIndex = rows.Count - 1;
      }

      public void AddColumn(string text, params object[] specifiers)
      {
         var cellData = new CellData { Text = text, Specifiers = specifiers };
         var row = rows[rowIndex];
         row.Add(cellData);
         if (row.Count > maxColumnCount)
         {
            maxColumnCount = row.Count;
         }
      }

      public void AddImage(FileName imageFile, ImageFileType imageFileType)
      {
         var cellData = new CellData { ImageFile = imageFile, ImageFileType = imageFileType };
         var row = rows[rowIndex];
         row.Add(cellData);
         if (row.Count > maxColumnCount)
         {
            maxColumnCount = row.Count;
         }
      }

      protected void createArrays()
      {
         if (arrayCreated)
         {
            return;
         }

         rowCount = rows.Count;
         columnCount = maxColumnCount;

         cellPadding = new Margins[rowCount];
         var defaultCellWidth = horizontalWidth / columnCount;
         cells = new TableCell[rowCount][];
         rowHeights = new float[rowCount];
         rowKeepInSamePage = new bool[rowCount];

         for (var i = 0; i < rowCount; i++)
         {
            cells[i] = new TableCell[columnCount];
            rowHeights[i] = 0F;
            rowKeepInSamePage[i] = false;
            cellPadding[i] = new Margins();
            var row = rows[i];
            var count = row.Count;
            for (var j = 0; j < columnCount; j++)
            {
               var tableCell = new TableCell(defaultCellWidth, i, j, this);
               cells[i][j] = tableCell;
               if (j < count)
               {
                  var cellData = row[j];
                  if (cellData.ImageFile.Map(out var imageFile) && cellData.ImageFileType.Map(out var imageFileType))
                  {
                     tableCell.Image(imageFile.FullPath, imageFileType);
                  }
                  else
                  {
                     var paragraph = tableCell.Paragraph();
                     SetParagraphProperties(paragraph, cellData.Text, cellData.Specifiers);
                  }
               }
            }
         }

         arrayCreated = true;
      }

      public Maybe<ColorDescriptor> HeaderBackgroundColor { get; set; }

      public Maybe<ColorDescriptor> RowBackgroundColor { get; set; }

      public Maybe<ColorDescriptor> RowAltBackgroundColor { get; set; }

      public override Alignment Alignment
      {
         get => alignment;
         set => alignment = value;
      }

      public override Margins Margins
      {
         get
         {
            createArrays();
            return margins;
         }
      }

      public override CharFormat DefaultCharFormat => defaultCharFormat;

      public override bool StartNewPage
      {
         get => startNewPage;
         set => startNewPage = value;
      }

      public int RowCount => rowCount;

      public int ColumnCount => columnCount;

      public int TitleRowCount
      {
         get => titleRowCount;
         set => titleRowCount = value;
      }

      public Margins[] CellPadding
      {
         get
         {
            createArrays();
            return cellPadding;
         }
      }

      public override string BlockHead
      {
         set => throw "BlockHead is not supported for tables.".Throws();
      }

      public override string BlockTail
      {
         set => throw "BlockTail is not supported for tables.".Throws();
      }

      public TableCell this[int row, int col]
      {
         get
         {
            createArrays();
            var cell = cells[row][col];
            if (cell.IsMerged)
            {
               return cell.MergeInfo.Representative;
            }
            else
            {
               return cell;
            }
         }
      }

      protected void assertColumnInRange(int column)
      {
         column.Must().BeBetween(0).Until(columnCount).OrThrow("Column index out of range");
      }

      protected void assertRowInRange(int row)
      {
         row.Must().BeBetween(0).Until(rowCount).OrThrow("Row index out of range");
      }

      public void SetColumnWidth(int column, float width)
      {
         createArrays();
         assertColumnInRange(column);

         for (var i = 0; i < rowCount; i++)
         {
            cells[i][column].IsMerged.Must().Not.BeTrue().OrThrow("Column width cannot be set because some cell in this column has been merged.");
         }

         for (var i = 0; i < rowCount; i++)
         {
            cells[i][column].Width = width;
         }
      }

      public void SetRowHeight(int row, float height)
      {
         createArrays();
         assertRowInRange(row);

         for (var i = 0; i < columnCount; i++)
         {
            var cell = cells[row][i];
            if (cell.IsMerged && cell.MergeInfo.Representative.MergeInfo.RowSpan > 1)
            {
               throw "Row height cannot be set because some cell in this row has been merged.".Throws();
            }
         }

         rowHeights[row] = height;
      }

      public void SetRowKeepInSamePage(int row, bool allow)
      {
         createArrays();

         assertRowInRange(row);

         rowKeepInSamePage[row] = allow;
      }

      public void PendingMerge(int topRow, int leftColumn, int rowSpan, int colSpan)
      {
         pendingMerges.Enqueue((topRow, leftColumn, rowSpan, colSpan));
      }

      public void ActivatePendingMerges()
      {
         while (pendingMerges.Dequeue().Map(out var topRow, out var leftColumn, out var rowSpan, out var colSpan))
         {
            Merge(topRow, leftColumn, rowSpan, colSpan);
         }
      }

      public TableCell Merge(int topRow, int leftColumn, int rowSpan, int colSpan)
      {
         createArrays();

         assertRowInRange(topRow);
         assertColumnInRange(leftColumn);

         rowSpan.Must().BeGreaterThanOrEqual(1).OrThrow("Row span out of range");
         (topRow + rowSpan - 1).Must().BeLessThan(rowCount).OrThrow("Row span out of range");

         colSpan.Must().BeGreaterThanOrEqual(1).OrThrow("Column span out of range");
         (leftColumn + colSpan - 1).Must().BeLessThan(columnCount).OrThrow("Column span out of range");

         if (colSpan == 1 && rowSpan == 1)
         {
            return this[topRow, leftColumn];
         }

         for (var i = 0; i < rowSpan; i++)
         {
            for (var j = 0; j < colSpan; j++)
            {
               cells[topRow + i][leftColumn + j].IsMerged.Must().Not.BeTrue()
                  .OrThrow("Cannot merge cells because some of the cells has been merged.");
            }
         }

         float width = 0;
         for (var i = 0; i < rowSpan; i++)
         {
            for (var j = 0; j < colSpan; j++)
            {
               if (i == 0)
               {
                  width += cells[topRow][leftColumn + j].Width;
               }

               var tableCell = cells[topRow + i][leftColumn + j];
               var cellMergeInfo = new CellMergeInfo(cells[topRow][leftColumn], rowSpan, colSpan, i, j);
               tableCell.MergeInfo = cellMergeInfo;
               if (i != 0 || j != 0)
               {
                  tableCell.TransferBlocksTo(cellMergeInfo.Representative);
               }
            }
         }

         cells[topRow][leftColumn].Width = width;
         representatives.Add(cells[topRow][leftColumn]);

         return cells[topRow][leftColumn];
      }

      protected void validateAllMergedCellBorders()
      {
         foreach (var representative in representatives)
         {
            validateMergedCellBorders(representative);
         }
      }

      protected static void assertRepresentativeMustBeMerged(TableCell representative)
      {
         representative.IsMerged.Must().BeTrue().OrThrow("Invalid representative (cell is not merged).");
      }

      protected void validateMergedCellBorders(TableCell representative)
      {
         assertRepresentativeMustBeMerged(representative);

         validateMergedCellBorder(representative, Direction.Top);
         validateMergedCellBorder(representative, Direction.Right);
         validateMergedCellBorder(representative, Direction.Bottom);
         validateMergedCellBorder(representative, Direction.Left);
      }

      protected void validateMergedCellBorder(TableCell representative, Direction direction)
      {
         assertRepresentativeMustBeMerged(representative);

         var statistics = new Hash<Border, int>();
         var limit = direction is Direction.Top or Direction.Bottom ? representative.MergeInfo.ColumnSpan : representative.MergeInfo.RowSpan;

         for (var i = 0; i < limit; i++)
         {
            int rowSpan;
            int columnSpan;
            if (direction is Direction.Top or Direction.Bottom)
            {
               rowSpan = direction == Direction.Top ? 0 : representative.MergeInfo.RowSpan - 1;
               columnSpan = i;
            }
            else
            {
               columnSpan = direction == Direction.Right ? representative.MergeInfo.ColumnSpan - 1 : 0;
               rowSpan = i;
            }

            var border = cells[representative.RowIndex + rowSpan][representative.ColumnIndex + columnSpan].Borders[direction];

            if (statistics.ContainsKey(border))
            {
               statistics[border] += 1;
            }
            else
            {
               statistics[border] = 1;
            }
         }

         var majorityCount = -1;
         var majorityBorder = representative.Borders[direction];
         foreach (var (border, count) in statistics)
         {
            if (count > majorityCount)
            {
               majorityCount = count;
               majorityBorder.Style = border.Style;
               majorityBorder.Width = border.Width;
               majorityBorder.Color = border.Color;
            }
         }
      }

      public void SetInnerBorder(BorderStyle style, float width) => SetInnerBorder(style, width, new ColorDescriptor(0));

      public void SetInnerBorder(BorderStyle style, float width, ColorDescriptor color)
      {
         createArrays();

         for (var i = 0; i < rowCount; i++)
         {
            for (var j = 0; j < columnCount; j++)
            {
               if (i == 0)
               {
                  cells[i][j].Borders[Direction.Bottom].Style = style;
                  cells[i][j].Borders[Direction.Bottom].Width = width;
                  cells[i][j].Borders[Direction.Bottom].Color = color;
               }
               else if (i == rowCount - 1)
               {
                  cells[i][j].Borders[Direction.Top].Style = style;
                  cells[i][j].Borders[Direction.Top].Width = width;
                  cells[i][j].Borders[Direction.Top].Color = color;
               }
               else
               {
                  cells[i][j].Borders[Direction.Top].Style = style;
                  cells[i][j].Borders[Direction.Top].Width = width;
                  cells[i][j].Borders[Direction.Top].Color = color;
                  cells[i][j].Borders[Direction.Bottom].Style = style;
                  cells[i][j].Borders[Direction.Bottom].Color = color;
                  cells[i][j].Borders[Direction.Bottom].Width = width;
               }

               if (j == 0)
               {
                  cells[i][j].Borders[Direction.Right].Style = style;
                  cells[i][j].Borders[Direction.Right].Width = width;
                  cells[i][j].Borders[Direction.Right].Color = color;
               }
               else if (j == columnCount - 1)
               {
                  cells[i][j].Borders[Direction.Left].Style = style;
                  cells[i][j].Borders[Direction.Left].Width = width;
                  cells[i][j].Borders[Direction.Left].Color = color;
               }
               else
               {
                  cells[i][j].Borders[Direction.Right].Style = style;
                  cells[i][j].Borders[Direction.Right].Width = width;
                  cells[i][j].Borders[Direction.Right].Color = color;
                  cells[i][j].Borders[Direction.Left].Style = style;
                  cells[i][j].Borders[Direction.Left].Width = width;
                  cells[i][j].Borders[Direction.Left].Color = color;
               }
            }
         }
      }

      public void SetOuterBorder(BorderStyle style, float width) => SetOuterBorder(style, width, new ColorDescriptor(0));

      public void SetOuterBorder(BorderStyle style, float width, ColorDescriptor color)
      {
         createArrays();

         for (var i = 0; i < columnCount; i++)
         {
            cells[0][i].Borders[Direction.Top].Style = style;
            cells[0][i].Borders[Direction.Top].Width = width;
            cells[0][i].Borders[Direction.Top].Color = color;
            cells[rowCount - 1][i].Borders[Direction.Bottom].Style = style;
            cells[rowCount - 1][i].Borders[Direction.Bottom].Width = width;
            cells[rowCount - 1][i].Borders[Direction.Bottom].Color = color;
         }

         for (var i = 0; i < rowCount; i++)
         {
            cells[i][0].Borders[Direction.Left].Style = style;
            cells[i][0].Borders[Direction.Left].Width = width;
            cells[i][0].Borders[Direction.Left].Color = color;
            cells[i][columnCount - 1].Borders[Direction.Right].Style = style;
            cells[i][columnCount - 1].Borders[Direction.Right].Width = width;
            cells[i][columnCount - 1].Borders[Direction.Right].Color = color;
         }
      }

      public void SetHeaderBorderColors(ColorDescriptor colorOuter, ColorDescriptor colorInner)
      {
         createArrays();

         for (var j = 0; j < columnCount; j++)
         {
            cells[0][j].Borders[Direction.Top].Color = colorOuter;
            cells[0][j].Borders[Direction.Bottom].Color = colorInner;

            if (j == 0)
            {
               cells[0][j].Borders[Direction.Right].Color = colorInner;
               cells[0][j].Borders[Direction.Left].Color = colorOuter;
            }
            else if (j == columnCount - 1)
            {
               cells[0][j].Borders[Direction.Right].Color = colorOuter;
               cells[0][j].Borders[Direction.Left].Color = colorInner;
            }
            else
            {
               cells[0][j].Borders[Direction.Right].Color = colorInner;
               cells[0][j].Borders[Direction.Left].Color = colorInner;
            }
         }
      }

      public override string Render()
      {
         createArrays();

         var result = new StringBuilder();

         validateAllMergedCellBorders();

         for (var i = 0; i < rowCount; i++)
         {
            for (var j = 0; j < columnCount; j++)
            {
               if (cells[i][j].IsMerged && cells[i][j].MergeInfo.Representative != cells[i][j])
               {
                  continue;
               }

               cells[i][j].DefaultCharFormat.CopyFrom(defaultCharFormat);
            }
         }

         var topMargin = margins[Direction.Top] - fontSize;

         if (startNewPage || topMargin > 0)
         {
            result.Append(@"{\pard");
            if (startNewPage)
            {
               result.Append(@"\pagebb");
            }

            if (margins[Direction.Top] >= 0)
            {
               result.Append(@"\sl-" + topMargin.PointsToTwips());
            }
            else
            {
               result.Append(@"\sl-1");
            }

            result.AppendLine(@"\slmult0\par}");
         }

         for (var i = 0; i < rowCount; i++)
         {
            var columnAccumulator = 0;

            result.Append($@"{{\trowd\trgaph\trpaddl{CellPadding[i][Direction.Left].PointsToTwips()}");
            result.Append($@"\trpaddt{CellPadding[i][Direction.Top].PointsToTwips()}");
            result.Append($@"\trpaddr{CellPadding[i][Direction.Right].PointsToTwips()}");
            result.Append($@"\trpaddb{CellPadding[i][Direction.Bottom].PointsToTwips()}");

            switch (alignment)
            {
               case Alignment.Left:
                  result.Append(@"\trql");
                  break;
               case Alignment.Right:
                  result.Append(@"\trqr");
                  break;
               case Alignment.Center:
                  result.Append(@"\trqc");
                  break;
               case Alignment.FullyJustify:
                  result.Append(@"\trqj");
                  break;
            }

            result.AppendLine();
            if (margins[Direction.Left] >= 0)
            {
               result.AppendLine($@"\trleft{margins[Direction.Left].PointsToTwips()}");
               columnAccumulator = margins[Direction.Left].PointsToTwips();
            }

            if (rowHeights[i] > 0)
            {
               result.Append($@"\trrh{rowHeights[i].PointsToTwips()}");
            }

            if (rowKeepInSamePage[i])
            {
               result.Append(@"\trkeep");
            }

            if (i < titleRowCount)
            {
               result.Append(@"\trhdr");
            }

            result.AppendLine();

            for (var j = 0; j < columnCount; j++)
            {
               if (cells[i][j].IsMerged && !cells[i][j].IsBeginOfColumnSpan)
               {
                  continue;
               }

               var nextCellLeftBorderClearance = j < columnCount - 1 ? this[i, j + 1].OuterLeftBorderClearance : 0;
               columnAccumulator += this[i, j].Width.PointsToTwips();
               var columnRightPosition = columnAccumulator;

               if (nextCellLeftBorderClearance < 0)
               {
                  columnRightPosition += nextCellLeftBorderClearance.PointsToTwips();
                  columnRightPosition = columnRightPosition == 0 ? 1 : columnRightPosition;
               }

               for (var direction = Direction.Top; direction <= Direction.Left; direction++)
               {
                  var border = this[i, j].Borders[direction];
                  if (border.Style != BorderStyle.None)
                  {
                     result.Append(@"\clbrdr");
                     switch (direction)
                     {
                        case Direction.Top:
                           result.Append("t");
                           break;
                        case Direction.Right:
                           result.Append("r");
                           break;
                        case Direction.Bottom:
                           result.Append("b");
                           break;
                        case Direction.Left:
                           result.Append("l");
                           break;
                     }

                     result.Append($@"\brdrw{border.Width.PointsToTwips()}");
                     result.Append(@"\brdr");
                     switch (border.Style)
                     {
                        case BorderStyle.Single:
                           result.Append("s");
                           break;
                        case BorderStyle.Dotted:
                           result.Append("dot");
                           break;
                        case BorderStyle.Dashed:
                           result.Append("dash");
                           break;
                        case BorderStyle.Double:
                           result.Append("db");
                           break;
                        default:
                           throw "Unknown border style".Throws();
                     }

                     result.Append($@"\brdrcf{border.Color.Value}");
                  }
               }

               if (this[i, j].BackgroundColor.Map(out var backgroundColor))
               {
                  result.Append($@"\clcbpat{backgroundColor.Value}");
               }
               else if (i == 0 && HeaderBackgroundColor.Map(out var headerBackgroundColor))
               {
                  result.Append($@"\clcbpat{headerBackgroundColor.Value}");
               }
               else if (RowBackgroundColor.Map(out var rowBackgroundColor) && (RowAltBackgroundColor.IsNone || i % 2 == 0))
               {
                  result.Append($@"\clcbpat{rowBackgroundColor.Value}");
               }
               else if (RowBackgroundColor.IsSome && RowAltBackgroundColor.Map(out var rowAltBackgroundColor) && i % 2 != 0)
               {
                  result.Append($@"\clcbpat{rowAltBackgroundColor.Value}");
               }

               if (cells[i][j].IsMerged && cells[i][j].MergeInfo.RowSpan > 1)
               {
                  if (cells[i][j].IsBeginOfRowSpan)
                  {
                     result.Append(@"\clvmgf");
                  }
                  else
                  {
                     result.Append(@"\clvmrg");
                  }
               }

               switch (cells[i][j].AlignmentVerticalAlignment)
               {
                  case VerticalAlignment.Top:
                     result.Append(@"\clvertalt");
                     break;
                  case VerticalAlignment.Middle:
                     result.Append(@"\clvertalc");
                     break;
                  case VerticalAlignment.Bottom:
                     result.Append(@"\clvertalb");
                     break;
               }

               result.AppendLine($@"\cellx{columnRightPosition}");
            }

            for (var j = 0; j < columnCount; j++)
            {
               if (!cells[i][j].IsMerged || cells[i][j].IsBeginOfColumnSpan)
               {
                  result.Append(cells[i][j].Render());
               }
            }

            result.AppendLine(@"\row}");
         }

         if (margins[Direction.Bottom] >= 0)
         {
            result.Append($@"\sl-{margins[Direction.Bottom].PointsToTwips()}\slmult");
         }

         return result.ToString();
      }
   }
}