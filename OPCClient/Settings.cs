using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OPCClient
{
	[System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.17929"), System.ComponentModel.DesignerCategory("code"), System.Diagnostics.DebuggerStepThrough, XmlRoot(Namespace = "", IsNullable = false), XmlType(AnonymousType = true)]
	[System.Serializable]
	public class Settings
	{
		private string serverIdentifierField;

		private SettingsDataBase dataBaseField;

		private System.Collections.Generic.List<SettingsOPCServer> oPCServersField;

		private SettingsDefaultOPCServer defaultOPCServerField;

		public string ServerIdentifier
		{
			get
			{
				return this.serverIdentifierField;
			}
			set
			{
				this.serverIdentifierField = value;
			}
		}

		public SettingsDataBase DataBase
		{
			get
			{
				return this.dataBaseField;
			}
			set
			{
				this.dataBaseField = value;
			}
		}

		[XmlArrayItem("OPCServer", IsNullable = false)]
		public System.Collections.Generic.List<SettingsOPCServer> OPCServers
		{
			get
			{
				return this.oPCServersField;
			}
			set
			{
				this.oPCServersField = value;
			}
		}

		public SettingsDefaultOPCServer DefaultOPCServer
		{
			get
			{
				return this.defaultOPCServerField;
			}
			set
			{
				this.defaultOPCServerField = value;
			}
		}
	}
}
