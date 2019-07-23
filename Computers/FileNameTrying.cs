using System;
using System.IO;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Computers
{
	public class FileNameTrying
	{
		FileName fileName;

		public FileNameTrying(FileName fileName) => this.fileName = fileName;

		public IResult<FileName> Delete() => tryTo(() =>
		{
			fileName.Delete();
			return fileName;
		});

		public FileName FileName => fileName;

		public IResult<FileName> CopyTo(FileName target, bool overwrite = false) => tryTo(() =>
		{
			fileName.CopyTo(target, overwrite);
			return target;
		});

		public IResult<FileName> CopyTo(FolderName targetFolder, bool overwrite = false) => tryTo(() =>
		{
			fileName.CopyTo(targetFolder, overwrite);
			return targetFolder.File(fileName.NameExtension);
		});

		public IResult<FileName> MoveTo(FileName target, bool overwrite = false) => tryTo(() =>
		{
			fileName.MoveTo(target, overwrite);
			return target;
		});

		public IResult<FileName> MoveTo(FolderName targetFolder, bool overwrite = false, bool unique = false)
		{
			return tryTo(() => fileName.MoveTo(targetFolder, overwrite, unique));
		}

		public IResult<FileName> MustExist() => assert(fileName.Exists(), () => fileName, $"File {fileName} doesn't exist");

		public IResult<bool> Exists() => tryTo(() => fileName.Exists());

		public IResult<string> Text => tryTo(() => fileName.Text);

		public IResult<Unit> SetText(string text) => tryTo(() => fileName.Text = text).Unit;

      public IResult<byte[]> Bytes => tryTo(() => fileName.Bytes);

      public IResult<Unit> SetBytes(byte[] bytes) => tryTo(() => fileName.Bytes = bytes).Unit;

		public IResult<string[]> Lines => tryTo(() => fileName.Lines);

		public IResult<DateTime> CreationTime => tryTo(() => fileName.CreationTime);

		public IResult<Unit> SetCreationTime(DateTime creationTime) => tryTo(() => { fileName.CreationTime = creationTime; });

		public IResult<DateTime> LastAccessTime => tryTo(() => fileName.LastAccessTime);

		public IResult<Unit> SetLastAccessTime(DateTime lastAccessTime) => tryTo(() => { fileName.LastAccessTime = lastAccessTime; });

		public IResult<DateTime> LastWriteTime => tryTo(() => fileName.LastWriteTime);

		public IResult<Unit> SetLastWriteTime(DateTime lastWriteTime) => tryTo(() => { fileName.LastWriteTime = lastWriteTime; });

		public IResult<string> Execute(string arguments, bool wait = true, bool useShellExecute = false, bool createNoWindow = true)
		{
			return tryTo(() => fileName.Execute(arguments, wait, useShellExecute, createNoWindow));
		}

		public IResult<string> Execute(FileName passedFileName, bool wait = true, bool useShellExecute = false,
			bool createNoWindow = true)
		{
			return tryTo(() => fileName.Execute(passedFileName, wait, useShellExecute, createNoWindow));
		}

		public IResult<long> Length => tryTo(() => fileName.Length);

		public IResult<string> Size => tryTo(() => fileName.Size);

		public IResult<FileName> Rename(string name = null, string extension = null, bool overwrite = false, bool unique = false)
		{
			return tryTo(() => fileName.Rename(name, extension, overwrite, unique));
		}

		public IResult<FileNameMappedReader> Reader() => tryTo(() => fileName.Reader());

		public IResult<FileStream> WritingStream() => tryTo(() => fileName.WritingStream());

		public IResult<FileStream> AppendingStream() => tryTo(() => fileName.AppendingStream());

		public IResult<FileStream> ReadingStream() => tryTo(() => fileName.ReadingStream());

		public IResult<FileStream> ReadWriteStream() => tryTo(() => fileName.ReadWriteStream());

		public IResult<TextReader> TextReader() =>
			from readingStream in ReadingStream()
			from reader in tryTo(() => (TextReader)new StreamReader(readingStream))
			select reader;

		public IResult<TextWriter> TextWriter() =>
			from writingStream in WritingStream()
			from writer in tryTo(() => (TextWriter)new StreamWriter(writingStream))
			select writer;
	}
}