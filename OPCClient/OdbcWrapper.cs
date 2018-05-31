using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OPCClient
{
	public static class OdbcWrapper
	{
		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		public static extern int SQLDataSources(int EnvHandle, int Direction, System.Text.StringBuilder ServerName, int ServerNameBufferLenIn, ref int ServerNameBufferLenOut, System.Text.StringBuilder Driver, int DriverBufferLenIn, ref int DriverBufferLenOut);

		[System.Runtime.InteropServices.DllImport("odbc32.dll")]
		public static extern int SQLAllocEnv(ref int EnvHandle);
	}
}
