using Core.Collections;
using Core.Computers;
using Core.Monads;
using Core.ObjectGraphs;
using static Core.Monads.AttemptFunctions;

namespace Core.Data.Configurations
{
   public class DataGraphs
   {
      public ObjectGraph ConnectionsGraph { get; set; }

      public ObjectGraph CommandsGraph { get; set; }

      public ObjectGraph AdaptersGraph { get; set; }

      public IResult<string> Command(string adapterName)
      {
         return
            from adapterGraph in AdaptersGraph.Result[adapterName]
            from commandName in tryTo(() => adapterGraph.FlatMap("command", g => g.Value, () => adapterName))
            from commandGraph in CommandsGraph.Result[commandName]
            from commandText in commandGraph.Map("file", g => ((FileName)g.Value).Text)
               .Or(() => commandGraph.Map("command", g => g.Value))
               .Or(() => commandGraph.Value.Some()).Result("Couldn't find command")
            select commandText;
      }
   }
}