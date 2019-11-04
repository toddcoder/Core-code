using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Arrays;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Dates;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using Core.ObjectGraphs.Parsers;
using Core.Objects;
using Core.RegularExpressions;
using Core.Strings;
using static System.Activator;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;
using static Core.ObjectGraphs.Parsers.BaseParser;

namespace Core.ObjectGraphs
{
   public class ObjectGraph : IHash<string, ObjectGraph>
   {
      public class Try
      {
         public static IResult<ObjectGraph> Serialize(object obj, Predicate<string> exclude, StringHash signatures)
         {
            return tryTo(() => ObjectGraph.Serialize(obj, exclude, signatures));
         }

         public static IResult<ObjectGraph> Serialize(object obj) => Serialize(obj, sig => false, "");

         public static IResult<ObjectGraph> Serialize(object obj, StringHash signatures)
         {
            return Serialize(obj, sig => false, signatures);
         }

         public static IResult<ObjectGraph> Serialize(object obj, Predicate<string> exclude)
         {
            return Serialize(obj, exclude, "");
         }

         public static IResult<ObjectGraph> FromFile(FileName file) => tryTo(() => ObjectGraph.FromFile(file));

         public static IResult<(ObjectGraph objectGraph, IMaybe<Replacer> replacer)> FromFileWithReplacer(FileName file)
         {
            return tryTo(() =>
            {
               var objectGraph = ObjectGraph.FromFile(file, out var replacer);
               return (objectGraph, replacer).Success();
            });
         }

         public static IResult<ObjectGraph> FromSingleLine(string singleLine)
         {
            return tryTo(() => ObjectGraph.FromSingleLine(singleLine));
         }
      }

      public const string ROOT_NAME = "$root";
      const string REGEX_LINE = "^" + REGEX_NAME + REGEX_TYPE + "/s* ('=>' | '->') /s*" + REGEX_VALUE + "$";
      const string REGEX_GROUP = "^" + REGEX_NAME + REGEX_TYPE + "$";

      public static IMaybe<ObjectGraph> Some(string name, string value = "", string type = "")
      {
         return new ObjectGraph(name, value, type).Some();
      }

      public static implicit operator ObjectGraph(string source) => FromSingleLine(source);

      public static ObjectGraph Root(params string[] childSources)
      {
         var rootGraph = RootObjectGraph();
         foreach (var child in childSources.Select(childSource => (ObjectGraph)childSource))
         {
            rootGraph[child.Key] = child;
         }

         return rootGraph;
      }

      public static ObjectGraph RootObjectGraph() => new ObjectGraph(ROOT_NAME);

      public static ObjectGraph Serialize(object obj) => Serialize(obj, sig => false, "");

      public static ObjectGraph Serialize(object obj, StringHash signatures) => Serialize(obj, sig => false, signatures);

      public static ObjectGraph Serialize(object obj, Predicate<string> exclude) => Serialize(obj, exclude, "");

      public static ObjectGraph Serialize(object obj, Predicate<string> exclude, StringHash signatures)
      {
         obj.Must().Not.BeNull().Assert("Object serialized can't be null");

         var graph = RootObjectGraph();
         var type = obj.GetType();
         var evaluator = new PropertyEvaluator(obj);
         var infos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
         foreach (var info in infos)
         {
            var signature = info.Name;
            var childName = signature.ToCamel();
            if (evaluator.Attribute<SignatureAttribute>(signature).If(out var a))
            {
               childName = a.Signature;
            }

            if (exclude(signature))
            {
               continue;
            }

            if (((IHash<string, object>)evaluator).If(signature, out var objectValue))
            {
               if (objectValue.IsNull())
               {
                  continue;
               }

               var childType = evaluator.Type(signature);
               graph[childName] = getGraph(objectValue.Some(), childName, childType, exclude, signatures);
            }
         }

         return graph;
      }

