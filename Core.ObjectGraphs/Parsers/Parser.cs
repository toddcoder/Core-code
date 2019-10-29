using System.Collections.Generic;
using Core.Assertions;
using Core.Computers;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.ObjectGraphs.Replacer;

namespace Core.ObjectGraphs.Parsers
{
   public class Parser
   {
      public static ObjectGraph FromSource(string source, IMaybe<string> configFolder, string configs = DEFAULT_CONFIGS)
      {
         return new Parser(source, configFolder).Parse(configs);
      }

      public static ObjectGraph FromSource(string source) => FromSource(source, none<string>());

      public static ObjectGraph FromFile(FileName file) => FromSource(file.Text, file.Folder.ToString().Some());

      public const string GRAPH_ROOT = "$root";

      public const string DEFAULT_CONFIGS = @"C:\Configurations";

      string source;
      IMaybe<string> anyConfigFolder;

      public IMaybe<Replacer> Replacer { get; set; }

      public Parser(string source, IMaybe<string> anyConfigFolder)
      {
         this.source = source;
         Replacer = none<Replacer>();
         this.anyConfigFolder = anyConfigFolder;
      }

      public Parser(string source) : this(source, none<string>()) { }

      public ParserTrying TryTo => new ParserTrying(this);

      public ObjectGraph Parse(string configs = DEFAULT_CONFIGS)
      {
         var replacer = Duplicate(Replacer, source);
         Replacer = replacer.Some();
         replacer["configs"] = configs;
         if (anyConfigFolder.If(out var configFolder))
         {
            replacer["config"] = configFolder;
         }

         var parsed = replacer.Parse();
         BaseParser.TabCount = -1;
         var rootParser = new RootParser(GRAPH_ROOT, replacer);
         var lines = preprocess(parsed, replacer);
         var objectGraph = rootParser.Parse(lines);
         setPath("", objectGraph);

         return objectGraph;
      }

      static void setPath(string parentPath, ObjectGraph graph)
      {
         var path = parentPath.IsEmpty() ? GRAPH_ROOT : $"{parentPath}/{graph.Name}";
         graph.Path = path;
         foreach (var child in graph.Children)
         {
            setPath(path, child);
         }
      }

      static string[] preprocess(string source, Replacer replacer)
      {
         var lines = source.Split("/r /n | /r | /n");
         var matcher = new Matcher();
         var newLines = new List<string>();
         foreach (var line in lines)
         {
            if (matcher.IsMatch(line, "^ /(/t*) '#include' /s+ /(.+) $"))
            {
               var indent = matcher[0, 1];
               FileName file = replacer.ReplaceVariables(matcher[0, 2]);

               file.Must().Exist().Assert($"Included file {file} doesn't exist");

               var innerLines = preprocess(file.Text, replacer);
               indentLines(innerLines, indent);
               newLines.AddRange(innerLines);
            }
            else
            {
               newLines.Add(line);
            }
         }

         return newLines.ToArray();
      }

      static void indentLines(string[] lines, string indent)
      {
         for (var i = 0; i < lines.Length; i++)
         {
            lines[i] = indent + lines[i];
         }
      }
   }
}