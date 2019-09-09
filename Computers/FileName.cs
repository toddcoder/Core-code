using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Computers
{
   public class FileName : IComparable, IComparable<FileName>
   {
      public class Try
      {
         public static IResult<FileName> FromString(string file)
         {
            return assert(IsValidFileName(file), () => new FileName(file), () => $"{file} is an invalid file name");
         }
      }

      const string REGEX_VALID_FILENAME = @"^ ((['a-zA-Z'] ':' | '\') ('\' -['\']+)* '\')? (-['.']+ ('.' -['.\']+)?) $";

      public static FileName operator |(FileName target, FileName source)
      {
         source.CopyTo(target, true);
         return target;
      }

      public static FileName operator &(FileName target, FileName source)
      {
         source.MoveTo(target, true);
         return target;
      }

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

      public static IMaybe<string> ResolvedFileName(string name)
      {
         try
         {
            var folder = getDirectoryName(name);
            var fileName = Path.GetFileNameWithoutExtension(name);
            var extension = Path.GetExtension(name);

            return Path.Combine(ResolveFolder(folder), fileName + extension).Some();
         }
         catch
         {
            return none<string>();
         }
      }

      public static bool IsValidUnresolvedFileName(string name) => name.IsMatch(REGEX_VALID_FILENAME);

      public static bool IsValidFileName(string name)
      {
         return ResolvedFileName(name).FlatMap(IsValidUnresolvedFileName, () => false);
      }

      public static bool IsValidShortFileName(string name) => name.IsMatch(@"^ (-['//:*?<>|' /n '\']+)+ $");

      public static string MakeFileNameValid(string fileName)
      {
         if (fileName.IsMatch("^ 'clock$' | 'aux' | 'con' | 'nul' | 'prn' | 'com' /d | 'lpt' /d"))
         {
            fileName += "_" + fileName;
         }

         if (fileName.IsMatch("^ '.'+ $"))
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

      public static implicit operator FileName(string fileName) => new FileName(fileName);

      static string getDirectoryName(string fullPath)
      {
         var directoryName = Path.GetDirectoryName(fullPath);
         if (directoryName.IsEmpty())
         {
            directoryName = Environment.CurrentDirectory;
         }

         return directoryName;
      }

      public static bool operator ==(FileName left, FileName right) => (left?.ToString() ?? "") == (right?.ToString() ?? "");

      public static bool operator !=(FileName left, FileName right) => !(left == right);

      FolderName folder;
      string name;
      string extension;
      string fullPath;
      StringBuilder buffer;
      bool useBuffer;

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
            var matcher = new Matcher();
            matcher.RequiredMatch(value, "^ /(.+) '.' /(-['.']+) $", $"Couldn't extract name and extension from {value}");

            var fileName = matcher.FirstGroup;
            var fileExtension = matcher.SecondGroup;
            name = fileName;
            extension = "." + fileExtension;
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
               using (var reader = new FileNameMappedReader(this))
               {
                  return reader.ReadToEnd();
               }
            }
         }
         set
         {
            using (var file = File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(file, Encoding))
            {
               writer.Write(value);
               writer.Flush();
               writer.Close();
            }
         }
      }

      public string GetText(Encoding encoding)
      {
         using (var reader = new FileNameMappedReader(this, encoding))
         {
            return reader.ReadToEnd();
         }
      }

      public void SetText(string text, Encoding encoding)
      {
         using (var file = File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
         using (var writer = new StreamWriter(file, encoding))
         {
            writer.Write(text);
            writer.Flush();
            writer.Close();
         }
      }

      public byte[] Bytes
      {
         get
         {
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Write))
            {
               var length = (int)stream.Length;
               var bytes = new byte[length];
               stream.Read(bytes, 0, length);

               return bytes;
            }
         }
         set
         {
            using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
               stream.Write(value, 0, value.Length);
               stream.Flush(true);
            }
         }
      }

      public string[] Lines
      {
         get => Text.Split(splitPattern(SplitType));
         set => Text = string.Join(splitLiteral(SplitType), value);
      }

      public int LineCount
      {
         get
         {
            using (var reader = new StreamReader(fullPath))
            {
               var count = 0;
               while (reader.ReadLine() != null)
               {
                  count++;
               }

               return count;
            }
         }
      }

      public bool Valid { get; private set; }

      public IMaybe<FileName> Parent
      {
         get
         {
            var clone = Clone();
            if (clone.Folder.Parent.If(out var folderName))
            {
               clone.Folder = folderName;
               return clone.Some();
            }
            else
            {
               return none<FileName>();
            }
         }
      }

      public string[] Parts => fullPath.Split(@"'\'");

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

      public IMaybe<FileName> Truncated(int limit)
      {
         if (limit < 3)
         {
            return none<FileName>();
         }
         else if (fullPath.Length <= limit)
         {
            return Clone().Some();
         }
         else
         {
            var parts = fullPath.Split(@"'\'");
            string result;
            var index = parts.Length - 4;

            do
            {
               result = join(parts, index);
               index = -1;
            } while (index >= 0 && result.Length > limit);

            if (result.Length <= limit)
            {
               var truncatedName = new FileName(result);
               truncatedName.replaceTildes();

               return truncatedName.Some();
            }
            else
            {
               return none<FileName>();
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
            var selectedSubfolders = subfolders.Skip(skipCount).ToArray().Stringify(@"\");
            return $@"{selectedSubfolders}\{NameExtension}";
         }
      }

      void replaceTildes()
      {
         folder = new FolderName(folder.ToString().Replace("~~~", "..."));
         fullPath = fullPath.Replace("~~~", "...");
      }

      static string join(string[] parts, int index)
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

      public IResult<FileName> Next()
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
                     return targetFile.Success();
                  }
               }

               return $"Couldn't generate next file for {fullPath}".Failure<FileName>();
            }
            else
            {
               return this.Success();
            }
         }
         catch (Exception exception)
         {
            return failure<FileName>(exception);
         }
      }

      public FileName Clone() => new FileName(folder.Clone(), name.Copy(), extension.Copy());

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
         return $"{name}-{folder.Files.Count(f => f.Name.IsMatch($"'{name}-' /d10")).FormatAs("D.10")}";
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

      FileName CreateRandomFileName(FolderName folderName, string subfolder)
      {
         var newFolder = folderName[subfolder];
         newFolder.CreateIfNonExistent();

         return newFolder + uniqueID() + extension;
      }

      bool getAttr(FileAttributes attribute) => Bits32<FileAttributes>.GetBit(info().Attributes, attribute);

      void setAttr(FileAttributes attribute, bool value)
      {
         var info = this.info();
         info.Attributes = Bits32<FileAttributes>.SetBit(info.Attributes, attribute, value);
      }

      void setFullPath()
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

      void initialize(FolderName aFolder, string aName, string anExtension)
      {
         if (aName.IsMatch("^ '~'"))
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

      FileInfo info() => new FileInfo(fullPath);

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

         using (var reader = new StreamReader(fullPath))
         {
            var actualLength = reader.Read(charBuffer, start, length);
            return actualLength == 0 ? "" : new string(charBuffer, start, actualLength);
         }
      }

      public string GetText(int start, int length, Encoding encoding)
      {
         var charBuffer = new char[length];

         using (var reader = new StreamReader(fullPath, encoding))
         {
            var actualLength = reader.Read(charBuffer, start, length);
            return actualLength == 0 ? "" : new string(charBuffer, start, actualLength);
         }
      }

      public string GetText(int length) => GetText(0, length);

      public string GetText(int length, Encoding encoding) => GetText(0, length, encoding);

      public byte[] GetBytes(int start, int length)
      {
         using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
         {
            var bytes = new byte[length];
            stream.Read(bytes, start, length);

            return bytes;
         }
      }

      public byte[] GetBytes(int length) => GetBytes(0, length);

      StringBuilder getBuffer() => buffer ?? (buffer = new StringBuilder());

      void appendText(string text)
      {
         folder.CreateIfNonExistent();
         using (var file = File.Open(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
         using (var writer = new StreamWriter(file, Encoding))
         {
            writer.Write(text);
         }
      }

      public void Append(string text)
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
            appendText(text);
         }
      }

      public void Append(string format, params object[] args) => Append(string.Format(format, args));

      public void Append(byte[] bytes)
      {
         folder.CreateIfNonExistent();
         using (var stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.Write))
         {
            stream.Write(bytes, 0, bytes.Length);
         }
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

      public override string ToString() => fullPath;

      public virtual string ToURL(string host)
      {
         return $"file://{host}/{fullPath.Substitute("^ /l ':\'", "").Substitute(@"^ '\\'", "").Substitute("'\'+", "/")}";
      }

      public virtual string ToURL()
      {
         return $"file://{fullPath.Substitute("^ /l ':\'", "").Substitute("^ '\\'", "").Substitute("'\'+", "/")}";
      }

      public Process Start() => global::System.Diagnostics.Process.Start(fullPath);

      public Process Start(string arguments) => global::System.Diagnostics.Process.Start(fullPath, arguments);

      public Process Process() => new Process { StartInfo = new ProcessStartInfo(fullPath) };

      public Process Process(string arguments) => new Process { StartInfo = new ProcessStartInfo(fullPath, arguments) };

      public int ExitCode { get; set; }

      public string Execute(string arguments, bool wait = true, bool useShellExecute = false, bool createNoWindow = true)
      {
         using (var process = Process())
         {
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
      }

      public string Execute(FileName fileName, bool wait = true, bool useShellExecute = false, bool createNoWindow = true)
      {
         return Execute(fileName.ToString().Quotify(), wait, useShellExecute, createNoWindow);
      }

      public FileStream WritingStream(bool shared = false)
      {
         folder.CreateIfNonExistent();
         var fileShare = shared ? FileShare.ReadWrite : FileShare.Read;

         return new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Read, fileShare);
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
         return new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
      }

      public void MakeEmpty(bool hidden)
      {
         File.Open(fullPath, FileMode.Create).Dispose();
         Hidden = hidden;
      }

      public void Clear()
      {
         using (var stream = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
         using (var writer = new StreamWriter(stream, Encoding))
         {
            writer.Write("");
            writer.Flush();
         }
      }

      public override int GetHashCode() => fullPath.GetHashCode();

      public int CompareTo(object obj) => obj is FileName fn ? ToString().CompareTo(fn.ToString()) : -1;

      public override bool Equals(object obj) => obj is FileName fn && fullPath.Same(fn.ToString());

      public FileNameMappedReader Reader() => new FileNameMappedReader(this);

      public FileNameTrying TryTo => new FileNameTrying(this);

      public IResult<object> NewObject(string typeName, params object[] args) => tryTo(() =>
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
   }
}