      static ObjectGraph getGraph(IMaybe<object> anyObject, string childName, Type childType, Predicate<string> exclude, StringHash signatures)
      {
         anyObject.Must().HaveValue().Assert("Object value not initialized");
         var obj = anyObject.Must().Value;

         if (childType.IsArray)
         {
            var array = (Array)obj;
            var list = new List<string>();
            for (var i = 0; i < array.Length; i++)
            {
               list.Add(array.GetValue(i).ToNonNullString());
            }

            return new ObjectGraph(childName, $"[{list.Stringify()}]");
         }
         else if (!(obj is IConvertible))
         {
            return Serialize(obj, exclude, signatures);
         }
         else
         {
            return new ObjectGraph(childName, obj.ToNonNullString());
         }
      }

      public static ObjectGraph FromFile(FileName file, string configs = Parser.DEFAULT_CONFIGS)
      {
         return FromString(file.Text, file.Folder, configs);
      }

      public static ObjectGraph FromString(string source, FolderName currentFolder, string configs = Parser.DEFAULT_CONFIGS)
      {
         return new Parser(source, currentFolder.ToString().Some()).Parse(configs);
      }

      public static ObjectGraph FromString(string source, string configs = Parser.DEFAULT_CONFIGS)
      {
         return new Parser(source, none<string>()).Parse(configs);
      }

      public static ObjectGraph FromFile(FileName file, out IMaybe<Replacer> replacer, string configs = Parser.DEFAULT_CONFIGS)
      {
         return FromString(file.Text, file.Folder, out replacer, configs);
      }

      public static ObjectGraph FromString(string source, FolderName currentFolder, out IMaybe<Replacer> replacer,
         string configs = Parser.DEFAULT_CONFIGS)
      {
         var parser = new Parser(source, currentFolder.ToString().Some());
         var graph = parser.Parse(configs);
         replacer = parser.Replacer;

         return graph;
      }

      public static ObjectGraph FromString(string source, out IMaybe<Replacer> replacer, string configs = Parser.DEFAULT_CONFIGS)
      {
         var parser = new Parser(source, none<string>());
         var graph = parser.Parse(configs);
         replacer = parser.Replacer;

         return graph;
      }

      public static ObjectGraph FromSingleLine(string singleLine)
      {
         var destringifier = new Destringifier(singleLine);
         var parsed = destringifier.Parse();

         var matcher = new Matcher();

         using (var writer = new ObjectGraphWriter())
         {
            foreach (var token in parsed.Split("/s* /(['{}[];']) /s*"))
            {
               switch (token)
               {
                  case "{":
                  case "[":
                     writer.Begin();
                     break;
                  case "}":
                  case "]":
                     writer.End();
                     break;
                  default:
                     if (matcher.IsMatch(token, REGEX_LINE))
                     {
                        var name = matcher[0, 1];
                        var type = matcher[0, 2];
                        var value = matcher[0, 3];
                        value = destringifier.Restring(value, false);
                        writer.Write(name, value, type);
                     }
                     else if (matcher.IsMatch(token, REGEX_GROUP))
                     {
                        var name = matcher[0, 1];
                        var type = matcher[0, 2];
                        writer.Write(name, type: type);
                     }

                     break;
               }
            }

            return writer.ToObjectGraph();
         }
      }

      public static ObjectGraph Unflatten(Hash<string, string> flattened, string connector = "/")
      {
         var rootGraph = RootObjectGraph();
         var matcher = new Matcher();

         var pattern = $"/s* '{connector.Escape()}' /s*";

         foreach (var (key, value1) in flattened)
         {
            var names = key.Split(pattern);
            var currentGraph = rootGraph;
            foreach (var name in names.Tail())
            {
               ObjectGraph graph;
               if (matcher.IsMatch(name, "^ /(-[':']+) ':' /(.+) }"))
               {
                  var graphName = matcher[0, 1];
                  var type = matcher[0, 2];
                  graph = currentGraph.DefaultTo(graphName, new ObjectGraph(graphName, type: type));
                  currentGraph[graphName] = graph;
               }
               else
               {
                  graph = currentGraph.DefaultTo(name, () => new ObjectGraph(name));
                  currentGraph[name] = graph;
               }

               currentGraph = graph;
            }

            currentGraph.Value = value1;
         }

         return rootGraph;
      }

