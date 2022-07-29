using System;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

[Obsolete("Use UiAction")]
public class MessageProgress : UiAction
{
   protected MessageProgress(Control control, bool center = false, bool is3D = true) : base(control, center, is3D)
   {
   }
}