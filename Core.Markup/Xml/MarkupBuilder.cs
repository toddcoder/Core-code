using System;
using System.Text;
using System.Xml;
using Core.Assertions;
using Core.Computers;
using Core.DataStructures;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Markup.Xml
{
   public class MarkupBuilder
   {
      public static Result<MarkupBuilder> HtmlFromString(string html)
      {
         try
         {
            html.Must().Not.BeNullOrEmpty().OrThrow();

            var document = new XmlDocument();
            document.LoadXml(html);
            using var nodeReader = new XmlNodeReader(document);
            var settings = new XmlReaderSettings
            {
               ValidationType = ValidationType.None,
               IgnoreWhitespace = true
            };
            using var reader = XmlReader.Create(nodeReader, settings);

            var builder = new MarkupBuilder("html");
            var element = builder.Root;
            var elementStack = new MaybeStack<Element>();

            while (reader.Read())
            {
               switch (reader.NodeType)
               {
                  case XmlNodeType.Element when reader.Value != "html":
                     elementStack.Push(element);
                     element = new Element
                     {
                        Name = reader.Name
                     };
                     break;
                  case XmlNodeType.Text:
                     element.Text = reader.Value;
                     break;
                  case XmlNodeType.EndElement when reader.Value != "html":
                     if (elementStack.Pop().If(out element))
                     {
                     }
                     else
                     {
                        return fail("Uneven elements");
                     }

                     break;
                  case XmlNodeType.Attribute:
                  {
                     var attribute = new Attribute(reader.Name, reader.Value, QuoteType.Double);
                     element.Attributes[attribute.Name] = attribute;
                     break;
                  }
               }
            }

            return builder;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

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

      public override string ToString() => ToStringRendering(_ => true);

      public void RenderToFile(FileName file) => RenderToFile(file, _ => true);

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

      public string ToPlainText() => string.Empty;
   }
}