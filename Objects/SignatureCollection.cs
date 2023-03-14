using System.Collections.Generic;
using Core.Matching;

namespace Core.Objects;

public class SignatureCollection : List<Signature>
{
   public SignatureCollection(string signature)
   {
      foreach (var singleSignature in signature.Unjoin("'.'; f"))
      {
         Add(singleSignature);
      }
   }

   public void Add(string signature) => Add(new Signature(signature));
}