using System;
using System.Data;
using System.Runtime.InteropServices;

namespace OPCClient
{
	public static class SqlServerSmo
	{
		[System.Runtime.InteropServices.DllImport("Microsoft.SqlServer.Smo.dll")]
		public static extern System.Data.DataTable EnumAvailableSqlServers(string name);
        public static extern System.Data.DataTable EnumAvailableSqlServers(bool localOnly);
	}
}
