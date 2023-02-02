using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Assertions;
using Core.Strings;

namespace Core.Matching.Parsers;

public class Parser
{
   public static string ToFriendly(string source)
   {
      var parser = new Parser();
      return parser.Parse(source);
   }

   protected List<BaseParser> parsers;

   public Parser() => parsers = new List<BaseParser>
   {
      new StringParser(),
      new CommentParser(),
      new SpanBreakParser(),
      new OverrideParser(),
      new RemainderParser(),
      new GroupReferenceParser(),
      new SlashClassParser(),
      new NamedBackReferenceParser(),
      new OptionGroupParser(),
      new NamedCapturingGroupParser(),
      new CapturingGroupParser(),
      new LookAroundParser(),
      new AtomicGroupParser(),
      new ConditionalParser(),
      new NonCapturingGroupParser(),
      new OutsideRangeParser(),
      new ClassParser(),
      new QuoteParser(),
      new NumericQuantificationParser(),
      new NumericQuantification2Parser(),
      new UnmodifiedParser()
   };

   public string Parse(string source)
   {
      source = source.Trim();
      var index = 0;
      var content = new StringBuilder();

      while (index < source.Length)
      {
         var added = false;
         foreach (var _result in parsers
                     .Select(parser => parser.Scan(source, ref index))
                     .Where(result => result))
         {
            if (_result is (true, var result))
            {
               content.Append(result);
            }

            added = true;
            break;
         }

         added.Must().BeTrue().OrThrow($"Didn't recognize {source.Drop(index)}");
      }

      return content.ToString();
   }
}