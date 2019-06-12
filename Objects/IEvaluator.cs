using System;

namespace Core.Objects
{
   public interface IEvaluator
   {
      object this[string signature] { get; set; }

      object this[Signature signature] { get; set; }

      Type Type(string signature);

      Type Type(Signature signature);

      bool Contains(string signature);

      Signature[] Signatures { get; }
   }
}