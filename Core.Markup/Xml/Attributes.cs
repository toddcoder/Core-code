using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;

namespace Core.Markup.Xml;

public class Attributes : IEnumerable<Attribute>
{
   protected StringHash<Attribute> attributes;

   public Attributes()
   {
      attributes = new StringHash<Attribute>(true);
      Quote = QuoteType.Double;
   }

   public QuoteType Quote { get; set; }

   public Attribute Add(string name, string text)
   {
      var attribute = new Attribute(name, text, Quote);
      attributes[name] = attribute;

      return attribute;
   }

   public bool Contains(string name) => attributes.ContainsKey(name);

   public IEnumerator<Attribute> GetEnumerator() => attributes.Select(attribute => attribute.Value).GetEnumerator();

   public Attribute this[string name]
   {
      get => attributes[name];
      set => attributes[name] = value;
   }

   public override string ToString()
   {
      return attributes.Count != 0 ? $" {attributes.Select(i => i.Value.ToString()).ToString(" ")}" : string.Empty;
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}