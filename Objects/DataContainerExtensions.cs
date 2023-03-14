namespace Core.Objects;

public static class DataContainerExtensions
{
   public static DataContainer Contain(this object value, string name) => new() { [name] = value };
}