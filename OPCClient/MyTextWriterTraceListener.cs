using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OPCClient
{
	public sealed class MyTextWriterTraceListener : System.Diagnostics.TextWriterTraceListener
	{
		public MyTextWriterTraceListener() : base(System.IO.File.Create(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "OPCClient.log")))
		{
			base.Writer.Flush();
		}

		public MyTextWriterTraceListener(System.IO.FileStream stream, string FileName) : base(stream, FileName)
		{
			base.Writer = new System.IO.StreamWriter(stream, System.Text.Encoding.GetEncoding(1251));
		}

		public override void Write(string x)
		{
			base.Write(string.Format("{0:F}: {1}", System.DateTime.Now, x));
		}

		public override void WriteLine(string x)
		{
			base.Writer.Flush();
			base.WriteLine(string.Format("{0:F}: {1}", System.DateTime.Now, x));
		}
	}
}
