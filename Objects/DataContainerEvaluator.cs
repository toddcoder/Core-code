using System;
using System.Linq;
using Core.Collections;
using Core.Exceptions;

namespace Core.Objects
{
   public class DataContainerEvaluator : IEvaluator, IHash<string, object>, IHash<Signature, object>
   {
      DataContainer data;

      public DataContainerEvaluator(DataContainer data) => this.data = data;

      object IEvaluator.this[string signature]
      {
         get => data[signature];
         set => data[signature] = value;
      }

      public bool ContainsKey(string key) => data.ContainsKey(key);

      object IEvaluator.this[Signature signature]
      {
         get => data[signature.Name];
         set => data[signature.Name] = value;
      }

      public bool ContainsKey(Signature key) => data.ContainsKey(key.Name);

      public Type Type(string signature)
      {
         if (data.If(signature, out var value))
         {
            return value?.GetType();
         }
         else
         {
            throw "Value isn't set".Throws();
         }
      }

      public Type Type(Signature signature) => Type(signature.Name);

      public bool Contains(string signature) => data.ContainsKey(signature);

      public Signature[] Signatures => data.KeyArray().Select(key => new Signature(key)).ToArray();

      object IHash<string, object>.this[string key] => data[key];

      object IHash<Signature, object>.this[Signature key] => data[key.Name];
   }
}