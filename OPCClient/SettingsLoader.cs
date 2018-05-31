using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace OPCClient
{
	internal class SettingsLoader
	{
		private System.Threading.ReaderWriterLock m_rwlock;

		private string OptionsFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings.xml");

		public Settings tmp_listObject;

		public event options_changed optChanged;

		public SettingsLoader()
		{
			try
			{
				this.m_rwlock = new System.Threading.ReaderWriterLock();
				if (!System.IO.File.Exists(this.OptionsFile))
				{
					this.Reset();
				}
				else
				{
					this.LoadOptions();
				}
			}
			catch (System.Exception ex)
			{
				Program.showErrorMessage(ex.Message);
			}
		}

		private void OptionsChanged()
		{
			if (this.optChanged != null)
			{
				this.optChanged();
			}
		}

		public void LoadOptions()
		{
			System.IO.FileStream fileStream = null;
			this.m_rwlock.AcquireWriterLock(new System.TimeSpan(0, 1, 0));
			try
			{
				this.tmp_listObject = null;
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
				fileStream = new System.IO.FileStream(this.OptionsFile, System.IO.FileMode.Open);
				this.tmp_listObject = (Settings)xmlSerializer.Deserialize(fileStream);
				if (this.tmp_listObject != null)
				{
					if (this.tmp_listObject.DataBase != null)
					{
						this.tmp_listObject.DataBase.xmlChanged += new xmlsettings_changed(this.xml_changed);
					}
					if (this.tmp_listObject.OPCServers != null && this.tmp_listObject.OPCServers.Count > 0)
					{
						foreach (SettingsOPCServer current in this.tmp_listObject.OPCServers)
						{
							current.xmlChanged += new xmlsettings_changed(this.xml_changed);
						}
					}
					if (this.tmp_listObject.DefaultOPCServer != null)
					{
						this.tmp_listObject.DefaultOPCServer.xmlChanged += new xmlsettings_changed(this.xml_changed);
					}
				}
			}
			catch (System.Exception arg)
			{
				Program.showErrorMessage(string.Format("Загрузка опций - {0}", arg));
				this.Reset();
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				this.m_rwlock.ReleaseWriterLock();
			}
		}

		private void xml_changed()
		{
			this.OptionsChanged();
		}

		public void Reset()
		{
			try
			{
				this.tmp_listObject = new Settings();
				this.tmp_listObject.ServerIdentifier = "KIASU";
				this.tmp_listObject.DataBase = new SettingsDataBase();
				this.tmp_listObject.DataBase.DataSource = "10.160.40.160";
				this.tmp_listObject.DataBase.InitialCatalog = "nurba";
				this.tmp_listObject.DataBase.UserID = "sa";
				this.tmp_listObject.DataBase.Password = "3t\"Nlg8XmM";
				this.tmp_listObject.OPCServers = new System.Collections.Generic.List<SettingsOPCServer>();
				SettingsOPCServer settingsOPCServer = new SettingsOPCServer();
				settingsOPCServer.IpAddressServer = "192.168.10.10";
				settingsOPCServer.NameServer = "OPCServer.WinCC";
				this.tmp_listObject.OPCServers.Add(settingsOPCServer);
				settingsOPCServer = new SettingsOPCServer();
				settingsOPCServer.IpAddressServer = "192.168.10.20";
				settingsOPCServer.NameServer = "OPCServer.WinCC";
				this.tmp_listObject.OPCServers.Add(settingsOPCServer);
				settingsOPCServer = new SettingsOPCServer();
				settingsOPCServer.IpAddressServer = "192.168.10.40";
				settingsOPCServer.NameServer = "OPCServer.WinCC";
				this.tmp_listObject.OPCServers.Add(settingsOPCServer);
				this.tmp_listObject.DefaultOPCServer.IpAddressServer = "192.168.10.10";
				this.tmp_listObject.DefaultOPCServer.NameServer = "OPCServer.WinCC";
			}
			catch (System.Exception ex)
			{
				Program.showErrorMessage("ResetOptions: " + ex.Message);
			}
		}

		private void CreateTableAbonents()
		{
		}

		public void Save()
		{
			System.IO.StreamWriter streamWriter = null;
			this.m_rwlock.AcquireReaderLock(new System.TimeSpan(0, 1, 0));
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
				streamWriter = new System.IO.StreamWriter(this.OptionsFile, false, System.Text.Encoding.GetEncoding(1251));
				XmlWriter xmlWriter = XmlWriter.Create(streamWriter, new XmlWriterSettings
				{
					Indent = true,
					NewLineOnAttributes = true,
					Encoding = null
				});
				xmlSerializer.Serialize(xmlWriter, this.tmp_listObject, new XmlSerializerNamespaces(new XmlQualifiedName[]
				{
					new XmlQualifiedName(string.Empty)
				}));
				streamWriter.Close();
			}
			catch (System.Exception ex)
			{
				Program.showErrorMessage(string.Format("Сохранение опций - {0}", ex.Message));
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
				}
				this.m_rwlock.ReleaseReaderLock();
			}
		}
	}
}
