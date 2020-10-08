using Core.RegularExpressions;
using Core.Strings;

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
         var key = anyReference.Map(r => r.Key).DefaultTo(() => "");
         if (anyReference.If(out var reference))
         {
            name = reference.Name;
         }

         var value = tokens[3];
         if (!value.IsMatch("'//(' /s* ')'"))
         {
            value = value.TrimEnd();
         }

         if (value.StartsWith("[") && value.EndsWith("]"))
         {
            value = value.Drop(1).Drop(-1).Trim();
            value = replacer.Replace(value);
            var values = value.Split("/s* ',' /s*");
            var result = new ObjectGraph(name, "", type, key);
            for (var i = 0; i < values.Length; i++)
            {
               var itemGraph = new ObjectGraph(i.ToString(), values[i], key: i.ToString());
               result[itemGraph.Key] = itemGraph;
            }

            Result = result;

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