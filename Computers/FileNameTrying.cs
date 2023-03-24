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

   public Optional<FileName> Delete() => tryTo(() =>
   {
      fileName.Delete();
      return fileName;
   });

   public FileName FileName => fileName;

   public Optional<FileName> CopyTo(FileName target, bool overwrite = false) => tryTo(() =>
   {
      fileName.CopyTo(target, overwrite);
      return target;
   });

   public Optional<FileName> CopyTo(FolderName targetFolder, bool overwrite = false) => tryTo(() =>
   {
      fileName.CopyTo(targetFolder, overwrite);
      return targetFolder.File(fileName.NameExtension);
   });

   public Optional<FileName> MoveTo(FileName target, bool overwrite = false) => tryTo(() =>
   {
      fileName.MoveTo(target, overwrite);
      return target;
   });

   public Optional<FileName> MoveTo(FolderName targetFolder, bool overwrite = false, bool unique = false)
   {
      return tryTo(() => fileName.MoveTo(targetFolder, overwrite, unique));
   }

   public Optional<bool> Exists() => tryTo(() => fileName.Exists());

   public Optional<FileName> Existing() => Exists().Map(_ => fileName);

   public Optional<string> Text => tryTo(() => fileName.Text);

   public Optional<Unit> SetText(string text) => tryTo(() => fileName.Text = text).Unit;

   public Optional<byte[]> Bytes => tryTo(() => fileName.Bytes);

   public Optional<Unit> SetBytes(byte[] bytes) => tryTo(() => fileName.Bytes = bytes).Unit;

   public Optional<string[]> Lines => tryTo(() => fileName.Lines);

   public Optional<DateTime> CreationTime => tryTo(() => fileName.CreationTime);

   public Optional<Unit> SetCreationTime(DateTime creationTime) => tryTo(() => { fileName.CreationTime = creationTime; });

   public Optional<DateTime> LastAccessTime => tryTo(() => fileName.LastAccessTime);

   public Optional<Unit> SetLastAccessTime(DateTime lastAccessTime) => tryTo(() => { fileName.LastAccessTime = lastAccessTime; });

   public Optional<DateTime> LastWriteTime => tryTo(() => fileName.LastWriteTime);

   public Optional<Unit> SetLastWriteTime(DateTime lastWriteTime) => tryTo(() => { fileName.LastWriteTime = lastWriteTime; });

   public Optional<string> Execute(string arguments, bool wait = true, bool useShellExecute = false, bool createNoWindow = true)
   {
      return tryTo(() => fileName.Execute(arguments, wait, useShellExecute, createNoWindow));
   }

   public Optional<string> Execute(FileName passedFileName, bool wait = true, bool useShellExecute = false,
      bool createNoWindow = true)
   {
      return tryTo(() => fileName.Execute(passedFileName, wait, useShellExecute, createNoWindow));
   }

   public Optional<string> Open(string arguments = "") => tryTo(() => fileName.Open(arguments));

   public Optional<string> Open(FileName file) => tryTo(() => fileName.Open(file));

   public Optional<long> Length => tryTo(() => fileName.Length);

   public Optional<string> Size => tryTo(() => fileName.Size);

   public Optional<FileName> Rename(string name = null, string extension = null, bool overwrite = false, bool unique = false)
   {
      return tryTo(() => fileName.Rename(name, extension, overwrite, unique));
   }

   public Optional<FileNameMappedReader> Reader() => tryTo(() => fileName.Reader());

   public Optional<FileStream> WritingStream() => tryTo(() => fileName.WritingStream());

   public Optional<FileStream> AppendingStream() => tryTo(() => fileName.AppendingStream());

   public Optional<FileStream> ReadingStream() => tryTo(() => fileName.ReadingStream());

   public Optional<FileStream> ReadWriteStream() => tryTo(() => fileName.ReadWriteStream());

   public Optional<TextReader> TextReader()
   {
      return
         from readingStream in ReadingStream()
         from reader in tryTo(() => (TextReader)new StreamReader(readingStream))
         select reader;
   }

   public Optional<TextWriter> TextWriter()
   {
      return
         from writingStream in WritingStream()
         from writer in tryTo(() => (TextWriter)new StreamWriter(writingStream))
         select writer;
   }

   public Optional<string> GetText(Encoding encoding) => tryTo(() => fileName.GetText(encoding));

   public Optional<Unit> SetText(string text, Encoding encoding) => tryTo(() => fileName.SetText(text, encoding));

   public Optional<string[]> SetLines(string[] lines) => tryTo(() => fileName.Lines = lines);

   public Optional<Unit> CreateIfNonexistent() => tryTo(() => fileName.CreateIfNonexistent());

   public Optional<Unit> Append(string text) => tryTo(() => fileName.Append(text));

   public Optional<Unit> Append(string text, Encoding encoding) => tryTo(() => fileName.Append(text, encoding));

   public Optional<FileName> Serialize(int limit = 1000) => tryTo(() => fileName.Serialize(limit));
}