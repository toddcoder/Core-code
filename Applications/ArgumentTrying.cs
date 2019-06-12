using Core.Computers;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Applications
{
	public class ArgumentTrying
	{
		Argument argument;

		public ArgumentTrying(Argument argument) => this.argument = argument;

		public IResult<FileName> FileName
		{
			get
			{
				var text = argument.Text;
				return
					from valid in assert(Computers.FileName.IsValidFileName(text), () => (FileName)text,
						() => $"{text} invalid file name")
					from exists in valid.TryTo.MustExist()
					select exists;
			}
		}

		public IResult<FolderName> FolderName
		{
			get
			{
				var text = argument.Text;
				return
					from valid in assert(Computers.FolderName.IsValidFolderName(text), () => (FolderName)text,
						() => $"{text} invalid folder name")
					from exists in valid.TryTo.Exists()
					select exists;
			}
		}
	}
}