using System;
using System.Runtime.InteropServices;

namespace Core.WinForms.Controls;

public class TaskBarProgress
{
   public enum TaskBarState
   {
      NoProgress = 0,
      Indeterminate = 0x1,
      Normal = 0x2,
      Error = 0x4,
      Paused = 0x8
   }

   [ComImport, Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
   protected interface ITaskbarList3
   {
      [PreserveSig]
      void HrInit();

      [PreserveSig]
      void AddTab(IntPtr hwnd);

      [PreserveSig]
      void DeleteTab(IntPtr hwnd);

      [PreserveSig]
      void ActivateTab(IntPtr hwnd);

      [PreserveSig]
      void SetActiveAlt(IntPtr hwnd);

      [PreserveSig]
      void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

      [PreserveSig]
      void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);

      [PreserveSig]
      void SetProgressState(IntPtr hwnd, TaskBarState state);
   }

   [ComImport, Guid("56fdf344-fd6d-11d0-958a-006097c9a090"), ClassInterface(ClassInterfaceType.None)]
   protected class TaskBarInstance
   {
   }

   protected static ITaskbarList3 taskbarInstance = (ITaskbarList3)new TaskBarInstance();

   protected static bool taskbarSupported = Environment.OSVersion.Version >= new Version(6, 1);

   protected IntPtr windowHandle;
   protected ulong progressMax;

   public TaskBarProgress(IntPtr windowHandle, double progressMax)
   {
      this.windowHandle = windowHandle;
      this.progressMax = (ulong)progressMax;

      State = TaskBarState.Normal;
   }

   public TaskBarState State
   {
      set
      {
         if (taskbarSupported)
         {
            taskbarInstance.SetProgressState(windowHandle, value);
         }
      }
   }

   public double Value
   {
      set
      {
         if (taskbarSupported)
         {
            taskbarInstance.SetProgressValue(windowHandle, (ulong)value, progressMax);
         }
      }
   }
}