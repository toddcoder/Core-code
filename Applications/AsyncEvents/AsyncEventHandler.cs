using System;
using System.Threading.Tasks;

namespace Core.Applications.AsyncEvents
{
   public delegate Task AsyncEventHandler<in TArgs>(object sender, TArgs args) where TArgs : EventArgs;
}