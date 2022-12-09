using Core.Computers;

namespace Core.Applications;

public interface ICommandFile
{
   FileName CommandFile(string name);
}