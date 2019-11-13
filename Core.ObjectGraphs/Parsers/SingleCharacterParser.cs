using Core.Strings;

namespace Core.ObjectGraphs.Parsers
{
   public class SingleCharacterParser : BaseParser
   {
      public override string Pattern => $"^ {{sp}} {REGEX_NAME}{REGEX_TYPE}/s* /(['=+,.-']) $";

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

         var character = tokens[3];
         string value;
         switch (character)
         {
            case "=":
               value = matcher.IsMatch(name, "^ ['@$.'] /(.+) $") ? matcher[0, 1] : name;
               value = value.ToPascal();
               break;
            case "+":
               value = "true";
               break;
            case "-":
               value = "false";
               break;
            case ",":
            case ".":
               value = "";
               break;
            default:
               return false;
         }

         Result = new ObjectGraph(name, value, type, key);
         return true;
      }
   }
}