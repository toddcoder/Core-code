using System;
using System.Text;
using Core.Assertions;
using Core.Computers;
using static Core.Strings.StringFunctions;

namespace Core.Internet.Sgml
{
   public class SgmlBuilder
   {
      public enum DocType
      {
         None,
         Strict,
         Transitional,
         FrameSet
      }

      public static SgmlBuilder AsHtml(bool includeHead)
      {
         var builder = new SgmlBuilder("html") { IsHtml = true, IncludeHeader = false, Tidy = false };
         if (includeHead)
         {
            builder.Root.Children.Add("head");
         }

         builder.Root.Children.Add("body");

         return builder;
      }

      bool tidy;
      Encoding encoding;
      bool includeHeader;
      QuoteType quote;
      Element root;
      bool isHtml;
      DocType docType;

      public SgmlBuilder(string rootName)
      {
         rootName.MustAs(nameof(rootName)).Not.BeNullOrEmpty().Assert();

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

      void addDocType(StringBuilder result)
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
         if (tidy)
         {
            return result.ToString().Tidy(encoding, includeHeader, QuoteChar);
         }
         else
         {
            return result.ToString();
         }
      }

      public override string ToString() => ToStringRendering(element => true);

      public void RenderToFile(FileName file) => RenderToFile(file, element => true);

      public void RenderToFile(FileName file, Func<Element, bool> callback)
      {
         var tempFile = file.Folder.File(guid(), ".xml");
         tempFile.Text = "";
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