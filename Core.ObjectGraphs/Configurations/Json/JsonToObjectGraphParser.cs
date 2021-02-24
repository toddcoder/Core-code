using System.Collections.Generic;
using Core.Enumerables;
using Core.Monads;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class JsonToObjectGraphParser
   {
      protected string source;
      protected ObjectGraph objectGraph;
      protected Stack<ObjectGraph> stack;
      protected List<string> arrayValues;
      protected bool inArray;

      public JsonToObjectGraphParser(string source)
      {
         this.source = source;
         stack = new Stack<ObjectGraph>();

         objectGraph = ObjectGraph.RootObjectGraph();

         arrayValues = new List<string>();
         inArray = false;
      }

      protected IResult<JsonObject> parse()
      {
         var jsonParser = new JsonParser(source);
         jsonParser.ParseValue += (sender, e) =>
         {
            var (tokenType, name, value) = e;
            parseValue(tokenType, name, value);
         };

         return jsonParser.Parse();
      }

      public IResult<ObjectGraph> Parse() => parse().Map(_ => objectGraph);

      public IResult<(JsonObject, ObjectGraph)> ParseBoth()
      {
         return parse().Map(jsonObject => (jsonObject, objectGraph));
      }

      protected void parseValue(TokenType tokenType, string name, string value)
      {
         switch (tokenType)
         {
            case TokenType.None:
               break;
            case TokenType.ObjectOpen:
               stack.Push(objectGraph);
               objectGraph = new ObjectGraph(name);
               break;
            case TokenType.ObjectClose:
               if (stack.Count > 0)
               {
                  var parent = stack.Pop();
                  parent[objectGraph.Name] = objectGraph;
                  objectGraph = parent;
               }

               break;
            case TokenType.ArrayOpen:
               arrayValues.Clear();
               inArray = true;
               break;
            case TokenType.ArrayClose:
               objectGraph[name] = $"{name} -> [{arrayValues.ToString(", ")}]";
               inArray = false;
               break;
            case TokenType.String:
            case TokenType.Number:
            case TokenType.True:
            case TokenType.False:
               if (inArray)
               {
                  arrayValues.Add(value);
               }
               else
               {
                  objectGraph[name] = $"{name} -> {value}";
               }

               break;
            case TokenType.Null:
               break;
         }
      }
   }
}