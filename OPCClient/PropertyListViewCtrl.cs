using Opc;
using Opc.Cpx;
using Opc.Da;
using Opc.SampleClient;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OPCClient
{
	public class PropertyListViewCtrl : System.Windows.Forms.UserControl
	{
		private const int ID = 0;

		private const int DESCRIPTION = 1;

		private const int VALUE = 2;

		private const int DATA_TYPE = 3;

		private const int ITEM_PATH = 4;

		private const int ITEM_NAME = 5;

		private const int ERROR = 6;

		private System.Windows.Forms.ListView PropertiesLV;

		private System.Windows.Forms.ContextMenu PopupMenu;

		private System.Windows.Forms.MenuItem RemoveMI;

		private System.Windows.Forms.MenuItem EditMI;

		private System.Windows.Forms.MenuItem CopyMI;

		private System.Windows.Forms.MenuItem ViewMI;

		private System.ComponentModel.Container components = null;

		private readonly string[] ColumnNames = new string[]
		{
			"ID",
			"Description",
			"Value",
			"Data Type",
			"Item Path",
			"Item Name",
			"Result"
		};

		private BrowseElement m_element = null;

		public PropertyListViewCtrl()
		{
			this.InitializeComponent();
			this.PropertiesLV.SmallImageList = Resources.Instance.ImageList;
			this.SetColumns(this.ColumnNames);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
				{
					this.components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.PropertiesLV = new System.Windows.Forms.ListView();
			this.PopupMenu = new System.Windows.Forms.ContextMenu();
			this.ViewMI = new System.Windows.Forms.MenuItem();
			this.CopyMI = new System.Windows.Forms.MenuItem();
			this.EditMI = new System.Windows.Forms.MenuItem();
			this.RemoveMI = new System.Windows.Forms.MenuItem();
			base.SuspendLayout();
			this.PropertiesLV.ContextMenu = this.PopupMenu;
			this.PropertiesLV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PropertiesLV.FullRowSelect = true;
			this.PropertiesLV.MultiSelect = false;
			this.PropertiesLV.Name = "PropertiesLV";
			this.PropertiesLV.Size = new Size(432, 272);
			this.PropertiesLV.TabIndex = 0;
			this.PropertiesLV.View = System.Windows.Forms.View.Details;
			this.PropertiesLV.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PropertiesLV_MouseDown);
			this.PropertiesLV.DoubleClick += new System.EventHandler(this.ViewMI_Click);
			this.PopupMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
			{
				this.ViewMI
			});
			this.ViewMI.Index = 0;
			this.ViewMI.Text = "&View...";
			this.ViewMI.Click += new System.EventHandler(this.ViewMI_Click);
			this.CopyMI.Index = -1;
			this.CopyMI.Text = "";
			this.EditMI.Index = -1;
			this.EditMI.Text = "";
			this.RemoveMI.Index = -1;
			this.RemoveMI.Text = "";
			this.AllowDrop = true;
			base.Controls.AddRange(new System.Windows.Forms.Control[]
			{
				this.PropertiesLV
			});
			base.Name = "PropertyListViewCtrl";
			base.Size = new Size(432, 272);
			base.ResumeLayout(false);
		}

		public void Initialize(BrowseElement element)
		{
			this.PropertiesLV.Items.Clear();
			if (element != null && element.Properties != null)
			{
				this.m_element = element;
				ItemProperty[] properties = element.Properties;
				for (int i = 0; i < properties.Length; i++)
				{
					ItemProperty property = properties[i];
					this.AddProperty(property);
				}
				this.AdjustColumns();
			}
		}

		private void SetColumns(string[] columns)
		{
			this.PropertiesLV.Clear();
			for (int i = 0; i < columns.Length; i++)
			{
				string text = columns[i];
				System.Windows.Forms.ColumnHeader columnHeader = new System.Windows.Forms.ColumnHeader();
				columnHeader.Text = text;
				this.PropertiesLV.Columns.Add(columnHeader);
			}
			this.AdjustColumns();
		}

		private void AdjustColumns()
		{
			for (int i = 0; i < this.PropertiesLV.Columns.Count; i++)
			{
				if (i == 0 || i == 2)
				{
					this.PropertiesLV.Columns[i].Width = -2;
				}
				else
				{
					bool flag = true;
					foreach (System.Windows.Forms.ListViewItem listViewItem in this.PropertiesLV.Items)
					{
						if (listViewItem.SubItems[i].Text != "")
						{
							flag = false;
							this.PropertiesLV.Columns[i].Width = -2;
							break;
						}
					}
					if (flag)
					{
						this.PropertiesLV.Columns[i].Width = 0;
					}
				}
			}
		}

		private object GetFieldValue(ItemProperty property, int fieldID)
		{
			object result;
			switch (fieldID)
			{
			case 0:
				result = property.ID.ToString();
				break;
			case 1:
				result = property.Description;
				break;
			case 2:
				result = property.Value;
				break;
			case 3:
				result = ((property.Value != null) ? property.Value.GetType() : null);
				break;
			case 4:
				result = property.ItemPath;
				break;
			case 5:
				result = property.ItemName;
				break;
			case 6:
				result = property.ResultID;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		private void AddProperty(ItemProperty property)
		{
			System.Windows.Forms.ListViewItem listViewItem = new System.Windows.Forms.ListViewItem((string)this.GetFieldValue(property, 0), Resources.IMAGE_EXPLODING_BOX);
			listViewItem.SubItems.Add(Opc.Convert.ToString(this.GetFieldValue(property, 1)));
			listViewItem.SubItems.Add(Opc.Convert.ToString(this.GetFieldValue(property, 2)));
			listViewItem.SubItems.Add(Opc.Convert.ToString(this.GetFieldValue(property, 3)));
			listViewItem.SubItems.Add(Opc.Convert.ToString(this.GetFieldValue(property, 4)));
			listViewItem.SubItems.Add(Opc.Convert.ToString(this.GetFieldValue(property, 5)));
			listViewItem.SubItems.Add(Opc.Convert.ToString(this.GetFieldValue(property, 6)));
			listViewItem.Tag = property;
			this.PropertiesLV.Items.Add(listViewItem);
		}

		private void ViewMI_Click(object sender, System.EventArgs e)
		{
			if (this.PropertiesLV.SelectedItems.Count > 0)
			{
				object tag = this.PropertiesLV.SelectedItems[0].Tag;
				if (tag != null && tag.GetType() == typeof(ItemProperty))
				{
					ItemProperty itemProperty = (ItemProperty)tag;
					if (itemProperty.Value != null)
					{
						if (itemProperty.ID == Property.VALUE)
						{
							ComplexItem complexItem = ComplexTypeCache.GetComplexItem(this.m_element);
							if (complexItem != null)
							{
								new EditComplexValueDlg().ShowDialog(complexItem, itemProperty.Value, true, true);
							}
						}
						else if (itemProperty.Value.GetType().IsArray)
						{
							new EditArrayDlg().ShowDialog(itemProperty.Value, true);
						}
					}
				}
			}
		}

		private void PropertiesLV_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				this.ViewMI.Enabled = false;
				System.Windows.Forms.ListViewItem itemAt = this.PropertiesLV.GetItemAt(e.X, e.Y);
				if (itemAt != null)
				{
					itemAt.Selected = true;
					if (this.PropertiesLV.SelectedItems.Count == 1)
					{
						this.ViewMI.Enabled = true;
					}
				}
			}
		}
	}
}
