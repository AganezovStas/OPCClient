using Excel;
using Opc.Da;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace OPCClient
{
	internal class InitData
	{
		private ItemsData _resultData;

		private int _errNum = 0;

		private string _errTxt = "";

		private string _errItemName = "";

		private string _errItemAddr = "";

		private string _errTypeRW = "";

		private System.Data.DataTable dataTable;

		private TypeCnfFile typeFile;

		private ItemsData _itemsData;

		private ErrCode err;

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

		public InitData(string pathFile, TypeCnfFile typeFile, ItemsData items, out ErrCode errInit)
		{
			this._resultData = items;
			this.ClearItems();
			ErrCode errCode;
			switch (typeFile)
			{
			case TypeCnfFile.xlsxTable1:
			case TypeCnfFile.xlsxTable2:
				errCode = this.SelectInitTable(pathFile, typeFile);
				break;
			case TypeCnfFile.txtFile:
				errCode = this.InitFromTxt(pathFile);
				break;
			case TypeCnfFile.xmlFile:
				errCode = this.InitFromXML(pathFile);
				break;
			default:
				errCode = ErrCode.initFileNotFound;
				break;
			}
			if (errCode != ErrCode.ok)
			{
				this.ClearItems();
			}
			errInit = errCode;
		}

		public InitData(System.Data.DataTable dataTable, TypeCnfFile typeFile, ItemsData items, out ErrCode errInit)
		{
			this._resultData = items;
			this.ClearItems();
			ErrCode errCode = this.InitFromDataTable(dataTable);
			if (errCode != ErrCode.ok)
			{
				this.ClearItems();
			}
			errInit = errCode;
		}

		private void ErrCodeParse(ErrCode errCode)
		{
			switch (errCode)
			{
			case ErrCode.ok:
				this._errNum = 0;
				this._errTxt = "ОК";
				break;
			case ErrCode.initFileNotFound:
				this._errNum = 1;
				this._errTxt = "Файл инициализации не найден";
				break;
			case ErrCode.initFileIncorrect:
				this._errNum = 2;
				this._errTxt = "Файл инициализации заполнен неверно";
				break;
			default:
				switch (errCode)
				{
				case ErrCode.sameNameOrAddr:
					this._errNum = 10;
					this._errTxt = string.Concat(new string[]
					{
						"Файл инициализации заполнен неверно - имя ",
						this._errItemName,
						" или адрес ",
						this._errItemAddr,
						" повторяются"
					});
					break;
				case ErrCode.xmlNoElement:
					this._errNum = 12;
					this._errTxt = "Xml файл инициализации заполнен неверно - отсутствуют некоторые дочер элементы узлов Item";
					break;
				case ErrCode.xmlTypeRWFlt:
					this._errNum = 14;
					this._errTxt = "Xml файл инициализации заполнен неверно - TypeRW" + this._errTypeRW + " не определен";
					break;
				}
				break;
			}
		}

		private ErrCode SelectInitTable(string pathFile, TypeCnfFile typeFile)
		{
			ErrCode errCode = ErrCode.ok;
			System.IO.FileStream fileStream;
			ErrCode result;
			try
			{
				fileStream = System.IO.File.Open(pathFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			}
			catch (System.IO.FileNotFoundException var_2_10)
			{
				this.ErrCodeParse(ErrCode.initFileNotFound);
				result = ErrCode.initFileNotFound;
				return result;
			}
			IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
			excelDataReader.IsFirstRowAsColumnNames = true;
			System.Data.DataSet dataSet = excelDataReader.AsDataSet();
			fileStream.Close();
			if (typeFile == TypeCnfFile.xlsxTable1)
			{
				if (dataSet.Tables.Contains("Read"))
				{
					errCode = this.TableRead(dataSet.Tables["Read"]);
					if (errCode != ErrCode.ok)
					{
						result = errCode;
						return result;
					}
				}
				if (dataSet.Tables.Contains("Write"))
				{
					errCode = this.TableWrite(dataSet.Tables["Write"]);
					if (errCode != ErrCode.ok)
					{
						result = errCode;
						return result;
					}
				}
			}
			else
			{
				errCode = this.TableOver(dataSet.Tables[0]);
			}
			result = errCode;
			return result;
		}

		private ErrCode InitFromDataTable(System.Data.DataTable dataTable)
		{
			return this.TableOver(dataTable);
		}

		private ErrCode TableRead(System.Data.DataTable dTable)
		{
			string text = "";
			string text2 = "";
			ErrCode result;
			try
			{
				foreach (System.Data.DataRow dataRow in dTable.Rows)
				{
					text = dataRow[0].ToString();
					text2 = dataRow[1].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrRead.Add(text2, text);
						this._resultData.NameRead.Add(text, text2);
						this._resultData.ValueRead.Add(text, new object());
					}
					text = dataRow[2].ToString();
					text2 = dataRow[3].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrRead.Add(text2, text);
						this._resultData.NameRead.Add(text, text2);
						this._resultData.ValueRead.Add(text, new object());
					}
					text = dataRow[4].ToString();
					text2 = dataRow[5].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrRead.Add(text2, text);
						this._resultData.NameRead.Add(text, text2);
						this._resultData.ValueRead.Add(text, new object());
					}
					text = dataRow[6].ToString();
					text2 = dataRow[7].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrRead.Add(text2, text);
						this._resultData.NameRead.Add(text, text2);
						this._resultData.ValueRead.Add(text, new object());
					}
					text = dataRow[8].ToString();
					text2 = dataRow[9].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrRead.Add(text2, text);
						this._resultData.NameRead.Add(text, text2);
						this._resultData.ValueRead.Add(text, new object());
					}
				}
			}
			catch (System.ArgumentException var_3_2DA)
			{
				this._errItemName = text2;
				this._errItemAddr = text;
				this.ErrCodeParse(ErrCode.sameNameOrAddr);
				result = ErrCode.sameNameOrAddr;
				return result;
			}
			this.CreateItemsRead();
			result = ErrCode.ok;
			return result;
		}

		private ErrCode TableWrite(System.Data.DataTable dTable)
		{
			string text = "";
			string text2 = "";
			ErrCode result;
			try
			{
				foreach (System.Data.DataRow dataRow in dTable.Rows)
				{
					text = dataRow[0].ToString();
					text2 = dataRow[1].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrWrite.Add(text2, text);
						this._resultData.NameWrite.Add(text, text2);
					}
					text = dataRow[2].ToString();
					text2 = dataRow[3].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrWrite.Add(text2, text);
						this._resultData.NameWrite.Add(text, text2);
					}
					text = dataRow[4].ToString();
					text2 = dataRow[5].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrWrite.Add(text2, text);
						this._resultData.NameWrite.Add(text, text2);
					}
					text = dataRow[6].ToString();
					text2 = dataRow[7].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrWrite.Add(text2, text);
						this._resultData.NameWrite.Add(text, text2);
					}
					text = dataRow[8].ToString();
					text2 = dataRow[9].ToString();
					if (text != "" && text2 != "")
					{
						this._resultData.AddrWrite.Add(text2, text);
						this._resultData.NameWrite.Add(text, text2);
					}
				}
			}
			catch (System.ArgumentException var_3_267)
			{
				this._errItemName = text2;
				this._errItemAddr = text;
				this.ErrCodeParse(ErrCode.sameNameOrAddr);
				result = ErrCode.sameNameOrAddr;
				return result;
			}
			this.CreateItemsWrite();
			result = ErrCode.ok;
			return result;
		}

		private ErrCode TableOver(System.Data.DataTable dTable)
		{
			string text = "";
			string text2 = "";
			ErrCode result;
			try
			{
				foreach (System.Data.DataRow dataRow in dTable.Rows)
				{
					text = dataRow[0].ToString();
					text2 = dataRow[1].ToString();
					string text3 = dataRow[3].ToString().ToLower();
					if (text != "" && text2 != "")
					{
						if (!this._resultData.AddrRead.ContainsValue(text2))
						{
							if (text3 == "read")
							{
								this._resultData.AddrRead.Add(text2, text);
								this._resultData.NameRead.Add(text, text2);
								this._resultData.ValueRead.Add(text, new object());
							}
							else if (text3 == "write")
							{
								this._resultData.AddrWrite.Add(text2, text);
								this._resultData.NameWrite.Add(text, text2);
							}
							else
							{
								if (!(text3 == "read/write"))
								{
									this._errTypeRW = text3;
									this.ErrCodeParse(ErrCode.xmlTypeRWFlt);
									result = ErrCode.xmlTypeRWFlt;
									return result;
								}
								this._resultData.AddrRead.Add(text2, text);
								this._resultData.NameRead.Add(text, text2);
								this._resultData.ValueRead.Add(text, new object());
								this._resultData.AddrWrite.Add(text2, text);
								this._resultData.NameWrite.Add(text, text2);
							}
						}
					}
				}
			}
			catch (System.ArgumentException var_4_204)
			{
				this._errItemName = text2;
				this._errItemAddr = text;
				this.ErrCodeParse(ErrCode.sameNameOrAddr);
				result = ErrCode.sameNameOrAddr;
				return result;
			}
			this.CreateItemsRead();
			this.CreateItemsWrite();
			result = ErrCode.ok;
			return result;
		}

		private ErrCode InitFromTxt(string pathFile)
		{
			bool flag = false;
			bool flag2 = false;
			System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(100);
			ErrCode result;
			try
			{
				System.IO.StreamReader streamReader = System.IO.File.OpenText(pathFile);
				while (!streamReader.EndOfStream)
				{
					list.Add(streamReader.ReadLine());
				}
				streamReader.Close();
			}
			catch (System.IO.FileNotFoundException var_4_3F)
			{
				this.ErrCodeParse(ErrCode.initFileNotFound);
				result = ErrCode.initFileNotFound;
				return result;
			}
			string text = "";
			string text2 = "";
			try
			{
				foreach (string current in list)
				{
					if (current.Trim().ToLower() == "read_data")
					{
						flag = true;
						flag2 = false;
					}
					else if (current.Trim().ToLower() == "write_data")
					{
						flag2 = true;
						flag = false;
					}
					else
					{
						if (flag)
						{
							if (current.Trim().Length < 5 || current.Trim().Substring(0, 2) == "//")
							{
								continue;
							}
							string[] array = current.Trim().Split(new char[]
							{
								' '
							}, System.StringSplitOptions.RemoveEmptyEntries);
							text = array[1];
							text2 = array[0];
							this._resultData.AddrRead.Add(text, text2);
							this._resultData.NameRead.Add(text2, text);
							this._resultData.ValueRead.Add(text2, new object());
						}
						if (flag2)
						{
							if (current.Trim().Length >= 5 && !(current.Trim().Substring(0, 2) == "//"))
							{
								string[] array = current.Trim().Split(new char[]
								{
									' '
								}, System.StringSplitOptions.RemoveEmptyEntries);
								text = array[1];
								text2 = array[0];
								this._resultData.AddrWrite.Add(text, text2);
								this._resultData.NameWrite.Add(text2, text);
							}
						}
					}
				}
			}
			catch (System.ArgumentException var_9_241)
			{
				this._errItemName = text;
				this._errItemAddr = text2;
				this.ErrCodeParse(ErrCode.sameNameOrAddr);
				result = ErrCode.sameNameOrAddr;
				return result;
			}
			catch (System.IndexOutOfRangeException var_10_263)
			{
				this.ErrCodeParse(ErrCode.initFileIncorrect);
				result = ErrCode.initFileIncorrect;
				return result;
			}
			this.CreateItemsRead();
			this.CreateItemsWrite();
			result = ErrCode.ok;
			return result;
		}

		private ErrCode InitFromXML(string pathFile)
		{
			XDocument xDocument = new XDocument();
			ErrCode result;
			try
			{
				xDocument = XDocument.Load(pathFile);
			}
			catch (System.IO.FileNotFoundException var_1_12)
			{
				this.ErrCodeParse(ErrCode.initFileNotFound);
				result = ErrCode.initFileNotFound;
				return result;
			}
			catch (XmlException var_2_24)
			{
				this.ErrCodeParse(ErrCode.initFileIncorrect);
				result = ErrCode.initFileIncorrect;
				return result;
			}
			string text = "";
			string text2 = "";
			try
			{
				System.Collections.Generic.IEnumerable<XElement> enumerable = xDocument.Descendants("Item");
				if (enumerable.Count<XElement>() == 0)
				{
					result = ErrCode.initFileIncorrect;
					return result;
				}
				foreach (XElement current in enumerable)
				{
					text = current.Element("Name").Value;
					text2 = current.Element("Addr").Value;
					string text3 = current.Element("TypeRW").Value.ToLower();
					if (text3 == "read")
					{
						this._resultData.AddrRead.Add(text, text2);
						this._resultData.NameRead.Add(text2, text);
						this._resultData.ValueRead.Add(text2, new object());
					}
					else
					{
						if (!(text3 == "write"))
						{
							this._errTypeRW = text3;
							this.ErrCodeParse(ErrCode.xmlTypeRWFlt);
							result = ErrCode.xmlTypeRWFlt;
							return result;
						}
						this._resultData.AddrWrite.Add(text, text2);
						this._resultData.NameWrite.Add(text2, text);
					}
				}
			}
			catch (System.NullReferenceException var_8_1B7)
			{
				this.ErrCodeParse(ErrCode.xmlNoElement);
				result = ErrCode.xmlNoElement;
				return result;
			}
			catch (System.ArgumentException var_9_1C9)
			{
				this._errItemName = text;
				this._errItemAddr = text2;
				this.ErrCodeParse(ErrCode.sameNameOrAddr);
				result = ErrCode.sameNameOrAddr;
				return result;
			}
			this.CreateItemsRead();
			this.CreateItemsWrite();
			result = ErrCode.ok;
			return result;
		}

		private void CreateItemsRead()
		{
			foreach (System.Collections.Generic.KeyValuePair<string, string> current in this._resultData.AddrRead)
			{
				this._resultData.OpcRead[current.Value] = new Item();
				this._resultData.OpcRead[current.Value].ItemName = current.Value;
			}
		}

		private void CreateItemsWrite()
		{
			foreach (System.Collections.Generic.KeyValuePair<string, string> current in this._resultData.AddrWrite)
			{
				this._resultData.OpcWrite[current.Value] = new Item();
				this._resultData.OpcWrite[current.Value].ItemName = current.Value;
			}
		}

		private void ClearItems()
		{
			this._resultData.AddrRead.Clear();
			this._resultData.NameRead.Clear();
			this._resultData.ValueRead.Clear();
			this._resultData.OpcRead.Clear();
			this._resultData.OpcReadResult.Clear();
			this._resultData.AddrWrite.Clear();
			this._resultData.NameWrite.Clear();
			this._resultData.OpcWrite.Clear();
			this._resultData.OpcWriteResult.Clear();
		}
	}
}
