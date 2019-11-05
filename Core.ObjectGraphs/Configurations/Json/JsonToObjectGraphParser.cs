using System.Collections.Generic;
using Core.Enumerables;
using Core.Monads;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class JsonToObjectGraphParser
   {
      string source;
      ObjectGraph objectGraph;
      Stack<ObjectGraph> stack;
      List<string> arrayValues;
      bool inArray;

      public JsonToObjectGraphParser(string source)
      {
         this.source = source;
         stack = new Stack<ObjectGraph>();

         objectGraph = ObjectGraph.RootObjectGraph();
         stack.Push(objectGraph);

         arrayValues = new List<string>();
         inArray = false;
      }

      public IResult<(JsonObject, ObjectGraph)> Parse()
      {
         var jsonParser = new JsonParser(source);
         jsonParser.ParseValue += (sender, e) =>
         {
            var (tokenType, name, value) = e;
            parseValue(tokenType, name, value);
         };

         return jsonParser.Parse().Map(jsonObject => (jsonObject, objectGraph));
      }

      void parseValue(TokenType tokenType, string name, string value)
      {
         if (inArray)
         {
            if (tokenType == TokenType.ArrayClose)
            {
               objectGraph[name] = $"{name} -> [{arrayValues.Stringify()}]";
               inArray = false;
            }
            else
            {
               arrayValues.Add(value);
            }
         }
         else
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
                  var parent = stack.Pop();
                  parent[objectGraph.Name] = objectGraph;
                  objectGraph = parent;
                  break;
               case TokenType.ArrayOpen:
                  arrayValues.Clear();
                  inArray = true;
                  break;
               case TokenType.String:
               case TokenType.Number:
               case TokenType.True:
               case TokenType.False:
                  objectGraph[name] = $"{name} -> {value}";
                  break;
               case TokenType.Null:
                  break;
            }
         }
      }
   }
}