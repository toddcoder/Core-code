﻿using System;

namespace Core.ObjectGraphs
{
   public class Defaulting
   {
      ObjectGraph objectGraph;
      Func<string> defaultValue;

      public Defaulting(ObjectGraph objectGraph, Func<string> defaultValue)
      {
         this.objectGraph = objectGraph;
         this.defaultValue = defaultValue;
      }

      public string this[string graphName] => objectGraph.FlatMap(graphName, defaultValue);
   }
}