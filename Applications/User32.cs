using System;
using System.Runtime.InteropServices;

namespace Core.Applications
{
   public static class User32
   {
      public enum Styles : uint
      {
         Overlapped = 0x00000000,
         Popup = 0x80000000,
         Child = 0x40000000,
         Minimize = 0x20000000,
         Visible = 0x10000000,
         Disabled = 0x08000000,
         ClipSiblings = 0x04000000,
         ClipChildren = 0x02000000,
         Maximize = 0x01000000,
         Caption = 0x00C00000,
         Border = 0x00800000,
         DialogFrame = 0x00400000,
         VerticalScroll = 0x00200000,
         HorizontalScroll = 0x00100000,
         SystemMenu = 0x00080000,
         ThickFrame = 0x00040000,
         Group = 0x00020000,
         TabStop = 0x00010000,
         Style = 0xFFFFFFF0
      }

      public enum Messages
      {
         FirstWindow = 0,
         LastWindow = 1,
         NextWindow = 2,
         PreviousWindow = 3,
         OwnerWindow = 4,
         ChildWindow = 5,

         Null = 0x0000,
         Create = 0x0001,
         Destroy = 0x0002,
         Move = 0x0003,
         Size = 0x0005,
         KillFocus = 0x0008,
         SetRedraw = 0x000B,
         GetText = 0x000D,
         GetTextLength = 0x000E,
         Paint = 0x000F,
         EraseBackground = 0x0014,
         ShowWindow = 0x0018,

         FontChange = 0x001d,
         SetCursor = 0x0020,
         MouseActivate = 0x0021,
         ChildActivate = 0x0022,

         DrawItem = 0x002B,
         MeasureItem = 0x002C,
         DeleteItem = 0x002D,
         KeyToItem = 0x002E,
         CharacterToItem = 0x002F,

         SetFont = 0x0030,
         CompareItem = 0x0039,
         WindowPositionChanging = 0x0046,
         WindowPositionChanged = 0x0047,
         Notify = 0x004E,
         NotifyFormat = 0x0055,
         StyleChanging = 0x007C,
         StyleChanged = 0x007D,
         ClientMouseMove = 0x00A0,
         ClientLeftButtonDown = 0x00A1,

         ClientCreate = 0x0081,
         ClientDestroy = 0x0082,
         ClientCalculateSize = 0x0083,
         ClientHitTest = 0x0084,
         ClientPaint = 0x0085,
         GetDialogCode = 0x0087,

         GetSelection = 0x00B0,
         SetSelection = 0x00B1,
         GetRectangle = 0x00B2,
         SetRectangle = 0x00B3,
         SetRectangleNoPaint = 0x00B4,
         Scroll = 0x00B5,
         LineScroll = 0x00B6,
         GetModify = 0x00B8,
         SetModify = 0x00B9,
         GetLineCount = 0x00BA,
         LineIndex = 0x00BB,
         SetHandle = 0x00BC,
         GetHandle = 0x00BD,
         GetThumb = 0x00BE,
         LineLength = 0x00C1,
         LineFromCharacter = 0x00C9,
         GetFirstVisibleLine = 0x00CE,
         SetMargins = 0x00D3,
         GetMargins = 0x00D4,
         PositionFromCharacter = 0x00D6,
         CharacterFromPosition = 0x00D7,

         KeyFirst = 0x0100,
         KeyDown = 0x0100,
         KeyUp = 0x0101,
         Character = 0x0102,
         DeadCharacter = 0x0103,
         SystemKeyDown = 0x0104,
         SystemKeyUp = 0x0105,
         SystemCharacter = 0x0106,
         SystemDeadCharacter = 0x0107,

         Command = 0x0111,
         SystemCommand = 0x0112,
         Timer = 0x0113,
         HorizontalScroll = 0x0114,
         VerticalScroll = 0x0115,
         UpdateUIState = 0x0128,
         QueryUIState = 0x0129,
         MouseFirst = 0x0200,
         MouseMove = 0x0200,
         LeftButtonDown = 0x0201,
         LeftButtonUp = 0x0202,
         ParentNotify = 0x0210,

