using System;
using System.IO;
using System.Linq;

namespace Core.Applications
{
	public class Resources<T>
	{
		protected Type type;
		protected string nameSpace;
		protected string[] names;

		public Resources()
		{
			type = typeof(T);
			nameSpace = type.Namespace + ".";
			names = type.Assembly.GetManifestResourceNames();
		}

		public string String(string name)
		{
			using (var reader = new StreamReader(Stream(name)))
         {
            return reader.ReadToEnd();
         }
      }

		public Stream Stream(string name) => type.Assembly.GetManifestResourceStream(nameSpace + name);

		public bool Contains(string name) => names.Contains(nameSpace + name);
	}
}