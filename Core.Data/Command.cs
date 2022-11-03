﻿using System;
using Core.Computers;
using Core.Configurations;
using Core.Dates.DateIncrements;
using Core.Exceptions;
using Core.Monads;
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
         var _text = commandSetting.Maybe.String("text");
         if (_text)
         {
            command = _text;
         }
         else
         {
            var _fileName = commandSetting.Maybe.String("file");
            if (_fileName)
            {
               FileName file = ~_fileName;
               command = file.Text;
            }
            else
            {
               return fail("Require 'text' or 'file' values");
            }
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
      var _text = commandSetting.Maybe.String("text");
      if (_text)
      {
         Text = _text;
      }
      else
      {
         var _fileName = commandSetting.Maybe.String("file");
         if (_fileName)
         {
            FileName file = ~_fileName;
            Text = file.Text;
         }
         else
         {
            throw "Require 'text' or 'file' values".Throws();
         }
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