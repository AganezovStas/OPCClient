using System;
using System.Runtime.CompilerServices;

namespace OPCClient
{
	public class ErrorEventArgs : System.EventArgs
	{
		public int ErrorCode
		{
			get;
			private set;
		}

		public string ErrorText
		{
			get;
			private set;
		}

		public string ErrorMethodName
		{
			get;
			private set;
		}

		internal ErrorEventArgs(int ErrorCode, string ErrorText, [CallerMemberName] string ErrorMethodName = "")
		{
			this.ErrorCode = ErrorCode;
			this.ErrorMethodName = ErrorMethodName;
			this.ErrorText = ErrorText;
		}
	}
}