      protected string name;
      protected string type;
      protected string value;
      protected Lazy<Hash<string, ObjectGraph>> children;
      protected string key;
      protected string path;

      public ObjectGraph(string name, string value = "", string type = "", string key = "")
      {
         if (name.EndsWith("*"))
         {
            this.name = name.Drop(-1);
            this.key = $"{this.name}{uniqueID()}";
         }
         else
         {
            this.name = name;
            this.key = key.IsEmpty() ? name : key;
         }

         this.type = type;
         this.value = value;
         children = new Lazy<Hash<string, ObjectGraph>>(() => new Hash<string, ObjectGraph>());
         Replacer = none<Replacer>();
         path = "";
      }

      public string Name => name;

      internal void SetName(string newName) => name = newName;

      public string Type
      {
         get => type;
         set => type = value;
      }

      public string FullName => type.IsNotEmpty() ? $"{name}: {type}" : name;

      public string Path
      {
         get => path;
         set => path = value;
      }

      public string Tag => type.IsEmpty() ? name : type;

      public virtual string Value
      {
         get => value;
         set => this.value = value;
      }

      public string Key
      {
         get => key;
         set => key = value;
      }

      public bool IsTrue => value == "true";

      ObjectGraph getChildValue(string graphName)
      {
         children.Value.Must().HaveKeyOf(graphName).Assert($"'{graphName}' graph is not found under <{Path}> @ {LineNumber}: {LineSource}");
         var childValue = children.Value[graphName];
         childValue.Replacer = Replacer;

         return childValue;
      }

      public void Force(string graphName)
      {
         var _ = this[graphName];
      }

      public virtual ObjectGraph this[string graphName]
      {
         get
         {
            HasChildren.Must().Be().Assert($"{key} has no children");
            if (graphName.IsMatch("'//'"))
            {
               var childName = graphName.KeepUntil("/");
               var child = getChildValue(childName);
               var rest = graphName.DropUntil("/").Drop(1);

               return child[rest];
            }
            else
            {
               return getChildValue(graphName);
            }
         }
         set
         {
            if (value.IsNull())
            {
               if (children.Value.ContainsKey(graphName))
               {
                  children.Value.Remove(graphName);
               }
            }
            else
            {
               if (value.Name == ROOT_NAME)
               {
                  foreach (var childGraph in value.Children)
                  {
                     children.Value[childGraph.Name] = childGraph;
                  }
               }
               else
               {
                  value.Path = $"{pathRoot()}/{value.Name}";
                  children.Value[graphName] = value;
               }
            }
         }
      }

      public ObjectGraph Replace(Hash<string, string> replacements)
      {
         if (HasChildren)
         {
            var replaced = new ObjectGraph(name, value, type, key);
            foreach (var child in Children)
            {
               replaced[child.key] = child.Replace(replacements);
            }

            return replaced;
         }
         else if (value.Contains("{"))
         {
            return new ObjectGraph(name, replacements.Format(value), type, key);
         }
         else
         {
            return this;
         }
      }

      public ObjectGraph Replace(string regex, string replacement)
      {
         if (HasChildren)
         {
            var replaced = new ObjectGraph(name, value, type, key);
            foreach (var child in Children)
            {
               replaced[child.key] = child.Replace(regex, replacement);
            }

            return replaced;
         }
         else
         {
            return new ObjectGraph(name, value.Substitute(regex, replacement), type, key);
         }
      }

      public ObjectGraph Clone()
      {
         var self = new ObjectGraph(name, value, type, key);
         if (HasChildren)
         {
            foreach (var child in Children)
            {
               self[child.key] = child.Clone();
            }
         }

         return self;
      }

      public void Merge(ObjectGraph sourceGraph) => children.Value[sourceGraph.Name] = sourceGraph;

      string pathRoot() => name == ROOT_NAME ? ROOT_NAME : Path;

