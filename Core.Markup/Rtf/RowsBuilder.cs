using System;
using System.Linq;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class RowsBuilder
{
   public static RowBuilder operator |(RowsBuilder rowsBuilder, Feature feature) => feature switch
   {
      Feature.Bold => rowsBuilder.Bold(),
      Feature.Italic => rowsBuilder.Italic(),
      Feature.Underline => rowsBuilder.Underline(),
      Feature.Bullet => rowsBuilder.Bullet(),
      Feature.NewPage => rowsBuilder.NewPage(),
      Feature.NewPageAfter => rowsBuilder.NewPageAfter(),
      _ => new RowBuilder(rowsBuilder.table)
   };

   public static RowBuilder operator |(RowsBuilder rowsBuilder, Alignment alignment) => rowsBuilder.Alignment(alignment);

   public static RowBuilder operator |(RowsBuilder rowsBuilder, ForegroundColorDescriptor foregroundColor)
   {
      return rowsBuilder.ForegroundColor(foregroundColor);
   }

   public static RowBuilder operator |(RowsBuilder rowsBuilder, BackgroundColorDescriptor backgroundColor)
   {
      return rowsBuilder.BackgroundColor(backgroundColor);
   }

   public static RowBuilder operator |(RowsBuilder rowsBuilder, Hyperlink hyperlink) => rowsBuilder.Hyperlink(hyperlink);

   public static RowBuilder operator |(RowsBuilder rowsBuilder, FontDescriptor font) => rowsBuilder.Font(font);

   public static RowBuilder operator |(RowsBuilder rowsBuilder, float fontSize) => rowsBuilder.FontSize(fontSize);

   public static RowBuilder operator |(RowsBuilder rowsBuilder, FirstLineIndent firstLineIndent) => rowsBuilder.FirstLineIndent(firstLineIndent);

   public static RowBuilder operator |(RowsBuilder rowsBuilder, (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      return rowsBuilder.Margins(margins);
   }

   public static RowBuilder operator |(RowsBuilder rowsBuilder, Style style) => rowsBuilder.Style(style);

   protected Table table;
   protected string[] columns;

   public RowsBuilder(Table table, string[] columns)
   {
      this.table = table;
      this.columns = columns;
   }

   protected RowBuilder applyToColumns(Action<RowBuilder> action)
   {
      var rowBuilder = new RowBuilder(table);

      if (columns.Take(1).FirstOrNone() is (true, var firstValue))
      {
         action(rowBuilder.Row(firstValue));
      }

      foreach (var column in columns.Skip(1))
      {
         action(rowBuilder.Column(column));
      }

      return rowBuilder;
   }

   public RowBuilder Italic(bool on = true) => applyToColumns(rb => rb.Italic(on));

   public virtual RowBuilder Bold(bool on = true) => applyToColumns(rb => rb.Bold(on));

   public virtual RowBuilder Underline(bool on = true) => applyToColumns(rb => rb.Underline(on));

   public virtual RowBuilder Bullet() => applyToColumns(rb => rb.Bullet());

   public virtual RowBuilder NewPage() => applyToColumns(rb => rb.NewPage());

   public virtual RowBuilder NewPageAfter() => applyToColumns(rb => rb.NewPageAfter());

   public RowBuilder Alignment(Alignment alignment) => applyToColumns(rb => rb.Alignment(alignment));

   public RowBuilder ForegroundColor(ForegroundColorDescriptor foregroundColor) => applyToColumns(rb => rb.ForegroundColor(foregroundColor));

   public RowBuilder BackgroundColor(BackgroundColorDescriptor backgroundColor) => applyToColumns(rb => rb.BackgroundColor(backgroundColor));

   public RowBuilder Hyperlink(Hyperlink hyperlink) => applyToColumns(rb => rb.Hyperlink(hyperlink));

   public RowBuilder Font(FontDescriptor font) => applyToColumns(rb => rb.Font(font));

   public RowBuilder FontSize(float fontSize) => applyToColumns(rb => rb.FontSize(fontSize));

   public RowBuilder FirstLineIndent(FirstLineIndent firstLineIndent) => applyToColumns(rb => rb.FirstLineIndent(firstLineIndent));

   public RowBuilder Margins((Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins) => applyToColumns(rb => rb.Margins(margins));

   public RowBuilder Style(Style style) => applyToColumns(rb => rb.Style(style));
}