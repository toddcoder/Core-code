using System;
using System.Collections.Generic;
using Core.Collections;
using Core.Data.Fields;
using Core.Data.Parameters;

namespace Core.Data.Setups
{
   public interface ISetupObject
   {
      string ConnectionString { get; }

      CommandSourceType CommandSourceType { get; }

      string Command { get; }

      TimeSpan CommandTimeout { get; }

      IEnumerable<Parameter> Parameters();

      IEnumerable<Field> Fields();

      IHash<string, string> Attributes { get; }

      ISetup Setup();
   }
}