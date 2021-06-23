using System;
using Core.Computers;
using Core.Configurations;
using Core.Dates;
using Core.Dates.DateIncrements;
using Core.Exceptions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data
{
   public class Command
   {
      public static Result<Command> FromGroup(Group commandGroup)
      {
         var name = commandGroup.Key;
         return
            from values in getValues(commandGroup)
            select new Command { Name = name, Text = values.text, CommandTimeout = values.timeout };
      }

      protected static Result<(string text, TimeSpan timeout)> getValues(Group commandGroup)
      {
         try
         {
            string command;
            if (commandGroup.GetValue("text").If(out var text))
            {
               command = text;
            }
            else if (commandGroup.GetValue("file").If(out var fileName))
            {
               FileName file = fileName;
               command = file.Text;
            }
            else
            {
               return "Require 'text' or 'file' values".Failure<(string, TimeSpan)>();
            }

            var timeout = commandGroup.GetValue("timeout").Map(s => s.ToTimeSpan()).DefaultTo(() => 30.Seconds());

            return (command, timeout).Success();
         }
         catch (Exception exception)
         {
            return failure<(string, TimeSpan)>(exception);
         }
      }

      public Command(Group commandGroup)
      {
         Name = commandGroup.Key;
         if (commandGroup.GetValue("text").If(out var text))
         {
            Text = text;
         }
         else if (commandGroup.GetValue("file").If(out var fileName))
         {
            FileName file = fileName;
            Text = file.Text;
         }
         else
         {
            throw "Require 'text' or 'file' values".Throws();
         }

         CommandTimeout = commandGroup.GetValue("timeout").Map(s => s.ToTimeSpan()).DefaultTo(() => 30.Seconds());
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