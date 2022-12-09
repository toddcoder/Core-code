using System.Collections.Generic;
using System.Linq;
using Core.Collections;

namespace Core.Applications;

public static class ArgumentsExtensions
{
   public static Hash<string, string> Switches(this IEnumerable<Argument> arguments, string pattern, string keyReplacement,
      string valueReplacement = "$0")
   {
      return new Arguments(arguments.ToArray()).Switches(pattern, keyReplacement, valueReplacement);
   }
}