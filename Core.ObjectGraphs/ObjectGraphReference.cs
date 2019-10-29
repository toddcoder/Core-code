namespace Core.ObjectGraphs
{
	public class ObjectGraphReference
	{
      public ObjectGraphReference(string name, string key)
		{
			Name = name;
			Key = key;
		}

		public string Name { get; }

      public string Key { get; }
   }
}