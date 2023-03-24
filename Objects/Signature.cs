using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Objects;

public class Signature
{
   public const string REGEX_FORMAT = "^ /(/w+) ('[' /(/d*) ']')? $; f";

   public Signature(string signature)
   {
      var _openIndex = signature.Find("[");
      if (_openIndex)
      {
         Name = signature.Keep(_openIndex);
         Index = Maybe.Int32(signature.Drop(_openIndex + 1).KeepUntil("]"));
      }
      else
      {
         Name = signature;
         Index = nil;
      }
   }

   public string Name { get; set; }

   public Optional<int> Index { get; set; }

   public override string ToString() => Index.Map(i => $"{Name}[{i}]") | Name;
}