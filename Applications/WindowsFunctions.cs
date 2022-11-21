using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Core.Assertions;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.Applications;

public static class WindowsFunctions
{
   [DllImport("user32.dll")]
   private static extern bool GetKeyboardState([Out] byte[] keyStates);

   private static bool getKeyboardState(byte[] keyStates)
   {
      keyStates.Must().Not.BeNull().OrThrow();
      keyStates.Must().HaveLengthOf(256).OrThrow();

      return GetKeyboardState(keyStates);
   }

   public static byte[] keyboardState()
   {
      var keyStates = new byte[256];
      getKeyboardState(keyStates).Must().BeTrue().OrThrow<Win32Exception>(Marshal.GetLastWin32Error());

      return keyStates;
   }

   public static bool anyKeyPressed() => keyboardState().Skip(8).Any(state => (state & 0x80) != 0);

   public static byte keyState(VirtualKey virtualKey) => keyboardState()[(int)virtualKey];

   public static byte keyState(char key)
   {
      var ordinal = (int)key;
      var bytes = keyboardState();
      if (key.Between('0').And('9'))
      {
         return bytes[ordinal];
      }
      else if (key.Between('A').And('Z'))
      {
         return bytes[ordinal];
      }
      else if (key.Between('a').And('z'))
      {
         return bytes[ordinal - 32];
      }
      else
      {
         throw fail("Character must be between 0-9, A-Z, a-z");
      }
   }

   public static bool isPressed(byte state) => (state & 0x80) != 0;

   public static bool isPressed(VirtualKey virtualKey) => isPressed(keyState(virtualKey));

   public static bool isPressed(char key) => isPressed(keyState(key));
}