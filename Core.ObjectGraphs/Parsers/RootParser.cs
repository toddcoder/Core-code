using Core.Collections;
using Core.Strings;

namespace Core.ObjectGraphs.Parsers
{
   public class RootParser
   {
      string name;
      string type;
      string key;
      GroupParser groupParser;
      SingleParser singleParser;
      SingleCharacterParser singleCharacterParser;
      ReplacementParser replacementParser;
      MacroParser macroParser;
      ArrayItemParser arrayItemParser;
      Replacer replacer;

      public RootParser(string name, Replacer replacer, string type = "", string key = "")
      {
         BaseParser.TabCount++;
         this.name = name;
         this.type = type;
         this.key = key;
         groupParser = new GroupParser();
         singleParser = new SingleParser();
         singleCharacterParser = new SingleCharacterParser();
         replacementParser = new ReplacementParser();
         macroParser = new MacroParser();
         arrayItemParser = new ArrayItemParser();
         this.replacer = replacer;
      }

      public int Position { get; set; }

      public ObjectGraph Parse(string[] source, int position = 0)
      {
         singleParser.Source = source;
         groupParser.Source = source;
         singleCharacterParser.Source = source;
         replacementParser.Source = source;
         macroParser.Source = source;
         arrayItemParser.Source = source;

         var result = new ObjectGraph(name, "", type, key);
         var length = source.Length;
         while (position < length)
         {
            if (source[position].IsEmpty())
            {
               position++;
               continue;
            }

            var lineNumber = position + 1;
            var lineSource = source[position];
            if (singleParser.Parse(source, position, replacer))
            {
               position = update(singleParser, result, lineNumber, lineSource);
            }
            else if (singleCharacterParser.Parse(source, position, replacer))
            {
               position = update(singleCharacterParser, result, lineNumber, lineSource);
            }
            else if (macroParser.Parse(source, position, replacer))
            {
               position = macroParser.Position;
            }
            else if (replacementParser.Parse(source, position, replacer))
            {
               position = replacementParser.Position;
            }
            else if (arrayItemParser.Parse(source, position, replacer))
            {
               position = update(arrayItemParser, result, lineNumber, lineSource);
            }
            else if (groupParser.Parse(source, position, replacer))
            {
               position = update(groupParser, result, lineNumber, lineSource);
            }
            else if (BaseParser.TabCount > -1)
            {
               BaseParser.TabCount--;
               position--;
               break;
            }
            else
            {
               break;
            }
         }

         Position = position;
         return result;
      }

      static int update(BaseParser parser, ObjectGraph result, int lineNumber, string lineSource)
      {
         var objectGraph = parser.Result;
         objectGraph.LineNumber = lineNumber;
         objectGraph.LineSource = lineSource.Trim();
         objectGraph.TabCount = BaseParser.TabCount;
         if (result.If(objectGraph.Key, out _))
         {
            objectGraph.Key = Replacer.Key(objectGraph.Name);
         }

         result[objectGraph.Key] = objectGraph;

         return parser.Position;
      }
   }
}