      public bool ContainsKey(string childName)
      {
         if (HasChildren)
         {
            if (childName.IsMatch("'//'"))
            {
               var firstName = childName.KeepUntil("/");
               if (children.Value.ContainsKey(firstName))
               {
                  return children.Value[firstName].ContainsKey(childName.DropUntil("/").Drop(1));
               }
               else
               {
                  return false;
               }
            }
            else
            {
               return children.Value.ContainsKey(childName);
            }
         }
         else
         {
            return false;
         }
      }

      public Result Result => new Result(this);

      public Hash<string, ObjectGraph> ChildCollection => children.Value;

      public bool HasChildren => children.IsValueCreated && children.Value.Count > 0;

      public IEnumerable<ObjectGraph> Children => HasChildren ? children.Value.ValueArray() : new ObjectGraph[0];

      public bool ChildExists(string childName) => HasChildren && children.Value.ContainsKey(childName);

      public bool IsEmpty => !HasChildren && value.IsEmpty();

      protected virtual void write(StringWriter writer, int indentation, bool isAttribute)
      {
         var isNotRoot = name != ROOT_NAME;
         if (isNotRoot)
         {
            var indent = "\t".Repeat(indentation);
            writer.Write($"{indent}{(isAttribute ? "." : "")}{FullName}");
         }

         if (HasChildren)
         {
            if (isNotRoot)
            {
               writer.WriteLine();
            }

            foreach (var child in Children)
            {
               child.write(writer, indentation + 1, false);
            }
         }
         else
         {
            writer.WriteLine($" -> \"{Value.ReplaceAll(("\"", @"\"""), (@"\", @"\\"))}\"");
         }
      }

      public override string ToString()
      {
         using (var writer = new StringWriter())
         {
            write(writer, -1, false);
            return writer.ToString();
         }
      }

      public virtual IResult<T> Object<T>(params object[] args) where T : class => Object(typeof(T), args).Map(o => (T)o);

      public virtual IResult<object> Object(Type objectType, params object[] args) => tryTo(() =>
      {
         if (objectType.IsArray)
         {
            return getArray(objectType.GetElementType());
         }
         else
         {
            var obj = CreateInstance(objectType, args);
            var evaluator = new PropertyEvaluator(obj);

            foreach (var child in Children)
            {
               child.setValue(evaluator);
            }

            return obj;
         }
      });

      public virtual IResult<object> Convert(Type conversionType)
      {
         if (conversionType.IsEnum)
         {
            return Value.Enumeration(conversionType);
         }
         else if (conversionType.IsArray)
         {
            return getArray(Value);
         }
         else
         {
            return Value.AsObject().Otherwise(e => System.Convert.ChangeType(Value, conversionType));
         }
      }

      static IResult<object> getArray(string source)
      {
         if (source.StartsWith("["))
         {
            source = source.Drop(1);
         }

         if (source.EndsWith("]"))
         {
            source = source.Drop(-1);
         }

         source = source.Trim();

         var items = source.Split("/s* ',' /s*");

         return
            from notEmpty in items.Must().HaveLengthOf(1).Try("Array can't be empty")
            from type in items[0].Type()
            from instance in tryTo(() => Array.CreateInstance(type, items.Length))
            from parsed in items.Select(i => i.Parsed(type)).IfAllSuccesses()
            from array in tryTo(parsed.ToArray)
            select (object)array;
      }

      IResult<object[]> getArguments(Type arrayType)
      {
         var convertedChildren = Children.Select(c => c.HasChildren ? c.Object(arrayType) : c.Convert(arrayType)).ToArray();
         return convertedChildren.IfAllSuccesses().Map(c => c.ToArray());
      }

      protected virtual IResult<object> getArray(Type arrayType) =>
         from arguments in getArguments(arrayType)
         from array in tryTo(() => Array.CreateInstance(arrayType, arguments.Length))
         from objects in tryTo(() =>
         {
            arguments.CopyTo(array, 0);
            return (object)array;
         })
         select objects;

