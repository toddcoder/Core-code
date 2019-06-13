using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Exceptions;
using Core.Monads;
using Core.Numbers;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications
{
	public class Arguments : IEnumerable<Argument>
	{
		static string[] splitArguments(string arguments)
		{
			var destringifier = new Destringifier(arguments);
			return destringifier.Parse().Split("/s+").Select(s => destringifier.Restring(s, false)).ToArray();
		}

		Argument[] arguments;
		string[] originalArguments;
		int length;

		public Arguments(string[] arguments)
		{
			originalArguments = arguments;
			length = originalArguments.Length;
			this.arguments = new Argument[length];
			for (var i = 0; i < length; i++)
				this.arguments[i] = new Argument(originalArguments[i], i);
		}

		public Arguments(string arguments) : this(splitArguments(arguments)) { }

		public Arguments()
		{
			originalArguments = new string[0];
			arguments = new Argument[0];
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
				throw $"Expected exact count of {exactCount}".Throws();
		}

		public void AssertCount(int minimumCount, int maximumCount)
		{
			if (!length.Between(minimumCount).And(maximumCount))
				$"Count must between {minimumCount} and {maximumCount}--found {length}".Throws();
		}

		public void AssertMinimumCount(int minimumCount)
		{
			if (length < minimumCount)
				throw $"Count must be at least {minimumCount}--found {length}".Throws();
		}

		public void AssertMaximumCount(int maximumCount)
		{
			if (length > maximumCount)
				throw $"Count must be at most {maximumCount}--found {length}".Throws();
		}

		public IMaybe<Argument> Argument(int index) => when(Exists(index), () => arguments[index]);

		public IEnumerator<Argument> GetEnumerator()
		{
			foreach (var argument in arguments)
				yield return argument;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public Hash<string, string> Switches(string pattern, string keyReplacement = "$0", string valueReplacement = "$1")
		{
			var result = new Hash<string, string>();

			foreach (var text in arguments.Select(argument => argument.Text))
				if (text.IsMatch(pattern, true))
				{
					var key = text.Substitute(pattern, keyReplacement, true);
					var value = text.Substitute(pattern, valueReplacement, true);
					result[key] = value;
				}

			return result;
		}

		public ArgumentsTrying TryTo => new ArgumentsTrying(this);
	}
}