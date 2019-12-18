using System;
using System.IO.Compression;
using System.Linq;
using Core.Assertions;
using Core.Computers;
using Core.Objects;
using CompressedFile = System.IO.Compression.ZipFile;

namespace Core.Zip
{
   public class Zipper : ITryAsync<ZipperTrying, ZipperAsync>
   {
      protected ZipContinue zipContinue;
      protected LateLazy<FileName> zipFile;

      public event EventHandler<ZipEventArgs> BeforeZip;
      public event EventHandler<ZipEventArgs> AfterZip;
      public event EventHandler<ZipEventArgs> CancelledFile;
      public event EventHandler<ZipEventArgs> CancelledFolder;
      public event EventHandler CancelledZip;

      public Zipper()
      {
         zipContinue = ZipContinue.Unset;
         zipFile = new LateLazy<FileName>();

         CompressionLevel = CompressionLevel.Optimal;
         IncludeSubfolders = true;
      }

      public CompressionLevel CompressionLevel { get; set; }

      public bool AutoDelete { get; set; }

      public bool Recursive { get; set; }

      public bool IncludeSubfolders { get; set; }

      public bool DeleteFolderIfZipExists { get; set; }

      public FileName ZipFile => zipFile.Value;

      public void ZipFolder(FolderName folder, string name, Predicate<FileName> include)
      {
         folder.Must().Not.BeNull().Assert($"{nameof(folder)} must not be null");
         name.Must().BeNullOrEmpty().Assert($"{nameof(name)} must not be null or empty");
         include.Must().BeNull().Assert($"{nameof(include)} must not be null");

         zipContinue = ZipContinue.Zip;
         zipFolder(folder, name, include);
      }

      public void ZipFolder(FolderName folder, string name) => ZipFolder(folder, name, f => true);

      protected void zipFolder(FolderName folder, string name, Predicate<FileName> include)
      {
         if (zipContinue != ZipContinue.NoZip)
         {
            zipFile.OverrideWith(() => folder.UniqueFileName(name, ".zip"));
            using (var zipArchive = CompressedFile.Open(zipFile.Value.ToString(), ZipArchiveMode.Create))
            {
               if (Recursive)
               {
                  foreach (var file in folder.Files.Where(file => include(file)).Where(file => !beforeEvents(file)))
                  {
                     if (zipContinue == ZipContinue.Zip)
                     {
                        zipArchive.CreateEntryFromFile(file.ToString(), file.NameExtension, CompressionLevel);
                        if (AutoDelete && !afterEvents(file))
                        {
                           file.Delete();
                        }
                     }
                     else
                     {
                        return;
                     }

                     if (IncludeSubfolders)
                     {
                        foreach (var subfolder in folder.Folders)
                        {
                           zipFolder(subfolder, name, include);
                        }
                     }
                  }
               }
               else
               {
                  zipFolder(zipArchive, folder, include, "");
               }
            }
         }
      }

      void zipFolder(ZipArchive zipArchive, FolderName folder, Predicate<FileName> include, string prefix)
      {
         foreach (var file in folder.Files.Where(f => include(f) && !beforeEvents(f) && f != zipFile.Value))
         {
            if (zipContinue == ZipContinue.Zip)
            {
               zipArchive.CreateEntryFromFile(file.ToString(), $"{prefix}{file.NameExtension}", CompressionLevel);
               if (AutoDelete && !afterEvents(file))
               {
                  file.Delete();
               }
            }
            else
            {
               return;
            }
         }

         if (IncludeSubfolders)
         {
            foreach (var subfolder in folder.Folders)
            {
               zipFolder(zipArchive, subfolder, include, $"{prefix}\\{subfolder.Name}");
            }
         }
      }

      protected bool beforeEvents(FileName file)
      {
         var args = new ZipEventArgs(file);

         BeforeZip?.Invoke(this, args);
         switch (args.ZipEventCancel)
         {
            case ZipEventCancel.File:
               CancelledFile?.Invoke(this, args);
               return true;
            case ZipEventCancel.Folder:
               CancelledFolder?.Invoke(this, args);
               return false;
            case ZipEventCancel.Zip:
               zipContinue = ZipContinue.NoZip;
               CancelledZip?.Invoke(this, new EventArgs());

               return false;
            default:
               return false;
         }
      }

      protected bool afterEvents(FileName file)
      {
         var args = new ZipEventArgs(file);
         AfterZip?.Invoke(this, args);

         return args.ZipEventCancel == ZipEventCancel.Delete;
      }

      public void Replace(FolderName folder)
      {
         folder.Must().Not.BeNull().Assert();

         zipContinue = ZipContinue.Zip;
         zipFile.OverrideWith(() => folder.Parent.Map(f => f).DefaultTo(() => @"C:\").File(folder.Name, ".zip"));

         if (DeleteFolderIfZipExists && zipFile.Value.Exists())
         {
            folder.DeleteAll();
         }

         using (var zipArchive = CompressedFile.Open(zipFile.Value.ToString(), ZipArchiveMode.Create))
         {
            zipFolder(zipArchive, folder, f => true, "");
         }

         zipFile.Value.Must().Exist().Assert($"Zip file {zipFile.Value} doesn't exist");

         folder.DeleteAll();
      }

      public void Unzip(FileName file, FolderName targetFolder)
      {
         CompressedFile.ExtractToDirectory(file.ToString(), targetFolder.ToString());
      }

      public ZipperTrying TryTo => new ZipperTrying(this);

      public ZipperAsync Async => new ZipperAsync(this);
   }
}