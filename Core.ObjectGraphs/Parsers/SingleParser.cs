using Core.Assertions;
using Core.Computers;
using Core.RegularExpressions;

namespace Core.ObjectGraphs.Parsers
{
   public class SingleParser : BaseParser
   {
      public override string Pattern => $"^ {{sp}} {REGEX_NAME}{REGEX_TYPE} /s* {REGEX_STEM} /s* {REGEX_VALUE} $";

      public override bool Scan(string[] tokens, int position, Replacer replacer)
      {
         var name = tokens[1];
         name = replacer.Replace(name);
         var type = tokens[2];
         type = replacer.Replace(type);
         var anyReference = Replacer.ResolveAsterisk(name);
         var key = anyReference.FlatMap(r => r.Key, () => "");
         if (anyReference.If(out var reference))
         {
            name = reference.Name;
         }

         var value = tokens[3];
         if (!value.IsMatch("'//(' /s* ')'"))
         {
            value = value.TrimEnd();
         }

         if (value.StartsWith("@"))
         {
            value = value.Substring(1);
            value = replacer.Replace(value);
            FileName file = value;

            file.Must().Exist().Assert();

            var graph = ObjectGraph.FromFile(file);
            graph.SetName(name);
            Result = graph;

            return true;
         }
         else
         {
            value = replacer.Replace(value);
            value = value.TrimEnd(' ');
            Result = new ObjectGraph(name, value, type, key);

            return true;
         }
      }
   }
}