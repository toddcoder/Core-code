using System;

namespace Core.ObjectGraphs
{
   [AttributeUsage(AttributeTargets.Property)]
   public class SignatureAttribute : Attribute
   {
      protected string signature;

      public SignatureAttribute(string signature) => this.signature = signature;

      public string Signature => signature;
   }
}