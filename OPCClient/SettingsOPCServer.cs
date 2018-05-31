using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OPCClient
{
	[System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.17929"), System.ComponentModel.DesignerCategory("code"), System.Diagnostics.DebuggerStepThrough, XmlType(AnonymousType = true)]
	[System.Serializable]
	public class SettingsOPCServer
	{
		private string ipAddressServerField;

		private string nameServerField;

		[XmlIgnore]
		[System.NonSerialized]
		private xmlsettings_changed _xmlChanged;

		public event xmlsettings_changed xmlChanged
		{
			add
			{
				this._xmlChanged = (xmlsettings_changed)System.Delegate.Combine(this._xmlChanged, value);
			}
			remove
			{
				this._xmlChanged = (xmlsettings_changed)System.Delegate.Remove(this._xmlChanged, value);
			}
		}

		public string IpAddressServer
		{
			get
			{
				return this.ipAddressServerField;
			}
			set
			{
				this.ipAddressServerField = value;
				if (this._xmlChanged != null)
				{
					this._xmlChanged();
				}
			}
		}

		public string NameServer
		{
			get
			{
				return this.nameServerField;
			}
			set
			{
				this.nameServerField = value;
				if (this._xmlChanged != null)
				{
					this._xmlChanged();
				}
			}
		}
	}
}
