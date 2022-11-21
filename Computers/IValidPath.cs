using Core.Monads;

namespace Core.Computers;

public interface IValidPath<T> where T : IFullPath
{
   Result<T> Validate(bool allowRelativePaths = false);

   bool IsValid { get; }
}