         NextMenu = 0x0213,
         Sizing = 0x0214,
         CaptureChanged = 0x0215,
         Moving = 0x0216,

         IMESetContext = 0x0281,
         IMENotify = 0x0282,
         IMEControl = 0x0283,
         IMECompositionFull = 0x0284,
         IMESelect = 0x0285,
         IMECharacter = 0x0286,
         IMERequest = 0x0288,
         IMEKeyDown = 0x0290,
         IMEKeyUp = 0x0291,
         ClientMoveHover = 0x02A0,
         ClientMouseLeave = 0x02A2,
         MouseHover = 0x02A1,
         MouseLeave = 0x02A3,

         Cut = 0x0300,
         Copy = 0x0301,
         Paste = 0x0302,
         Clear = 0x0303,
         Undo = 0x0304,
         RenderFormat = 0x0305,
         RenderAllFormats = 0x0306,
         DestroyClipboard = 0x0307,
         DrawClipboard = 0x0308,
         PaintClipboard = 0x0309,
         VerticalScrollClipboard = 0x030A,
         SizeClipboard = 0x030B,
         AskClipboardFormatName = 0x030C,
         ChangeClipboardChain = 0x030D,
         HorizontalScrollClipboard = 0x030E,
         QueryNewPalette = 0x030F,
         PaletteIsChanging = 0x0310,
         PaletteChanged = 0x0311,
         HotKey = 0x0312,

         User = 0x0400,
         ScrollCaret = User + 49,

         CanPaste = User + 50,
         DisplayBand = User + 51,
         ExGetSelection = User + 52,
         ExLimitText = User + 53,
         ExLineFromCharacter = User + 54,
         ExSetSelection = User + 55,
         FindText = User + 56,
         FormatRange = User + 57,
         GetCharacterFormat = User + 58,
         GetEventMask = User + 59,
         GetOLEInterface = User + 60,
         GetParagraphFormat = User + 61,
         GetSelectedText = User + 62,
         HideSelection = User + 63,
         PasteSpecial = User + 64,
         RequestResize = User + 65,
         SelectionType = User + 66,
         SetBackgroundColor = User + 67,
         SetCharacterFormat = User + 68,
         SetEventMask = User + 69,
         SetOLECallback = User + 70,
         SetParagraphFormat = User + 71,
         SetTargetDevice = User + 72,
         StreamIn = User + 73,
         StreamOut = User + 74,
         GetTextRange = User + 75,
         FindWordBreak = User + 76,
         SetOptions = User + 77,
         GetOptions = User + 78,
         FindTextEx = User + 79,

         TabDeleteItem = 0x1308,
         TabInsertItem = 0x133E,
         TabGetItemRectangle = 0x130A,
         TabGetCursorSelected = 0x130B,
         TabSetCursorSelection = 0x130C,
         TabAdjustRectangle = 0x1328,
         TabSetItemSize = 0x1329,
         TabSetPadding = 0x132B,

         OCMBase = User + 0x1c00,
         OCMCommand = OCMBase + Command,
         OCMDrawItem = OCMBase + DrawItem,
         OCMMeasureItem = OCMBase + MeasureItem,
         OCMDeleteItem = OCMBase + DeleteItem,
         OCMKeyToItem = OCMBase + KeyToItem,
         OCMCharacterToItem = OCMBase + CharacterToItem,
         OCMCompareItem = OCMBase + CompareItem,
         OCMHorizontalScroll = OCMBase + HorizontalScroll,
         OCMVerticalScroll = OCMBase + VerticalScroll,
         OCMParentNotify = OCMBase + ParentNotify,
         OCMNotify = OCMBase + Notify
      }

      public const int SELECTION = 0x0001;
      public const int LEFT_MARGIN = 0x0001;
      public const int RIGHT_MARGIN = 0x0002;

