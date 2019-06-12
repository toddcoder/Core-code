using Core.Computers;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications
{
	public class Argument
	{
		string text;
		int index;
		IMaybe<FileName> fileName;
		IMaybe<FolderName> folderName;

		public Argument(string text, int index)
		{
			this.text = text;
			this.index = index;
			fileName = none<FileName>();
			folderName = none<FolderName>();
		}

		public string Text => text;

		public IMaybe<FileName> FileName
		{
			get
			{
				if (Computers.FileName.IsValidFileName(text))
				{
					FileName file = text;
					fileName = when(file.Exists(), () => file);
				}
				else
					fileName = none<FileName>();

				return fileName;
			}
		}

		public IMaybe<FolderName> FolderName
		{
			get
			{
				if (Computers.FolderName.IsValidFolderName(text))
				{
					FolderName folder = text;
					folderName = when(folder.Exists(), () => folder);
				}
				else
					folderName = none<FolderName>();

				return folderName;
			}
		}

		public int Index => index;

		public ArgumentTrying TryTo => new ArgumentTrying(this);
	}
}