﻿using System;
using System.Windows.Forms;

namespace WinFormsTest;

internal static class Program
{
   [STAThread]
   internal static void Main()
   {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
   }
}