      [Flags]
      public enum Flags
      {
         NoSize = 0x0001,
         NoMove = 0x0002,
         NoZOrder = 0x0004,
         NoRedraw = 0x0008,
         NoActivate = 0x0010,
         FrameChanged = 0x0020,
         ShowWindow = 0x0040,
         HideWindow = 0x0080,
         NoCopyBits = 0x0100,
         NoOwnerZOrder = 0x0200,
         NoSendChanging = 0x0400
      }

      [Flags]
      public enum Show
      {
         Hide = 0,
         Show = 5
      }

      private static Type messagesType = typeof(Messages);

      public static string Mnemonic(int z)
      {
         foreach (int ix in Enum.GetValues(messagesType))
         {
            if (z == ix)
            {
               return Enum.GetName(messagesType, ix);
            }
         }

         return z.ToString("X4");
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct WindowPosition
      {
         public IntPtr Handle;

         public IntPtr HandleInsertAfter;

         public int X, Y, CX, CY, Flags;
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
      public struct StyleStructure
      {
         public int Old;
         public int New;
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
      public struct CreateStructure
      {
         public IntPtr Parameters;
         public IntPtr Instance;
         public IntPtr MenuHandle;
         public IntPtr ParentHandle;
         public int CY;
         public int CX;
         public int Y;
         public int X;
         public int Style;
         public string Name;
         public string Class;
         public int ExtendedStyle;
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct CharacterFormat
      {
         public int Size;
         public uint Mask;
         public uint Effects;
         public int YHeight;
         public int YOffset;
         public int TextColor;
         public byte CharacterSet;
         public byte PitchAndFamily;
         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
         public char[] FaceName;
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct PointInt
      {
         public int X;
         public int Y;
      }

      public static void updateBegin(IntPtr handle) => SendMessage(handle, (int)Messages.SetRedraw, 0, IntPtr.Zero);

      public static void updateEnd(IntPtr handle) => SendMessage(handle, (int)Messages.SetRedraw, 1, IntPtr.Zero);

      [DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
      public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

      [DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
      public static extern IntPtr SendMessage(IntPtr hWnd,
         [MarshalAs(UnmanagedType.I4)] Messages msg,
         int wParam,
         IntPtr lParam);

      [DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
      public static extern IntPtr SendMessage(IntPtr hWnd,
         [MarshalAs(UnmanagedType.I4)] Messages msg,
         int wParam,
         int lParam);

      [DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
      public static extern int SendMessage(IntPtr hWnd, int msg, int wparam, IntPtr lparam);

      [DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
      public static extern int SendMessage(IntPtr hWnd, int msg, int wparam, int lparam);

      [DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
      public static extern int SendMessageRef(IntPtr hWnd, int msg, out int wparam, out int lparam);

      [DllImport("User32.dll", CharSet = CharSet.Auto)]
      public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

      [DllImport("User32.dll", CharSet = CharSet.Auto)]
      public static extern int GetClassName(IntPtr hWnd, char[] className, int maxCount);

      [DllImport("user32.dll")]
      public static extern int SetWindowPos(IntPtr handle, IntPtr handleInsertAfter, int x, int y, int cx, int cy, uint flags);

      [DllImport("user32.dll", SetLastError = true)]
      public static extern uint GetWindowLong(IntPtr handle, uint index);

      [DllImport("user32.dll")]
      public static extern int SetWindowLong(IntPtr handle, uint index, uint newLong);

      [DllImport("user32.dll")]
      public static extern int SendMessage(IntPtr handle, int message, bool boolParameter, int intParameter);

      public static int SendMessage(IntPtr handle, Messages message, bool boolParameter, int intParameter)
      {
         return SendMessage(handle, (int)message, boolParameter, intParameter);
      }

      [DllImport("user32.dll")]
      private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

      public static void windowShow(IntPtr handle) => ShowWindow(handle, (int)Show.Show);

      public static void windowHide(IntPtr handle) => ShowWindow(handle, (int)Show.Hide);
   }
}