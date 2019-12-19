using Core.Assertions;
using Core.Monads;

namespace Core.Computers
{
	public static class FileFolderExtensions
	{
		public static IResult<FileName> File(this string fileName)
		{
			FileName file = fileName;
         return file.MustAs(nameof(file)).Exist().Try();
      }

		public static IResult<FolderName> Folder(this string folderName)
		{
			FolderName folder = folderName;
         return folder.MustAs(nameof(folder)).Exist().Try();
      }
	}
}