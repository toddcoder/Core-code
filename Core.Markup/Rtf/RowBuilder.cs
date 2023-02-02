using Core.Monads;

namespace Core.Markup.Rtf;

public class RowBuilder
{
   public static RowBuilder operator |(RowBuilder rowBuilder, Feature feature) => feature switch
   {
      Feature.Bold => rowBuilder.Bold(),
      Feature.Italic => rowBuilder.Italic(),
      Feature.Underline => rowBuilder.Underline(),
      Feature.Bullet => rowBuilder.Bullet(),
      Feature.NewPage => rowBuilder.NewPage(),
      Feature.NewPageAfter => rowBuilder.NewPageAfter(),
      _ => rowBuilder
   };

   public static RowBuilder operator |(RowBuilder rowBuilder, Alignment alignment) => rowBuilder.Alignment(alignment);

   public static RowBuilder operator |(RowBuilder rowBuilder, ForegroundColorDescriptor foregroundColor)
   {
      return rowBuilder.ForegroundColor(foregroundColor);
   }

   public static RowBuilder operator |(RowBuilder rowBuilder, BackgroundColorDescriptor backgroundColor)
   {
      return rowBuilder.BackgroundColor(backgroundColor);
   }

   public static RowBuilder operator |(RowBuilder rowBuilder, Hyperlink hyperlink) => rowBuilder.Hyperlink(hyperlink);

   public static RowBuilder operator |(RowBuilder rowBuilder, FontDescriptor font) => rowBuilder.Font(font);

   public static RowBuilder operator |(RowBuilder rowBuilder, float fontSize) => rowBuilder.FontSize(fontSize);

   public static RowBuilder operator |(RowBuilder rowBuilder, FirstLineIndent firstLineIndent) => rowBuilder.FirstLineIndent(firstLineIndent);

   public static RowBuilder operator |(RowBuilder rowBuilder, (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      return rowBuilder.Margins(margins);
   }

   public static RowBuilder operator |(RowBuilder rowBuilder, Style style) => rowBuilder.Style(style);

   public static RowBuilder operator |(RowBuilder rowBuilder, string columnText) => rowBuilder.Column(columnText);

   protected Table table;
   protected PendingFormatter formatter;

   public RowBuilder(Table table)
   {
      this.table = table;
      formatter = new PendingFormatter(this.table);
   }

   public RowBuilder Row(string columnText)
   {
      table.Row();
      formatter = table.ColumnPendingFormatter(columnText);

      return this;
   }

   public RowBuilder Column(string columnText)
   {
      formatter = table.ColumnPendingFormatter(columnText);
      return this;
   }

   public virtual RowBuilder Italic(bool on = true)
   {
      formatter.Italic(on);
      return this;
   }

   public virtual RowBuilder Bold(bool on = true)
   {
      formatter.Bold(on);
      return this;
   }

   public virtual RowBuilder Underline(bool on = true)
   {
      formatter.Underline(on);
      return this;
   }

   public virtual RowBuilder Bullet()
   {
      formatter.Bullet();
      return this;
   }

   public virtual RowBuilder NewPage()
   {
      formatter.NewPage();
      return this;
   }

   public virtual RowBuilder NewPageAfter()
   {
      formatter.NewPageAfter();
      return this;
   }

   public RowBuilder Alignment(Alignment alignment)
   {
      formatter.Alignment(alignment);
      return this;
   }

   public RowBuilder ForegroundColor(ForegroundColorDescriptor foregroundColor)
   {
      formatter.ForegroundColor(foregroundColor);
      return this;
   }

   public RowBuilder BackgroundColor(BackgroundColorDescriptor backgroundColor)
   {
      if (table.CurrentCell is (true, var currentCell))
      {
         currentCell.BackgroundColor = backgroundColor;
      }
      else
      {
         formatter.BackgroundColor(backgroundColor);
      }

      return this;
   }

   public RowBuilder Hyperlink(Hyperlink hyperlink)
   {
      formatter.Hyperlink(hyperlink);
      return this;
   }

   public RowBuilder Font(FontDescriptor font)
   {
      formatter.Font(font);
      return this;
   }

   public RowBuilder FontSize(float fontSize)
   {
      formatter.FontSize(fontSize);
      return this;
   }

   public RowBuilder FirstLineIndent(FirstLineIndent firstLineIndent)
   {
      formatter.FirstLineIndent(firstLineIndent);
      return this;
   }

   public RowBuilder Margins((Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      formatter.Margins(margins);
      return this;
   }

   public RowBuilder Style(Style style)
   {
      formatter.Style(style);
      return this;
   }
}