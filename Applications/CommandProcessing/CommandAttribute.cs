using System;
using System.Linq;
using Core.Collections;
using Core.Monads;
using Core.Matching;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute, IHash<string, string>
{
   protected static StringHash getReplacements(string source)
   {
      var hash = new StringHash(true);

      var items = source.Unjoin(@"/s* -(< '\') ';' /s*").Select(i => i.Replace(@"\;", ";")).ToArray();
      foreach (var item in items)
      {
         var _result = item.Matches("^ /(-[':']+) ':' /s* /(.+) $");
         if (_result)
         {
            var (key, value) = ~_result;
            hash[key.TrimEnd()] = value;
         }
      }

      return hash;
   }

   protected StringHash replacements;

   protected CommandAttribute(string name, Maybe<string> helpText, Maybe<string> switchPattern, bool initialize = true, string replacements = "")
   {
      Name = name;
      HelpText = helpText;
      SwitchPattern = switchPattern;
      Initialize = initialize;

      this.replacements = getReplacements(replacements);
   }

   public CommandAttribute(string name, bool initialize = true, string replacements = "") : this(name, nil, nil, initialize, replacements)
   {
   }

   public CommandAttribute(string name, string helpText, bool initialize = true, string replacements = "") : this(name, helpText, nil, initialize,
      replacements)
   {
   }

   public CommandAttribute(string name, string helpText, string switchPattern, bool initialize = true, string replacements = "") :
      this(name, helpText.Some(), switchPattern.Some(), initialize, replacements)
   {
   }

   public string Name { get; }

   public Maybe<string> HelpText { get; }

   public Maybe<string> SwitchPattern { get; }

   public bool Initialize { get; }

   public string this[string key]
   {
      get => replacements[key];
      set => replacements[key] = value;
   }

   public bool ContainsKey(string key) => replacements.ContainsKey(key);

   public Result<Hash<string, string>> AnyHash() => replacements;
}