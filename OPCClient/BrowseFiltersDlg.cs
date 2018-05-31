using Opc.Da;
using Opc.SampleClient;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OPCClient
{
	public class BrowseFiltersDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button CancelBTN;

		private System.Windows.Forms.Button OkBTN;

		private System.Windows.Forms.Panel ButtonsPN;

		private PropertyFiltersCtrl PropertyFiltersCTRL;

		private System.Windows.Forms.NumericUpDown MaxElementsReturnedCTRL;

		private System.Windows.Forms.TextBox ElementNameFilterTB;

		private System.Windows.Forms.Label ReturnPropertiesLB;

		private System.Windows.Forms.Label ElementNameFilterLB;

		private System.Windows.Forms.CheckBox ReturnPropertiesCB;

		private System.Windows.Forms.Label VendorFilterLB;

		private System.Windows.Forms.TextBox VendorFilterTB;

		private System.Windows.Forms.Label BrowseFilterLB;

		private EnumCtrl BrowseFilterCTRL;

		private System.Windows.Forms.Label MaxElementsReturnedLB;

		private System.Windows.Forms.Button ApplyBTN;

		private System.Windows.Forms.Panel TopPN;

		private System.ComponentModel.IContainer components = null;

		private BrowseFiltersChangedCallback m_callback = null;

		public BrowseFiltersDlg()
		{
			this.InitializeComponent();
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
			this.OkBTN = new System.Windows.Forms.Button();
			this.CancelBTN = new System.Windows.Forms.Button();
			this.ButtonsPN = new System.Windows.Forms.Panel();
			this.ApplyBTN = new System.Windows.Forms.Button();
			this.PropertyFiltersCTRL = new PropertyFiltersCtrl();
			this.MaxElementsReturnedCTRL = new System.Windows.Forms.NumericUpDown();
			this.ElementNameFilterTB = new System.Windows.Forms.TextBox();
			this.ReturnPropertiesLB = new System.Windows.Forms.Label();
			this.ElementNameFilterLB = new System.Windows.Forms.Label();
			this.ReturnPropertiesCB = new System.Windows.Forms.CheckBox();
			this.VendorFilterLB = new System.Windows.Forms.Label();
			this.VendorFilterTB = new System.Windows.Forms.TextBox();
			this.BrowseFilterLB = new System.Windows.Forms.Label();
			this.BrowseFilterCTRL = new EnumCtrl();
			this.MaxElementsReturnedLB = new System.Windows.Forms.Label();
			this.TopPN = new System.Windows.Forms.Panel();
			this.ButtonsPN.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.MaxElementsReturnedCTRL).BeginInit();
			this.TopPN.SuspendLayout();
			base.SuspendLayout();
			this.OkBTN.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.OkBTN.Location = new Point(4, 8);
			this.OkBTN.Name = "OkBTN";
			this.OkBTN.TabIndex = 1;
			this.OkBTN.Text = "OK";
			this.OkBTN.Click += new System.EventHandler(this.OkBTN_Click);
			this.CancelBTN.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBTN.Location = new Point(280, 8);
			this.CancelBTN.Name = "CancelBTN";
			this.CancelBTN.TabIndex = 0;
			this.CancelBTN.Text = "Cancel";
			this.CancelBTN.Click += new System.EventHandler(this.CancelBTN_Click);
			this.ButtonsPN.Controls.AddRange(new System.Windows.Forms.Control[]
			{
				this.ApplyBTN,
				this.CancelBTN,
				this.OkBTN
			});
			this.ButtonsPN.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ButtonsPN.Location = new Point(4, 274);
			this.ButtonsPN.Name = "ButtonsPN";
			this.ButtonsPN.Size = new Size(360, 36);
			this.ButtonsPN.TabIndex = 0;
			this.ApplyBTN.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom);
			this.ApplyBTN.Location = new Point(143, 7);
			this.ApplyBTN.Name = "ApplyBTN";
			this.ApplyBTN.TabIndex = 2;
			this.ApplyBTN.Text = "Apply";
			this.ApplyBTN.Click += new System.EventHandler(this.ApplyBTN_Click);
			this.PropertyFiltersCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PropertyFiltersCTRL.Location = new Point(4, 124);
			this.PropertyFiltersCTRL.Name = "PropertyFiltersCTRL";
			this.PropertyFiltersCTRL.PropertyIDs = new PropertyID[0];
			this.PropertyFiltersCTRL.ReturnAllProperties = true;
			this.PropertyFiltersCTRL.ReturnPropertyValues = true;
			this.PropertyFiltersCTRL.Size = new Size(360, 150);
			this.PropertyFiltersCTRL.TabIndex = 0;
			this.MaxElementsReturnedCTRL.Location = new Point(112, 24);
			//System.Windows.Forms.NumericUpDown arg_36B_0 = this.MaxElementsReturnedCTRL;
			int[] array = new int[4];
			array[0] = 10000;
