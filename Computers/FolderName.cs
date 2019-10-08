﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Core.Arrays;
using Core.Dates.Now;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.RegularExpressions;
using Core.Strings;
using static System.IO.Directory;
using static Core.Computers.ComputerFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Computers
{
   public class FolderName : IComparable, IComparable<FolderName>, IEquatable<FolderName>
   {
      public class Try
      {
         public static IResult<FolderName> FromString(string folder)
         {
            return assert(IsValidFolderName(folder), () => new FolderName(folder), () => $"{folder} is an invalid folder name");
         }
      }

      const int MAX_PATH = 260;

      public static implicit operator FolderName(string folder) => new FolderName(folder);

      public static bool operator ==(FolderName lhs, FolderName rhs) => lhs.Equals(rhs);

      public static bool operator !=(FolderName lhs, FolderName rhs) => !(lhs == rhs);

      public static Target operator |(FolderName folder, Target.Option option) => new Target(folder, option);

      public static FileName operator |(FolderName folder, FileName file)
      {
         file.CopyTo(folder);

         return file;
      }

      public static FileName operator &(FolderName folder, FileName file)
      {
         file.MoveTo(folder);

         return file;
      }

      public static FolderName Create(string folder)
      {
         var newFolder = new FolderName(folder);
         newFolder.CreateIfNonExistent();

         return newFolder;
      }

      public static FolderName CreateRootOnly(string root) => new FolderName(root, new string[0]);

      static FolderName specialFolder(Environment.SpecialFolder folder) => Environment.GetFolderPath(folder);

      public static FolderName Temp => Path.GetTempPath();

      public static FolderName MyDocuments => specialFolder(Environment.SpecialFolder.MyDocuments);

      public static FolderName ApplicationData => specialFolder(Environment.SpecialFolder.ApplicationData);

      public static FolderName LocalApplicationData => specialFolder(Environment.SpecialFolder.LocalApplicationData);

      public static FolderName UserProfile => specialFolder(Environment.SpecialFolder.UserProfile);

      public static FolderName Windows => Environment.GetEnvironmentVariable("windir");

      public static FolderName System => Environment.SystemDirectory;

      public static FolderName Current
      {
         get => Environment.CurrentDirectory;
         set => Environment.CurrentDirectory = value.FullPath;
      }

      public static FolderName ProgramFiles => specialFolder(Environment.SpecialFolder.ProgramFiles);

      public static FolderName ProgramFilesX86 => specialFolder(Environment.SpecialFolder.ProgramFilesX86);

      public static FolderName CorporateBase => @"C:\Enterprise";

      public static FolderName Configurations => CorporateBase["Configurations"];

      public static IMaybe<FolderName> ExecutableFolder
      {
         get
         {
            var matcher = new Matcher();
            if (matcher.IsMatch(Environment.GetCommandLineArgs()[0], @"^ /(.+) '\' -['\']+ '.'('exe' | 'dll') $"))
            {
               return ((FolderName)matcher.FirstGroup).Some();
            }
            else
            {
               return none<FolderName>();
            }
         }
      }

      public static bool IsValidFolderName(string name) => IsValidUnresolvedFolderName(FileName.ResolveFolder(name));

      public static bool IsValidUnresolvedFolderName(string name)
      {
         return FileName.IsValidUnresolvedFileName(name) || name.IsMatch(@"^ ['a-z'] ':\\' $");
      }

      public static FileName operator +(FolderName folder, string file) => folder.File(file);

      public static FileName operator +(FolderName folder, FileName file) => folder.File(file.NameExtension);

      [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
      static extern bool PathRelativePathTo(StringBuilder path, string from, FileAttributes fromAttributes, string to,
         FileAttributes toAttributes);

      protected string root;
      protected string[] subfolders;
      protected string fullPath;
      protected Equatable<FolderName> equatable;

      public event EventHandler<FileArgs> FileSuccess;

      public FolderName(string folder)
      {
         setFullPath(folder);
         equatable = new Equatable<FolderName>(this, "fullPath");
      }

      public FolderName(string root, params string[] subfolders)
      {
         if (subfolders.Length == 0)
         {
            initialize(root);
         }
         else
         {
            initialize(root, subfolders);
         }
      }

      public FolderName(string root, string subfolders) => initialize(root, getSubfolders(subfolders));

      public FolderName()
      {
         root = "";
         subfolders = new string[] { };
         fullPath = "";
         Valid = false;
         Encoding = Encoding.ASCII;
      }

      DirectoryInfo info() => new DirectoryInfo(fullPath);

      bool getAttr(FileAttributes attribute) => Bits32<FileAttributes>.GetBit(info().Attributes, attribute);

      void setAttr(FileAttributes attribute, bool value)
      {
         var info = this.info();
         info.Attributes = Bits32<FileAttributes>.SetBit(info.Attributes, attribute, value);
      }

      public DateTime CreationTime
      {
         get => info().CreationTime;
         set => info().CreationTime = value;
      }

      public DateTime LastAccessTime
      {
         get => info().LastAccessTime;
         set => info().LastAccessTime = value;
      }

      public DateTime LastWriteTime
      {
         get => info().LastWriteTime;
         set => info().LastWriteTime = value;
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

      public bool Temporary
      {
         get => getAttr(FileAttributes.Temporary);
         set => setAttr(FileAttributes.Temporary, value);
      }

      public FolderName this[string subfolder] => new FolderName(root, subfolders.Append(subfolder));

      public IMaybe<FolderName> Parent
      {
         get
         {
            if (subfolders.Length > 0)
            {
               var length = subfolders.Length - 1;
               if (length <= 0)
               {
                  return CreateRootOnly(root).Some();
               }
               else
               {
                  var parentArray = new string[length];
                  Array.ConstrainedCopy(subfolders, 0, parentArray, 0, length);
                  return new FolderName(root, parentArray).Some();
               }
            }
            else
            {
               return none<FolderName>();
            }
         }
      }

      public IMaybe<FolderName> Parents(int count)
      {
         if (count > 0)
         {
            var result = none<FolderName>();
            var self = this;
            for (var i = 0; i < count; i++)
            {
               result = self.Parent;
               if (result.If(out var parent))
               {
                  self = parent;
               }
               else
               {
                  return result;
               }
            }

            return result;
         }
         else
         {
            return none<FolderName>();
         }
      }

      public string FullPath
      {
         get => fullPath;
         set => setFullPath(value);
      }

      public string Root
      {
         get => root;
         set
         {
            root = value;
            setFullPath();
         }
      }

      public string Name
      {
         get => subfolders.Last("");
         set
         {
            subfolders[subfolders.Length - 1] = value;
            setFullPath();
         }
      }

      public string[] Subfolders
      {
         get => subfolders;
         set
         {
            subfolders = value;
            setFullPath();
         }
      }

      public string NonRoot
      {
         get => string.Join(@"\", subfolders);
         set
         {
            if (value.IsNotEmpty())
            {
               var newValue = value;
               while (newValue.StartsWith(@"\"))
               {
                  newValue = newValue.Substring(1);
               }

               subfolders = newValue.Split('\\');
            }
            else
            {
               subfolders = new string[0];
            }
         }
      }

      public string[] SubfoldersReversed
      {
         get
         {
            if (subfolders.Length > 0)
            {
               var copy = new string[subfolders.Length];
               subfolders.CopyTo(copy, 0);

               return copy.Reverse().ToArray();
            }
            else
            {
               return subfolders;
            }
         }
      }

      public Encoding Encoding { get; set; }

      public IEnumerable<FileName> Files => getFiles(fullPath);

      public IEnumerable<FileName> FilesAsync() => getFilesAsync(fullPath);

      public IEnumerable<FileName> FilesAsync(CancellationToken token) => getFilesAsync(fullPath, token);

      public IEnumerable<FolderName> Folders => getFolders(fullPath);

      public IEnumerable<FolderName> FoldersAsync() => getFoldersAsync(fullPath);

      public IEnumerable<FolderName> FoldersAsync(CancellationToken token) => getFoldersAsync(fullPath, token);

      public IEnumerable<FileName> LocalAndParentFiles => getLocalAndParentFiles(fullPath);

      public IEnumerable<FileName> LocalAndParentFilesAsync() => getLocalAndParentFilesAsync(fullPath);

      public IEnumerable<FileName> LocalAndParentFilesAsync(CancellationToken token) => getLocalAndParentFilesAsync(fullPath, token);

      public IEnumerable<FolderName> LocalAndParentFolders => getLocalAndParentFolders(fullPath);

      public IEnumerable<FolderName> LocalAndParentFoldersAsync() => getLocalAndParentFoldersAsync(fullPath);

      public IEnumerable<FolderName> LocalAndParentFoldersAsync(CancellationToken token) => getLocalAndParentFoldersAsync(fullPath, token);

      public int FileCount => getFileCount(fullPath);

      public bool Valid { get; private set; }

      static string[] getSubfolders(string subfolders)
      {
         if (subfolders.IsNotEmpty())
         {
            while (subfolders.StartsWith(@"\"))
            {
               subfolders = subfolders.Substring(1);
            }

            return subfolders.Split(@"'\'");
         }
         else
         {
            return new string[0];
         }
      }

      void setFullPath()
      {
         try
         {
            fullPath = Path.Combine(root, string.Join(@"\", subfolders));
            Valid = true;
         }
         catch
         {
            Valid = false;
         }
      }

      void initialize(string newRoot, string[] newSubfolders)
      {
         root = newRoot;
         subfolders = newSubfolders;
         setFullPath();
      }

      void initialize(string newRoot) => initialize(newRoot, new string[0]);

      void setFullPath(string folder)
      {
         folder = replaceTilde(folder);
         folder = Path.GetFullPath(folder);
         var folderRoot = GetDirectoryRoot(folder);

         string folderSubfolders;
         if (folder.StartsWith(folderRoot))
         {
            folderSubfolders = folder.Substring(folderRoot.Length);
         }
         else
         {
            var matcher = new Matcher();
            folderSubfolders = matcher.IsMatch(folder, @"^ '\'1 /(.+) $") ? matcher[0, 1] : "";
         }

         if (folderSubfolders.StartsWith(@"\"))
         {
            folderSubfolders = folderSubfolders.Substring(1);
         }

         initialize(folderRoot, folderSubfolders.IsEmpty() ? new string[0] : folderSubfolders.Split(@"'\'"));
      }

      public void CreateIfNonExistent()
      {
         if (subfolders.Length > 0)
         {
            var currentFolder = $@"{root}\{subfolders[0]}";
            createIfNonExistent(currentFolder);

            for (var i = 1; i < subfolders.Length; i++)
            {
               currentFolder += $@"\{subfolders[i]}";
               createIfNonExistent(currentFolder);
            }
         }
      }

      static void createIfNonExistent(string folder)
      {
         if (!Directory.Exists(folder))
         {
            CreateDirectory(folder);
         }
      }

      public void CopyTo(FolderName targetFolder, bool overwrite)
      {
         foreach (var file in Files)
         {
            var targetFile = file.Clone();
            targetFile.Folder = targetFolder;
            file.CopyTo(targetFile, overwrite);
         }
      }

      public void CopyTo(FolderName targetFolder, string includePattern, string excludePattern = "", bool overwrite = true)
      {
         bool include(FileName f) => f.NameExtension.IsMatch(includePattern);

         Predicate<FileName> exclude;
         if (excludePattern.IsEmpty())
         {
            exclude = f => true;
         }
         else
         {
            exclude = f => !f.NameExtension.IsMatch(excludePattern);
         }

         foreach (var file in Files.Where(f => include(f) && exclude(f)))
         {
            if (overwrite)
            {
               file.CopyTo(targetFolder, true);
               FileSuccess?.Invoke(this, new FileArgs(file, file, "Copied"));
            }
            else if (targetFolder.File(file).Next().If(out var candidateFile))
            {
               file.CopyTo(candidateFile, true);
               FileSuccess?.Invoke(this, new FileArgs(file, candidateFile, "Copied"));
            }
         }
      }

      public bool Exists() => Directory.Exists(fullPath);

      public DateTime Time => GetCreationTime(fullPath);

      public void Delete()
      {
         if (Exists())
         {
            Directory.Delete(fullPath);
         }
      }

      public string NameToMatch => Name;

      public void DeleteAll() => Directory.Delete(fullPath, true);

      public void DeleteFiles()
      {
         foreach (var fileName in Files)
         {
            fileName.Delete();
         }
      }

      public void MoveTo(FolderName targetFolder) => Move(fullPath, targetFolder.ToString());

      public FolderName Subfolder(string name)
      {
         var folder = this[name];
         folder.CreateIfNonExistent();

         return folder;
      }

      public bool IsParentOf(FolderName folder)
      {
         if (root.Same(folder.Root) && subfolders.Length == folder.Subfolders.Length)
         {
            if (subfolders.Length > 0)
            {
               if (subfolders.Length + 1 == folder.Subfolders.Length)
               {
                  var selfSubfolders = subfolders;
                  var folderSubfolders = folder.Subfolders;

                  return !selfSubfolders.Where((t, i) => !t.Same(folderSubfolders[i])).Any();
               }
            }
            else
            {
               return true;
            }
         }

         return false;
      }

      public bool IsChildOf(FolderName folder) => folder.IsParentOf(this);

      protected static IEnumerable<FileName> getFiles(string folder) => GetFiles(folder).Select(f => (FileName)f);

      protected static IEnumerable<FileName> getFilesAsync(string folder) => GetFiles(folder).AsParallel().Select(f => (FileName)f);

      protected static IEnumerable<FileName> getFilesAsync(string folder, CancellationToken token)
      {
         return GetFiles(folder).AsParallel().WithCancellation(token).Select(f => (FileName)f);
      }

      protected static IEnumerable<FolderName> getFolders(string folder)
      {
         return GetDirectories(folder).Select(f => (FolderName)f);
      }

      protected static IEnumerable<FolderName> getFoldersAsync(string folder)
      {
         return GetDirectories(folder).AsParallel().Select(f => (FolderName)f);
      }

      protected static IEnumerable<FolderName> getFoldersAsync(string folder, CancellationToken token)
      {
         return GetDirectories(folder).AsParallel().WithCancellation(token).Select(f => (FolderName)f);
      }

      protected static IEnumerable<FileName> getLocalAndParentFiles(string folder)
      {
         if (folder == null)
         {
            yield break;
         }

         foreach (var file in getFiles(folder))
         {
            yield return file;
         }

         var parent = GetParent(folder)?.FullName;
         foreach (var file in getLocalAndParentFiles(parent))
         {
            yield return file;
         }
      }

      protected static IEnumerable<FileName> getLocalAndParentFilesAsync(string folder)
      {
         if (folder == null)
         {
            yield break;
         }

         foreach (var file in getFilesAsync(folder))
         {
            yield return file;
         }

         var parent = GetParent(folder)?.FullName;
         foreach (var file in getLocalAndParentFilesAsync(parent))
         {
            yield return file;
         }
      }

      protected static IEnumerable<FileName> getLocalAndParentFilesAsync(string folder, CancellationToken token)
      {
         if (folder == null)
         {
            yield break;
         }

         foreach (var file in getFilesAsync(folder, token))
         {
            yield return file;
         }

         var parent = GetParent(folder)?.FullName;
         foreach (var file in getLocalAndParentFilesAsync(parent, token))
         {
            yield return file;
         }
      }

      protected static IEnumerable<FolderName> getLocalAndParentFolders(string folder)
      {
         if (folder == null)
         {
            yield break;
         }

         foreach (var subFolder in getFolders(folder))
         {
            yield return subFolder;
         }

         var parent = GetParent(folder)?.FullName;

         foreach (var subFolder in getLocalAndParentFolders(parent))
         {
            yield return subFolder;
         }

         if (parent != null)
         {
            yield return parent;
         }
      }

      public static IEnumerable<FolderName> getLocalAndParentFoldersAsync(string folder)
      {
         if (folder == null)
         {
            yield break;
         }

         foreach (var subFolder in getFoldersAsync(folder))
         {
            yield return subFolder;
         }

         var parent = GetParent(folder)?.FullName;

         foreach (var subFolder in getLocalAndParentFoldersAsync(parent))
         {
            yield return subFolder;
         }

         if (parent != null)
         {
            yield return parent;
         }
      }

      public static IEnumerable<FolderName> getLocalAndParentFoldersAsync(string folder, CancellationToken token)
      {
         if (folder == null)
         {
            yield break;
         }

         foreach (var subFolder in getFoldersAsync(folder, token))
         {
            yield return subFolder;
         }

         var parent = GetParent(folder)?.FullName;

         foreach (var subFolder in getLocalAndParentFoldersAsync(parent, token))
         {
            yield return subFolder;
         }

         if (parent != null)
         {
            yield return parent;
         }
      }

      static int getFileCount(string folder) => getFiles(folder).Count();

      public FileName File(string name) => new FileName(this, name);

      public FileName File(string name, string extension) => new FileName(this, name, extension);

      public FileName File(FileName fileName)
      {
         var newFileName = fileName.Clone();
         newFileName.Folder = this;

         return newFileName;
      }

      public FolderName Clone() => subfolders.Length > 0 ? new FolderName(root, subfolders) : new FolderName(root, "");

      public override int GetHashCode() => equatable.GetHashCode();

      public int CompareTo(object obj) => obj is FolderName fn ? fullPath.CompareTo(fn.ToString()) : -1;

      public int CompareTo(FolderName other) => fullPath.CompareTo(other.fullPath);

      public bool Equals(FolderName other) => equatable.Equals(other);

      public override string ToString() => fullPath;

      public override bool Equals(object obj) => obj is FolderName fn && fn.ToString().Same(fullPath);

      public FolderName Guarantee()
      {
         CreateIfNonExistent();
         return this;
      }

      public FolderName Today => Subfolder(NowServer.Now.ToString("yyyy-MM-dd"));

      public FolderNameTrying TryTo => new FolderNameTrying(this);

      public FileName UniqueFileName(string name, string extension) => FileName.UniqueFileName(this, name, extension);

      protected string relativeTo(string otherPath, FileAttributes fileAttributes)
      {
         var path = new StringBuilder(MAX_PATH);
         var result = PathRelativePathTo(path, fullPath, FileAttributes.Directory, otherPath, fileAttributes);
         if (result)
         {
            return path.ToString();
         }
         else
         {
            throw "Couldn't determine relative path".Throws();
         }
      }

      public string RelativeTo(FileName file) => relativeTo(file.ToString(), FileAttributes.Normal);

      public string RelativeTo(FolderName folder) => relativeTo(folder.fullPath, FileAttributes.Directory);

      public FolderName AbsoluteFolder(string relativePath) => Path.Combine(fullPath, relativePath);

      public FileName AbsoluteFile(string relativePath) => Path.Combine(fullPath, relativePath);

      public string AbsoluteString(string relativePath) => Path.Combine(fullPath, relativePath);

      public bool WasCreated()
      {
         if (!Directory.Exists(fullPath))
         {
            CreateDirectory(fullPath);
            return true;
         }
         else
         {
            return false;
         }
      }

      public bool IsEmpty => fullPath.IsEmpty();

      public bool IsNotEmpty => !IsEmpty;

      public bool Contains(string nameExtension) => getFiles(fullPath).Any(f => f.NameExtension == nameExtension);

      public IMaybe<FileName> ExistingFile(string nameExtension) => getFiles(fullPath).Where(f => f.NameExtension == nameExtension).FirstOrNone();
   }
}