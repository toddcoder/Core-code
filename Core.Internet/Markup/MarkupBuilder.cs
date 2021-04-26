using System;
using System.Text;
using Core.Assertions;
using Core.Computers;
using static Core.Strings.StringFunctions;

namespace Core.Internet.Markup
{
   public class MarkupBuilder
   {
      public enum DocType
      {
         None,
         Strict,
         Transitional,
         FrameSet
      }

      public static MarkupBuilder AsHtml(bool includeHead)
      {
         var builder = new MarkupBuilder("html") { IsHtml = true, IncludeHeader = false, Tidy = false };
         if (includeHead)
         {
            builder.Root.Children.Add("head");
         }

         builder.Root.Children.Add("body");

         return builder;
      }

      protected bool tidy;
      protected Encoding encoding;
      protected bool includeHeader;
      protected QuoteType quote;
      protected Element root;
      protected bool isHtml;
      protected DocType docType;

      public MarkupBuilder(string rootName)
      {
         rootName.Must().Not.BeNullOrEmpty().OrThrow();

         tidy = true;
         encoding = Encoding.UTF8;
         includeHeader = false;
         quote = QuoteType.Double;
         root = new Element { Name = rootName };
         isHtml = false;
         docType = DocType.None;
      }

      public bool Tidy
      {
         get => tidy;
         set => tidy = value;
      }

      public Encoding Encoding
      {
         get => encoding;
         set => encoding = value;
      }

      public bool IncludeHeader
      {
         get => includeHeader;
         set => includeHeader = value;
      }

      public QuoteType Quote
      {
         get => quote;
         set => quote = value;
      }

      public DocType DocumentType
      {
         get => docType;
         set => docType = value;
      }

      public Element Root => root;

      public bool IsHtml
      {
         get => isHtml;
         set
         {
            isHtml = value;
            root.Children.IsHtml = value;
            root.Siblings.IsHtml = value;
         }
      }

      public char QuoteChar => quote == QuoteType.Double ? '"' : '\'';

      protected void addDocType(StringBuilder result)
      {
         if (docType != DocType.None)
         {
            result.Append("<!DOCTYPE html PUBLIC \"-//DTD XHTML 1.0 ");
            result.Append(docType);
            result.Append("//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-");
            result.Append(docType.ToString().ToLower());
            result.Append(".dtd\">");
         }
      }

      public string ToStringRendering(Func<Element, bool> callback)
      {
         var result = new StringBuilder();
         addDocType(result);
         result.Append(root.ToStringRendering(callback));
         var asString = result.ToString();

         return tidy ? asString.Tidy(encoding, includeHeader, QuoteChar) : asString;
      }

      public override string ToString() => ToStringRendering(element => true);

      public void RenderToFile(FileName file) => RenderToFile(file, element => true);

      public void RenderToFile(FileName file, Func<Element, bool> callback)
      {
         var tempFile = file.Folder.File(guid(), ".xml");
         tempFile.Text = string.Empty;
         tempFile.Hidden = true;
         tempFile.BufferSize = 512;

         var newDocType = new StringBuilder();
         addDocType(newDocType);

         if (docType != DocType.None)
         {
            tempFile.Append(newDocType.ToString());
         }

         root.RenderToFile(tempFile, callback);

         tempFile.Flush();

         if (tidy)
         {
            file.Text = tempFile.Text.Tidy(encoding, includeHeader, QuoteChar);
            tempFile.Delete();
         }
         else
         {
            tempFile.MoveTo(file);
         }
      }
   }
}