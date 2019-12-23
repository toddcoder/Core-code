using System;
using System.IO;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Numbers;
using Core.RegularExpressions;
using static Core.Assertions.AssertionFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Strings
{
	public class Formatter
	{
		const string REGEX_NAME = "-(< '//') '{' /([/w '-']+) /([',:']+ -['}']+)? '}'";

		public static Formatter WithStandard(bool includeFolders)
		{
			var formatter = new Formatter();
			addStandard(formatter, includeFolders);

			return formatter;
		}

		public static Formatter WithStandard(bool includeFolders, Hash<string, string> initializers)
		{
			var formatter = new Formatter(initializers);
			addStandard(formatter, includeFolders);

			return formatter;
		}

		static void addStandard(Formatter formatter, bool includeFolders)
		{
			formatter["guid"] = guid();
			formatter["now"] = DateTime.Now.ToString();
			formatter["uid"] = uniqueID();
			formatter["ulid"] = uniqueID();
			if (includeFolders)
			{
				formatter["docs"] = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				formatter["windir"] = Environment.GetEnvironmentVariable("windir");
				formatter["tempdir"] = Path.GetTempPath();
				formatter["system32"] = Environment.GetFolderPath(Environment.SpecialFolder.System);
				formatter["progs"] = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				formatter["configs"] = @"C:\Enterprise\Configurations";
				formatter["projects"] = @"C:\Enterprise\Projects";
				formatter["enterprise"] = @"C:\Enterprise";
				formatter["services"] = @"C:\Enterprise\Services";
			}
		}

		static string format(string source, bool addStandard, bool includeFolders, params string[] args)
      {
         assert(() => args.Length).Must().BeEven().OrThrow();

			var formatter = addStandard ? WithStandard(includeFolders) : new Formatter();
			for (var i = 0; i < args.Length; i += 2)
			{
				formatter[args[i]] = args[i + 1];
			}

			return formatter.Format(source);
		}

		public static string Format(string source, params string[] args) => format(source, false, false, args);

		public static string FormatWithStandard(string source, bool includeFolders, params string[] args)
		{
			return format(source, true, includeFolders, args);
		}

		public static string[] NamesInString(string source)
		{
			if (source.IsNotEmpty())
			{
				var matcher = new Matcher();
				if (matcher.IsMatch(source, REGEX_NAME, true, true))
				{
					return 0.Until(matcher.MatchCount).Select(i => matcher[i, 1]).ToArray();
				}
				else
				{
					return new string[0];
				}
			}
			else
			{
				return new string[0];
			}
		}

		protected Hash<string, string> names;

		public Formatter() => names = new AutoHash<string, string>(n => "");

		public Formatter(Hash<string, string> initializers) => names = initializers;

		public Formatter(Formatter formatter)
		{
			foreach (var item in formatter.names)
			{
				names[item.Key] = item.Value;
			}
		}

		public string this[string name]
		{
			get => names[name];
			set => names[name] = value;
		}

		public string[] Names => names.Select(item => item.Key).ToArray();

		public string[] Values => names.Select(item => item.Value).ToArray();

		public Hash<string, string> Replacements => names;

		public virtual string Format(string source)
		{
			if (source.IsNotEmpty())
			{
				var matcher = new Matcher();
				matcher.Evaluate(source, REGEX_NAME, true, true);
				for (var i = 0; i < matcher.MatchCount; i++)
				{
					var name = matcher[i, 1];
					if (names.ContainsKey(name))
					{
						matcher[i] = getText(name, matcher[i, 2]);
					}
				}

				return matcher.ToString().Replace("/{", "{");
			}
			else
			{
				return "";
			}
		}

		protected virtual string getText(string name, string format)
		{
			if (names.If(name, out var text))
			{
				if (format.IsNotEmpty())
				{
					var anObject = text.ToObject();
					if (anObject.If(out var obj))
					{
						text = string.Format($"{{0{format}}}", obj);
					}
				}

				return text;
			}
			else
			{
				return "";
			}
		}

		public void Merge(Formatter formatter, bool overwriteOriginalValues)
		{
			if (overwriteOriginalValues)
			{
				foreach (var item in formatter.names)
				{
					names[item.Key] = item.Value;
				}
			}
			else
			{
				foreach (var item in formatter.names)
				{
					var name = item.Key;
					if (!names.ContainsKey(name))
					{
						names[name] = item.Value;
					}
				}
			}
		}

		public virtual bool ContainsName(string name) => names.ContainsKey(name);
	}
}