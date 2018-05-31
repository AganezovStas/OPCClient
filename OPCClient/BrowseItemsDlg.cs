using Opc;
using Opc.Da;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace OPCClient
{
	public class BrowseItemsDlg : System.Windows.Forms.Form
	{
		private BrowseTreeCtrl BrowseCTRL;

		private System.Windows.Forms.Panel ButtonsPN;

		private System.Windows.Forms.Button DoneBTN;

		private System.Windows.Forms.Panel LeftPN;

		private System.Windows.Forms.Panel RightPN;

		private System.Windows.Forms.Splitter SplitterVL;

		private PropertyListViewCtrl PropertiesCTRL;

		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.Button button1;

		private System.Windows.Forms.Panel CenterPN;

		private System.Windows.Forms.Splitter SplitterVR;

		private TagListGridCtrl TagGridCTRL;

		public string tag = "";

		private Opc.Da.Server m_server = null;

		private ItemIdentifier m_itemID = null;

		private BrowseElement _element = null;

		private System.Collections.Generic.List<System.Windows.Forms.TreeNode> TreeN = new System.Collections.Generic.List<System.Windows.Forms.TreeNode>();

		public BrowseElement Element
		{
			get
			{
				return this._element;
			}
		}

		public BrowseItemsDlg()
		{
			this.InitializeComponent();
			this.BrowseCTRL.ElementSelected += new ElementSelected_EventHandler(this.OnElementSelected);
			this.BrowseCTRL.ItemPicked += new ItemPicked_EventHandler(this.BrowseCTRL_ItemPicked);
			this.TagGridCTRL.ElementSelected += new ElementSelected_EventHandler(this.TagGridCTRL_ElementSelected);
			this.TagGridCTRL.ElementPicked += new ElementSelected_EventHandler(this.TagGridCTRL_ElementPicked);
		}

		private void TagGridCTRL_ElementSelected(BrowseElement element)
		{
			this.PropertiesCTRL.Initialize(element);
			if (element != null)
			{
				this.DoneBTN.Enabled = element.IsItem;
			}
			else
			{
				this.DoneBTN.Enabled = false;
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowseItemsDlg));
            this.LeftPN = new System.Windows.Forms.Panel();
            this.BrowseCTRL = new OPCClient.BrowseTreeCtrl();
            this.RightPN = new System.Windows.Forms.Panel();
            this.PropertiesCTRL = new OPCClient.PropertyListViewCtrl();
            this.ButtonsPN = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.DoneBTN = new System.Windows.Forms.Button();
            this.SplitterVL = new System.Windows.Forms.Splitter();
            this.CenterPN = new System.Windows.Forms.Panel();
            this.TagGridCTRL = new OPCClient.TagListGridCtrl();
            this.SplitterVR = new System.Windows.Forms.Splitter();
            this.LeftPN.SuspendLayout();
            this.RightPN.SuspendLayout();
            this.ButtonsPN.SuspendLayout();
            this.CenterPN.SuspendLayout();
            this.SuspendLayout();
            // 
            // LeftPN
            // 
            this.LeftPN.Controls.Add(this.BrowseCTRL);
            this.LeftPN.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftPN.Location = new System.Drawing.Point(0, 0);
            this.LeftPN.Name = "LeftPN";
            this.LeftPN.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.LeftPN.Size = new System.Drawing.Size(341, 548);
            this.LeftPN.TabIndex = 6;
            // 
            // BrowseCTRL
            // 
            this.BrowseCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BrowseCTRL.Location = new System.Drawing.Point(4, 4);
            this.BrowseCTRL.Name = "BrowseCTRL";
            this.BrowseCTRL.Size = new System.Drawing.Size(337, 544);
            this.BrowseCTRL.TabIndex = 1;
            // 
            // RightPN
            // 
            this.RightPN.Controls.Add(this.PropertiesCTRL);
            this.RightPN.Dock = System.Windows.Forms.DockStyle.Right;
            this.RightPN.Location = new System.Drawing.Point(919, 0);
            this.RightPN.Name = "RightPN";
            this.RightPN.Padding = new System.Windows.Forms.Padding(0, 4, 4, 0);
            this.RightPN.Size = new System.Drawing.Size(266, 548);
            this.RightPN.TabIndex = 8;
            // 
            // PropertiesCTRL
            // 
            this.PropertiesCTRL.AllowDrop = true;
            this.PropertiesCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertiesCTRL.Location = new System.Drawing.Point(0, 4);
            this.PropertiesCTRL.Name = "PropertiesCTRL";
            this.PropertiesCTRL.Size = new System.Drawing.Size(262, 544);
            this.PropertiesCTRL.TabIndex = 0;
            // 
            // ButtonsPN
            // 
            this.ButtonsPN.Controls.Add(this.button1);
            this.ButtonsPN.Controls.Add(this.DoneBTN);
            this.ButtonsPN.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPN.Location = new System.Drawing.Point(0, 548);
            this.ButtonsPN.Name = "ButtonsPN";
            this.ButtonsPN.Size = new System.Drawing.Size(1185, 36);
            this.ButtonsPN.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(355, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DoneBTN
            // 
            this.DoneBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.DoneBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DoneBTN.Location = new System.Drawing.Point(555, 8);
            this.DoneBTN.Name = "DoneBTN";
            this.DoneBTN.Size = new System.Drawing.Size(75, 23);
            this.DoneBTN.TabIndex = 0;
            this.DoneBTN.Text = "Done";
            this.DoneBTN.Click += new System.EventHandler(this.DoneBTN_Click);
            // 
            // SplitterVL
            // 
            this.SplitterVL.Location = new System.Drawing.Point(341, 0);
            this.SplitterVL.Name = "SplitterVL";
            this.SplitterVL.Size = new System.Drawing.Size(3, 548);
            this.SplitterVL.TabIndex = 9;
            this.SplitterVL.TabStop = false;
            // 
            // CenterPN
            // 
            this.CenterPN.Controls.Add(this.TagGridCTRL);
            this.CenterPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CenterPN.Location = new System.Drawing.Point(344, 0);
            this.CenterPN.Name = "CenterPN";
            this.CenterPN.Size = new System.Drawing.Size(575, 548);
            this.CenterPN.TabIndex = 10;
            // 
            // TagGridCTRL
            // 
            this.TagGridCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TagGridCTRL.Location = new System.Drawing.Point(0, 0);
            this.TagGridCTRL.Name = "TagGridCTRL";
            this.TagGridCTRL.Size = new System.Drawing.Size(575, 548);
            this.TagGridCTRL.TabIndex = 0;
            // 
            // SplitterVR
            // 
            this.SplitterVR.Dock = System.Windows.Forms.DockStyle.Right;
            this.SplitterVR.Location = new System.Drawing.Point(916, 0);
            this.SplitterVR.Name = "SplitterVR";
            this.SplitterVR.Size = new System.Drawing.Size(3, 548);
            this.SplitterVR.TabIndex = 11;
            this.SplitterVR.TabStop = false;
            // 
            // BrowseItemsDlg
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1185, 584);
            this.Controls.Add(this.SplitterVR);
            this.Controls.Add(this.CenterPN);
            this.Controls.Add(this.RightPN);
            this.Controls.Add(this.SplitterVL);
            this.Controls.Add(this.LeftPN);
            this.Controls.Add(this.ButtonsPN);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BrowseItemsDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Browse Address Space";
            this.LeftPN.ResumeLayout(false);
            this.RightPN.ResumeLayout(false);
            this.ButtonsPN.ResumeLayout(false);
            this.CenterPN.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		public ItemIdentifier ShowDialog(Opc.Da.Server server)
		{
			ItemIdentifier result;
			try
			{
				if (server == null)
				{
					throw new System.ArgumentNullException("server");
				}
				this.m_server = server;
				this.m_itemID = null;
				BrowseFilters browseFilters = new BrowseFilters();
				browseFilters.ReturnAllProperties = false;
				browseFilters.ReturnPropertyValues = false;
				this.BrowseCTRL.ShowSingleServer(this.m_server, browseFilters);
				this.PropertiesCTRL.Initialize(null);
				if (base.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				{
					result = null;
				}
				else
				{
					result = this.m_itemID;
				}
			}
			finally
			{
				this.BrowseCTRL.Clear();
			}
			return result;
		}

		public System.Windows.Forms.DialogResult Initialize(Opc.Da.Server server)
		{
			if (server == null)
			{
				throw new System.ArgumentNullException("server");
			}
			this.m_server = server;
			BrowseFilters browseFilters = new BrowseFilters();
			browseFilters.ReturnAllProperties = true;
			browseFilters.ReturnPropertyValues = true;
			this.BrowseCTRL.ShowSingleServer(this.m_server, browseFilters);
			this.PropertiesCTRL.Initialize(null);
			this.TagGridCTRL.Initialize(this.m_server, browseFilters, this.BrowseCTRL);
			System.Windows.Forms.DialogResult result = base.ShowDialog();
			this.BrowseCTRL.Clear();
			return result;
		}

		private void OnElementSelected(BrowseElement element)
		{
			this.TagGridCTRL.FillTagsTable(element);
			if (element != null)
			{
				this.DoneBTN.Enabled = element.IsItem;
			}
			else
			{
				this.DoneBTN.Enabled = false;
			}
			this._element = element;
		}

		private void DoneBTN_Click(object sender, System.EventArgs e)
		{
			this.TagGridCTRL.Pick();
		}

		private void BrowseCTRL_ItemPicked(ItemIdentifier itemID)
		{
			this.m_itemID = itemID;
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void TagGridCTRL_ElementPicked(BrowseElement element)
		{
			this._element = element;
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void Find(System.Windows.Forms.TreeNodeCollection Nodes, string find)
		{
			foreach (System.Windows.Forms.TreeNode treeNode in Nodes)
			{
				if (treeNode.Text.Contains(find) && treeNode.Nodes.Count != 0)
				{
					this.TreeN.Add(treeNode);
					break;
				}
				if (this.BrowseCTRL.IsBrowseElementNode(treeNode))
				{
					if (treeNode.Nodes.Count >= 1 && treeNode.Nodes[0].Text == "")
					{
						if (!treeNode.Text.Contains("alrosa_w") && !treeNode.Text.Contains("Internal") && !treeNode.Text.Contains("OPC") && !treeNode.Text.Contains("List of"))
						{
							this.BrowseCTRL.Browse(treeNode);
						}
					}
				}
				if (this.TreeN.Count > 0)
				{
					break;
				}
				if (treeNode.Nodes.Count > 0)
				{
					this.Find(treeNode.Nodes, find);
				}
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			this.TreeN.Clear();
			this.Find(this.BrowseCTRL.View.Nodes, this.tag);
			foreach (System.Windows.Forms.TreeNode current in this.TreeN)
			{
				this.BrowseCTRL.View.Focus();
				this.BrowseCTRL.View.SelectedNode = current;
				System.Threading.Thread.Sleep(500);
			}
		}
	}
}
