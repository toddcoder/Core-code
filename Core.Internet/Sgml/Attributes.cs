using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;

namespace Core.Internet.Sgml
{
   public class Attributes : IEnumerable<Attribute>
   {
      Hash<string, Attribute> attributes;

      public Attributes() => attributes = new Hash<string, Attribute>();

      public QuoteType Quote { get; set; } = QuoteType.Double;

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
         return attributes.Count != 0 ? $" {attributes.Select(i => i.Value.ToString()).ToString(" ")}" : "";
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}