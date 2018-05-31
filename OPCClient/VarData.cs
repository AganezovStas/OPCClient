using System;

namespace OPCClient
{
	public class VarData
	{
		public object Value = new object();

		public string Name = "";

		public string Address = "";

		public TypeRW TypeRW = TypeRW.Read;
	}
}
