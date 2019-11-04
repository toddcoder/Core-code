using Core.Assertions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Computers
{
	public static class FileFolderExtensions
	{
		public static IResult<FileName> File(this string fileName)
		{
			FileName result = fileName;
         return result.Must().Exist().Try();
      }

		public static IResult<FolderName> Folder(this string folderName)
		{
			FolderName result = folderName;
         return result.Must().Exist().Try();
      }
	}
}