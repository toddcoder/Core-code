using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;

namespace Core.Objects
{
   public class Signature
   {
      public const string REGEX_FORMAT = "^ /(/w+) ('[' /(/d*) ']')? $";

      public Signature(string signature)
      {
         var matcher = new Matcher();

         matcher.RequiredMatch(signature, REGEX_FORMAT, $"Couldn't determine elements of signature \"{signature}\"");

         Name = matcher.FirstGroup;
         Index = matcher.SecondGroup.AsInt();
      }

      public string Name { get; set; }

      public IMaybe<int> Index { get; set; }

      public override string ToString() => Index.Map(i => $"{Name}[{i}]").DefaultTo(() => Name);
   }
}