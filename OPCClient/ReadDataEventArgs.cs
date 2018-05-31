using Opc.Da;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OPCClient
{
	public class ReadDataEventArgs : System.EventArgs
	{
		public System.Collections.Generic.List<VarData> Vars
		{
			get;
			private set;
		}

		internal ReadDataEventArgs(ItemValueResult[] values, ItemsData items)
		{
			this.Vars = new System.Collections.Generic.List<VarData>(values.Count<ItemValueResult>());
			for (int i = 0; i < values.Length; i++)
			{
				ItemValueResult itemValueResult = values[i];
				if (itemValueResult.Value != null && itemValueResult.ItemName != null && items.NameRead.ContainsKey(itemValueResult.ItemName))
				{
					VarData varData = new VarData();
					varData.Address = itemValueResult.ItemName;
					varData.Name = items.NameRead[itemValueResult.ItemName];
					varData.Value = itemValueResult.Value;
					varData.TypeRW = TypeRW.Read;
					this.Vars.Add(varData);
				}
			}
		}
	}
}
