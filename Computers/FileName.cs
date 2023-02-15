using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Core.Assertions;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.Strings;
using static Core.Computers.FullPathFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Computers;

public class FileName : IComparable, IComparable<FileName>, IEquatable<FileName>, IFullPath, IValidPath<FileName>
{
   public class Try
   {
      [Obsolete("Use static FromString")]
      public static Result<FileName> FromString(string file)
      {
         return file.Must().BeAValidFileName().OrFailure().Map(f => (FileName)f);
      }
   }

   public static Result<FileName> FromString(string file) => file.Must().BeAValidFileName().OrFailure().Map(f => (FileName)f);

   [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
   public static extern bool PathYetAnotherMakeUniqueName(StringBuilder uniqueName, string path, string shortTemplate, string longTemplate);

   protected const string REGEX_VALID_FILENAME = @"^ ((['a-zA-Z'] ':' | '\') ('\' -['\']+)* '\')? (-['.']+ ('.' -['.\']+)?) $; f";

   public static bool operator true(FileName file) => file.Exists();

   public static bool operator false(FileName file) => !file.Exists();

   public static bool operator !(FileName file) => !file.Exists();

   public static implicit operator bool(FileName file) => file.Exists();

   public static FolderName ResolveFolder(FolderName folder) => ResolveFolder(folder.ToString());

   public static string ResolveFolder(string folder)
   {
      if (folder.Has("{"))
      {
         return Formatter.WithStandard(true).Format(folder);
      }
      else if (folder == "." || folder.IsEmpty())
      {
         return Environment.CurrentDirectory;
      }
      else
      {
         return folder;
      }
   }

   public static void DeleteExitingFile(FileName target)
   {
      if (target.Exists())
      {
         target.Delete();
      }
   }

   public static FileName RandomFileName(FolderName folder, string extension)
   {
      var fileName = new FileName(folder, "", extension);
      fileName.Uniquify();

      return fileName;
   }

   public static FileName RandomFileName(string extension) => RandomFileName(FolderName.Temp, extension);

   public static FileName UniqueFileName(FolderName folder, string extension) => folder.File(uniqueID(), extension);

   public static FileName UniqueFileName(FolderName folder, string name, string extension)
   {
      return folder.File(UniqueName(folder, name), extension);
   }

   public static Maybe<string> ResolvedFileName(string name)
   {
      try
      {
         var folder = getDirectoryName(name);
         var fileName = Path.GetFileNameWithoutExtension(name);
         var extension = Path.GetExtension(name);

         return Path.Combine(ResolveFolder(folder), fileName + extension);
      }
      catch
      {
         return nil;
      }
   }

   public static bool IsValidUnresolvedFileName(string name) => name.IsMatch(REGEX_VALID_FILENAME);

   public static bool IsValidFileName(string name)
   {
      return ResolvedFileName(name).Map(IsValidUnresolvedFileName) | false;
   }

   public static bool IsValidShortFileName(string name) => name.IsMatch(@"^ (-['//:*?<>|' /n '\']+)+ $; f");

   public static string MakeFileNameValid(string fileName)
   {
      if (fileName.IsMatch("^ 'clock$' | 'aux' | 'con' | 'nul' | 'prn' | 'com' /d | 'lpt' /d; f"))
      {
         fileName += "_" + fileName;
      }

      if (fileName.IsMatch("^ '.'+ $; f"))
      {
         fileName = fileName.Replace(".", "_dot_");
      }

      fileName = fileName.Replace("*", "_star_");
      fileName = fileName.Replace("/", "_slash_");
      fileName = fileName.Replace(":", "_colon_");
      fileName = fileName.Replace("<", "_lt_");
      fileName = fileName.Replace(">", "_gt_");
      fileName = fileName.Replace("?", "_query_");
      fileName = fileName.Replace(@"\", "_backslash_");
      fileName = fileName.Replace("|", "_pipe_");

      return fileName.Truncate(255, false);
   }

   public static implicit operator FileName(string fileName) => new(fileName);

   protected static string getDirectoryName(string fullPath)
   {
      var directoryName = Path.GetDirectoryName(fullPath);
      if (directoryName.IsEmpty())
      {
         directoryName = Environment.CurrentDirectory;
      }

      return directoryName;
   }

   public static bool operator ==(FileName left, FileName right) => left.Equals(right);

   public static bool operator !=(FileName left, FileName right) => !(left == right);

   protected FolderName folder;
   protected string name;
   protected string extension;
   protected string fullPath;
   protected StringBuilder buffer;
   protected bool useBuffer;

   public event EventHandler<FileCopyProgressArgs> Percentage;
   public event EventHandler<FileCopyFinishedArgs> Finished;
   public FileName(FolderName folder, string name, string extension) => initialize(folder, name, extension);

   public FileName(FolderName folder, string name)
   {
      initialize(folder, Path.GetFileNameWithoutExtension(name), Path.GetExtension(name));
   }

   public FileName(string fullPath)
   {
      initialize(getDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath), Path.GetExtension(fullPath));
   }

   public FileName()
   {
      folder = null;
      name = "";
      extension = "";
      fullPath = "";
      Valid = false;
      BufferSize = 2048;
      Encoding = Encoding.ASCII;
      SplitType = SplitType.CRLF;
      useBuffer = true;
   }

   public FolderName Folder
   {
      get => folder;
      set
      {
         folder = value;
         setFullPath();
      }
   }

   public string Name
   {
      get => name;
      set
      {
         name = value;
         setFullPath();
      }
   }

   public string NameExtension
   {
      get => name + extension;
      set
      {
         var result = value.Matches("^ /(.+) '.' /(-['.']+) $; f").Required($"Couldn't extract name and extension from {value}");
         result.Must().HaveMatchCountOf(1).OrThrow($"Couldn't extract name and extension from {value}");

         var fileName = result.FirstGroup;
         var fileExtension = result.SecondGroup;
         name = fileName;
         extension = $".{fileExtension}";
         setFullPath();
      }
   }

   public string Extension
   {
      get => extension;
      set => setExtension(value);
   }

   public string FullPath => fullPath;

   public DateTime CreationTime
   {
      get => info().CreationTime;
      set => File.SetCreationTime(fullPath, value);
   }

   public DateTime LastAccessTime
   {
      get => info().LastAccessTime;
      set => File.SetLastAccessTime(fullPath, value);
   }

   public DateTime LastWriteTime
   {
      get => info().LastWriteTime;
      set => File.SetLastWriteTime(fullPath, value);
   }

   public bool Archive
   {
      get => getAttr(FileAttributes.Archive);
      set => setAttr(FileAttributes.Archive, value);
   }

   public bool Compressed
   {
      get => getAttr(FileAttributes.Compressed);
      set => setAttr(FileAttributes.Compressed, value);
   }

   public bool Directory
   {
      get => getAttr(FileAttributes.Directory);
      set => setAttr(FileAttributes.Directory, value);
   }

   public bool Hidden
   {
      get => getAttr(FileAttributes.Hidden);
      set => setAttr(FileAttributes.Hidden, value);
   }

   public bool ReadOnly
   {
      get => getAttr(FileAttributes.ReadOnly);
      set => setAttr(FileAttributes.ReadOnly, value);
   }

   public bool System
   {
      get => getAttr(FileAttributes.System);
      set => setAttr(FileAttributes.System, value);
   }

   public bool Temporary
   {
      get => getAttr(FileAttributes.Temporary);
      set => setAttr(FileAttributes.Temporary, value);
   }

   public long Length => info().Length;

   public int BufferSize { get; set; }

   public SplitType SplitType { get; set; }

   public bool Locked
   {
      get
      {
         if (Exists())
         {
            var info = new FileInfo(fullPath);
            try
            {
               using (info.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
               {
                  return false;
               }
            }
            catch
            {
               return true;
            }
         }
         else
         {
            return false;
         }
      }
   }

   public Encoding Encoding { get; set; }

   public bool UseBuffer
   {
      get => useBuffer;
      set => useBuffer = value;
   }

   public string Text
   {
      get
      {
         if (Length == 0)
         {
            return "";
         }
         else
         {
            return GetText(Encoding);
         }
      }
      set => SetText(value, Encoding);
   }

   public string GetText(Encoding encoding)
   {
      using var file = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
      using var reader = new StreamReader(file, encoding);

      try
      {
         return reader.ReadToEnd();
      }
      finally
      {
         reader?.Close();
      }
   }

   public void SetText(string text, Encoding encoding)
   {
      using var file = File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
      using var writer = new StreamWriter(file, encoding);

      try
      {
         writer.Write(text);
      }
      finally
      {
         writer.Flush();
         writer.Close();
      }
   }

   public byte[] Bytes
   {
      get
      {
         using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Write);
         var length = (int)stream.Length;
         var bytes = new byte[length];
         _ = stream.Read(bytes, 0, length);

         return bytes;
      }
      set
      {
         using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
         stream.Write(value, 0, value.Length);
         stream.Flush(true);
      }
   }

   public string[] Lines
   {
      get => Text.Unjoin(splitPattern(SplitType));
      set => Text = string.Join(splitLiteral(SplitType), value);
   }

   public int LineCount
   {
      get
      {
         using var reader = new StreamReader(fullPath);
         var count = 0;
         while (reader.ReadLine() != null)
         {
            count++;
         }

         return count;
      }
   }

   public bool Valid { get; private set; }

   public Maybe<FileName> Parent
   {
      get
      {
         var clone = Clone();
         var _folder = clone.Folder.Parent;
         if (_folder)
         {
            clone.Folder = _folder;
            return clone;
         }
         else
         {
            return nil;
         }
      }
   }

   public string[] Parts => fullPath.Unjoin(@"'\'; f");

   public bool IsRooted => Path.IsPathRooted(fullPath);

   public FileVersionInfo FileVersionInfo => FileVersionInfo.GetVersionInfo(fullPath);

   public string ChangeText
   {
      get
      {
         var result = new StringBuilder(name);

         result.Append(extension);
         result.Append(Archive);
         result.Append(Compressed);
         result.Append(Directory);
         result.Append(Hidden);
         result.Append(System);
         result.Append(Temporary);
         result.Append(CreationTime);
         result.Append(LastAccessTime);
         result.Append(LastWriteTime);

         return result.ToString();
      }
   }

   public void Uniquify()
   {
      name = guid();
      setFullPath();
   }

   public Maybe<FileName> Unique()
   {
      var uniqueName = new StringBuilder(260);
      if (PathYetAnotherMakeUniqueName(uniqueName, FullPath, null, NameExtension))
      {
         return Folder + uniqueName.ToString();
      }
      else
      {
         return nil;
      }
   }

   public Maybe<FileName> Truncated(int limit)
   {
      if (limit < 3)
      {
         return nil;
      }
      else if (fullPath.Length <= limit)
      {
         return Clone();
      }
      else
      {
         var parts = fullPath.Unjoin(@"'\'; f");
         string result;
         var index = parts.Length - 4;

         do
         {
            result = join(parts, index);
            index -= 1;
         } while (index >= 0 && result.Length > limit);

         if (result.Length <= limit)
         {
            var truncatedName = new FileName(result);
            truncatedName.replaceTildes();

            return truncatedName;
         }
         else
         {
            return nil;
         }
      }
   }

   public string TruncateBySubfolder(int levels)
   {
      var subfolders = Folder.Subfolders;
      var skipCount = subfolders.Length - levels;
      if (skipCount <= 0)
      {
         return $@"\{NameExtension}";
      }

      {
         var selectedSubfolders = subfolders.Skip(skipCount).ToArray().ToString(@"\");
         return $@"{selectedSubfolders}\{NameExtension}";
      }
   }

   protected void replaceTildes()
   {
      folder = new FolderName(folder.ToString().Replace("~~~", "..."));
      fullPath = fullPath.Replace("~~~", "...");
   }

   protected static string join(string[] parts, int index)
   {
      if (index == 0)
      {
         return @"~~~\" + string.Join(@"\", parts, parts.Length - 2, 2);
      }
      else
      {
         return string.Join(@"\", parts, 0, index) + @"\~~~\" + string.Join(@"\", parts, parts.Length - 2, 2);
      }
   }

   public Result<FileName> Next()
   {
      try
      {
         if (Exists())
         {
            for (var i = 0; i <= 999; i++)
            {
               var suffix = $".{i:D3}";
               var targetFile = Folder.File(name + suffix, extension);
               if (!targetFile.Exists())
               {
                  return targetFile;
               }
            }

            return fail($"Couldn't generate next file for {fullPath}");
         }
         else
         {
            return this;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public FileName Clone() => new(folder.Clone(), name.Copy(), extension.Copy());

   public bool Exists() => File.Exists(fullPath);

   public DateTime Time => CreationTime;

   public void Delete() => File.Delete(fullPath);

   public string NameToMatch => NameExtension;

   public void CopyTo(FileName target, bool overwrite = false)
   {
      target.folder.CreateIfNonExistent();
      File.Copy(fullPath, target.ToString(), overwrite);
   }

   public void CopyTo(FolderName targetFolder, bool overwrite = false)
   {
      targetFolder.CreateIfNonExistent();
      var target = targetFolder.File(NameExtension);
      CopyTo(target, overwrite);
   }

   public void MoveTo(FileName target, bool overwrite = false)
   {
      target.folder.CreateIfNonExistent();
      if (overwrite && target.Exists())
      {
         target.Delete();
      }

      File.Move(fullPath, target.ToString());
   }

   public FileName MoveTo(FolderName targetFolder, bool overwrite = false, bool unique = false)
   {
      var target = MoveTarget(targetFolder, unique);
      MoveTo(target, overwrite);

      return target;
   }

   public static string UniqueName(FolderName folder, string name)
   {
      return $"{name}-{folder.Files.Count(f => f.Name.IsMatch($"'{name}-' /d10; f")).FormatAs("D.10")}";
   }

   public static FileName UniqueFileName(FolderName targetFolder, FileName fileName)
   {
      return targetFolder.File(UniqueName(targetFolder, fileName.Name), fileName.Extension);
   }

   public FileName MoveTarget(FolderName targetFolder, bool unique)
   {
      return unique ? UniqueFileName(targetFolder, this) : targetFolder.File(Name, extension);
   }

   public void WriteTo(FileName target, bool overwriteTarget = false, bool deleteSelf = false)
   {
      if (overwriteTarget)
      {
         target.Delete();
      }

      var workingFile = CreateRandomFileName(Folder, "working");
      workingFile.Text = Text;
      workingFile.MoveTo(target);
      if (deleteSelf)
      {
         Delete();
      }
   }

   public void WriteToBinary(FileName target, bool overwriteTarget, bool deleteSelf)
   {
      if (overwriteTarget)
      {
         target.Delete();
      }

      var workingFile = CreateRandomFileName(Folder, "working");
      workingFile.Bytes = Bytes;
      workingFile.MoveTo(target);
      if (deleteSelf)
      {
         Delete();
      }
   }

   public void WriteToBinary(FileName target, bool overwriteTarget = false) => WriteToBinary(target, overwriteTarget, false);

   protected FileName CreateRandomFileName(FolderName folderName, string subfolder)
   {
      var newFolder = folderName[subfolder];
      newFolder.CreateIfNonExistent();

      return newFolder + uniqueID() + extension;
   }

   protected bool getAttr(FileAttributes attribute) => Bits32<FileAttributes>.GetBit(info().Attributes, attribute);

   protected void setAttr(FileAttributes attribute, bool value)
   {
      var info = this.info();
      info.Attributes = Bits32<FileAttributes>.SetBit(info.Attributes, attribute, value);
   }

   protected void setFullPath()
   {
      try
      {
         fullPath = Path.Combine(folder.ToString(), name + extension);
         Valid = true;
      }
      catch
      {
         Valid = false;
      }
   }

   protected void initialize(FolderName aFolder, string aName, string anExtension)
   {
      if (aName.IsMatch("^ '~'; f"))
      {
         var fullName = ComputerFunctions.replaceTilde(aFolder.ToString());
         folder = getDirectoryName(fullName);
         name = Path.GetFileNameWithoutExtension(fullName);
      }
      else
      {
         folder = ResolveFolder(aFolder);
         name = aName;
      }

      setExtension(anExtension);
      setFullPath();
      BufferSize = 2048;
      Encoding = Encoding.ASCII;
      SplitType = SplitType.CRLF;
   }

   protected void setExtension(string anExtension)
   {
      if (anExtension.IsEmpty())
      {
         extension = "";
      }
      else if (anExtension.StartsWith("."))
      {
         extension = anExtension;
      }
      else
      {
         extension = "." + anExtension;
      }

      setFullPath();
   }

   protected FileInfo info() => new(fullPath);

   public void AppendToExtension(string anExtension)
   {
      if (anExtension.IsNotEmpty())
      {
         if (anExtension.StartsWith("."))
         {
            extension += anExtension;
         }
         else
         {
            extension += "." + anExtension;
         }

         setFullPath();
      }
   }

   public FileName Rename(string name = null, string extension = null, bool overwrite = false, bool unique = false)
   {
      var newFile = Clone();
      if (name != null)
      {
         newFile.Name = name;
      }

      if (extension != null)
      {
         newFile.Extension = extension;
      }

      if (unique)
      {
         newFile.Name = UniqueName(Folder, newFile.Name);
      }

      MoveTo(newFile, overwrite);

      return newFile;
   }

   public string GetText(int start, int length)
   {
      var charBuffer = new char[length];

      using var reader = new StreamReader(fullPath);
      var actualLength = reader.Read(charBuffer, start, length);
      return actualLength == 0 ? "" : new string(charBuffer, start, actualLength);
   }

   public string GetText(int start, int length, Encoding encoding)
   {
      var charBuffer = new char[length];

      using var reader = new StreamReader(fullPath, encoding);
      var actualLength = reader.Read(charBuffer, start, length);

      return actualLength == 0 ? "" : new string(charBuffer, start, actualLength);
   }

   public string GetText(int length) => GetText(0, length);

   public string GetText(int length, Encoding encoding) => GetText(0, length, encoding);

   public byte[] GetBytes(int start, int length)
   {
      using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
      var bytes = new byte[length];
      _ = stream.Read(bytes, start, length);

      return bytes;
   }

   public byte[] GetBytes(int length) => GetBytes(0, length);

   protected StringBuilder getBuffer() => buffer ??= new StringBuilder();

   protected void appendText(string text) => appendText(text, Encoding);

   protected void appendText(string text, Encoding encoding)
   {
      folder.CreateIfNonExistent();
      using var file = File.Open(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
      using var writer = new StreamWriter(file, encoding);
      writer.Write(text);
   }

   public void Append(string text) => Append(text, Encoding);

   public void Append(string text, Encoding encoding)
   {
      if (useBuffer)
      {
         var textBuffer = getBuffer();
         folder.CreateIfNonExistent();
         textBuffer.Append(text);
         if (textBuffer.Length > BufferSize)
         {
            appendText(textBuffer.ToString());
            textBuffer.Length = 0;
         }
      }
      else
      {
         appendText(text, encoding);
      }
   }

   public void Append(byte[] bytes)
   {
      folder.CreateIfNonExistent();
      using var stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.Write);
      stream.Write(bytes, 0, bytes.Length);
   }

   public void Append(string[] lines)
   {
      var stringBuffer = getBuffer();
      folder.CreateIfNonExistent();
      foreach (var line in lines)
      {
         stringBuffer.Append(line);

         if (stringBuffer.Length > BufferSize)
         {
            appendText(stringBuffer.ToString());
            stringBuffer.Length = 0;
         }
      }

      Flush();
   }

   public void Flush()
   {
      var stringBuffer = getBuffer();

      if (stringBuffer.Length > 0)
      {
         appendText(stringBuffer.ToString());
         stringBuffer.Length = 0;
      }
   }

   public int CompareTo(FileName other) => ToString().CompareTo(other.ToString());

   public bool Equals(FileName other) => fullPath == (other?.fullPath ?? "");

   public override string ToString() => fullPath;

   [Obsolete("Use Uri")]
   public virtual string ToURL(string host)
   {
      return $"file://{host}/{fullPath.Substitute("^ /l ':\'; f", "").Substitute(@"^ '\\'; f", "").Substitute(@"'\'+; f", "/")}";
   }

   [Obsolete("Use Uri")]
   public virtual string ToURL()
   {
      return $"file://{fullPath.Substitute("^ /l ':\'; f", "").Substitute("^ '\\'; f", "").Substitute(@"'\'+; f", "/")}";
   }

   public virtual string Uri(UriKind uriKind = UriKind.Absolute)
   {
      var uri = new Uri(FullPath, uriKind);
      return uri.AbsoluteUri;
   }

   public Process Start() => global::System.Diagnostics.Process.Start(fullPath);

   public Process Start(string arguments) => global::System.Diagnostics.Process.Start(fullPath, arguments);

   public Process Process() => new() { StartInfo = new ProcessStartInfo(fullPath) };

   public Process Process(string arguments) => new() { StartInfo = new ProcessStartInfo(fullPath, arguments) };

   public int ExitCode { get; set; }

   public string Execute(string arguments, bool wait = true, bool useShellExecute = false, bool createNoWindow = true)
   {
      using var process = Process();
      process.StartInfo = new ProcessStartInfo(fullPath, arguments)
      {
         UseShellExecute = useShellExecute, CreateNoWindow = createNoWindow
      };
      if (wait)
      {
         process.StartInfo.RedirectStandardOutput = true;
         process.StartInfo.RedirectStandardError = true;
         process.StartInfo.RedirectStandardInput = true;
      }

      process.Start();
      if (wait)
      {
         process.WaitForExit();
         ExitCode = process.ExitCode;
         return process.StandardOutput.ReadToEnd();
      }
      else
      {
         return "";
      }
   }

   public string Execute(FileName fileName, bool wait = true, bool useShellExecute = false, bool createNoWindow = true)
   {
      return Execute(fileName.ToString().Quotify(), wait, useShellExecute, createNoWindow);
   }

   public FileStream WritingStream(bool shared = false)
   {
      folder.CreateIfNonExistent();
      var fileShare = shared ? FileShare.ReadWrite : FileShare.Write;

      return new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write, fileShare);
   }

   public FileStream AppendingStream(bool shared = false)
   {
      folder.CreateIfNonExistent();
      var fileShare = shared ? FileShare.ReadWrite : FileShare.Write;

      return new FileStream(fullPath, FileMode.Append, FileAccess.Write, fileShare);
   }

   public FileStream ReadingStream(bool shared = false)
   {
      var fileShare = shared ? FileShare.ReadWrite : FileShare.Read;
      return new FileStream(fullPath, FileMode.Open, FileAccess.Read, fileShare);
   }

   public FileStream ReadWriteStream()
   {
      return new(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
   }

   public void MakeEmpty(bool hidden)
   {
      File.Open(fullPath, FileMode.Create).Dispose();
      Hidden = hidden;
   }

   public void Clear()
   {
      using var stream = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
      using var writer = new StreamWriter(stream, Encoding);
      writer.Write("");
      writer.Flush();
   }

   public override int GetHashCode() => fullPath.GetHashCode();

   public int CompareTo(object obj) => obj is FileName fn ? ToString().CompareTo(fn.ToString()) : -1;

   public override bool Equals(object obj) => obj is FileName otherFileName && Equals(otherFileName);

   public FileNameMappedReader Reader() => new(this);

   public FileNameTrying TryTo => new(this);

   public Result<object> NewObject(string typeName, params object[] args) => tryTo(() =>
   {
      var assembly = Assembly.LoadFile(fullPath);
      var type = assembly.GetType(typeName, true);

      return type.New(args);
   });

   public string Size => Length.ByteSize();

   public void CreateIfNonexistent()
   {
      if (!Exists())
      {
         Text = "";
      }
   }

   public bool IsEmpty => fullPath.IsEmpty();

   public bool IsNotEmpty => !IsEmpty;

   public Maybe<FileName> Indexed(int maximumIndex = 1000)
   {
      for (var index = 0; index < maximumIndex; index++)
      {
         var formattedIndex = index.ToString().PadLeft(3, '0');
         var newName = $"{name}.{formattedIndex}{extension}";
         var newFile = folder + newName;
         if (!newFile)
         {
            return newFile;
         }
      }

      return nil;
   }

   public FileNameCore Core => new(this);

   public Result<FileName> Validate(bool allowRelativePaths = false) => ValidatePath(this, allowRelativePaths).Map(s => (FileName)s);

   public bool IsValid => Validate(true);

   public FileName Serialize(int limit = 1000)
   {
      var baseName = name;
      var _length = baseName.Matches("'-' /d+ $").Map(r => r.Length);
      if (_length)
      {
         baseName = baseName.Drop(-_length);
      }

      var currentFile = new FileName(folder, baseName, extension);
      if (currentFile)
      {
         for (var i = 1; i < limit; i++)
         {
            var newName = $"{name}-{i}";
            var newFile = new FileName(folder, newName, extension);
            if (!newFile)
            {
               return newFile;
            }
         }

         throw fail($"Limited to {limit}");
      }
      else
      {
         return currentFile;
      }
   }

   public IEnumerable<string> Reading()
   {
      using var file = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
      using var reader = new StreamReader(file, Encoding);

      while (true)
      {
         var line = reader.ReadLine();
         if (line is null)
         {
            break;
         }

         yield return line;
      }

      reader?.Dispose();
      file?.Dispose();
   }

   protected enum CopyProgressResult : uint
   {
      ProgressContinue = 0,
      ProgressCancel = 1,
      ProgressStop = 2,
      ProgressQuiet = 3
   }

   protected enum CopyProgressCallbackReason : uint
   {
      CallbackChunkFinished = 0x00000000,
      CallbackStreamSwitch = 0x00000001
   }

   [Flags]
   protected enum CopyFileFlags : uint
   {
      CopyFileFailIfExists = 0x00000001,
      CopyFileNoBuffering = 0x00001000,
      CopyFileRestartable = 0x00000002,
      CopyFileOpenSourceForWrite = 0x00000004,
      CopyFileAllowDecryptedDestination = 0x00000008
   }

   protected delegate CopyProgressResult CopyProgressRoutine(long totalFileSize, long totalBytesTransferred, long streamSize,
      long streamBytesTransferred, uint streamNumber, CopyProgressCallbackReason callbackReason, IntPtr sourceFile, IntPtr destinationFile,
      IntPtr data);

   [DllImport("kernel32", SetLastError = true)]
   protected static extern bool CopyFileEx(string existingFile, string newFileName, CopyProgressRoutine progressRoutine, IntPtr data, ref int cancel,
      CopyFileFlags copyFlags);

   public Result<Unit> CopyToNotify(FileName targetFile, bool overwrite = true, bool noBuffering = false)
   {
      var stopwatch = new Stopwatch();

      CopyProgressResult copyProgressHandler(long total, long transferred, long streamSize, long streamByteTransferred, uint streamNumber,
         CopyProgressCallbackReason reason, IntPtr sourceFile, IntPtr destinationFile, IntPtr data)
      {
         if (reason == CopyProgressCallbackReason.CallbackChunkFinished)
         {
            Percentage?.Invoke(this, new FileCopyProgressArgs(transferred / (double)total * 100));
         }

         if (transferred >= total)
         {
            Finished?.Invoke(this, new FileCopyFinishedArgs(total, stopwatch.Elapsed));
         }

         return CopyProgressResult.ProgressContinue;
      }

      try
      {
         stopwatch.Start();

         Bits32<CopyFileFlags> copyFileFlags = CopyFileFlags.CopyFileRestartable;
         copyFileFlags[CopyFileFlags.CopyFileFailIfExists] = !overwrite;
         copyFileFlags[CopyFileFlags.CopyFileNoBuffering] = noBuffering;

         var cancel = 0;
         var result = CopyFileEx(fullPath, targetFile.fullPath, copyProgressHandler, IntPtr.Zero, ref cancel, copyFileFlags);

         stopwatch.Stop();

         return result ? unit : new Win32Exception(Marshal.GetLastWin32Error());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<Unit> CopyToNotify(FolderName folder, bool overwrite = true, bool noBuffering = false)
   {
      var targetFile = folder + NameExtension;
      return CopyToNotify(targetFile, overwrite, noBuffering);
   }
}