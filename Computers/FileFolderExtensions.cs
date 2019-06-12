using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Computers
{
	public static class FileFolderExtensions
	{
		public static IResult<FileName> File(this string fileName)
		{
			FileName result = fileName;
			return assert(result.Exists, () => result, $"File {fileName} doesn't exist");
		}

		public static IResult<FolderName> Folder(this string folderName)
		{
			FolderName result = folderName;
			return assert(result.Exists(), () => result, $"Folder {folderName} doesn't exist");
		}
	}
}