using Core.Objects;
using Core.RegexMatching;

namespace Core.Strings
{
   public class ObjectFormatter
   {
      protected const string REGEX_NAME = "-(< '//') '{' /([/w '-']+) /([',:']+ -['}']+)? '}'; f";

      protected PropertyEvaluator evaluator;

      public ObjectFormatter(object obj) => evaluator = new PropertyEvaluator(obj);

      public string Format(string source) => source.Matches(REGEX_NAME).Map(result =>
      {
         for (var i = 0; i < result.MatchCount; i++)
         {
            var name = result[i, 1].ToCamel();
            var format = result[i, 2];
            if (evaluator.ContainsKey(name))
            {
               var obj1 = evaluator[name];
               result[i, 0] = obj1 is null ? "" : string.Format("{{0" + format + "}}", obj1);
            }
         }

         return result.ToString();
      }).DefaultTo(() => "");
   }
}