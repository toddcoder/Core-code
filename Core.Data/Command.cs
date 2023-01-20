using System;
using Core.Computers;
using Core.Configurations;
using Core.Dates.DateIncrements;
using Core.Monads;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Data;

public class Command
{
   public static Result<Command> FromSetting(Setting commandSetting)
   {
      var name = commandSetting.Key;
      return
         from values in getValues(commandSetting)
         select new Command { Name = name, Text = values.text, CommandTimeout = values.timeout };
   }

   protected static Result<(string text, TimeSpan timeout)> getValues(Setting commandSetting)
   {
      try
      {
         string command;
         var _text = lazy.maybe<string>();
         var _fileName = lazy.maybe<string>();
         if (_text.ValueOf(commandSetting.Maybe.String("text")))
         {
            command = _text;
         }
         else if (_fileName.ValueOf(commandSetting.Maybe.String("file")))
         {
            FileName file = _fileName.Value;
            command = file.Text;
         }
         else
         {
            return fail("Require 'text' or 'file' values");
         }

         var timeout = commandSetting.Maybe.String("timeout").Map(Maybe.TimeSpan) | (() => 30.Seconds());

         return (command, timeout);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Command(Setting commandSetting)
   {
      Name = commandSetting.Key;
      var _text = lazy.maybe<string>();
      var _fileName = lazy.maybe<string>();
      if (_text.ValueOf(commandSetting.Maybe.String("text")))
      {
         Text = _text;
      }
      else if (_fileName.ValueOf(commandSetting.Maybe.String("file")))
      {
         FileName file = _fileName.Value;
         Text = file.Text;
      }
      else
      {
         throw fail("Require 'text' or 'file' values");
      }

      CommandTimeout = commandSetting.Maybe.String("timeout").Map(Maybe.TimeSpan) | (() => 30.Seconds());
   }

   internal Command()
   {
      Name = string.Empty;
      CommandTimeout = 30.Seconds();
      Text = string.Empty;
   }

   public string Name { get; set; }

   public TimeSpan CommandTimeout { get; set; }

   public string Text { get; set; }
}