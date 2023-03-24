using Core.Monads;

namespace Core.Computers;

public interface IValidPath<T> where T : IFullPath
{
   Optional<T> Validate(bool allowRelativePaths = false);

   bool IsValid { get; }
}