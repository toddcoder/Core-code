using Core.Objects;
using Core.RegularExpressions;

namespace Core.Strings
{
   public class ObjectFormatter
   {
      protected const string REGEX_NAME = "-(< '//') '{' /([/w '-']+) /([',:']+ -['}']+)? '}'";

      protected PropertyEvaluator evaluator;

      public ObjectFormatter(object obj) => evaluator = new PropertyEvaluator(obj);

      public string Format(string source) => source.Matcher(REGEX_NAME).Map(matcher =>
      {
         for (var i = 0; i < matcher.MatchCount; i++)
         {
            var name = matcher[i, 1].ToCamel();
            var format = matcher[i, 2];
            if (evaluator.ContainsKey(name))
            {
               var obj1 = evaluator[name];
               matcher[i, 0] = obj1 is null ? "" : string.Format("{{0" + format + "}}", obj1);
            }
         }

         return matcher.ToString();
      }).DefaultTo(() => "");
   }
}