using System;

namespace Core.Applications.Writers
{
	public class ActionWriter : BaseWriter
	{
		Action<string> action;

		public ActionWriter(Action<string> action) => this.action = action;

		protected override void writeRaw(string text) => action(text);
	}
}