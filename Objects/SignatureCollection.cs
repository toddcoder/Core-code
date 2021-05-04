using System.Collections.Generic;
using Core.RegularExpressions;

namespace Core.Objects
{
   public class SignatureCollection : List<Signature>
   {
      public SignatureCollection(string signature)
      {
         foreach (var singleSignature in signature.Split("'.'"))
         {
            Add(singleSignature);
         }
      }

      public void Add(string signature) => Add(new Signature(signature));
   }
}