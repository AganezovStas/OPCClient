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
	public class BrowseTreeCtrl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView BrowseTV;

		private System.Windows.Forms.ContextMenu PopupMenu;

		private System.Windows.Forms.MenuItem ConnectMI;

		private System.Windows.Forms.MenuItem DisconnectMI;

		private System.Windows.Forms.MenuItem RefreshMI;

		private System.Windows.Forms.MenuItem EditFiltersMI;

		private System.Windows.Forms.MenuItem Separator02;

		private System.Windows.Forms.MenuItem SetLoginMI;

		private System.Windows.Forms.MenuItem Separator01;

		private System.Windows.Forms.MenuItem PickMI;

		private System.Windows.Forms.MenuItem PickChildrenMI;

		private System.Windows.Forms.MenuItem menuItem1;

		private System.Windows.Forms.MenuItem ViewComplexTypeMI;

		private System.ComponentModel.Container components = null;

		private IDiscovery m_discovery = null;

		private Specification m_specification;

		private BrowseFilters m_filters = null;

		private System.Windows.Forms.TreeNode m_localServers = null;

		private System.Windows.Forms.TreeNode m_localNetwork = null;

		private System.Windows.Forms.TreeNode m_singleServer = null;

		public event ServerPicked_EventHandler ServerPicked;

		public event ItemPicked_EventHandler ItemPicked;

		public event ElementSelected_EventHandler ElementSelected;

		public System.Windows.Forms.TreeView View
		{
			get
			{
				return this.BrowseTV;
			}
		}

		public Opc.Da.Server SelectedServer
		{
			get
			{
				Opc.Da.Server server = this.FindServer(this.BrowseTV.SelectedNode);
				Opc.Da.Server result;
				if (server != null)
				{
					result = (Opc.Da.Server)server.Duplicate();
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public ItemIdentifier SelectedItem
		{
			get
			{
				System.Windows.Forms.TreeNode selectedNode = this.BrowseTV.SelectedNode;
				ItemIdentifier result;
				if (this.IsBrowseElementNode(selectedNode))
				{
					BrowseElement browseElement = (BrowseElement)selectedNode.Tag;
					if (browseElement.IsItem)
					{
						result = new ItemIdentifier(browseElement.ItemPath, browseElement.ItemName);
						return result;
					}
				}
				if (this.IsItemPropertyNode(selectedNode))
				{
					ItemProperty itemProperty = (ItemProperty)selectedNode.Tag;
					if (itemProperty.ItemName != null && this.ItemPicked != null)
					{
						result = new ItemIdentifier(itemProperty.ItemPath, itemProperty.ItemName);
						return result;
					}
				}
				result = null;
				return result;
			}
		}

		public ConnectData SelectedConnectData
		{
			get
			{
				return this.FindConnectData(this.BrowseTV.SelectedNode);
			}
		}

		public BrowseTreeCtrl()
		{
			this.InitializeComponent();
			this.BrowseTV.ImageList = Resources.Instance.ImageList;
			this.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Clear();
				if (this.components != null)
				{
					this.components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.BrowseTV = new System.Windows.Forms.TreeView();
			this.PopupMenu = new System.Windows.Forms.ContextMenu();
			this.PickMI = new System.Windows.Forms.MenuItem();
			this.PickChildrenMI = new System.Windows.Forms.MenuItem();
			this.Separator01 = new System.Windows.Forms.MenuItem();
			this.EditFiltersMI = new System.Windows.Forms.MenuItem();
			this.RefreshMI = new System.Windows.Forms.MenuItem();
			this.Separator02 = new System.Windows.Forms.MenuItem();
			this.SetLoginMI = new System.Windows.Forms.MenuItem();
			this.ConnectMI = new System.Windows.Forms.MenuItem();
			this.DisconnectMI = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.ViewComplexTypeMI = new System.Windows.Forms.MenuItem();
			base.SuspendLayout();
			this.BrowseTV.ContextMenu = this.PopupMenu;
			this.BrowseTV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BrowseTV.ImageIndex = -1;
			this.BrowseTV.Location = new Point(0, 0);
			this.BrowseTV.Name = "BrowseTV";
			this.BrowseTV.SelectedImageIndex = -1;
			this.BrowseTV.Size = new Size(400, 400);
			this.BrowseTV.TabIndex = 0;
			this.BrowseTV.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BrowseTV_MouseDown);
			this.BrowseTV.DoubleClick += new System.EventHandler(this.PickMI_Click);
			this.BrowseTV.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.BrowseTV_AfterSelect);
			this.BrowseTV.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.BrowseTV_BeforeExpand);
			this.PopupMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
			{
				this.PickMI,
				this.PickChildrenMI,
				this.Separator01,
				this.EditFiltersMI,
				this.RefreshMI,
				this.Separator02,
				this.SetLoginMI,
				this.ConnectMI,
				this.DisconnectMI,
				this.menuItem1,
				this.ViewComplexTypeMI
			});
			this.PickMI.Index = 0;
			this.PickMI.Text = "&Select";
			this.PickMI.Click += new System.EventHandler(this.PickMI_Click);
			this.PickChildrenMI.Index = 1;
			this.PickChildrenMI.Text = "Select Chil&dren";
			this.PickChildrenMI.Click += new System.EventHandler(this.PickChildrenMI_Click);
			this.Separator01.Index = 2;
			this.Separator01.Text = "-";
			this.EditFiltersMI.Index = 3;
			this.EditFiltersMI.Text = "Set &Filters...";
			this.EditFiltersMI.Click += new System.EventHandler(this.EditFiltersMI_Click);
			this.RefreshMI.Index = 4;
			this.RefreshMI.Text = "&Refresh";
			this.Separator02.Index = 5;
			this.Separator02.Text = "-";
			this.SetLoginMI.Index = 6;
			this.SetLoginMI.Text = "Set &Login...";
			this.SetLoginMI.Click += new System.EventHandler(this.SetLoginMI_Click);
			this.ConnectMI.Index = 7;
			this.ConnectMI.Text = "&Connect...";
			this.ConnectMI.Click += new System.EventHandler(this.ConnectMI_Click);
			this.DisconnectMI.Index = 8;
			this.DisconnectMI.Text = "&Disconnect";
			this.DisconnectMI.Click += new System.EventHandler(this.DisconnectMI_Click);
			this.menuItem1.Index = 9;
			this.menuItem1.Text = "-";
			this.ViewComplexTypeMI.Index = 10;
			this.ViewComplexTypeMI.Text = "&View Complex Type...";
			this.ViewComplexTypeMI.Click += new System.EventHandler(this.ViewComplexTypeMI_Click);
			base.Controls.Add(this.BrowseTV);
			base.Name = "BrowseTreeCtrl";
			base.Size = new Size(400, 400);
			base.ResumeLayout(false);
		}

		public void ShowAllServers(IDiscovery discovery, Specification specification, BrowseFilters filters)
		{
			if (discovery == null)
			{
				throw new System.ArgumentNullException("discovery");
			}
			this.Clear();
			this.m_discovery = discovery;
			this.m_specification = specification;
			this.m_filters = ((filters == null) ? new BrowseFilters() : filters);
			this.BrowseTV.ContextMenu = this.PopupMenu;
			this.m_localServers = new System.Windows.Forms.TreeNode("Local Servers");
			this.m_localServers.ImageIndex = Resources.IMAGE_LOCAL_COMPUTER;
			this.m_localServers.SelectedImageIndex = Resources.IMAGE_LOCAL_COMPUTER;
			this.m_localServers.Tag = null;
			this.BrowseServers(this.m_localServers);
			this.BrowseTV.Nodes.Add(this.m_localServers);
			this.m_localNetwork = new System.Windows.Forms.TreeNode("Local Network");
			this.m_localNetwork.ImageIndex = Resources.IMAGE_LOCAL_NETWORK;
			this.m_localNetwork.SelectedImageIndex = Resources.IMAGE_LOCAL_NETWORK;
			this.m_localNetwork.Tag = null;
			this.BrowseNetwork(this.m_localNetwork);
			this.BrowseTV.Nodes.Add(this.m_localNetwork);
		}

		public void ShowSingleServer(Opc.Da.Server server, BrowseFilters filters)
		{
			if (server == null)
			{
				throw new System.ArgumentNullException("server");
			}
			this.Clear();
			this.m_discovery = null;
			this.m_filters = ((filters == null) ? new BrowseFilters() : filters);
			this.BrowseTV.ContextMenu = this.PopupMenu;
			this.m_singleServer = new System.Windows.Forms.TreeNode(server.Name);
			this.m_singleServer.ImageIndex = Resources.IMAGE_LOCAL_SERVER;
			this.m_singleServer.SelectedImageIndex = Resources.IMAGE_LOCAL_SERVER;
			this.m_singleServer.Tag = server.Duplicate();
			this.Connect(this.m_singleServer);
			this.BrowseTV.Nodes.Add(this.m_singleServer);
		}

		private void Connect(System.Windows.Forms.TreeNode node)
		{
			try
			{
				if (this.IsServerNode(node))
				{
					Opc.Da.Server server = (Opc.Da.Server)node.Tag;
					if (!server.IsConnected)
					{
						server.Connect(this.FindConnectData(node));
					}
					this.Browse(node);
				}
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}

		private void Disconnect(System.Windows.Forms.TreeNode node)
		{
			try
			{
				if (this.IsServerNode(node))
				{
					Opc.Da.Server server = (Opc.Da.Server)node.Tag;
					if (server.IsConnected)
					{
						server.Disconnect();
					}
					node.Nodes.Clear();
				}
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}

		private ConnectData FindConnectData(System.Windows.Forms.TreeNode node)
		{
			ConnectData result;
			if (node != null)
			{
				if (node.Tag != null && node.Tag.GetType() == typeof(ConnectData))
				{
					result = (ConnectData)node.Tag;
				}
				else
				{
					result = this.FindConnectData(node.Parent);
				}
			}
			else if (this.BrowseTV.Tag != null && this.BrowseTV.Tag.GetType() == typeof(ConnectData))
			{
				result = (ConnectData)this.BrowseTV.Tag;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void BrowseNetwork(System.Windows.Forms.TreeNode node)
		{
			try
			{
				node.Nodes.Clear();
				string[] array = this.m_discovery.EnumerateHosts();
				if (array != null)
				{
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string text = array2[i];
						System.Windows.Forms.TreeNode treeNode = new System.Windows.Forms.TreeNode(text);
						treeNode.ImageIndex = (treeNode.SelectedImageIndex = Resources.IMAGE_LOCAL_COMPUTER);
						treeNode.Tag = null;
						treeNode.Nodes.Add(new System.Windows.Forms.TreeNode());
						node.Nodes.Add(treeNode);
					}
					node.Expand();
				}
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}

		private void BrowseServers(System.Windows.Forms.TreeNode node)
		{
			try
			{
				node.Nodes.Clear();
				string host = null;
				if (node != this.m_localServers)
				{
					host = node.Text;
				}
				ConnectData connectData = this.FindConnectData(node);
				Opc.Server[] availableServers = this.m_discovery.GetAvailableServers(this.m_specification, host, connectData);
				if (availableServers != null)
				{
					Opc.Server[] array = availableServers;
					for (int i = 0; i < array.Length; i++)
					{
						Opc.Da.Server server = (Opc.Da.Server)array[i];
						System.Windows.Forms.TreeNode treeNode = new System.Windows.Forms.TreeNode(server.Name);
						treeNode.ImageIndex = (treeNode.SelectedImageIndex = Resources.IMAGE_LOCAL_SERVER);
						treeNode.Tag = server;
						node.Nodes.Add(treeNode);
					}
					node.Expand();
				}
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}

		public void Browse(System.Windows.Forms.TreeNode node)
		{
			try
			{
				Opc.Da.Server server = this.FindServer(node);
				ItemIdentifier itemID = null;
				if (node.Tag != null && node.Tag.GetType() == typeof(BrowseElement))
				{
					BrowseElement browseElement = (BrowseElement)node.Tag;
					itemID = new ItemIdentifier(browseElement.ItemPath, browseElement.ItemName);
				}
				node.Nodes.Clear();
				BrowsePosition browsePosition = null;
				BrowseElement[] array = server.Browse(itemID, this.m_filters, out browsePosition);
				if (array != null)
				{
					BrowseElement[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						BrowseElement browseElement2 = array2[i];
						if (!browseElement2.ItemName.Contains("alrosa_w") && !browseElement2.ItemName.Contains("List of"))
						{
							if (!browseElement2.IsItem)
							{
								this.AddBrowseElement(node, browseElement2);
							}
						}
					}
					node.Expand();
				}
				while (browsePosition != null)
				{
					System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("More items meeting search criteria exist. Continue browse?", "Browse Items", System.Windows.Forms.MessageBoxButtons.YesNo);
					if (dialogResult == System.Windows.Forms.DialogResult.No)
					{
						break;
					}
					array = server.BrowseNext(ref browsePosition);
					if (array != null)
					{
						BrowseElement[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							BrowseElement browseElement2 = array2[i];
							this.AddBrowseElement(node, browseElement2);
						}
						node.Expand();
					}
				}
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}

		private void GetProperties(System.Windows.Forms.TreeNode node)
		{
			try
			{
				Opc.Da.Server server = this.FindServer(node);
				BrowseElement browseElement = null;
				if (node.Tag != null && node.Tag.GetType() == typeof(BrowseElement))
				{
					browseElement = (BrowseElement)node.Tag;
				}
				if (browseElement.IsItem)
				{
					node.Nodes.Clear();
					ItemIdentifier itemIdentifier = new ItemIdentifier(browseElement.ItemPath, browseElement.ItemName);
					ItemPropertyCollection[] properties = server.GetProperties(new ItemIdentifier[]
					{
						itemIdentifier
					}, this.m_filters.PropertyIDs, this.m_filters.ReturnPropertyValues);
					if (properties != null)
					{
						ItemPropertyCollection[] array = properties;
						for (int i = 0; i < array.Length; i++)
						{
							ItemPropertyCollection itemPropertyCollection = array[i];
							foreach (ItemProperty property in itemPropertyCollection)
							{
								this.AddItemProperty(node, property);
							}
							browseElement.Properties = (ItemProperty[])itemPropertyCollection.ToArray(typeof(ItemProperty));
						}
					}
					node.Expand();
					if (this.ElementSelected != null)
					{
						this.ElementSelected(browseElement);
					}
				}
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}

		private bool IsHostNode(System.Windows.Forms.TreeNode node)
		{
			return node != null && (node == this.m_localServers || node == this.m_singleServer || node.Parent == this.m_localNetwork);
		}

		private bool IsServerNode(System.Windows.Forms.TreeNode node)
		{
			return node != null && node.Tag != null && typeof(Opc.Da.Server).IsInstanceOfType(node.Tag);
		}

		public bool IsBrowseElementNode(System.Windows.Forms.TreeNode node)
		{
			return node != null && node.Tag != null && node.Tag.GetType() == typeof(BrowseElement);
		}

		private bool IsItemPropertyNode(System.Windows.Forms.TreeNode node)
		{
			return node != null && node.Tag != null && node.Tag.GetType() == typeof(ItemProperty);
		}

		private Opc.Da.Server FindServer(System.Windows.Forms.TreeNode node)
		{
			Opc.Da.Server result;
			if (node != null)
			{
				if (this.IsServerNode(node))
				{
					result = (Opc.Da.Server)node.Tag;
				}
				else
				{
					result = this.FindServer(node.Parent);
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void PickNode(System.Windows.Forms.TreeNode node)
		{
			if (this.IsServerNode(node))
			{
				if (this.ServerPicked != null)
				{
					this.ServerPicked((Opc.Da.Server)node.Tag);
				}
			}
			else if (this.IsBrowseElementNode(node))
			{
				BrowseElement browseElement = (BrowseElement)node.Tag;
				if (browseElement.IsItem && this.ItemPicked != null)
				{
					this.ItemPicked(new ItemIdentifier(browseElement.ItemPath, browseElement.ItemName));
				}
			}
			else if (this.IsItemPropertyNode(node))
			{
				ItemProperty itemProperty = (ItemProperty)node.Tag;
				if (itemProperty.ItemName != null && this.ItemPicked != null)
				{
					this.ItemPicked(new ItemIdentifier(itemProperty.ItemPath, itemProperty.ItemName));
				}
			}
		}

		private void ViewComplexType(System.Windows.Forms.TreeNode node)
		{
			if (this.IsBrowseElementNode(node))
			{
				try
				{
					ComplexItem complexItem = ComplexTypeCache.GetComplexItem((BrowseElement)node.Tag);
					if (complexItem != null)
					{
						new EditComplexValueDlg().ShowDialog(complexItem, null, true, true);
					}
				}
				catch (System.Exception ex)
				{
					System.Windows.Forms.MessageBox.Show(ex.Message);
				}
			}
		}

		private void AddBrowseElement(System.Windows.Forms.TreeNode parent, BrowseElement element)
		{
			System.Windows.Forms.TreeNode treeNode = new System.Windows.Forms.TreeNode(element.Name);
			if (element.IsItem)
			{
				treeNode.ImageIndex = (treeNode.SelectedImageIndex = Resources.IMAGE_GREEN_SCROLL);
			}
			else
			{
				treeNode.ImageIndex = (treeNode.SelectedImageIndex = Resources.IMAGE_CLOSED_YELLOW_FOLDER);
			}
			treeNode.Tag = element;
			if (element.HasChildren)
			{
				treeNode.Nodes.Add(new System.Windows.Forms.TreeNode());
			}
			if (element.Properties != null)
			{
				ItemProperty[] properties = element.Properties;
				for (int i = 0; i < properties.Length; i++)
				{
					ItemProperty property = properties[i];
					this.AddItemProperty(treeNode, property);
				}
			}
			parent.Nodes.Add(treeNode);
		}

		private void AddItemProperty(System.Windows.Forms.TreeNode parent, ItemProperty property)
		{
			if (property.ResultID.Succeeded())
			{
				System.Windows.Forms.TreeNode treeNode = new System.Windows.Forms.TreeNode(property.Description);
				if (property.ItemName != null && property.ItemName != "")
				{
					treeNode.ImageIndex = (treeNode.SelectedImageIndex = Resources.IMAGE_GREEN_SCROLL);
				}
				else
				{
					treeNode.ImageIndex = (treeNode.SelectedImageIndex = Resources.IMAGE_EXPLODING_BOX);
				}
				treeNode.Tag = property;
				if (property.Value != null)
				{
					System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode(Opc.Convert.ToString(property.Value));
					treeNode2.ImageIndex = (treeNode2.SelectedImageIndex = Resources.IMAGE_LIST_BOX);
					treeNode2.Tag = property.Value;
					treeNode.Nodes.Add(treeNode2);
					if (property.Value.GetType().IsArray)
					{
						foreach (object current in ((System.Array)property.Value))
						{
							System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode(Opc.Convert.ToString(current));
							treeNode3.ImageIndex = (treeNode3.SelectedImageIndex = Resources.IMAGE_LIST_BOX);
							treeNode3.Tag = current;
							treeNode2.Nodes.Add(treeNode3);
						}
					}
				}
				parent.Nodes.Add(treeNode);
			}
		}

		public void Clear()
		{
			foreach (System.Windows.Forms.TreeNode parent in this.BrowseTV.Nodes)
			{
				this.Clear(parent);
			}
			this.BrowseTV.Nodes.Clear();
			if (this.m_discovery != null)
			{
				this.m_discovery.Dispose();
			}
			this.m_localServers = null;
			this.m_localNetwork = null;
			this.m_singleServer = null;
		}

		private void Clear(System.Windows.Forms.TreeNode parent)
		{
			foreach (System.Windows.Forms.TreeNode parent2 in parent.Nodes)
			{
				this.Clear(parent2);
			}
			if (this.IsServerNode(parent))
			{
				Opc.Da.Server server = (Opc.Da.Server)parent.Tag;
				if (server.IsConnected)
				{
					server.Disconnect();
				}
			}
		}

		private void BrowseTV_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			System.Windows.Forms.TreeNode node = e.Node;
			if (node != null && node.Parent == this.m_localNetwork)
			{
				if (node.Nodes.Count == 1 && node.Nodes[0].Text == "")
				{
					this.BrowseServers(node);
				}
			}
			if (this.IsServerNode(node))
			{
				if (node.Nodes.Count == 1 && node.Nodes[0].Text == "")
				{
					this.Connect(node);
				}
			}
			else if (this.IsBrowseElementNode(node))
			{
				if (node.Nodes.Count >= 1 && node.Nodes[0].Text == "")
				{
					this.Browse(node);
				}
			}
		}

		private void BrowseTV_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				System.Windows.Forms.TreeNode nodeAt = this.BrowseTV.GetNodeAt(e.X, e.Y);
				if (nodeAt != null)
				{
					this.PickMI.Enabled = false;
					this.EditFiltersMI.Enabled = false;
					this.RefreshMI.Enabled = false;
					this.SetLoginMI.Enabled = false;
					this.ConnectMI.Enabled = false;
					this.DisconnectMI.Enabled = false;
					this.ViewComplexTypeMI.Enabled = false;
					if (nodeAt == this.m_localNetwork || this.IsHostNode(nodeAt))
					{
						this.RefreshMI.Enabled = true;
						this.SetLoginMI.Enabled = true;
					}
					else if (this.IsServerNode(nodeAt))
					{
						this.PickMI.Enabled = true;
						this.ConnectMI.Enabled = !((Opc.Da.Server)nodeAt.Tag).IsConnected;
						this.DisconnectMI.Enabled = ((Opc.Da.Server)nodeAt.Tag).IsConnected;
						this.EditFiltersMI.Enabled = true;
						this.RefreshMI.Enabled = true;
					}
					else if (this.IsBrowseElementNode(nodeAt))
					{
						this.PickMI.Enabled = true;
						this.EditFiltersMI.Enabled = true;
						this.RefreshMI.Enabled = true;
						this.ViewComplexTypeMI.Enabled = (ComplexTypeCache.GetComplexItem((BrowseElement)nodeAt.Tag) != null);
					}
				}
			}
		}

		private void OnBrowseFiltersChanged(BrowseFilters filters)
		{
			this.m_filters = filters;
			if (this.IsBrowseElementNode(this.BrowseTV.SelectedNode))
			{
				BrowseElement browseElement = (BrowseElement)this.BrowseTV.SelectedNode.Tag;
				if (!browseElement.HasChildren)
				{
					this.GetProperties(this.BrowseTV.SelectedNode);
					return;
				}
			}
			this.Browse(this.BrowseTV.SelectedNode);
		}

		private void ConnectMI_Click(object sender, System.EventArgs e)
		{
			this.Connect(this.BrowseTV.SelectedNode);
		}

		private void DisconnectMI_Click(object sender, System.EventArgs e)
		{
			this.Disconnect(this.BrowseTV.SelectedNode);
		}

		private void EditFiltersMI_Click(object sender, System.EventArgs e)
		{
			new BrowseFiltersDlg().Show(System.Windows.Forms.Form.ActiveForm, this.m_filters, new BrowseFiltersChangedCallback(this.OnBrowseFiltersChanged));
		}

		private void RefreshMI_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.TreeNode selectedNode = this.BrowseTV.SelectedNode;
			if (selectedNode == this.m_localNetwork)
			{
				this.BrowseNetwork(selectedNode);
			}
			else if (this.IsHostNode(selectedNode))
			{
				this.BrowseServers(selectedNode);
			}
			else
			{
				this.Browse(selectedNode);
			}
		}

		private void SetLoginMI_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.TreeNode selectedNode = this.BrowseTV.SelectedNode;
			if (selectedNode == this.m_localNetwork | this.IsHostNode(selectedNode))
			{
				ConnectData connectData = (ConnectData)selectedNode.Tag;
				if (connectData == null)
				{
                    connectData = (ConnectData)(selectedNode.Tag = new ConnectData(null));
				}
				connectData.Credentials = new NetworkCredentialsDlg().ShowDialog(connectData.Credentials);
			}
		}

		private void PickMI_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.TreeNode selectedNode = this.BrowseTV.SelectedNode;
			if (selectedNode != null)
			{
				this.PickNode(selectedNode);
			}
		}

		private void PickChildrenMI_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.TreeNode selectedNode = this.BrowseTV.SelectedNode;
			if (selectedNode != null)
			{
				foreach (System.Windows.Forms.TreeNode node in selectedNode.Nodes)
				{
					this.PickNode(node);
				}
			}
		}

		private void BrowseTV_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			System.Windows.Forms.TreeNode selectedNode = this.BrowseTV.SelectedNode;
			if (this.ElementSelected != null)
			{
				if (this.IsBrowseElementNode(selectedNode))
				{
					this.ElementSelected((BrowseElement)selectedNode.Tag);
				}
				else
				{
					this.ElementSelected(null);
				}
			}
		}

		private void ViewComplexTypeMI_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.TreeNode selectedNode = this.BrowseTV.SelectedNode;
			if (selectedNode != null)
			{
				this.ViewComplexType(selectedNode);
			}
		}
	}
}