//			arg_36B_0.Maximum = new decimal(array);
            this.MaxElementsReturnedCTRL.Maximum = new decimal(array);
			this.MaxElementsReturnedCTRL.Name = "MaxElementsReturnedCTRL";
			this.MaxElementsReturnedCTRL.Size = new Size(72, 20);
			this.MaxElementsReturnedCTRL.TabIndex = 3;
			this.ElementNameFilterTB.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			this.ElementNameFilterTB.Location = new Point(112, 48);
			this.ElementNameFilterTB.Name = "ElementNameFilterTB";
			this.ElementNameFilterTB.Size = new Size(240, 20);
			this.ElementNameFilterTB.TabIndex = 5;
			this.ElementNameFilterTB.Text = "";
			this.ReturnPropertiesLB.Location = new Point(0, 96);
			this.ReturnPropertiesLB.Name = "ReturnPropertiesLB";
			this.ReturnPropertiesLB.Size = new Size(112, 23);
			this.ReturnPropertiesLB.TabIndex = 8;
			this.ReturnPropertiesLB.Text = "Return Properties";
			this.ReturnPropertiesLB.TextAlign = ContentAlignment.MiddleLeft;
			this.ElementNameFilterLB.Location = new Point(0, 48);
			this.ElementNameFilterLB.Name = "ElementNameFilterLB";
			this.ElementNameFilterLB.Size = new Size(112, 23);
			this.ElementNameFilterLB.TabIndex = 4;
			this.ElementNameFilterLB.Text = "Element Name Filter";
			this.ElementNameFilterLB.TextAlign = ContentAlignment.MiddleLeft;
			this.ReturnPropertiesCB.Checked = true;
			this.ReturnPropertiesCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ReturnPropertiesCB.Location = new Point(112, 96);
			this.ReturnPropertiesCB.Name = "ReturnPropertiesCB";
			this.ReturnPropertiesCB.Size = new Size(16, 24);
			this.ReturnPropertiesCB.TabIndex = 9;
			this.ReturnPropertiesCB.CheckedChanged += new System.EventHandler(this.ReturnPropertiesCB_CheckedChanged);
			this.VendorFilterLB.Location = new Point(0, 72);
			this.VendorFilterLB.Name = "VendorFilterLB";
			this.VendorFilterLB.Size = new Size(112, 23);
			this.VendorFilterLB.TabIndex = 6;
			this.VendorFilterLB.Text = "Vendor Filter";
			this.VendorFilterLB.TextAlign = ContentAlignment.MiddleLeft;
			this.VendorFilterTB.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			this.VendorFilterTB.Location = new Point(112, 72);
			this.VendorFilterTB.Name = "VendorFilterTB";
			this.VendorFilterTB.Size = new Size(240, 20);
			this.VendorFilterTB.TabIndex = 7;
			this.VendorFilterTB.Text = "";
			this.BrowseFilterLB.Name = "BrowseFilterLB";
			this.BrowseFilterLB.Size = new Size(112, 23);
			this.BrowseFilterLB.TabIndex = 0;
			this.BrowseFilterLB.Text = "Browse Filter";
			this.BrowseFilterLB.TextAlign = ContentAlignment.MiddleLeft;
			this.BrowseFilterCTRL.Location = new Point(112, 0);
			this.BrowseFilterCTRL.Name = "BrowseFilterCTRL";
			this.BrowseFilterCTRL.Size = new Size(152, 24);
			this.BrowseFilterCTRL.TabIndex = 1;
			this.MaxElementsReturnedLB.Location = new Point(0, 24);
			this.MaxElementsReturnedLB.Name = "MaxElementsReturnedLB";
			this.MaxElementsReturnedLB.Size = new Size(112, 23);
			this.MaxElementsReturnedLB.TabIndex = 2;
			this.MaxElementsReturnedLB.Text = "Maximum Returned";
			this.MaxElementsReturnedLB.TextAlign = ContentAlignment.MiddleLeft;
			this.TopPN.Controls.AddRange(new System.Windows.Forms.Control[]
			{
				this.ElementNameFilterTB,
				this.MaxElementsReturnedCTRL,
				this.BrowseFilterCTRL,
				this.BrowseFilterLB,
				this.ReturnPropertiesLB,
				this.VendorFilterLB,
				this.MaxElementsReturnedLB,
				this.ReturnPropertiesCB,
				this.ElementNameFilterLB,
				this.VendorFilterTB
			});
			this.TopPN.Dock = System.Windows.Forms.DockStyle.Top;
			this.TopPN.Location = new Point(4, 4);
			this.TopPN.Name = "TopPN";
			this.TopPN.Size = new Size(360, 120);
			this.TopPN.TabIndex = 32;
			this.AutoScaleBaseSize = new Size(5, 13);
			base.ClientSize = new Size(368, 310);
			base.Controls.AddRange(new System.Windows.Forms.Control[]
			{
				this.PropertyFiltersCTRL,
				this.ButtonsPN,
				this.TopPN
			});
			base.DockPadding.Left = 4;
			base.DockPadding.Right = 4;
			base.DockPadding.Top = 4;
			base.Name = "BrowseFiltersDlg";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Browse Filters";
			this.ButtonsPN.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.MaxElementsReturnedCTRL).EndInit();
			this.TopPN.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		public void Show(System.Windows.Forms.Form owner, BrowseFilters filters, BrowseFiltersChangedCallback callback)
		{
			if (callback == null)
			{
				throw new System.ArgumentNullException("callback");
			}
			base.Owner = owner;
			this.m_callback = callback;
			this.BrowseFilterCTRL.Value = filters.BrowseFilter;
			this.MaxElementsReturnedCTRL.Value = filters.MaxElementsReturned;
			this.ElementNameFilterTB.Text = filters.ElementNameFilter;
			this.VendorFilterTB.Text = filters.VendorFilter;
			this.ReturnPropertiesCB.Checked = (filters.PropertyIDs != null || filters.ReturnAllProperties);
			this.PropertyFiltersCTRL.ReturnAllProperties = filters.ReturnAllProperties;
			this.PropertyFiltersCTRL.ReturnPropertyValues = filters.ReturnPropertyValues;
			this.PropertyFiltersCTRL.PropertyIDs = filters.PropertyIDs;
			base.Show();
		}

		private void ApplyChanges()
		{
			BrowseFilters browseFilters = new BrowseFilters();
			browseFilters.BrowseFilter = (browseFilter)this.BrowseFilterCTRL.Value;
			browseFilters.MaxElementsReturned = (int)this.MaxElementsReturnedCTRL.Value;
			browseFilters.ElementNameFilter = this.ElementNameFilterTB.Text;
			browseFilters.VendorFilter = this.VendorFilterTB.Text;
			if (!this.ReturnPropertiesCB.Checked)
			{
				browseFilters.ReturnAllProperties = false;
				browseFilters.ReturnPropertyValues = false;
				browseFilters.PropertyIDs = null;
			}
			else
			{
				browseFilters.ReturnAllProperties = this.PropertyFiltersCTRL.ReturnAllProperties;
				browseFilters.ReturnPropertyValues = this.PropertyFiltersCTRL.ReturnPropertyValues;
				if (!browseFilters.ReturnAllProperties)
				{
					browseFilters.PropertyIDs = this.PropertyFiltersCTRL.PropertyIDs;
				}
				else
				{
					browseFilters.PropertyIDs = null;
				}
			}
			if (this.m_callback != null)
			{
				this.m_callback(browseFilters);
			}
		}

		private void OkBTN_Click(object sender, System.EventArgs e)
		{
			this.ApplyChanges();
			base.Close();
		}

		private void ApplyBTN_Click(object sender, System.EventArgs e)
		{
			this.ApplyChanges();
		}

		private void CancelBTN_Click(object sender, System.EventArgs e)
		{
			base.Close();
		}

		private void ReturnPropertiesCB_CheckedChanged(object sender, System.EventArgs e)
		{
			this.PropertyFiltersCTRL.Enabled = this.ReturnPropertiesCB.Checked;
			if (this.PropertyFiltersCTRL.Enabled)
			{
				if (this.PropertyFiltersCTRL.PropertyIDs.Length == 0)
				{
					this.PropertyFiltersCTRL.ReturnAllProperties = true;
				}
			}
		}
	}
}
