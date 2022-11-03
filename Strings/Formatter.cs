using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using static Core.Strings.StringFunctions;

namespace Core.Strings;

public class Formatter : IHash<string, string>
{
   protected const string REGEX_NAME = "/('//'0%2) '{' /([/w '.-']+) /([',:']+ -['}']+)? '}'; fim";

   public static Formatter WithStandard(bool includeFolders)
   {
      var formatter = new Formatter();
      addStandard(formatter, includeFolders);

      return formatter;
   }

   public static Formatter WithStandard(bool includeFolders, Dictionary<string, string> initializers)
   {
      var formatter = new Formatter(initializers);
      addStandard(formatter, includeFolders);

      return formatter;
   }

   protected static void addStandard(Formatter formatter, bool includeFolders)
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
      }
   }

   protected static string format(string source, bool addStandard, bool includeFolders, params string[] args)
   {
      args.Length.Must().BeEven().OrThrow();

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
         var _result = source.Matches(REGEX_NAME);
         if (_result)
         {
            var result = ~_result;
            return 0.Until(result.MatchCount).Select(i => result[i, 1]).ToArray();
         }
         else
         {
            return Array.Empty<string>();
         }
      }
      else
      {
         return Array.Empty<string>();
      }
   }

   protected AutoStringHash names;

   public Formatter() => names = new AutoStringHash(true, _ => string.Empty);

   public Formatter(Dictionary<string, string> initializers) => names = new AutoStringHash(true, initializers);

   public Formatter(Formatter formatter)
   {
      foreach (var (key, value) in formatter.names)
      {
         names[key] = value;
      }
   }

   public string this[string name]
   {
      get => names[name];
      set => names[name] = value;
   }

   public bool ContainsKey(string key) => names.ContainsKey(key);

   public Result<Hash<string, string>> AnyHash() => names.Success<Hash<string, string>>();

   public string[] Names => names.Select(item => item.Key).ToArray();

   public string[] Values => names.Select(item => item.Value).ToArray();

   public AutoStringHash Replacements => names;

   public virtual string Format(string source)
   {
      if (source.IsNotEmpty())
      {
         var _matches = source.Matches(REGEX_NAME);
         if (_matches)
         {
            foreach (var match in ~_matches)
            {
               var slashes = match.FirstGroup;
               var name = match.SecondGroup;
               var formatString = match.ThirdGroup;

               if (names.ContainsKey(name))
               {
                  var text = getText(name, formatString);
                  match.Text = slashes switch
                  {
                     "" => text,
                     "/" => match.Text.Drop(1),
                     "//" => $"/{text}",
                     _ => match.Text
                  };
               }
            }

            return (~_matches).ToString();
         }
         else
         {
            return source;
         }
      }
      else
      {
         return source;
      }
   }

   protected virtual string getText(string name, string format)
   {
      var _text = names.Maybe[name];
      if (_text)
      {
         var text = ~_text;
         if (format.IsNotEmpty())
         {
            var _object = text.ToObject();
            if (_object)
            {
               text = string.Format($"{{0{format}}}", ~_object);
            }
         }

         return text;
      }
      else
      {
         return string.Empty;
      }
   }

   public void Merge(Formatter formatter, bool overwriteOriginalValues)
   {
      if (overwriteOriginalValues)
      {
         foreach (var (key, value) in formatter.names)
         {
            names[key] = value;
         }
      }
      else
      {
         foreach (var (name, value) in formatter.names)
         {
            if (!names.ContainsKey(name))
            {
               names[name] = value;
            }
         }
      }
   }

   public virtual bool ContainsName(string name) => names.ContainsKey(name);
}