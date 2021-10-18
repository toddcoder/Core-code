using System;
using System.Text;
using Core.Computers;
using Core.Strings;

namespace Core.Markup.Xml
{
   public class Element : IRendering
   {
      protected string name;
      protected MarkupTextHolder text;
      protected Element parent;
      protected Elements siblings;
      protected Elements children;
      protected Attributes attributes;

      public Element()
      {
         name = "no-name";
         text = string.Empty;
         parent = null;
         siblings = new Elements();
         siblings.ElementAdded += (_, e) => e.Element.Parent = parent;
         children = new Elements();
         children.ElementAdded += (_, e) => e.Element.Parent = this;
         attributes = new Attributes();
      }

      public Element this[string elementName] => Children[elementName];

      public Element this[int index] => Children[index];

      public string Name
      {
         get => name;
         set => name = value;
      }

      public MarkupTextHolder Text
      {
         get => text;
         set => text = value;
      }

      public Element Parent
      {
         get => parent;
         set => parent = value;
      }

      public Elements Siblings => siblings;

      public Elements Children => children;

      public Attributes Attributes => attributes;

      public override string ToString() => ToStringRendering(_ => true);

      public virtual string ToStringRendering(Func<Element, bool> callback)
      {
         if (callback(this))
         {
            var element = new StringBuilder("<");

            element.Append(name);

            element.Append(attributes);

            var closed = children.IsEmpty && text.Text.IsEmpty();
            element.Append(closed ? " />" : ">");

            if (text.Text.IsNotEmpty())
            {
               element.Append(text.Text);
            }

            element.Append(children.ToStringRendering(callback));

            if (!closed)
            {
               element.Append($"</{name}>");
            }

            element.Append(siblings.ToStringRendering(callback));

            return element.ToString();
         }
         else
         {
            return string.Empty;
         }
      }

      public virtual void RenderToFile(FileName file) => RenderToFile(file, _ => true);

      public virtual void RenderToFile(FileName file, Func<Element, bool> callback)
      {
         if (callback(this))
         {
            file.Append("<");
            file.Append(name);

            file.Append(attributes.ToString());

            var closed = children.IsEmpty && text.Text.IsEmpty();
            file.Append(closed ? " />" : ">");

            if (text.Text.IsNotEmpty())
            {
               file.Append(text.Text);
            }

            children.RenderToFile(file, callback);

            if (!closed)
            {
               file.Append($"</{name}>");
            }

            siblings.RenderToFile(file, callback);
         }
      }
   }
}