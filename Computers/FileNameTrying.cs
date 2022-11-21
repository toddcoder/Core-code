using System;
using System.IO;
using System.Text;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Computers;

public class FileNameTrying
{
   protected FileName fileName;

   public FileNameTrying(FileName fileName) => this.fileName = fileName;

   public Result<FileName> Delete() => tryTo(() =>
   {
      fileName.Delete();
      return fileName;
   });

   public FileName FileName => fileName;

   public Result<FileName> CopyTo(FileName target, bool overwrite = false) => tryTo(() =>
   {
      fileName.CopyTo(target, overwrite);
      return target;
   });

   public Result<FileName> CopyTo(FolderName targetFolder, bool overwrite = false) => tryTo(() =>
   {
      fileName.CopyTo(targetFolder, overwrite);
      return targetFolder.File(fileName.NameExtension);
   });

   public Result<FileName> MoveTo(FileName target, bool overwrite = false) => tryTo(() =>
   {
      fileName.MoveTo(target, overwrite);
      return target;
   });

   public Result<FileName> MoveTo(FolderName targetFolder, bool overwrite = false, bool unique = false)
   {
      return tryTo(() => fileName.MoveTo(targetFolder, overwrite, unique));
   }

   public Result<bool> Exists() => tryTo(() => fileName.Exists());

   public Result<FileName> Existing() => Exists().Map(_ => fileName);

   public Result<string> Text => tryTo(() => fileName.Text);

   public Result<Unit> SetText(string text) => tryTo(() => fileName.Text = text).Unit;

   public Result<byte[]> Bytes => tryTo(() => fileName.Bytes);

   public Result<Unit> SetBytes(byte[] bytes) => tryTo(() => fileName.Bytes = bytes).Unit;

   public Result<string[]> Lines => tryTo(() => fileName.Lines);

   public Result<DateTime> CreationTime => tryTo(() => fileName.CreationTime);

   public Result<Unit> SetCreationTime(DateTime creationTime) => tryTo(() => { fileName.CreationTime = creationTime; });

   public Result<DateTime> LastAccessTime => tryTo(() => fileName.LastAccessTime);

   public Result<Unit> SetLastAccessTime(DateTime lastAccessTime) => tryTo(() => { fileName.LastAccessTime = lastAccessTime; });

   public Result<DateTime> LastWriteTime => tryTo(() => fileName.LastWriteTime);

   public Result<Unit> SetLastWriteTime(DateTime lastWriteTime) => tryTo(() => { fileName.LastWriteTime = lastWriteTime; });

   public Result<string> Execute(string arguments, bool wait = true, bool useShellExecute = false, bool createNoWindow = true)
   {
      return tryTo(() => fileName.Execute(arguments, wait, useShellExecute, createNoWindow));
   }

   public Result<string> Execute(FileName passedFileName, bool wait = true, bool useShellExecute = false,
      bool createNoWindow = true)
   {
      return tryTo(() => fileName.Execute(passedFileName, wait, useShellExecute, createNoWindow));
   }

   public Result<long> Length => tryTo(() => fileName.Length);

   public Result<string> Size => tryTo(() => fileName.Size);

   public Result<FileName> Rename(string name = null, string extension = null, bool overwrite = false, bool unique = false)
   {
      return tryTo(() => fileName.Rename(name, extension, overwrite, unique));
   }

   public Result<FileNameMappedReader> Reader() => tryTo(() => fileName.Reader());

   public Result<FileStream> WritingStream() => tryTo(() => fileName.WritingStream());

   public Result<FileStream> AppendingStream() => tryTo(() => fileName.AppendingStream());

   public Result<FileStream> ReadingStream() => tryTo(() => fileName.ReadingStream());

   public Result<FileStream> ReadWriteStream() => tryTo(() => fileName.ReadWriteStream());

   public Result<TextReader> TextReader()
   {
      return
         from readingStream in ReadingStream()
         from reader in tryTo(() => (TextReader)new StreamReader(readingStream))
         select reader;
   }

   public Result<TextWriter> TextWriter()
   {
      return
         from writingStream in WritingStream()
         from writer in tryTo(() => (TextWriter)new StreamWriter(writingStream))
         select writer;
   }

   public Result<string> GetText(Encoding encoding) => tryTo(() => fileName.GetText(encoding));

   public Result<Unit> SetText(string text, Encoding encoding) => tryTo(() => fileName.SetText(text, encoding));

   public Result<string[]> SetLines(string[] lines) => tryTo(() => fileName.Lines = lines);

   public Result<Unit> CreateIfNonexistent() => tryTo(() => fileName.CreateIfNonexistent());

   public Result<Unit> Append(string text) => tryTo(() => fileName.Append(text));

   public Result<Unit> Append(string text, Encoding encoding) => tryTo(() => fileName.Append(text, encoding));

   public Result<FileName> Serialize(int limit = 1000) => tryTo(() => fileName.Serialize(limit));
}