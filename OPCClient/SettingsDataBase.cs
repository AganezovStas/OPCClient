using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OPCClient
{
	[System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.17929"), System.ComponentModel.DesignerCategory("code"), System.Diagnostics.DebuggerStepThrough, XmlType(AnonymousType = true)]
	[System.Serializable]
	public class SettingsDataBase
	{
		private string dataSourceField;

		private string userIDField;

		private string passwordField;

		private bool integratedSecurityField;

		private string initialCatalogField;

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

		public string DataSource
		{
			get
			{
				return this.dataSourceField;
			}
			set
			{
				this.dataSourceField = value;
				if (this._xmlChanged != null)
				{
					this._xmlChanged();
				}
			}
		}

		public string UserID
		{
			get
			{
				return this.userIDField;
			}
			set
			{
				this.userIDField = value;
				if (this._xmlChanged != null)
				{
					this._xmlChanged();
				}
			}
		}

		public string Password
		{
			get
			{
				return this.passwordField;
			}
			set
			{
				this.passwordField = value;
				if (this._xmlChanged != null)
				{
					this._xmlChanged();
				}
			}
		}

		public bool IntegratedSecurity
		{
			get
			{
				return this.integratedSecurityField;
			}
			set
			{
				this.integratedSecurityField = value;
				if (this._xmlChanged != null)
				{
					this._xmlChanged();
				}
			}
		}

		public string InitialCatalog
		{
			get
			{
				return this.initialCatalogField;
			}
			set
			{
				this.initialCatalogField = value;
				if (this._xmlChanged != null)
				{
					this._xmlChanged();
				}
			}
		}
	}
}
