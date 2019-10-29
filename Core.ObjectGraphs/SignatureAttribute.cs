using System;

namespace Core.ObjectGraphs
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SignatureAttribute : Attribute
	{
		string signature;

		public SignatureAttribute(string signature) => this.signature = signature;

	   public string Signature => signature;
	}
}