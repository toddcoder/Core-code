using System;
using System.Text;
using Core.Computers;
using Core.Monads;
using static Core.Monads.MonadFunctions;
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
         var html = builder.Root.Required("Root was not set");
         if (includeHead)
         {
            html.Children.Add("head");
         }

         html.Children.Add("body");

         return builder;
      }

      bool tidy;
      Encoding encoding;
      bool includeHeader;
      QuoteType quote;
      IMaybe<Element> root;
      bool isHtml;
      DocType docType;

      public SgmlBuilder(string rootName)
      {
         tidy = true;
         encoding = Encoding.UTF8;
         includeHeader = false;
         quote = QuoteType.Double;
         root = new Element { Name = rootName }.Some();
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

      public IMaybe<Element> Root => root;

      public bool IsHtml
      {
         get => isHtml;
         set
         {
            isHtml = value;
            var setRoot = root.Required("Root must be set");
            setRoot.Children.IsHtml = value;
            setRoot.Siblings.IsHtml = value;
         }
      }

      public char QuoteChar => quote == QuoteType.Double ? '"' : '\'';

      public void Clear() => root = none<Element>();

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
         if (root.If(out var r))
         {
            var result = new StringBuilder();
            addDocType(result);
            result.Append(r.ToStringRendering(callback));
            if (tidy)
            {
               return result.ToString().Tidy(encoding, includeHeader, QuoteChar);
            }
            else
            {
               return result.ToString();
            }
         }
         else
         {
            return "";
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

         if (root.If(out var r))
         {
            if (docType != DocType.None)
            {
               tempFile.Append(newDocType.ToString());
            }

            r.RenderToFile(tempFile, callback);
         }
         else
         {
            tempFile.Text = "";
         }

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