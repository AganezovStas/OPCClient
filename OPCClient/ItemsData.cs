using Opc.Da;
using System;
using System.Collections.Generic;

namespace OPCClient
{
	internal class ItemsData
	{
		public System.Collections.Generic.Dictionary<string, Item> OpcRead = new System.Collections.Generic.Dictionary<string, Item>(100);

		public System.Collections.Generic.Dictionary<string, Item> OpcWrite = new System.Collections.Generic.Dictionary<string, Item>(100);

		public System.Collections.Generic.Dictionary<string, ItemResult> OpcReadResult = new System.Collections.Generic.Dictionary<string, ItemResult>(100);

		public System.Collections.Generic.Dictionary<string, ItemResult> OpcWriteResult = new System.Collections.Generic.Dictionary<string, ItemResult>(100);

		public System.Collections.Generic.Dictionary<string, string> AddrRead = new System.Collections.Generic.Dictionary<string, string>(100);

		public System.Collections.Generic.Dictionary<string, string> NameRead = new System.Collections.Generic.Dictionary<string, string>(100);

		public System.Collections.Generic.Dictionary<string, object> ValueRead = new System.Collections.Generic.Dictionary<string, object>(100);

		public System.Collections.Generic.Dictionary<string, string> AddrWrite = new System.Collections.Generic.Dictionary<string, string>(100);

		public System.Collections.Generic.Dictionary<string, string> NameWrite = new System.Collections.Generic.Dictionary<string, string>(100);
	}
}
