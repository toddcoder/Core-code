using System;
using Core.Collections;
using Core.Computers;
using Core.Dates;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.ObjectGraphs;

namespace Core.Data
{
   public class Command
   {
      public static IResult<Command> FromObjectGraph(ObjectGraph commandGraph)
      {
         var name = commandGraph.Name;
         return
            from values in getValues(commandGraph)
            select new Command { Name = name, Text = values.text, CommandTimeout = values.timeout };
      }

      protected static IResult<(string text, TimeSpan timeout)> getValues(ObjectGraph commandGraph)
      {
         if (commandGraph.HasChildren)
         {
            if (commandGraph.ChildExists("command"))
            {
               return
                  from timeout in commandGraph.FlatMap("timeout", g => g.Value.TimeSpan(), () => 30.Seconds().Success())
                  from textGraph in commandGraph.Require("text")
                  from text in textGraph.Value.Success()
                  select (text, timeout);
            }

            if (commandGraph.ChildExists("file"))
            {
               return
                  from timeout in commandGraph.FlatMap("timeout", g => g.Value.TimeSpan(), () => 30.Seconds().Success())
                  from fileGraph in commandGraph.Require("file")
                  from file in FileName.Try.FromString(fileGraph.Value)
                  from text in file.TryTo.Text
                  select (text, timeout);
            }
         }

         return (commandGraph.Value, 30.Seconds()).Success();
      }

      public Command(ObjectGraph commandGraph)
      {
         Name = commandGraph.Name;
         if (commandGraph.HasChildren)
         {
            CommandTimeout = commandGraph.FlatMap("timeout", g => g.Value.ToTimeSpan(), () => 30.Seconds());
            if (commandGraph.If("command", out var text))
            {
               Text = text.Value;
            }
            else
            {
               FileName fileName = commandGraph["file"].Value;
               Text = fileName.Text;
            }
         }
         else
         {
            Text = commandGraph.Value;
            CommandTimeout = 30.Seconds();
         }

         Text = commandGraph.Replace(Text);
      }

      internal Command()
      {
         Name = "";
         CommandTimeout = 30.Seconds();
         Text = "";
      }

      public string Name { get; set; }

      public TimeSpan CommandTimeout { get; set; }

      public string Text { get; set; }
   }
}