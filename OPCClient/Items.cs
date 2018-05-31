using Opc.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OPCClient
{
	public class Items
	{
		private OPCclient _client;

		private ItemsData _items;

		private int _errNum = 0;

		private string _errTxt = "";

		private string _errItemName = "";

		private string _errItemAddr = "";

		public int ErrNum
		{
			get
			{
				return this._errNum;
			}
		}

		public string ErrTxt
		{
			get
			{
				return this._errTxt;
			}
		}

		internal Items(ItemsData items, OPCclient client)
		{
			this._client = client;
			this._items = items;
		}

		private void ErrCodeParse(ErrCode errCode)
		{
			switch (errCode)
			{
			case ErrCode.ok:
				this._errNum = 0;
				this._errTxt = "ОК";
				break;
			case ErrCode.serverConnFlt:
				this._errNum = 4;
				this._errTxt = "Ошибка связи с сервером. Проверьте сетевое подключение";
				break;
			case ErrCode.nameNotFound:
				this._errNum = 7;
				this._errTxt = "Переменная с именем " + this._errItemName + " не существует";
				break;
			case ErrCode.addrNotFound:
				this._errNum = 8;
				this._errTxt = "Переменная с адресом " + this._errItemAddr + " не существует";
				break;
			case ErrCode.varTypeOther:
				this._errNum = 9;
				this._errTxt = "Переменная " + this._errItemName + " имеет другой тип";
				break;
			case ErrCode.sameNameOrAddr:
				this._errNum = 10;
				this._errTxt = string.Concat(new string[]
				{
					"Имя ",
					this._errItemName,
					" или адрес ",
					this._errItemAddr,
					" уже существуют"
				});
				break;
			case ErrCode.noNameOrAddr:
				this._errNum = 11;
				this._errTxt = "Нет такого имени " + this._errItemName + " или адреса " + this._errItemAddr;
				break;
			case ErrCode.xmlNoElement:
				this._errNum = 12;
				this._errTxt = "Xml документ заполнен неверно - отсутствуют некоторые дочер элементы узлов Item";
				break;
			case ErrCode.xmlNoNode:
				this._errNum = 13;
				this._errTxt = "Xml документ заполнен неверно - отсутствуют узлы Item";
				break;
			case ErrCode.xmlTypeRWFlt:
				this._errNum = 14;
				this._errTxt = "Xml документ заполнен неверно - TypeRW" + this._errItemAddr + " не определен";
				break;
			}
		}

		private ErrCode CheckXmlForAddItem(XDocument xDoc)
		{
			System.Collections.Generic.IEnumerable<XElement> enumerable = xDoc.Descendants("Item");
			ErrCode result;
			if (enumerable.Count<XElement>() == 0)
			{
				this.ErrCodeParse(ErrCode.xmlNoNode);
				result = ErrCode.xmlNoNode;
			}
			else
			{
				foreach (XContainer current in enumerable)
				{
					if (!current.Elements().Any((XElement e) => e.Name == "TypeRW"))
					{
						goto IL_E8;
					}
					if (!current.Elements().Any((XElement e) => e.Name == "Name"))
					{
						goto IL_E8;
					}
					bool arg_EA_0 = current.Elements().Any((XElement e) => e.Name == "Addr");
					IL_E9:
					if (!arg_EA_0)
					{
						this.ErrCodeParse(ErrCode.xmlNoElement);
						result = ErrCode.xmlNoElement;
						return result;
					}
					string value = current.Element("Name").Value;
					string value2 = current.Element("Addr").Value;
					string text = current.Element("TypeRW").Value.ToLower();
					if (text != "read" && text != "write")
					{
						this._errItemName = text;
						this.ErrCodeParse(ErrCode.xmlTypeRWFlt);
						result = ErrCode.xmlTypeRWFlt;
						return result;
					}
					if (text == "read")
					{
						if (this._items.AddrRead.ContainsKey(value) || this._items.NameRead.ContainsKey(value2))
						{
							this._errItemName = value;
							this._errItemAddr = value2;
							this.ErrCodeParse(ErrCode.sameNameOrAddr);
							result = ErrCode.sameNameOrAddr;
							return result;
						}
					}
					if (text == "write")
					{
						if (this._items.AddrWrite.ContainsKey(value) || this._items.NameWrite.ContainsKey(value2))
						{
							this._errItemName = value;
							this._errItemAddr = value2;
							this.ErrCodeParse(ErrCode.sameNameOrAddr);
							result = ErrCode.sameNameOrAddr;
							return result;
						}
					}
					continue;
					IL_E8:
					arg_EA_0 = false;
					goto IL_E9;
				}
				result = ErrCode.ok;
			}
			return result;
		}

		private ErrCode CheckXmlForRemoteItem(XDocument xDoc)
		{
			System.Collections.Generic.IEnumerable<XElement> enumerable = xDoc.Descendants("Item");
			ErrCode result;
			if (enumerable.Count<XElement>() == 0)
			{
				this.ErrCodeParse(ErrCode.xmlNoNode);
				result = ErrCode.xmlNoNode;
			}
			else
			{
				foreach (XContainer current in enumerable)
				{
					if (!current.Elements().Any((XElement e) => e.Name == "TypeRW"))
					{
						goto IL_E8;
					}
					if (!current.Elements().Any((XElement e) => e.Name == "Name"))
					{
						goto IL_E8;
					}
					bool arg_EA_0 = current.Elements().Any((XElement e) => e.Name == "Addr");
					IL_E9:
					if (!arg_EA_0)
					{
						this.ErrCodeParse(ErrCode.xmlNoElement);
						result = ErrCode.xmlNoElement;
						return result;
					}
					string value = current.Element("Name").Value;
					string value2 = current.Element("Addr").Value;
					string text = current.Element("TypeRW").Value.ToLower();
					if (text != "read" && text != "write")
					{
						this._errItemName = text;
						this.ErrCodeParse(ErrCode.xmlTypeRWFlt);
						result = ErrCode.xmlTypeRWFlt;
						return result;
					}
					if (text == "read")
					{
						if (!this._items.AddrRead.ContainsKey(value) || !this._items.NameRead.ContainsKey(value2))
						{
							this._errItemName = value;
							this._errItemAddr = value2;
							this.ErrCodeParse(ErrCode.noNameOrAddr);
							result = ErrCode.noNameOrAddr;
							return result;
						}
					}
					if (text == "write")
					{
						if (!this._items.AddrWrite.ContainsKey(value) || !this._items.NameWrite.ContainsKey(value2))
						{
							this._errItemName = value;
							this._errItemAddr = value2;
							this.ErrCodeParse(ErrCode.noNameOrAddr);
							result = ErrCode.noNameOrAddr;
							return result;
						}
					}
					continue;
					IL_E8:
					arg_EA_0 = false;
					goto IL_E9;
				}
				result = ErrCode.ok;
			}
			return result;
		}

		private ErrCode CheckForAddItem(string itemAddr, string itemName, TypeRW itemType)
		{
			ErrCode result;
			switch (itemType)
			{
			case TypeRW.Read:
				if (this._items.AddrRead.ContainsKey(itemName) || this._items.NameRead.ContainsKey(itemAddr))
				{
					this._errItemName = itemName;
					this._errItemAddr = itemAddr;
					this.ErrCodeParse(ErrCode.sameNameOrAddr);
					result = ErrCode.sameNameOrAddr;
					return result;
				}
				break;
			case TypeRW.Write:
				if (this._items.AddrWrite.ContainsKey(itemName) || this._items.NameWrite.ContainsKey(itemAddr))
				{
					this._errItemName = itemName;
					this._errItemAddr = itemAddr;
					this.ErrCodeParse(ErrCode.sameNameOrAddr);
					result = ErrCode.sameNameOrAddr;
					return result;
				}
				break;
			}
			result = ErrCode.ok;
			return result;
		}

		private ErrCode CheckForRemoteOfName(string itemName, TypeRW itemType)
		{
			ErrCode result;
			switch (itemType)
			{
			case TypeRW.Read:
				if (!this._items.AddrRead.ContainsKey(itemName) || !this._items.NameRead.ContainsKey(this._items.AddrRead[itemName]))
				{
					this._errItemName = itemName;
					this._errItemAddr = "";
					this.ErrCodeParse(ErrCode.noNameOrAddr);
					result = ErrCode.noNameOrAddr;
					return result;
				}
				break;
			case TypeRW.Write:
				if (!this._items.AddrWrite.ContainsKey(itemName) || !this._items.NameWrite.ContainsKey(this._items.AddrWrite[itemName]))
				{
					this._errItemName = itemName;
					this._errItemAddr = "";
					this.ErrCodeParse(ErrCode.noNameOrAddr);
					result = ErrCode.noNameOrAddr;
					return result;
				}
				break;
			}
			result = ErrCode.ok;
			return result;
		}

		private ErrCode CheckForRemoteOfAddr(string itemAddr, TypeRW itemType)
		{
			ErrCode result;
			switch (itemType)
			{
			case TypeRW.Read:
				if (!this._items.NameRead.ContainsKey(itemAddr) || !this._items.AddrRead.ContainsKey(this._items.NameRead[itemAddr]))
				{
					this._errItemAddr = itemAddr;
					this._errItemName = "";
					this.ErrCodeParse(ErrCode.noNameOrAddr);
					result = ErrCode.noNameOrAddr;
					return result;
				}
				break;
			case TypeRW.Write:
				if (!this._items.NameWrite.ContainsKey(itemAddr) || !this._items.AddrWrite.ContainsKey(this._items.NameWrite[itemAddr]))
				{
					this._errItemAddr = itemAddr;
					this._errItemName = "";
					this.ErrCodeParse(ErrCode.noNameOrAddr);
					result = ErrCode.noNameOrAddr;
					return result;
				}
				break;
			}
			result = ErrCode.ok;
			return result;
		}

		private void RefreshOpcItemsRead(string[] addrs, int operatType)
		{
			switch (operatType)
			{
			case 1:
			{
				Item[] array = new Item[addrs.Count<string>()];
				for (int i = 0; i < addrs.Count<string>(); i++)
				{
					array[i] = new Item();
					array[i].ItemName = addrs[i];
				}
				this._client.AddItemsRead(array);
				break;
			}
			case 2:
			{
				ItemResult[] array2 = new ItemResult[addrs.Count<string>()];
				for (int i = 0; i < addrs.Count<string>(); i++)
				{
					array2[i] = this._items.OpcReadResult[addrs[i]];
				}
				this._client.RemoveItemsRead(array2);
				break;
			}
			}
		}

		private void RefreshOpcItemsWrite(string[] addrs, int operatType)
		{
			switch (operatType)
			{
			case 1:
			{
				Item[] array = new Item[addrs.Count<string>()];
				for (int i = 0; i < addrs.Count<string>(); i++)
				{
					array[i] = new Item();
					array[i].ItemName = addrs[i];
				}
				this._client.AddItemsWrite(array);
				break;
			}
			case 2:
			{
				ItemResult[] array2 = new ItemResult[addrs.Count<string>()];
				for (int i = 0; i < addrs.Count<string>(); i++)
				{
					array2[i] = this._items.OpcWriteResult[addrs[i]];
				}
				this._client.RemoveItemsWrite(array2);
				break;
			}
			}
		}

		public void Add(string itemAddr, string itemName, TypeRW itemType, out int err)
		{
			if (!this._client.GetConnectionState())
			{
				this.ErrCodeParse(ErrCode.serverConnFlt);
				err = 4;
			}
			else
			{
				ErrCode errCode = this.CheckForAddItem(itemAddr, itemName, itemType);
				if (errCode != ErrCode.ok)
				{
					err = (int)errCode;
				}
				else
				{
					switch (itemType)
					{
					case TypeRW.Read:
						this._items.AddrRead.Add(itemName, itemAddr);
						this._items.NameRead.Add(itemAddr, itemName);
						this._items.ValueRead.Add(itemAddr, new object());
						this.RefreshOpcItemsRead(new string[]
						{
							itemAddr
						}, 1);
						break;
					case TypeRW.Write:
						this._items.AddrWrite.Add(itemName, itemAddr);
						this._items.NameWrite.Add(itemAddr, itemName);
						this.RefreshOpcItemsWrite(new string[]
						{
							itemAddr
						}, 1);
						break;
					}
					err = 0;
				}
			}
		}

		public void Add(System.Collections.Generic.List<VarData> vars, out int err)
		{
			if (!this._client.GetConnectionState())
			{
				this.ErrCodeParse(ErrCode.serverConnFlt);
				err = 4;
			}
			else
			{
				foreach (VarData current in vars)
				{
					ErrCode errCode = this.CheckForAddItem(current.Address, current.Name, current.TypeRW);
					if (errCode != ErrCode.ok)
					{
						err = (int)errCode;
						return;
					}
				}
				System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(vars.Count);
				System.Collections.Generic.List<string> list2 = new System.Collections.Generic.List<string>(vars.Count);
				foreach (VarData current in vars)
				{
					switch (current.TypeRW)
					{
					case TypeRW.Read:
						this._items.AddrRead.Add(current.Name, current.Address);
						this._items.NameRead.Add(current.Address, current.Name);
						this._items.ValueRead.Add(current.Address, new object());
						list.Add(current.Address);
						break;
					case TypeRW.Write:
						this._items.AddrWrite.Add(current.Name, current.Address);
						this._items.NameWrite.Add(current.Address, current.Name);
						list2.Add(current.Address);
						break;
					}
				}
				this.RefreshOpcItemsRead(list.ToArray<string>(), 1);
				this.RefreshOpcItemsWrite(list2.ToArray<string>(), 1);
				err = 0;
			}
		}

		public void Add(XDocument xDoc, out int err)
		{
			ErrCode errCode = this.CheckXmlForAddItem(xDoc);
			if (errCode != ErrCode.ok)
			{
				err = (int)errCode;
			}
			else if (!this._client.GetConnectionState())
			{
				this.ErrCodeParse(ErrCode.serverConnFlt);
				err = 4;
			}
			else
			{
				System.Collections.Generic.IEnumerable<XElement> enumerable = xDoc.Descendants("Item");
				System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(enumerable.Count<XElement>());
				System.Collections.Generic.List<string> list2 = new System.Collections.Generic.List<string>(enumerable.Count<XElement>());
				foreach (XElement current in enumerable)
				{
					string value = current.Element("Name").Value;
					string value2 = current.Element("Addr").Value;
					string a = current.Element("TypeRW").Value.ToLower();
					if (a == "read")
					{
						this._items.AddrRead.Add(value, value2);
						this._items.NameRead.Add(value2, value);
						this._items.ValueRead.Add(value2, new object());
						list.Add(value2);
					}
					if (a == "write")
					{
						this._items.AddrWrite.Add(value, value2);
						this._items.NameWrite.Add(value2, value);
						list2.Add(value2);
					}
				}
				this.RefreshOpcItemsRead(list.ToArray<string>(), 1);
				this.RefreshOpcItemsWrite(list2.ToArray<string>(), 1);
				err = 0;
			}
		}

		public void RemoveByName(string itemName, TypeRW itemType, out int err)
		{
			if (!this._client.GetConnectionState())
			{
				this.ErrCodeParse(ErrCode.serverConnFlt);
				err = 4;
			}
			else
			{
				ErrCode errCode = this.CheckForRemoteOfName(itemName, itemType);
				if (errCode != ErrCode.ok)
				{
					err = (int)errCode;
				}
				else
				{
					switch (itemType)
					{
					case TypeRW.Read:
					{
						string text = this._items.AddrRead[itemName];
						this._items.AddrRead.Remove(itemName);
						this._items.NameRead.Remove(text);
						this._items.ValueRead.Remove(text);
						this.RefreshOpcItemsRead(new string[]
						{
							text
						}, 2);
						break;
					}
					case TypeRW.Write:
					{
						string text2 = this._items.AddrWrite[itemName];
						this._items.AddrWrite.Remove(itemName);
						this._items.NameWrite.Remove(text2);
						this.RefreshOpcItemsWrite(new string[]
						{
							text2
						}, 2);
						break;
					}
					}
					err = 0;
				}
			}
		}

		public void RemoveByAddr(string itemAddr, TypeRW itemType, out int err)
		{
			if (!this._client.GetConnectionState())
			{
				this.ErrCodeParse(ErrCode.serverConnFlt);
				err = 4;
			}
			else
			{
				ErrCode errCode = this.CheckForRemoteOfAddr(itemAddr, itemType);
				if (errCode != ErrCode.ok)
				{
					err = (int)errCode;
				}
				else
				{
					switch (itemType)
					{
					case TypeRW.Read:
					{
						string key = this._items.NameRead[itemAddr];
						this._items.AddrRead.Remove(key);
						this._items.NameRead.Remove(itemAddr);
						this._items.ValueRead.Remove(itemAddr);
						this.RefreshOpcItemsRead(new string[]
						{
							itemAddr
						}, 2);
						break;
					}
					case TypeRW.Write:
					{
						string key2 = this._items.NameWrite[itemAddr];
						this._items.AddrWrite.Remove(key2);
						this._items.NameWrite.Remove(itemAddr);
						this.RefreshOpcItemsWrite(new string[]
						{
							itemAddr
						}, 2);
						break;
					}
					}
					err = 0;
				}
			}
		}

		public void RemoveByXml(XDocument xDoc, out int err)
		{
			ErrCode errCode = this.CheckXmlForRemoteItem(xDoc);
			if (errCode != ErrCode.ok)
			{
				err = (int)errCode;
			}
			else if (!this._client.GetConnectionState())
			{
				this.ErrCodeParse(ErrCode.serverConnFlt);
				err = 4;
			}
			else
			{
				System.Collections.Generic.IEnumerable<XElement> enumerable = xDoc.Descendants("Item");
				System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(enumerable.Count<XElement>());
				System.Collections.Generic.List<string> list2 = new System.Collections.Generic.List<string>(enumerable.Count<XElement>());
				foreach (XElement current in enumerable)
				{
					string value = current.Element("Name").Value;
					string value2 = current.Element("Addr").Value;
					string a = current.Element("TypeRW").Value.ToLower();
					if (a == "read")
					{
						this._items.AddrRead.Remove(value);
						this._items.NameRead.Remove(value2);
						this._items.ValueRead.Remove(value2);
						list.Add(value2);
					}
					if (a == "write")
					{
						this._items.AddrWrite.Remove(value);
						this._items.NameWrite.Remove(value2);
						list2.Add(value2);
					}
				}
				this.RefreshOpcItemsRead(list.ToArray<string>(), 2);
				this.RefreshOpcItemsWrite(list2.ToArray<string>(), 2);
				err = 0;
			}
		}

		public void Remove(System.Collections.Generic.List<VarData> vars, out int err)
		{
			if (!this._client.GetConnectionState())
			{
				this.ErrCodeParse(ErrCode.serverConnFlt);
				err = 4;
			}
			else
			{
				foreach (VarData current in vars)
				{
					ErrCode errCode = this.CheckForRemoteOfAddr(current.Address, current.TypeRW);
					if (errCode != ErrCode.ok)
					{
						err = (int)errCode;
						return;
					}
				}
				System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(vars.Count);
				System.Collections.Generic.List<string> list2 = new System.Collections.Generic.List<string>(vars.Count);
				foreach (VarData current in vars)
				{
					switch (current.TypeRW)
					{
					case TypeRW.Read:
					{
						string key = this._items.NameRead[current.Address];
						this._items.AddrRead.Remove(key);
						this._items.NameRead.Remove(current.Address);
						this._items.ValueRead.Remove(current.Address);
						list.Add(current.Address);
						break;
					}
					case TypeRW.Write:
					{
						string key2 = this._items.NameWrite[current.Address];
						this._items.AddrWrite.Remove(key2);
						this._items.NameWrite.Remove(current.Address);
						list2.Add(current.Address);
						break;
					}
					}
				}
				this.RefreshOpcItemsRead(list.ToArray<string>(), 2);
				this.RefreshOpcItemsWrite(list2.ToArray<string>(), 2);
				err = 0;
			}
		}

		public XDocument ElementAtName(string itemName, out int err)
		{
			XDocument xDocument = new XDocument(new object[]
			{
				new XElement("Items")
			});
			XDocument result;
			if (this._items.AddrRead.ContainsKey(itemName))
			{
				string text = this._items.AddrRead[itemName];
				xDocument.Element("Items").Add(new XElement("Item", new object[]
				{
					new XElement("Name", itemName),
					new XElement("Addr", text),
					new XElement("Value", this._items.ValueRead[text]),
					new XElement("TypeRW", "read")
				}));
				err = 0;
				result = xDocument;
			}
			else if (this._items.AddrWrite.ContainsKey(itemName))
			{
				string text = this._items.AddrWrite[itemName];
				xDocument.Element("Items").Add(new XElement("Item", new object[]
				{
					new XElement("Name", itemName),
					new XElement("Addr", text),
					new XElement("TypeRW", "write")
				}));
				err = 0;
				result = xDocument;
			}
			else
			{
				this._errItemName = itemName;
				this.ErrCodeParse(ErrCode.nameNotFound);
				err = 7;
				result = null;
			}
			return result;
		}

		public XDocument ElementAtAddr(string itemAddr, out int err)
		{
			XDocument xDocument = new XDocument(new object[]
			{
				new XElement("Items")
			});
			XDocument result;
			if (this._items.AddrRead.ContainsValue(itemAddr))
			{
				string content = this._items.NameRead[itemAddr];
				xDocument.Element("Items").Add(new XElement("Item", new object[]
				{
					new XElement("Name", content),
					new XElement("Addr", itemAddr),
					new XElement("Value", this._items.ValueRead[itemAddr]),
					new XElement("TypeRW", "read")
				}));
				err = 0;
				result = xDocument;
			}
			else if (this._items.AddrWrite.ContainsValue(itemAddr))
			{
				string content = this._items.NameWrite[itemAddr];
				xDocument.Element("Items").Add(new XElement("Item", new object[]
				{
					new XElement("Name", content),
					new XElement("Addr", itemAddr),
					new XElement("TypeRW", "write")
				}));
				err = 0;
				result = xDocument;
			}
			else
			{
				this._errItemAddr = itemAddr;
				this.ErrCodeParse(ErrCode.addrNotFound);
				err = 8;
				result = null;
			}
			return result;
		}

		public XDocument GetDataAsXml(out int err)
		{
			XDocument xDocument = new XDocument(new object[]
			{
				new XElement("Items")
			});
			foreach (System.Collections.Generic.KeyValuePair<string, string> current in this._items.AddrRead)
			{
				xDocument.Element("Items").Add(new XElement("Item", new object[]
				{
					new XElement("Name", current.Key),
					new XElement("Addr", current.Value),
					new XElement("Value", this._items.ValueRead[current.Value]),
					new XElement("TypeRW", "read")
				}));
			}
			foreach (System.Collections.Generic.KeyValuePair<string, string> current in this._items.AddrWrite)
			{
				xDocument.Element("Items").Add(new XElement("Item", new object[]
				{
					new XElement("Name", current.Key),
					new XElement("Addr", current.Value),
					new XElement("TypeRW", "write")
				}));
			}
			err = 0;
			return xDocument;
		}

		public System.Collections.Generic.List<VarData> GetData(out int err)
		{
			System.Collections.Generic.List<VarData> list = new System.Collections.Generic.List<VarData>(this._items.AddrRead.Count + this._items.AddrWrite.Count);
			foreach (System.Collections.Generic.KeyValuePair<string, string> current in this._items.AddrRead)
			{
				list.Add(new VarData
				{
					Address = current.Value,
					Name = current.Key,
					Value = this._items.ValueRead[current.Value],
					TypeRW = TypeRW.Read
				});
			}
			foreach (System.Collections.Generic.KeyValuePair<string, string> current in this._items.AddrWrite)
			{
				list.Add(new VarData
				{
					Address = current.Value,
					Name = current.Key,
					Value = new object(),
					TypeRW = TypeRW.Write
				});
			}
			err = 0;
			return list;
		}

		public void Clear(out int err)
		{
			if (!this._client.GetConnectionState())
			{
				this.ErrCodeParse(ErrCode.serverConnFlt);
				err = 4;
			}
			else
			{
				string[] addrs = this._items.AddrRead.Values.ToArray<string>();
				string[] addrs2 = this._items.AddrWrite.Values.ToArray<string>();
				this._items.AddrRead.Clear();
				this._items.NameRead.Clear();
				this._items.ValueRead.Clear();
				this._items.AddrWrite.Clear();
				this._items.NameWrite.Clear();
				this.RefreshOpcItemsRead(addrs, 2);
				this.RefreshOpcItemsWrite(addrs2, 2);
				err = 0;
			}
		}
	}
}
