using System;
using System.Threading.Tasks;

namespace Core.Applications.Async;

public delegate Task AsyncEventHandler<in TArgs>(object sender, TArgs args) where TArgs : EventArgs;