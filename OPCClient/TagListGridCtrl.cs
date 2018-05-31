using Opc;
using Opc.Da;
using Opc.SampleClient;
using OPCClient.Properties;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OPCClient
{
	public class TagListGridCtrl : System.Windows.Forms.UserControl
	{
		private const int ICON = 0;

		private const int ID = 1;

		private const int NAME = 2;

		private const int TYPE = 3;

		private const int PARAMETERS = 4;

		private const int LAST_MODIF = 5;

		private const int ELEMENT = 6;

		private MyDataGridView TagsDGV;

		private System.Windows.Forms.TextBox FilterTB;

		private System.ComponentModel.Container components = null;

		private readonly string[] ColumnNames = new string[]
		{
			"Icon",
			"ID",
			"Name",
			"Type",
			"Parameters",
			"Last modification",
			"Element"
		};

		private readonly System.Type[] ColumnTypes = new System.Type[]
		{
			typeof(Bitmap),
			typeof(int),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(System.DateTime),
			typeof(object)
		};

		private System.Data.DataTable TagsDT;

		private System.Data.DataView TagsDV;

		private BrowseTreeCtrl BrowseCTRL;

		private Color InactiveColor;

		private BrowseFilters m_filters = null;

		private Opc.Da.Server server;

		private Opc.SampleClient.Resources resources = new Opc.SampleClient.Resources();

		public event ElementSelected_EventHandler ElementSelected;

		public event ElementSelected_EventHandler ElementPicked;

		public TagListGridCtrl()
		{
			this.InitializeComponent();
			this.SetColumns(this.ColumnNames);
			this.InactiveColor = this.FilterTB.ForeColor;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.TagsDT = new System.Data.DataTable();
			this.FilterTB = new System.Windows.Forms.TextBox();
			this.TagsDGV = new MyDataGridView();
			((System.ComponentModel.ISupportInitialize)this.TagsDT).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.TagsDGV).BeginInit();
			base.SuspendLayout();
			this.TagsDT.TableName = "Tags";
			this.FilterTB.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.FilterTB.ForeColor = SystemColors.InactiveCaptionText;
			this.FilterTB.Location = new Point(380, 3);
			this.FilterTB.Name = "FilterTB";
			this.FilterTB.Size = new Size(223, 20);
			this.FilterTB.TabIndex = 1;
			this.FilterTB.Text = "Быстрый поиск";
			this.FilterTB.TextChanged += new System.EventHandler(this.FilterTB_TextChanged);
			this.FilterTB.Enter += new System.EventHandler(this.FilterTB_Enter);
			this.FilterTB.Leave += new System.EventHandler(this.FilterTB_Leave);
			this.TagsDGV.AllowUserToAddRows = false;
			this.TagsDGV.AllowUserToDeleteRows = false;
			this.TagsDGV.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			this.TagsDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.TagsDGV.Location = new Point(0, 29);
			this.TagsDGV.MultiSelect = false;
			this.TagsDGV.Name = "TagsDGV";
			this.TagsDGV.ReadOnly = true;
			this.TagsDGV.Size = new Size(606, 366);
			this.TagsDGV.TabIndex = 0;
			this.TagsDGV.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TagsDGV_CellDoubleClick);
			this.TagsDGV.SelectionChanged += new System.EventHandler(this.TagsDGV_SelectionChanged);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.FilterTB);
			base.Controls.Add(this.TagsDGV);
			base.Name = "TagListGridCtrl";
			base.Size = new Size(606, 395);
			((System.ComponentModel.ISupportInitialize)this.TagsDT).EndInit();
			((System.ComponentModel.ISupportInitialize)this.TagsDGV).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public void Initialize(Opc.Da.Server _server, BrowseFilters _filters, BrowseTreeCtrl _BrowseCTRL)
		{
			try
			{
				this.BrowseCTRL = _BrowseCTRL;
				this.server = (Opc.Da.Server)_server.Clone();
				if (!this.server.IsConnected)
				{
					this.server.Connect();
				}
				this.TagsDT.Rows.Clear();
				this.TagsDV = this.TagsDT.DefaultView;
				this.TagsDGV.DataSource = this.TagsDV;
				this.SetGrid();
				this.m_filters = new BrowseFilters();
				this.m_filters.ReturnAllProperties = true;
				this.m_filters.ReturnPropertyValues = true;
			}
			catch (System.Exception ex)
			{
				Program.showErrorMessage(ex.Message);
			}
		}

		public void Pick()
		{
			if (this.TagsDGV.CurrentRow != null)
			{
				this.TagsDGV_CellDoubleClick(this.TagsDGV, new System.Windows.Forms.DataGridViewCellEventArgs(this.TagsDGV.CurrentCell.ColumnIndex, this.TagsDGV.CurrentCell.RowIndex));
			}
		}

		private void Find(BrowseElement _element)
		{
			if (_element.IsItem)
			{
				if (_element.Properties != null)
				{
					int num = 0;
					string text = "";
					string text2 = "";
					string text3 = "";
					System.DateTime dateTime = System.DateTime.MinValue;
					ItemProperty[] properties = _element.Properties;
					int i = 0;
					while (i < properties.Length)
					{
						ItemProperty itemProperty = properties[i];
						string description = itemProperty.Description;
						if (description != null)
						{
							if (!(description == "ID"))
							{
								if (!(description == "Name"))
								{
									if (!(description == "Type"))
									{
										if (!(description == "Parameter"))
										{
											if (description == "Last modification")
											{
												dateTime = System.Convert.ToDateTime(itemProperty.Value);
											}
										}
										else
										{
											text3 = System.Convert.ToString(itemProperty.Value);
										}
									}
									else
									{
										text2 = System.Convert.ToString(itemProperty.Value);
									}
								}
								else
								{
									text = System.Convert.ToString(itemProperty.Value);
								}
							}
							else
							{
								num = System.Convert.ToInt32(itemProperty.Value);
							}
						}
						IL_101:
						i++;
						continue;
						goto IL_101;
					}
					this.TagsDT.Rows.Add(new object[]
					{
						OPCClient.Properties.Resources.editIcon,
						num,
						text,
						text2,
						text3,
						dateTime
					});
				}
			}
			ItemIdentifier itemID = new ItemIdentifier(_element.ItemPath, _element.ItemName);
			BrowsePosition browsePosition = new BrowsePosition(itemID, this.m_filters);
			if (_element.HasChildren)
			{
				BrowseElement[] array = this.server.Browse(itemID, this.m_filters, out browsePosition);
				if (array != null)
				{
					BrowseElement[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						BrowseElement browseElement = array2[i];
						if (!browseElement.ItemName.Contains("alrosa_w") && !browseElement.ItemName.Contains("Internal") && !browseElement.ItemName.Contains("OPC") && !browseElement.ItemName.Contains("List of"))
						{
							this.Find(browseElement);
						}
					}
				}
			}
		}

		private void SetColumns(string[] columns)
		{
			this.TagsDT.Columns.Clear();
			for (int i = 0; i <= 6; i++)
			{
				this.TagsDT.Columns.Add(this.ColumnNames[i], this.ColumnTypes[i]);
				this.TagsDT.Columns[i].Caption = this.TagsDT.Columns[i].ColumnName;
			}
		}

		private void SetGrid()
		{
			this.TagsDGV.Columns[1].Visible = false;
			this.TagsDGV.Columns[6].Visible = false;
			this.TagsDGV.Columns[0].HeaderText = "";
			foreach (System.Windows.Forms.DataGridViewColumn dataGridViewColumn in this.TagsDGV.Columns)
			{
				if (!(dataGridViewColumn.Name == "Icon"))
				{
					dataGridViewColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
				}
			}
			this.TagsDGV.Columns[0].Width = 20;
		}

		public void FillTagsTable(BrowseElement _element)
		{
			this.TagsDT.Clear();
			if (_element != null)
			{
				ItemIdentifier itemID = new ItemIdentifier(_element.ItemPath, _element.ItemName);
				BrowsePosition browsePosition = new BrowsePosition(itemID, this.m_filters);
				if (_element.HasChildren)
				{
					BrowseElement[] array = this.server.Browse(itemID, this.m_filters, out browsePosition);
					if (array != null)
					{
						BrowseElement[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							BrowseElement browseElement = array2[i];
							if (!browseElement.ItemName.Contains("alrosa_w") && !browseElement.ItemName.Contains("List of"))
							{
								this.Browse(browseElement);
							}
						}
					}
				}
			}
		}

		private void Browse(BrowseElement _element)
		{
			if (_element.Properties != null)
			{
				int num = 0;
				string text = "";
				string text2 = "";
				string text3 = "";
				System.DateTime dateTime = System.DateTime.MinValue;
				ItemProperty[] properties = _element.Properties;
				int i = 0;
				while (i < properties.Length)
				{
					ItemProperty itemProperty = properties[i];
					string description = itemProperty.Description;
					if (description != null)
					{
						if (!(description == "ID"))
						{
							if (!(description == "Name"))
							{
								if (!(description == "Type"))
								{
									if (!(description == "Parameter"))
									{
										if (description == "Last modification")
										{
											dateTime = System.Convert.ToDateTime(itemProperty.Value);
										}
									}
									else
									{
										text3 = System.Convert.ToString(itemProperty.Value);
									}
								}
								else
								{
									text2 = System.Convert.ToString(itemProperty.Value);
								}
							}
							else
							{
								text = System.Convert.ToString(itemProperty.Value);
							}
						}
						else
						{
							num = System.Convert.ToInt32(itemProperty.Value);
						}
					}
					IL_EE:
					i++;
					continue;
					goto IL_EE;
				}
				Bitmap bitmap;
				if (_element.IsItem)
				{
					bitmap = (Bitmap)this.resources.ImageList.Images[Opc.SampleClient.Resources.IMAGE_GREEN_SCROLL];
				}
				else
				{
					bitmap = (Bitmap)this.resources.ImageList.Images[Opc.SampleClient.Resources.IMAGE_CLOSED_YELLOW_FOLDER];
				}
				System.Data.DataRow dataRow = this.TagsDT.Rows.Add(new object[]
				{
					bitmap,
					num,
					text,
					text2,
					text3,
					dateTime,
					_element
				});
			}
		}

		private void FilterTB_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (this.FilterTB.Text != "Быстрый поиск")
				{
					this.TagsDV.RowFilter = string.Format("[Name] like '%{0}%' or [Parameters] like '%{0}%'", this.FilterTB.Text);
				}
			}
			catch
			{
			}
		}

		private void FilterTB_Enter(object sender, System.EventArgs e)
		{
			if (this.FilterTB.ForeColor == this.InactiveColor && this.FilterTB.Text == "Быстрый поиск")
			{
				this.FilterTB.Text = "";
				this.FilterTB.ForeColor = Color.Black;
			}
		}

		private void FilterTB_Leave(object sender, System.EventArgs e)
		{
			if (this.FilterTB.Text.Length == 0)
			{
				this.FilterTB.ForeColor = this.InactiveColor;
				this.FilterTB.Text = "Быстрый поиск";
			}
		}

		private void TagsDGV_SelectionChanged(object sender, System.EventArgs e)
		{
			if (this.TagsDGV.CurrentRow != null)
			{
				System.Data.DataRow row = (this.TagsDGV.CurrentRow.DataBoundItem as System.Data.DataRowView).Row;
				if (!row.IsNull("Element"))
				{
					this.ElementSelected((BrowseElement)row["Element"]);
				}
			}
			else
			{
				this.ElementSelected(null);
			}
		}

		private void TagsDGV_CellDoubleClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
		{
			if (this.TagsDGV.CurrentRow != null)
			{
				System.Data.DataRow row = (this.TagsDGV.CurrentRow.DataBoundItem as System.Data.DataRowView).Row;
				if (!row.IsNull("Element"))
				{
					BrowseElement browseElement = (BrowseElement)row["Element"];
					if (browseElement.IsItem)
					{
						if (browseElement.IsItem && this.ElementPicked != null)
						{
							this.ElementPicked(browseElement);
						}
					}
					else
					{
						this.FillTagsTable(browseElement);
					}
				}
			}
		}
	}
}
