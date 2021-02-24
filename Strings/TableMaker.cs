using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Enumerables;
using Core.Monads;
using static System.Math;

namespace Core.Strings
{
   public class TableMaker
   {
      protected interface IRow
      {
         void Evaluate(ColumnHeader[] columnHeaders);

         string Render(ColumnHeader[] columnHeaders, string columnSeparator);
      }

      protected class ColumnHeader
      {
         protected string header;
         protected Justification justification;
         protected int maxWidth;

         public ColumnHeader(string header, Justification justification)
         {
            this.header = header;
            this.justification = justification;
            maxWidth = this.header.Length;
         }

         public string Header => header;

         public Justification Justification => justification;

         public void Evaluate(string text) => maxWidth = Max(maxWidth, text.Length);

         public string Render(string text) => text.Justify(justification, maxWidth);

         public string Render() => header.Center(maxWidth);

         public int MaxWidth => maxWidth;
      }

      protected class Column
      {
         protected string text;

         public Column(string text) => this.text = text;

         public string Text => text;

         public void Evaluate(ColumnHeader columnHeader) => columnHeader.Evaluate(text);

         public string Render(ColumnHeader columnHeader) => columnHeader.Render(text);
      }

      protected class Row : IRow
      {
         protected Column[] columns;

         public Row(Column[] columns) => this.columns = columns;

         public void Evaluate(ColumnHeader[] columnHeaders)
         {
            for (var i = 0; i < columns.Length; i++)
            {
               columns[i].Evaluate(columnHeaders[i]);
            }
         }

         public string Render(ColumnHeader[] columnHeaders, string columnSeparator)
         {
            return columns.Zip(columnHeaders, (c, ch) => c.Render(ch)).ToString(columnSeparator);
         }
      }

      protected class Divider : IRow
      {
         protected char character;

         public Divider(char character) => this.character = character;

         public void Evaluate(ColumnHeader[] columnHeaders) { }

         public string Render(ColumnHeader[] columnHeaders, string columnSeparator)
         {
            var length = columnHeaders.Select(ch => ch.MaxWidth).Sum() + (columnHeaders.Length - 1) * columnSeparator.Length;
            return character.ToString().Repeat(length);
         }
      }

      protected class Line : IRow
      {
         public void Evaluate(ColumnHeader[] columnHeaders) { }

         public string Render(ColumnHeader[] columnHeaders, string columnSeparator) => "";
      }

      protected ColumnHeader[] columnHeaders;
      protected List<IRow> rows;
      protected bool hasHeaders;

      public TableMaker(params (string header, Justification justification)[] columns)
      {
         columnHeaders = columns.Select(c => new ColumnHeader(c.header, c.justification)).ToArray();
         rows = new List<IRow>();
         hasHeaders = true;
      }

      public TableMaker(params Justification[] justifications)
      {
         columnHeaders = justifications.Select(j => new ColumnHeader("", j)).ToArray();
         rows = new List<IRow>();
         hasHeaders = false;
      }

      public void Clear() => rows.Clear();

      public IMaybe<char> HeaderFoot { get; set; } = '='.Some();

      public string ColumnSeparator { get; set; } = " | ";

      public IMaybe<char> RowSeparator { get; set; } = '-'.Some();

      public TableMaker Add(params object[] items)
      {
         var headersLength = columnHeaders.Length;
         var columns = new Column[headersLength];
         var itemsLength = items.Length;
         var length = Min(headersLength, itemsLength);

         for (var i = 0; i < length; i++)
         {
            columns[i] = new Column(items[i].ToNonNullString());
         }

         if (itemsLength < headersLength)
         {
            for (var i = itemsLength; i < headersLength; i++)
            {
               columns[i] = new Column("");
            }
         }

         var row = new Row(columns.ToArray());
         row.Evaluate(columnHeaders);
         rows.Add(row);

         return this;
      }

      public TableMaker AddDivider(char character)
      {
         rows.Add(new Divider(character));
         return this;
      }

      public TableMaker AddLine()
      {
         rows.Add(new Line());
         return this;
      }

      public override string ToString()
      {
         var builder = new StringBuilder();

         int headerWidth;

         if (hasHeaders)
         {
            var header = columnHeaders.Select(ch => ch.Render()).ToString(ColumnSeparator);
            headerWidth = header.Length;
            builder.AppendLine(header);
            if (HeaderFoot.If(out var headerFoot))
            {
               builder.AppendLine(headerFoot.ToString().Repeat(headerWidth));
            }
            else
            {
               builder.AppendLine();
            }
         }
         else
         {
            headerWidth = columnHeaders.Select(ch => ch.MaxWidth).Sum() + (columnHeaders.Length - 1) * ColumnSeparator.Length;
         }

         var rowSeparator = "\r\n";
         if (RowSeparator.If(out var separator))
         {
            rowSeparator = rowSeparator + separator.ToString().Repeat(headerWidth) + rowSeparator;
         }

         foreach (var row in rows)
         {
            var renderedRow = row.Render(columnHeaders, ColumnSeparator);
            builder.Append(renderedRow);
            builder.Append(rowSeparator);
         }

         return builder.ToString();
      }
   }
}