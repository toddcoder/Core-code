using System.Collections.Generic;
using Core.RegexMatching;

namespace Core.Objects
{
   public class SignatureCollection : List<Signature>
   {
      public SignatureCollection(string signature)
      {
         foreach (var singleSignature in signature.Split("'.'; f"))
         {
            Add(singleSignature);
         }
      }

      public void Add(string signature) => Add(new Signature(signature));
   }
}