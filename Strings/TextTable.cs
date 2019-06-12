using System;
using System.Collections.Generic;
using System.Linq;
using Core.Numbers;
using Core.Objects;

namespace Core.Strings
{
	public class TextTable
	{
		public class FieldEventArgs : EventArgs
		{
			public string Field { get; set; }
		}

		[Flags]
		public enum TableDisplayType
		{
			None = 0,
			Lines = 1,
			Horizontal = 2,
			Header = 4
		}

		static string[] emptyColumns(int[] columnWidths)
		{
			return columnWidths.Select(cw => " ".Repeat(cw)).ToArray();
		}

		const string DEFAULT_VERTICAL = "|";
		const string DEFAULT_HORIZONTAL = "-";
		const string DEFAULT_CROSS = "|";

		List<string> fields;
		Padding padder;
		string[] columnHeaders;
		PadType[] padTypes;
		int length;
		string vertical;
		string horizontal;
		string cross;
		Bits32<TableDisplayType> displayOptions;
		bool columnHeadersEmpty;
		List<string> preHeaders;

		public event EventHandler<FieldEventArgs> RenderField;

		public TextTable(params string[] columns)
		{
			initialize(columns.Length);
			initializeColumnHeaders(columns);

			displayOptions[TableDisplayType.Header] = true;
		}

		public TextTable(int length)
		{
			initialize(length);
		}

		public TextTable(params int[] columnWidths)
			: this(emptyColumns(columnWidths)) { }

		public TableDisplayType DisplayOptions
		{
			get => displayOptions.Value;
			set => showingLines(value);
		}

		bool ShowColumnHeaders
		{
			get => displayOptions[TableDisplayType.Header];
			set => displayOptions[TableDisplayType.Header] = value;
		}

		public string Vertical
		{
			get => vertical;
			set
			{
				vertical = value;
				if (vertical.IsNotEmpty())
					displayOptions[TableDisplayType.Lines] = true;
			}
		}

		public string Horizontal
		{
			get { return horizontal; }
			set
			{
				horizontal = value;
				if (horizontal.IsNotEmpty())
					displayOptions[TableDisplayType.Header] = true;
			}
		}

		public string Cross
		{
			get { return cross; }
			set
			{
				cross = value;
				if (cross.IsNotEmpty())
					displayOptions[TableDisplayType.Lines] = true;
			}
		}

		public string LineTerminator { get; set; }

		public bool ShowLines
		{
			get { return displayOptions[TableDisplayType.Lines]; }
			set { displayOptions[TableDisplayType.Lines] = value; }
		}

		void showingLines(Bits32<TableDisplayType> value)
		{
			var didShowLines = displayOptions[TableDisplayType.Lines];
			displayOptions = value;
			var nowShowsLines = displayOptions[TableDisplayType.Lines];

			if (nowShowsLines ^ didShowLines)
				if (nowShowsLines)
				{
					vertical = DEFAULT_VERTICAL;
					horizontal = DEFAULT_HORIZONTAL;
					cross = DEFAULT_CROSS;
				}
				else
				{
					vertical = " ";
					horizontal = " ";
					cross = " ";
				}
		}

		public int Count
		{
			get { return fields.Count; }
		}

		void initialize(int lengthValue)
		{
			length = lengthValue;

			fields = new List<string>();
			//padder = new PadderArray(length);
			padTypes = new PadType[length];
			columnHeaders = new string[length];

			displayOptions = TableDisplayType.Lines | TableDisplayType.Horizontal;

			vertical = DEFAULT_VERTICAL;
			horizontal = DEFAULT_HORIZONTAL;
			cross = DEFAULT_CROSS;

			LineTerminator = "\r\n";
			columnHeadersEmpty = true;
			preHeaders = new List<string>();
		}

		void initializeColumnHeaders(string[] columns)
		{
			for (var i = 0; i < columns.Length; i++)
			{
				if (columns[i].StartsWith("-"))
				{
					columnHeaders[i] = columns[i].Substring(1);
					padTypes[i] = PadType.Left;
				}
				else if (columns[i].StartsWith("+"))
				{
					columnHeaders[i] = columns[i].Substring(1);
					padTypes[i] = PadType.Right;
				}
				else
				{
					columnHeaders[i] = columns[i];
					padTypes[i] = PadType.Center;
				}

				//padder.Evaluate(i, columnHeaders[i]);
			}

			columnHeadersEmpty = false;
			padder = new Padding(padTypes);
		}

		public void AddRow(params string[] columns)
		{
			if (columnHeadersEmpty && ShowColumnHeaders)
				initializeColumnHeaders(columns);
			else
				addNextRow(columns);
		}

		int getLength(int columnsLength)
		{
			return Math.Min(length, columnsLength);
		}

		void addNextRow(string[] columns)
		{
			var columnsLength = getLength(columns.Length);

			for (var i = 0; i < columnsLength; i++)
			{
				fields.Add(columns[i]);
				//padder.Evaluate(i, columns[i]);
			}

			for (var i = columnsLength; i < length; i++)
				fields.Add("");
		}

		public void AddPreHeaderRow(params string[] columns)
		{
			var columnsLength = getLength(columns.Length);

			for (var i = 0; i < columnsLength; i++)
			{
				preHeaders.Add(columns[i]);
				//padder.Evaluate(i, columns[i]);
			}

			for (var i = columnsLength; i < length; i++)
				preHeaders.Add("");
		}

		public override string ToString()
		{
			var recording = new FieldRecord
			{
				FieldDelimiter = vertical,
				RecordDelimiter = LineTerminator
			};
			var showHorizontal = displayOptions[TableDisplayType.Horizontal];

			if (showHorizontal)
				writeLine(recording);

			if (preHeaders.Count > 0)
			{
				renderColumns(recording, preHeaders, showHorizontal);
				recording.WriteField("");
				/*for (var i = 0; i < length; i++)
					recording.WriteField(padder.Pad(i, "", padTypes[i]));*/
				recording.WriteField("");
				recording.WriteRecord();
			}

			if (displayOptions[TableDisplayType.Header])
			{
				recording.WriteField("");
				for (var i = 0; i < length; i++)
					recording.WriteField(padder.PadCenter(i, columnHeaders[i]));
				recording.WriteField("");
				recording.WriteRecord();
				if (showHorizontal)
					writeLine(recording);
			}

			renderColumns(recording, fields, showHorizontal);

			return recording.ToString();
		}

		private void renderColumns(FieldRecord recording, List<string> preHeaders, bool showHorizontal)
		{
			var index = 0;

			foreach (var preHeader in preHeaders)
			{
				if (index == 0)
					recording.WriteField("");

				var field = preHeader;
				var args = new FieldEventArgs
				{
					Field = field
				};
				RenderField.Raise(this, args);
				field = args.Field;
				recording.WriteField(padder.Pad(index, field, padTypes[index]));

				if (++index != length)
					continue;

				recording.WriteField("");
				recording.WriteRecord();
				if (showHorizontal)
					writeLine(recording);
				index = 0;
			}
		}

		private void writeLine(FieldRecord recording)
		{
			recording.FieldDelimiter = cross;
			recording.WriteField("");
			for (var i = 0; i < length; i++)
				recording.WriteField(padder.Repeat(i, horizontal));
			recording.WriteField("");
			recording.WriteRecord();
			recording.FieldDelimiter = vertical;
		}
	}
}