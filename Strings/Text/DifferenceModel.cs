using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Collections;

namespace Core.Strings.Text
{
   public class DifferenceModel
   {
      public DifferenceModel()
      {
         OldDifferenceItems = new List<DifferenceItem>();
         NewDifferenceItems = new List<DifferenceItem>();
      }

      public List<DifferenceItem> OldDifferenceItems { get; }

      public IEnumerable<Difference> OldDifferences()
      {
         return OldDifferenceItems
            .Where(i => i.Type != DifferenceType.Unchanged && i.Type != DifferenceType.Imaginary)
            .Select(Difference.FromDifferenceItem);
      }

      public List<DifferenceItem> NewDifferenceItems { get; }

      public IEnumerable<Difference> NewDifferences()
      {
         return NewDifferenceItems
            .Where(i => i.Type != DifferenceType.Unchanged && i.Type != DifferenceType.Imaginary)
            .Select(Difference.FromDifferenceItem);
      }

      public IEnumerable<string> MergedDifferences()
      {
         var oldHash = OldDifferences().ToHash(d => d.Position, d => d.ToString());
         var newHash = NewDifferences().ToHash(d => d.Position, d => d.ToString());
         var keys = new Set<int>();
         keys.AddRange(oldHash.Keys.Where(k => k > -1));
         keys.AddRange(newHash.Keys.Where(k => k > -1));

         var builder = new StringBuilder();

         foreach (var key in keys.OrderBy(k => k))
         {
            if (oldHash.ContainsKey(key))
            {
               builder.Append(oldHash[key]);
            }

            if (newHash.ContainsKey(key))
            {
               builder.Append(" <=> ");
               builder.Append(newHash[key]);
            }

            yield return builder.ToString();

            builder.Clear();
         }
      }

      public override string ToString()
      {
         using (var writer = new StringWriter())
         {
            writer.WriteLine("old:");
            foreach (var item in OldDifferenceItems)
            {
               writer.WriteLine(item);
            }

            writer.WriteLine();

            writer.WriteLine("new:");
            foreach (var item in NewDifferenceItems)
            {
               writer.WriteLine(item);
            }

            return writer.ToString();
         }
      }
   }
}