      protected virtual void setValue(PropertyEvaluator evaluator)
      {
         string signature;
         var graphName = name.ToCamel();
         var anyWithAttribute = evaluator.WithAttribute<SignatureAttribute>().Where(s => s.Name.ToCamel() == graphName).FirstOrNone();
         if (anyWithAttribute.If(out var withAttribute))
         {
            signature = withAttribute.Name;
         }
         else if (type.IsNotEmpty())
         {
            signature = type;
         }
         else
         {
            signature = name.ToPascal();
         }

         if (evaluator.Contains(signature))
         {
            var conversionType = evaluator.Type(signature);
            var valueToSet = HasChildren ? Object(conversionType) : Convert(conversionType);
            if (valueToSet.If(out var result))
            {
               evaluator[signature] = result;
            }
         }
      }

      public void Fill(ref object obj)
      {
         var evaluator = new PropertyEvaluator(obj);
         if (obj?.GetType().IsArray ?? false)
         {
            obj = fillArray(obj);
         }
         else
         {
            foreach (var child in Children)
            {
               fillValue(child, evaluator);
            }
         }
      }

      protected object fillArray(object obj)
      {
         try
         {
            var elementType = obj.GetType().GetElementType();
            var count = children.Value.Count;
            var array = Array.CreateInstance(elementType, count);
            var index = 0;
            foreach (var item in children.Value.ValueArray().Select(g => g.Convert(elementType)))
            {
               if (item.If(out var i))
               {
                  array.SetValue(i, index++);
               }
            }

            return array;
         }
         catch (Exception exception)
         {
            throw $"{obj?.ToString() ?? "null"} -> {exception.Message}".Throws();
         }
      }

      protected static void fillValue(ObjectGraph graph, PropertyEvaluator evaluator)
      {
         try
         {
            var signature = "";
            if (evaluator.WithAttribute<SignatureAttribute>()
               .Where(sig => sig.Name == graph.Name)
               .FirstOrNone().If(out var s))
            {
               signature = s.Name;
            }
            else if (graph.Type.IsNotEmpty())
            {
               signature = graph.Type;
            }
            else
            {
               signature = graph.Name.ToPascal();
            }

            if (evaluator.Contains(signature))
            {
               var conversionType = evaluator.Type(signature);
               var obj = evaluator.DefaultTo(signature, () => createInstance(conversionType, graph));

               if (graph.HasChildren)
               {
                  if (obj == null)
                  {
                     obj = CreateInstance(conversionType);
                  }

                  graph.Fill(ref obj);
                  evaluator[signature] = obj;
               }
               else
               {
                  if (graph.Convert(conversionType).If(out var converted))
                  {
                     evaluator[signature] = converted;
                  }
               }
            }
         }
         catch (Exception exception)
         {
            throw $"{graph.name} -> {exception.Message}".Throws();
         }
      }

      protected static object createInstance(Type type, ObjectGraph graph)
      {
         if (type != typeof(string))
         {
            if (type.IsArray)
            {
               var elementType = type.GetElementType();
               return Array.CreateInstance(elementType, 0);
            }
            else
            {
               return graph.Convert(type).ForceValue();
            }
         }
         else
         {
            return "";
         }
      }

      public void SetChildren(ObjectGraph sourceGraph)
      {
         var some = sourceGraph;
         foreach (var child in sourceGraph.Children)
         {
            children.Value[child.Name] = some;
         }
      }

      public IMaybe<Replacer> Replacer { get; set; }

      public string Replace(string source) => Replacer.FlatMap(r => r.Replace(source), () => source);

      public static void Visit(ObjectGraph graph, Action<ObjectGraph> action)
      {
         if (graph.HasChildren)
         {
            foreach (var childGraph in graph.Children)
            {
               Visit(childGraph, action);
            }
         }
         else
         {
            action(graph);
         }
      }

      public void Visit(Action<ObjectGraph> action) => Visit(this, action);

      public static void Visit<T>(ObjectGraph graph, Action<ObjectGraph, T> action, T state)
      {
         if (graph.HasChildren)
         {
            foreach (var childGraph in graph.Children)
            {
               Visit(childGraph, action, state);
            }
         }
         else
         {
            action(graph, state);
         }
      }

      public void Visit<T>(Action<ObjectGraph, T> action, T state) => Visit(this, action, state);

