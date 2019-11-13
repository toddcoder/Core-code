using Core.Monads;

namespace Core.ObjectGraphs.Parsers
{
   public class GroupParser : BaseParser
   {
      public override string Pattern => $"^ {{sp}} {REGEX_NAME}{REGEX_TYPE}$";

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

         var rootParser = new RootParser(name, replacer, type, key);
         Result = rootParser.Parse(Source, position + 1);
         var index = rootParser.Position;
         overridePosition = (index + 1).Some();

         return true;
      }
   }
}