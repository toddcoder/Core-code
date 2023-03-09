﻿using System;
using System.Windows.Forms;
using Core.Computers;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms;

public static class DialogFunctions
{
   public static Optional<FileName> fileDialog(string title, FolderName defaultFolder, string fileName, string fileType, bool restoreDirectory = true,
      bool checkFileExists = true)
   {
      try
      {
         FileName file = fileName;
         var extension = file.Extension;
         var filter = $"{fileType} files (*{extension})|*{extension}|All files (*.*)|*.*";
         using var dialog = new OpenFileDialog
         {
            Title = title,
            InitialDirectory = defaultFolder.FullPath,
            FileName = fileName,
            DefaultExt = extension,
            Filter = filter,
            RestoreDirectory = restoreDirectory,
            CheckFileExists = checkFileExists,
         };
         if (dialog.ShowDialog() == DialogResult.OK)
         {
            return (FileName)dialog.FileName;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<FileName> fileDialog(string title, string fileName, string fileType, bool restoreDirectory = true,
      bool checkFileExists = true)
   {
      try
      {
         FileName file = fileName;
         var extension = file.Extension;
         var filter = $"{fileType} files (*{extension})|*{extension}|All files (*.*)|*.*";
         using var dialog = new OpenFileDialog
         {
            Title = title,
            FileName = fileName,
            DefaultExt = extension,
            Filter = filter,
            RestoreDirectory = restoreDirectory,
            CheckFileExists = checkFileExists
         };
         if (dialog.ShowDialog() == DialogResult.OK)
         {
            return (FileName)dialog.FileName;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<FolderName> folderDialog(string description, FolderName defaultFolder, Maybe<Environment.SpecialFolder> rootFolder)
   {
      try
      {
         using var dialog = new FolderBrowserDialog
         {
            Description = description,
            SelectedPath = defaultFolder.FullPath,
            ShowNewFolderButton = true
         };
         if (rootFolder)
         {
            dialog.RootFolder = rootFolder;
         }

         if (dialog.ShowDialog() == DialogResult.OK)
         {
            return (FolderName)dialog.SelectedPath;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<FolderName> folderDialog(string description, FolderName defaultFolder) => folderDialog(description, defaultFolder, nil);
}