using Opc.Da;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OPCClient
{
	public class PropertyFiltersCtrl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.CheckedListBox PropertyNamesLB;

		private System.Windows.Forms.CheckBox ReturnAllPropertiesCB;

		private System.Windows.Forms.CheckBox ReturnPropertyValuesCB;

		private System.Windows.Forms.Label ReturnAllPropertiesLB;

		private System.Windows.Forms.Label ReturnPropertyValuesLB;

		private System.Windows.Forms.Panel TopPN;

		private System.ComponentModel.IContainer components = null;

		public bool ReturnAllProperties
		{
			get
			{
				return this.ReturnAllPropertiesCB.Checked;
			}
			set
			{
				this.ReturnAllPropertiesCB.Checked = value;
			}
		}

		public bool ReturnPropertyValues
		{
			get
			{
				return this.ReturnPropertyValuesCB.Checked;
			}
			set
			{
				this.ReturnPropertyValuesCB.Checked = value;
			}
		}

		public PropertyID[] PropertyIDs
		{
			get
			{
				System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
				foreach (PropertyDescription propertyDescription in this.PropertyNamesLB.CheckedItems)
				{
					arrayList.Add(propertyDescription.ID);
				}
				return (PropertyID[])arrayList.ToArray(typeof(PropertyID));
			}
			set
			{
				for (int i = 0; i < this.PropertyNamesLB.Items.Count; i++)
				{
					this.PropertyNamesLB.SetItemChecked(i, false);
					if (value != null)
					{
						PropertyDescription propertyDescription = (PropertyDescription)this.PropertyNamesLB.Items[i];
						for (int j = 0; j < value.Length; j++)
						{
							PropertyID b = value[j];
							if (propertyDescription.ID == b)
							{
								this.PropertyNamesLB.SetItemChecked(i, true);
								break;
							}
						}
					}
				}
			}
		}

		public PropertyFiltersCtrl()
		{
			this.InitializeComponent();
			PropertyDescription[] array = PropertyDescription.Enumerate();
			PropertyDescription[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				PropertyDescription item = array2[i];
				this.PropertyNamesLB.Items.Add(item);
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
			this.ReturnAllPropertiesCB = new System.Windows.Forms.CheckBox();
			this.ReturnPropertyValuesCB = new System.Windows.Forms.CheckBox();
			this.PropertyNamesLB = new System.Windows.Forms.CheckedListBox();
			this.ReturnAllPropertiesLB = new System.Windows.Forms.Label();
			this.ReturnPropertyValuesLB = new System.Windows.Forms.Label();
			this.TopPN = new System.Windows.Forms.Panel();
			this.TopPN.SuspendLayout();
			base.SuspendLayout();
			this.ReturnAllPropertiesCB.Location = new Point(112, 0);
			this.ReturnAllPropertiesCB.Name = "ReturnAllPropertiesCB";
			this.ReturnAllPropertiesCB.Size = new Size(16, 24);
			this.ReturnAllPropertiesCB.TabIndex = 1;
			this.ReturnAllPropertiesCB.CheckedChanged += new System.EventHandler(this.ReturnAllPropertiesCB_CheckedChanged);
			this.ReturnPropertyValuesCB.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.ReturnPropertyValuesCB.Location = new Point(352, 0);
			this.ReturnPropertyValuesCB.Name = "ReturnPropertyValuesCB";
			this.ReturnPropertyValuesCB.Size = new Size(16, 24);
			this.ReturnPropertyValuesCB.TabIndex = 3;
			this.PropertyNamesLB.CheckOnClick = true;
			this.PropertyNamesLB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PropertyNamesLB.Location = new Point(0, 24);
			this.PropertyNamesLB.Name = "PropertyNamesLB";
			this.PropertyNamesLB.Size = new Size(368, 154);
			this.PropertyNamesLB.TabIndex = 0;
			this.ReturnAllPropertiesLB.Name = "ReturnAllPropertiesLB";
			this.ReturnAllPropertiesLB.Size = new Size(112, 23);
			this.ReturnAllPropertiesLB.TabIndex = 0;
			this.ReturnAllPropertiesLB.Text = "Return All Properties";
			this.ReturnAllPropertiesLB.TextAlign = ContentAlignment.MiddleLeft;
			this.ReturnPropertyValuesLB.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.ReturnPropertyValuesLB.Location = new Point(224, 0);
			this.ReturnPropertyValuesLB.Name = "ReturnPropertyValuesLB";
			this.ReturnPropertyValuesLB.Size = new Size(128, 23);
			this.ReturnPropertyValuesLB.TabIndex = 2;
			this.ReturnPropertyValuesLB.Text = "Return Property Values";
			this.ReturnPropertyValuesLB.TextAlign = ContentAlignment.MiddleLeft;
			this.TopPN.Controls.AddRange(new System.Windows.Forms.Control[]
			{
				this.ReturnAllPropertiesLB,
				this.ReturnAllPropertiesCB,
				this.ReturnPropertyValuesLB,
				this.ReturnPropertyValuesCB
			});
			this.TopPN.Dock = System.Windows.Forms.DockStyle.Top;
			this.TopPN.Name = "TopPN";
			this.TopPN.Size = new Size(368, 24);
			this.TopPN.TabIndex = 29;
			base.Controls.AddRange(new System.Windows.Forms.Control[]
			{
				this.PropertyNamesLB,
				this.TopPN
			});
			base.Name = "PropertyFiltersCtrl";
			base.Size = new Size(368, 184);
			this.TopPN.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void ReturnAllPropertiesCB_CheckedChanged(object sender, System.EventArgs e)
		{
			this.PropertyNamesLB.Enabled = !this.ReturnAllPropertiesCB.Checked;
		}
	}
}