      public static void Visit<T>(ObjectGraph graph, Action<ObjectGraph, T> action, T state, Func<T, T> newState)
      {
         if (graph.HasChildren)
         {
            foreach (var childGraph in graph.Children)
            {
               Visit(childGraph, action, newState(state), newState);
            }
         }
         else
         {
            action(graph, state);
         }
      }

      public void Visit<T>(Action<ObjectGraph, T> action, T state, Func<T, T> newState) => Visit(this, action, state, newState);

      public ObjectGraph Visit(Func<ObjectGraph, ObjectGraph> func)
      {
         var newGraph = func(this);
         if (HasChildren)
         {
            foreach (var childGraph in Children)
            {
               var newChildGraph = childGraph.Visit(func);
               newGraph[newChildGraph.key] = newChildGraph;
            }
         }

         return newGraph;
      }

      public Hash<string, string> Flatten(string connector = "/")
      {
         var hash = new Hash<string, string>();
         Visit(this, (g, n) => hash[n + g.FullName] = g.Value, FullName, n => n + connector);

         return hash;
      }

      public ObjectGraph Subgraph
      {
         set
         {
            if (value.Name == ROOT_NAME)
            {
               foreach (var child in value.Children)
               {
                  this[child.Name] = child;
               }
            }
            else
            {
               this[value.Name] = value;
            }
         }
      }

      public void Subgraphs(params ObjectGraph[] subgraphs)
      {
         foreach (var subgraph in subgraphs)
         {
            if (subgraph.Name == ROOT_NAME)
            {
               foreach (var child in subgraph.Children)
               {
                  this[child.Name] = child;
               }
            }
            else
            {
               this[subgraph.Name] = subgraph;
            }
         }
      }

      public int LineNumber { get; set; } = -1;

      public string LineSource { get; set; } = "";

      public int TabCount { get; set; }

      public string ValueOrChild(string childName, string defaultValue = "")
      {
         if (value.IsNotEmpty())
         {
            return value;
         }
         else if (ContainsKey(childName))
         {
            return this[childName].value;
         }
         else
         {
            return defaultValue;
         }
      }

      public ObjectGraphTrying TryTo => new ObjectGraphTrying(this);

      public Hash<string, string> ToStringHash() => Children.ToHash(g => g.name, g => g.value);

      public IResult<Hash<string, ObjectGraph>> AnyHash() => Children.ToHash(g => g.name).Success();

      public string[] ToArray() => Children.Select(g => g.name).ToArray();

      public string[] ValuesToArray() => Children.Select(g => g.value).ToArray();

      public IMaybe<ObjectGraph> FirstChild => maybe(HasChildren, () => children.Value[children.Value.KeyArray()[0]]);

      public bool ToBoolean(string key, bool defaultValue = false) => this.FlatMap(key, g => g.IsTrue, defaultValue);

      public string FlatMap(string key, string defaultValue) => this.FlatMap(key, g => g.Value, defaultValue);

      public string FlatMap(string key, Func<string> defaultValue) => this.FlatMap(key, g => g.Value, defaultValue);

      public bool FlatMap(string key, bool defaultValue) => this.FlatMap(key, g => g.IsTrue, defaultValue);

      public bool FlatMap(string key, Func<bool> defaultValue) => this.FlatMap(key, g => g.IsTrue, defaultValue);

      public int ToInt(string key, int defaultValue = 0) => this.FlatMap(key, g => g.Value.ToInt(defaultValue), defaultValue);

      public TimeSpan ToTimeSpan(string key, TimeSpan defaultTimeSpan)
      {
         return this.FlatMap(key, g => g.Value.ToTimeSpan(), () => defaultTimeSpan);
      }

      public TimeSpan ToTimeSpan(string key, Func<TimeSpan> defaultTimeSpan)
      {
         return this.FlatMap(key, g => g.Value.ToTimeSpan(), defaultTimeSpan);
      }

      public string ToString(string key, string defaultString) => FlatMap(key, defaultString);

      public string ToString(string key, Func<string> defaultString) => FlatMap(key, defaultString);

      public IMaybe<string> MapValue(string key) => this.Map(key, g => g.value);
   }
}