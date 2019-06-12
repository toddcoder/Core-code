using System;

namespace Core.Applications.Writers
{
	public class StringWriter : BaseWriter, IDisposable
	{
		System.IO.StringWriter writer;

		public StringWriter() => writer = new System.IO.StringWriter();

		void dispose() => writer?.Dispose();

		public void Dispose()
		{
			dispose();
			GC.SuppressFinalize(this);
		}

		~StringWriter() => dispose();

		protected override void writeRaw(string text) => writer.Write(text);

		public override string ToString() => writer.ToString();
	}
}