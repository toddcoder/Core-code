using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
   public class Signature
   {
      public const string REGEX_FORMAT = "^ /(/w+) ('[' /(/d*) ']')? $; f";

      public Signature(string signature)
      {
         if (signature.Find("[").If(out var openIndex))
         {
            Name = signature.Keep(openIndex);
            Index = signature.Drop(openIndex + 1).KeepUntil("]").AsInt();
         }
         else
         {
            Name = signature;
            Index = none<int>();
         }
      }

      public string Name { get; set; }

      public IMaybe<int> Index { get; set; }

      public override string ToString() => Index.Map(i => $"{Name}[{i}]").DefaultTo(() => Name);
   }
}