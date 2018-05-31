using Opc.Da;
using System;
using System.Xml.Linq;

namespace OPCClient
{
	public class ReadDataXmlEventArgs : System.EventArgs
	{
		public XDocument XDoc
		{
			get;
			private set;
		}

		internal ReadDataXmlEventArgs(ItemValueResult[] values, ItemsData items)
		{
			this.XDoc = new XDocument(new object[]
			{
				new XElement("Items")
			});
			for (int i = 0; i < values.Length; i++)
			{
				ItemValueResult itemValueResult = values[i];
				if (itemValueResult.Value != null && itemValueResult.ItemName != null && items.NameRead.ContainsKey(itemValueResult.ItemName))
				{
					this.XDoc.Element("Items").Add(new XElement("Item", new object[]
					{
						new XElement("Name", items.NameRead[itemValueResult.ItemName]),
						new XElement("Addr", itemValueResult.ItemName),
						new XElement("Value", itemValueResult.Value),
						new XElement("TypeRW", "read")
					}));
				}
			}
		}
	}
}
