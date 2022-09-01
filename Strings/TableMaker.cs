using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Enumerables;
using Core.Monads;
using static System.Math;
using static Core.Monads.MonadFunctions;

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

         public void Evaluate(ColumnHeader[] columnHeaders)
         {
         }

         public virtual string Render(ColumnHeader[] columnHeaders, string columnSeparator)
         {
            var length = dividerLength(columnHeaders, columnSeparator);
            return character.ToString().Repeat(length);
         }

         protected static int dividerLength(ColumnHeader[] columnHeaders, string columnSeparator)
         {
            return columnHeaders.Select(ch => ch.MaxWidth).Sum() + (columnHeaders.Length - 1) * columnSeparator.Length;
         }
      }

      protected class Line : IRow
      {
         public void Evaluate(ColumnHeader[] columnHeaders)
         {
         }

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
         HeaderFoot = '=';
         ColumnSeparator = " | ";
         RowSeparator = '-';
         Title = nil;
      }

      public TableMaker(params Justification[] justifications)
      {
         columnHeaders = justifications.Select(j => new ColumnHeader("", j)).ToArray();

         rows = new List<IRow>();
         hasHeaders = false;

         HeaderFoot = '=';
         ColumnSeparator = " | ";
         RowSeparator = '-';
         Title = nil;
      }

      public void Clear() => rows.Clear();

      public Maybe<char> HeaderFoot { get; set; }

      public string ColumnSeparator { get; set; }

      public Maybe<char> RowSeparator { get; set; }

      public Maybe<string> Title { get; set; }

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

         string getRowSeparator()
         {
            var rowSeparator = "\r\n";
            return RowSeparator.Map(rs => rowSeparator + rs.Repeat(headerWidth) + rowSeparator) | rowSeparator;
         }

         if (hasHeaders)
         {
            var header = columnHeaders.Select(ch => ch.Render()).ToString(ColumnSeparator);
            headerWidth = header.Length;

            if (Title.Map(out var title))
            {
               builder.Append(title.Center(headerWidth));
               builder.Append(getRowSeparator());
            }

            builder.AppendLine(header);
            if (HeaderFoot.Map(out var headerFoot))
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

         var rowSeparator = getRowSeparator();
         foreach (var renderedRow in rows.Select(row => row.Render(columnHeaders, ColumnSeparator)))
         {
            builder.Append(renderedRow);
            builder.Append(rowSeparator);
         }

         return builder.ToString();
      }
   }
}