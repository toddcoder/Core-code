using System;
using System.IO;
using System.Windows.Forms;
using Core.Computers;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms;

public class StandardDialog
{
   public StandardDialog()
   {
      AddExtension = true;
      CheckFileExists = true;
      CheckPathExists = true;
      CreatePrompt = false;
      DefaultExt = "";
      FileName = nil;
      Filter = nil;
      FilterIndex = 1;
      InitialFolder = nil;
      OverwritePrompt = true;
      RestoreFolder = false;
      RootFolder = Environment.SpecialFolder.Desktop;
      ShowNewFolderButton = true;
      Title = "";
      ValidateNames = true;
   }

   public bool AddExtension { get; set; }

   public bool CheckFileExists { get; set; }

   public bool CheckPathExists { get; set; }

   public bool CreatePrompt { get; set; }

   public string DefaultExt { get; set; }

   public Maybe<string> FileName { get; set; }

   public Maybe<string> Filter { get; set; }

   public int FilterIndex { get; set; }

   public Maybe<FolderName> InitialFolder { get; set; }

   public bool OverwritePrompt { get; set; }

   public bool RestoreFolder { get; set; }

   public Environment.SpecialFolder RootFolder { get; set; }

   public bool ShowNewFolderButton { get; set; }

   public string Title { get; set; }

   public bool ValidateNames { get; set; }

   protected string removeDot(string extension) => extension.StartsWith(".") ? extension.Drop(1) : extension;

   public Optional<FileName> OpenFileDialog(Form parentForm, string fileType = "")
   {
      try
      {
         using var dialog = new OpenFileDialog();
         dialog.AddExtension = AddExtension;
         dialog.CheckFileExists = CheckFileExists;
         dialog.CheckPathExists = CheckPathExists;
         if (FileName is (true, var fileName))
         {
            dialog.FileName = fileName;
            dialog.DefaultExt = removeDot(DefaultExt.IsEmpty() ? Path.GetExtension(fileName) : DefaultExt);
         }
         else
         {
            dialog.DefaultExt = removeDot(DefaultExt);
         }

         dialog.Filter = Filter | $"{fileType} files (*.{dialog.DefaultExt})|*.{dialog.DefaultExt}|All files (*.*)|*.*";
         dialog.FilterIndex = FilterIndex;
         dialog.InitialDirectory = InitialFolder.Map(f => f.FullPath) | "";
         dialog.RestoreDirectory = RestoreFolder;
         dialog.Title = Title;
         dialog.ValidateNames = ValidateNames;

         if (dialog.ShowDialog(parentForm) == DialogResult.OK)
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

   public Optional<FileName> SaveFileDialog(Form parentForm, string fileType = "")
   {
      try
      {
         using var dialog = new SaveFileDialog();
         dialog.AddExtension = AddExtension;
         dialog.CheckFileExists = false;
         dialog.CheckPathExists = CheckPathExists;
         dialog.CreatePrompt = CreatePrompt;
         if (FileName is (true, var fileName))
         {
            dialog.FileName = fileName;
            dialog.DefaultExt = removeDot(DefaultExt.IsEmpty() ? Path.GetExtension(fileName) : DefaultExt);
         }
         else
         {
            dialog.DefaultExt = removeDot(DefaultExt);
         }

         dialog.Filter = Filter | $"{fileType} files (*.{dialog.DefaultExt})|*.{dialog.DefaultExt}|All files (*.*)|*.*";
         dialog.FilterIndex = FilterIndex;
         dialog.InitialDirectory = InitialFolder.Map(f => f.FullPath) | "";
         dialog.OverwritePrompt = OverwritePrompt;
         dialog.RestoreDirectory = RestoreFolder;
         dialog.Title = Title;
         dialog.ValidateNames = ValidateNames;

         if (dialog.ShowDialog(parentForm) == DialogResult.OK)
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

   public Optional<FolderName> FolderBrowserDialog(Form parentForm)
   {
      try
      {
         using var browseFolderDialog = new FolderBrowserDialog();
         browseFolderDialog.RootFolder = RootFolder;
         browseFolderDialog.Description = Title;
         browseFolderDialog.SelectedPath = InitialFolder.Map(f => f.FullPath) | "";
         browseFolderDialog.ShowNewFolderButton = ShowNewFolderButton;

         if (browseFolderDialog.ShowDialog(parentForm) == DialogResult.OK)
         {
            return (FolderName)browseFolderDialog.SelectedPath;
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
}