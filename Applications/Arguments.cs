using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications;

public class Arguments : IEnumerable<Argument>
{
   protected static string[] splitArguments(string arguments)
   {
      var delimitedText = DelimitedText.BothQuotes();
      var destringified = delimitedText.Destringify(arguments.Replace(@"\", @"\\"));

      return destringified.Unjoin("/s+; f").Select(s => delimitedText.Restringify(s, RestringifyQuotes.None)).ToArray();
   }

   protected Argument[] arguments;
   protected string[] originalArguments;
   protected int length;

   public Arguments(string[] arguments)
   {
      originalArguments = arguments;
      length = originalArguments.Length;
      this.arguments = new Argument[length];
      for (var i = 0; i < length; i++)
      {
         this.arguments[i] = new Argument(originalArguments[i], i);
      }
   }

   public Arguments(string arguments) : this(splitArguments(arguments))
   {
   }

   public Arguments()
   {
      originalArguments = Array.Empty<string>();
      arguments = Array.Empty<Argument>();
   }

   internal Arguments(Argument[] arguments)
   {
      this.arguments = arguments;
      originalArguments = arguments.Select(a => a.Text).ToArray();
      length = this.arguments.Length;
   }

   public Argument this[int index] => arguments[index];

   public string[] OriginalArguments => originalArguments;

   public int Count => length;

   public bool Exists(int index) => index.Between(0).Until(length);

   public void AssertCount(int exactCount)
   {
      if (length != exactCount)
      {
         throw fail($"Expected exact count of {exactCount}");
      }
   }

   public void AssertCount(int minimumCount, int maximumCount)
   {
      if (!length.Between(minimumCount).And(maximumCount))
      {
         throw fail($"Count must between {minimumCount} and {maximumCount}--found {length}");
      }
   }

   public void AssertMinimumCount(int minimumCount)
   {
      if (length < minimumCount)
      {
         throw fail($"Count must be at least {minimumCount}--found {length}");
      }
   }

   public void AssertMaximumCount(int maximumCount)
   {
      if (length > maximumCount)
      {
         throw fail($"Count must be at most {maximumCount}--found {length}");
      }
   }

   public Maybe<Argument> Argument(int index) => maybe(Exists(index), () => arguments[index]);

   public IEnumerator<Argument> GetEnumerator()
   {
      foreach (var argument in arguments)
      {
         yield return argument;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public Hash<string, string> Switches(Pattern pattern, string keyReplacement = "$0", string valueReplacement = "$1")
   {
      var result = new Hash<string, string>();

      foreach (var text in arguments.Select(argument => argument.Text))
      {
         if (text.IsMatch(pattern))
         {
            var key = text.Substitute(pattern, keyReplacement);
            var value = text.Substitute(pattern, valueReplacement);
            result[key] = value;
         }
      }

      return result;
   }

   public ArgumentsTrying TryTo => new(this);
}