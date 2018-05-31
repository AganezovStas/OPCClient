using Opc;
using Opc.Da;
using OpcCom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace OPCClient
{
	public class OPCclient
	{
		private OpcCom.Factory _fact = new OpcCom.Factory();

		private SubscriptionState _groupStateRead;

		private SubscriptionState _groupStateWrite;

		private Opc.Da.Server _server;

		private Subscription _groupRead;

		private Subscription _groupWrite;

		private System.Collections.Generic.Dictionary<string, Item> _opcItemsRead;

		private System.Collections.Generic.Dictionary<string, Item> _opcItemsWrite;

		private System.Collections.Generic.Dictionary<string, ItemResult> _opcItemsReadResult;

		private System.Collections.Generic.Dictionary<string, ItemResult> _opcItemsWriteResult;

		private System.Collections.Generic.Dictionary<string, string> _itemAddrRead;

		private System.Collections.Generic.Dictionary<string, string> _itemNameRead;

		private System.Collections.Generic.Dictionary<string, object> _itemValueRead;

		private System.Collections.Generic.Dictionary<string, string> _itemAddrWrite;

		private System.Collections.Generic.Dictionary<string, string> _itemNameWrite;

		private ItemsData _itemsData = new ItemsData();

		private int _errNum = 0;

		private string _errTxt = "";

		private string _errNameItem = "";

		private bool _isServerTransData = false;

		public event System.EventHandler<ReadDataEventArgs> DataChange;

		public event System.EventHandler<ReadDataXmlEventArgs> DataChangeToXml;

		public event System.EventHandler<ErrorEventArgs> onError;

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

		public bool IsActive
		{
			get
			{
				return this._isServerTransData;
			}
		}

		public string URL
		{
			get
			{
				string result = "Неизвестно";
				if (this._server != null && this._server.Url != null)
				{
					result = this._server.Url.HostName + "\\" + this._server.Url.Path;
				}
				return result;
			}
		}

		public Opc.Da.Server server
		{
			get
			{
				return this._server;
			}
		}

		public OPCclient()
		{
			this._itemAddrRead = this._itemsData.AddrRead;
			this._itemNameRead = this._itemsData.NameRead;
			this._itemValueRead = this._itemsData.ValueRead;
			this._itemAddrWrite = this._itemsData.AddrWrite;
			this._itemNameWrite = this._itemsData.NameWrite;
			this._opcItemsRead = this._itemsData.OpcRead;
			this._opcItemsWrite = this._itemsData.OpcWrite;
			this._opcItemsReadResult = this._itemsData.OpcReadResult;
			this._opcItemsWriteResult = this._itemsData.OpcWriteResult;
		}

		private void ErrCodeParse(ErrCode errCode, [CallerMemberName] string ErrorMethodName = "")
		{
			switch (errCode)
			{
			case ErrCode.ok:
				this._errNum = 0;
				this._errTxt = "ОК";
				break;
			case ErrCode.serverNoAccess:
				this._errNum = 3;
				this._errTxt = "Ошибка доступа к серверу. Проверьте адрес и имя сервера";
				break;
			case ErrCode.serverConnFlt:
				this._errNum = 4;
				this._errTxt = "Ошибка связи с сервером. Проверьте сетевое подключение";
				break;
			case ErrCode.itemReadNoCorrect:
				this._errNum = 5;
				this._errTxt = "Переменная для чтения '" + this._errNameItem + "' не соответствует правилам описания переменных сервера";
				break;
			case ErrCode.itemWriteNoCorrect:
				this._errNum = 6;
				this._errTxt = "Переменная для записи '" + this._errNameItem + "' не соответствует правилам описания переменных сервера";
				break;
			case ErrCode.nameNotFound:
				this._errNum = 7;
				this._errTxt = "Переменная с именем " + this._errNameItem + " не существует";
				break;
			case ErrCode.varTypeOther:
				this._errNum = 9;
				this._errTxt = "Переменная с именем " + this._errNameItem + " имеет другой тип";
				break;
			case ErrCode.xmlNoElement:
				this._errNum = 12;
				this._errTxt = "Xml документ заполнен неверно - отсутствуют некоторые дочер элементы узлов Item";
				break;
			}
			if (this.onError != null)
			{
				this.onError(null, new ErrorEventArgs(this._errNum, this._errTxt, ErrorMethodName));
			}
		}

		private void DataReadChange(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				ItemValueResult itemValueResult = values[i];
				if (itemValueResult.Value != null && itemValueResult.ItemName != null && this._itemValueRead.ContainsKey(itemValueResult.ItemName))
				{
					this._itemValueRead[itemValueResult.ItemName] = itemValueResult.Value;
				}
			}
			if (this.DataChange != null)
			{
				this.DataChange(null, new ReadDataEventArgs(values, this._itemsData));
			}
			if (this.DataChangeToXml != null)
			{
				this.DataChangeToXml(null, new ReadDataXmlEventArgs(values, this._itemsData));
			}
		}

		private ErrCode CheckDataRead()
		{
			ItemResult[] array = this._opcItemsReadResult.Values.ToArray<ItemResult>();
			ItemValueResult[] array2 = this._groupRead.Read(array);
			int i = 0;
			ErrCode result;
			while (i < array2.Count<ItemValueResult>())
			{
				if (array2[i].Value == null && array2[i].ItemName != null)
				{
					this._errNameItem = this._itemNameRead[array2[i].ItemName];
					result = ErrCode.itemReadNoCorrect;
				}
				else
				{
					if (array2[i].ItemName != null)
					{
						i++;
						continue;
					}
					this._errNameItem = array[i].ItemName;
					result = ErrCode.nameNotFound;
				}
				return result;
			}
			result = ErrCode.ok;
			return result;
		}

		private ErrCode CheckDataRead(string[] addr)
		{
			ItemResult[] array = new ItemResult[addr.Count<string>()];
			int i;
			for (i = 0; i < addr.Count<string>(); i++)
			{
				array[i] = this._opcItemsReadResult[addr[i]];
			}
			ItemValueResult[] array2 = this._groupRead.Read(array);
			i = 0;
			ErrCode result;
			while (i < array2.Count<ItemValueResult>())
			{
				if (array2[i].Value == null && array2[i].ItemName != null)
				{
					this._errNameItem = this._itemNameRead[array2[i].ItemName];
					result = ErrCode.nameNotFound;
				}
				else
				{
					if (array2[i].ItemName != null)
					{
						i++;
						continue;
					}
					this._errNameItem = array[i].ItemName;
					result = ErrCode.itemReadNoCorrect;
				}
				return result;
			}
			result = ErrCode.ok;
			return result;
		}

		private ErrCode CheckDataWrite()
		{
			ItemResult[] array = this._opcItemsWriteResult.Values.ToArray<ItemResult>();
			ItemValueResult[] array2 = this._groupWrite.Read(array);
			int i = 0;
			ErrCode result;
			while (i < array2.Count<ItemValueResult>())
			{
				if (array2[i].Value == null && array2[i].ItemName != null)
				{
					this._errNameItem = this._itemNameWrite[array2[i].ItemName];
					result = ErrCode.nameNotFound;
				}
				else
				{
					if (array2[i].ItemName != null)
					{
						i++;
						continue;
					}
					this._errNameItem = array[i].ItemName;
					result = ErrCode.itemWriteNoCorrect;
				}
				return result;
			}
			result = ErrCode.ok;
			return result;
		}

		private ErrCode CheckDataWrite(string[] addr)
		{
			ItemResult[] array = new ItemResult[addr.Count<string>()];
			int i;
			for (i = 0; i < addr.Count<string>(); i++)
			{
				array[i] = this._opcItemsWriteResult[addr[i]];
			}
			ItemValueResult[] array2 = this._groupWrite.Read(array);
			i = 0;
			ErrCode result;
			while (i < array2.Count<ItemValueResult>())
			{
				if (array2[i].Value == null && array2[i].ItemName != null)
				{
					this._errNameItem = this._itemNameWrite[array2[i].ItemName];
					result = ErrCode.nameNotFound;
				}
				else
				{
					if (array2[i].ItemName != null)
					{
						i++;
						continue;
					}
					this._errNameItem = array[i].ItemName;
					result = ErrCode.itemWriteNoCorrect;
				}
				return result;
			}
			result = ErrCode.ok;
			return result;
		}

		internal void AddItemsRead(Item[] items)
		{
			ItemResult[] array = this._groupRead.AddItems(items);
			ItemResult[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ItemResult itemResult = array2[i];
				this._opcItemsReadResult.Add(itemResult.ItemName, itemResult);
			}
			ItemValueResult[] array3 = this._groupRead.Read(array);
			ItemValueResult[] array4 = array3;
			for (int i = 0; i < array4.Length; i++)
			{
				ItemValueResult itemValueResult = array4[i];
				if (itemValueResult.Value != null && itemValueResult.ItemName != null)
				{
					this._itemValueRead[itemValueResult.ItemName] = itemValueResult.Value;
				}
			}
		}

		internal void AddItemsWrite(Item[] items)
		{
			ItemResult[] array = this._groupWrite.AddItems(items);
			ItemResult[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ItemResult itemResult = array2[i];
				this._opcItemsWriteResult.Add(itemResult.ItemName, itemResult);
			}
		}

		internal void RemoveItemsRead(ItemResult[] items)
		{
			this._groupRead.RemoveItems(items);
			for (int i = 0; i < items.Length; i++)
			{
				ItemResult itemResult = items[i];
				this._opcItemsReadResult.Remove(itemResult.ItemName);
			}
		}

		internal void RemoveItemsWrite(ItemResult[] items)
		{
			this._groupWrite.RemoveItems(items);
			for (int i = 0; i < items.Length; i++)
			{
				ItemResult itemResult = items[i];
				this._opcItemsWriteResult.Remove(itemResult.ItemName);
			}
		}

		public int Init(string pathFile, TypeCnfFile typeFile)
		{
			int result;
			if (this._isServerTransData)
			{
				result = 0;
			}
			else
			{
				ErrCode errCode = ErrCode.ok;
				InitData initData = new InitData(pathFile, typeFile, this._itemsData, out errCode);
				if (errCode != ErrCode.ok)
				{
					this._errNum = initData.ErrNum;
					this._errTxt = initData.ErrTxt;
					result = (int)errCode;
				}
				else
				{
					this.ErrCodeParse(ErrCode.ok, "Init");
					result = 0;
				}
			}
			return result;
		}

		public int Init(System.Data.DataTable dataTable, TypeCnfFile typeFile)
		{
			int result;
			if (this._isServerTransData)
			{
				result = 0;
			}
			else
			{
				ErrCode errCode = ErrCode.ok;
				InitData initData = new InitData(dataTable, typeFile, this._itemsData, out errCode);
				if (errCode != ErrCode.ok)
				{
					this._errNum = initData.ErrNum;
					this._errTxt = initData.ErrTxt;
					result = (int)errCode;
				}
				else
				{
					this.ErrCodeParse(ErrCode.ok, "Init");
					result = 0;
				}
			}
			return result;
		}

		public int Connect(string ipAddrServer, string nameServer, int updInterval)
		{
			int result;
			if (this._isServerTransData)
			{
				result = 0;
			}
			else
			{
				URL url = new URL("opcda://" + ipAddrServer + "/" + nameServer);
				this._server = new Opc.Da.Server(this._fact, null);
				try
				{
					this._server.Connect(url, new ConnectData(new System.Net.NetworkCredential("root","rootnakyn")));
				}
				catch (System.Exception var_1_5C)
                {
                    this.ErrCodeParse(ErrCode.serverNoAccess, "Connect");
					result = 3;
					return result;
				}
				this._groupStateRead = new SubscriptionState();
				this._groupStateRead.Active = true;
				this._groupStateRead.Name = "GroupRead";
				this._groupStateRead.UpdateRate = updInterval;
				this._groupRead = (Subscription)this._server.CreateSubscription(this._groupStateRead);
				ItemResult[] array = this._groupRead.AddItems(this._opcItemsRead.Values.ToArray<Item>());
				this._opcItemsReadResult.Clear();
				ItemResult[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					ItemResult itemResult = array2[i];
					this._opcItemsReadResult.Add(itemResult.ItemName, itemResult);
				}
				ItemValueResult[] array3 = this._groupRead.Read(array);
				ItemValueResult[] array4 = array3;
				for (int i = 0; i < array4.Length; i++)
				{
					ItemValueResult itemValueResult = array4[i];
					if (itemValueResult.Value != null && itemValueResult.ItemName != null)
					{
						this._itemValueRead[itemValueResult.ItemName] = itemValueResult.Value;
					}
				}
				this._groupRead.DataChanged += new DataChangedEventHandler(this.DataReadChange);
				this._groupStateWrite = new SubscriptionState();
				this._groupStateWrite.Active = true;
				this._groupStateWrite.Name = "GroupWrite";
				this._groupWrite = (Subscription)this._server.CreateSubscription(this._groupStateWrite);
				ItemResult[] array5 = this._groupWrite.AddItems(this._opcItemsWrite.Values.ToArray<Item>());
				this._opcItemsWriteResult.Clear();
				array2 = array5;
				for (int i = 0; i < array2.Length; i++)
				{
					ItemResult itemResult = array2[i];
					this._opcItemsWriteResult.Add(itemResult.ItemName, itemResult);
				}
				this._isServerTransData = true;
				this.ErrCodeParse(ErrCode.ok, "Connect");
				result = 0;
			}
			return result;
		}

		public void Browse()
		{
		}

		public void RefreshItems()
		{
			this.CheckDataRead();
			ItemResult[] array = this._opcItemsReadResult.Values.ToArray<ItemResult>();
			this._opcItemsReadResult.Clear();
			ItemResult[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ItemResult itemResult = array2[i];
				this._opcItemsReadResult.Add(itemResult.ItemName, itemResult);
			}
			ItemValueResult[] array3 = this._groupRead.Read(array);
			ItemValueResult[] array4 = array3;
			for (int i = 0; i < array4.Length; i++)
			{
				ItemValueResult itemValueResult = array4[i];
				if (itemValueResult.Value != null && itemValueResult.ItemName != null)
				{
					this._itemValueRead[itemValueResult.ItemName] = itemValueResult.Value;
				}
			}
		}

		public void Disconnect()
		{
			if (this._isServerTransData)
			{
				try
				{
					this._server.Disconnect();
					this._isServerTransData = false;
					this._server.CancelSubscription(this._groupRead);
					this._server.CancelSubscription(this._groupWrite);
				}
				catch (System.Exception var_0_4A)
				{
					this.ErrCodeParse(ErrCode.serverConnFlt, "Disconnect");
				}
			}
		}

		public bool GetConnectionState()
		{
			bool result;
			try
			{
				this.ErrCodeParse(ErrCode.ok, "GetConnectionState");
				this._server.GetStatus();
				result = true;
			}
			catch (System.Exception var_0_1F)
			{
				this.ErrCodeParse(ErrCode.serverConnFlt, "GetConnectionState");
				result = false;
			}
			return result;
		}

		public Items GetItems()
		{
			return new Items(this._itemsData, this);
		}

		public void CheckInputData(out int err)
		{
			if (!this._isServerTransData)
			{
				this.ErrCodeParse(ErrCode.serverConnFlt, "CheckInputData");
				err = 4;
			}
			else
			{
				ErrCode errCode = this.CheckDataRead();
				if (errCode != ErrCode.ok)
				{
					this.ErrCodeParse(errCode, "CheckInputData");
					err = (int)errCode;
				}
				else
				{
					ErrCode errCode2 = this.CheckDataWrite();
					if (errCode2 != ErrCode.ok)
					{
						this.ErrCodeParse(errCode2, "CheckInputData");
						err = (int)errCode2;
					}
					else
					{
						err = 0;
					}
				}
			}
		}

		public void CheckInputData(System.Collections.Generic.List<VarData> vars, out int err)
		{
			if (!this._isServerTransData)
			{
				this.ErrCodeParse(ErrCode.serverConnFlt, "CheckInputData");
				err = 4;
			}
			else
			{
				System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(vars.Count<VarData>());
				System.Collections.Generic.List<string> list2 = new System.Collections.Generic.List<string>(vars.Count<VarData>());
				foreach (VarData current in vars)
				{
					if (current.TypeRW == TypeRW.Read)
					{
						list.Add(current.Address);
					}
					if (current.TypeRW == TypeRW.Write)
					{
						list2.Add(current.Address);
					}
				}
				ErrCode errCode = this.CheckDataRead(list.ToArray<string>());
				if (errCode != ErrCode.ok)
				{
					this.ErrCodeParse(errCode, "CheckInputData");
					err = (int)errCode;
				}
				else
				{
					ErrCode errCode2 = this.CheckDataWrite(list2.ToArray<string>());
					if (errCode2 != ErrCode.ok)
					{
						this.ErrCodeParse(errCode2, "CheckInputData");
						err = (int)errCode2;
					}
					else
					{
						err = 0;
					}
				}
			}
		}

		public void CheckInputData(XDocument xDoc, out int err)
		{
			if (!this._isServerTransData)
			{
				this.ErrCodeParse(ErrCode.serverConnFlt, "CheckInputData");
				err = 4;
			}
			else
			{
				System.Collections.Generic.IEnumerable<XElement> enumerable = xDoc.Descendants("Item");
				System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(enumerable.Count<XElement>());
				System.Collections.Generic.List<string> list2 = new System.Collections.Generic.List<string>(enumerable.Count<XElement>());
				try
				{
					foreach (XContainer current in enumerable)
					{
						string value = current.Element("Name").Value;
						string value2 = current.Element("Addr").Value;
						string a = current.Element("TypeRW").Value.ToLower();
						if (a == "read")
						{
							list.Add(value2);
						}
						if (a == "write")
						{
							list2.Add(value2);
						}
					}
				}
				catch (System.ArgumentException var_7_128)
				{
					this.ErrCodeParse(ErrCode.xmlNoElement, "CheckInputData");
					err = 12;
					return;
				}
				ErrCode errCode = this.CheckDataRead(list.ToArray<string>());
				if (errCode != ErrCode.ok)
				{
					this.ErrCodeParse(errCode, "CheckInputData");
					err = (int)errCode;
				}
				else
				{
					ErrCode errCode2 = this.CheckDataWrite(list2.ToArray<string>());
					if (errCode2 != ErrCode.ok)
					{
						this.ErrCodeParse(errCode2, "CheckInputData");
						err = (int)errCode2;
					}
					else
					{
						err = 0;
					}
				}
			}
		}

		public bool ReadBool(string name, out bool err)
		{
			bool result;
			try
			{
				err = false;
				result = (bool)this._itemValueRead[this._itemAddrRead[name]];
			}
			catch (System.Collections.Generic.KeyNotFoundException var_0_24)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "ReadBool");
				result = false;
			}
			catch (System.InvalidCastException var_1_41)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.varTypeOther, "ReadBool");
				result = false;
			}
			return result;
		}

		public int ReadInt16(string name, out bool err)
		{
			int result;
			try
			{
				err = false;
				result = (int)((short)this._itemValueRead[this._itemAddrRead[name]]);
			}
			catch (System.Collections.Generic.KeyNotFoundException var_0_24)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "ReadInt16");
				result = 0;
			}
			catch (System.InvalidCastException var_1_41)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.varTypeOther, "ReadInt16");
				result = 0;
			}
			return result;
		}

		public int ReadInt32(string name, out bool err)
		{
			int result;
			try
			{
				err = false;
				result = (int)this._itemValueRead[this._itemAddrRead[name]];
			}
			catch (System.Collections.Generic.KeyNotFoundException var_0_24)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "ReadInt32");
				result = 0;
			}
			catch (System.InvalidCastException var_1_41)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.varTypeOther, "ReadInt32");
				result = 0;
			}
			return result;
		}

		public double ReadReal(string name, out bool err)
		{
			double result;
			try
			{
				err = false;
				System.Type type = this._itemValueRead[this._itemAddrRead[name]].GetType();
				if (type == typeof(object))
				{
					throw new System.Collections.Generic.KeyNotFoundException();
				}
				result = System.Convert.ToDouble(this._itemValueRead[this._itemAddrRead[name]]);
			}
			catch (System.Collections.Generic.KeyNotFoundException var_1_61)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "ReadReal");
				result = 0.0;
			}
			catch (System.InvalidCastException var_2_87)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.varTypeOther, "ReadReal");
				result = 0.0;
			}
			catch (System.Exception var_3_AE)
			{
				err = true;
				this._errNameItem = name;
				result = 0.0;
			}
			return result;
		}

		public string ReadString(string name, out bool err)
		{
			string result;
			try
			{
				err = false;
				result = (string)this._itemValueRead[this._itemAddrRead[name]];
			}
			catch (System.Collections.Generic.KeyNotFoundException var_0_24)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "ReadString");
				result = "";
			}
			catch (System.InvalidCastException var_1_45)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.varTypeOther, "ReadString");
				result = "";
			}
			return result;
		}

		public void WriteBool(string name, bool setValue, out bool err)
		{
			try
			{
				Item item = this._opcItemsWriteResult[this._itemAddrWrite[name]];
				ItemValue[] array = new ItemValue[]
				{
					new ItemValue(item)
				};
				array[0].Value = setValue;
				this._groupWrite.Write(array);
			}
			catch (System.Collections.Generic.KeyNotFoundException var_2_49)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "WriteBool");
				return;
			}
			err = false;
		}

		public void WriteInt16(string name, short setValue, out bool err)
		{
			try
			{
				Item item = this._opcItemsWriteResult[this._itemAddrWrite[name]];
				ItemValue[] array = new ItemValue[]
				{
					new ItemValue(item)
				};
				array[0].Value = setValue;
				this._groupWrite.Write(array);
			}
			catch (System.Collections.Generic.KeyNotFoundException var_2_49)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "WriteInt16");
				return;
			}
			err = false;
		}

		public void WriteInt32(string name, int setValue, out bool err)
		{
			try
			{
				Item item = this._opcItemsWriteResult[this._itemAddrWrite[name]];
				ItemValue[] array = new ItemValue[]
				{
					new ItemValue(item)
				};
				array[0].Value = setValue;
				this._groupWrite.Write(array);
			}
			catch (System.Collections.Generic.KeyNotFoundException var_2_49)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "WriteInt32");
				return;
			}
			err = false;
		}

		public void WriteReal(string name, double setValue, out bool err)
		{
			try
			{
				Item item = this._opcItemsWriteResult[this._itemAddrWrite[name]];
				ItemValue[] array = new ItemValue[]
				{
					new ItemValue(item)
				};
				array[0].Value = setValue;
				this._groupWrite.Write(array);
			}
			catch (System.Collections.Generic.KeyNotFoundException var_2_49)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "WriteReal");
				return;
			}
			err = false;
		}

		public void WriteString(string name, string setValue, out bool err)
		{
			try
			{
				Item item = this._opcItemsWriteResult[this._itemAddrWrite[name]];
				ItemValue[] array = new ItemValue[]
				{
					new ItemValue(item)
				};
				array[0].Value = setValue;
				this._groupWrite.Write(array);
			}
			catch (System.Collections.Generic.KeyNotFoundException var_2_44)
			{
				err = true;
				this._errNameItem = name;
				this.ErrCodeParse(ErrCode.nameNotFound, "WriteString");
				return;
			}
			err = false;
		}
	}
}
