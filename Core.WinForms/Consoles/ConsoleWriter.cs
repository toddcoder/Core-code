using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Consoles
{
	public class TextBoxWriter : TextWriter
	{
		protected TextBoxConsole console;
		protected IMaybe<StringBuilder> buffer;

		public TextBoxWriter(TextBoxConsole console)
		{
			this.console = console;
			buffer = when(this.console.Buffer, () => new StringBuilder());
		}

		public bool AutoStop { get; set; }

		public override void Write(char value)
		{
			if (value != '\r')
				if (buffer.If(out var b))
					b.Append(value);
				else
				{
					if (AutoStop)
						console.StopUpdating();
					console.Write(value);
					if (AutoStop)
						console.ResumeUpdating();
				}
		}

		protected void flush()
		{
			if (buffer.If(out var b))
			{
				if (AutoStop)
					console.StopUpdating();
				foreach (var ch in b.ToString())
					console.Write(ch);

				b.Clear();

				if (AutoStop)
					console.ResumeUpdating();
			}
			else
				console.Clear();
		}

		public override void Flush() => flush();

		public override Task FlushAsync() => Task.Run(() => flush());

		public override Encoding Encoding => Encoding.UTF8;
	}
}