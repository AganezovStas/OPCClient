using OPCClient.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OPCClient
{
	public class ChoiceOPCServersForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.DataGridView dataGridView1;

		private System.Windows.Forms.GroupBox groupBox1;

		private System.Windows.Forms.Button btn_Add;

		private System.Windows.Forms.Button btn_Remove;

		private System.Windows.Forms.Button btn_Edit;

		private System.Windows.Forms.Label lbl_Default;

		private System.Windows.Forms.Label label1;

		private System.Windows.Forms.Button button3;

		private System.Windows.Forms.DataGridViewTextBoxColumn IpAddressServer;

		private System.Windows.Forms.DataGridViewTextBoxColumn NameServer;

		public ChoiceOPCServersForm()
		{
			this.InitializeComponent();
		}

		private void ChoiceOPCServersForm_Load(object sender, System.EventArgs e)
		{
			this.Bind();
		}

		private void Bind()
		{
			try
			{
				this.dataGridView1.Rows.Clear();
				foreach (SettingsOPCServer current in Program.sL.tmp_listObject.OPCServers)
				{
					this.dataGridView1.Rows.Add(new object[]
					{
						current.IpAddressServer,
						current.NameServer
					});
				}
				if (Program.sL.tmp_listObject != null && Program.sL.tmp_listObject.DefaultOPCServer != null)
				{
					this.lbl_Default.Text = Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer + "\\" + Program.sL.tmp_listObject.DefaultOPCServer.NameServer;
				}
				this.dataGridView1.Refresh();
			}
			catch (System.Exception e)
			{
				Program.showErrorMessage(e, this);
			}
		}

		private void btn_Add_Click(object sender, System.EventArgs e)
		{
			EditOPCServerForm editOPCServerForm = new EditOPCServerForm();
			editOPCServerForm.tb_NameServer.Text = "OPCServer.WinCC";
			if (editOPCServerForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				SettingsOPCServer settingsOPCServer = new SettingsOPCServer();
				settingsOPCServer.IpAddressServer = editOPCServerForm.tb_IPAddrServer.Text;
				settingsOPCServer.NameServer = editOPCServerForm.tb_NameServer.Text;
				Program.sL.tmp_listObject.OPCServers.Add(settingsOPCServer);
				if (Program.sL.tmp_listObject.DefaultOPCServer == null)
				{
					Program.sL.tmp_listObject.DefaultOPCServer = new SettingsDefaultOPCServer();
					Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer = settingsOPCServer.IpAddressServer;
					Program.sL.tmp_listObject.DefaultOPCServer.NameServer = settingsOPCServer.NameServer;
				}
				Program.sL.Save();
				this.Bind();
			}
		}

		private void btn_Edit_Click(object sender, System.EventArgs e)
		{
			if (this.dataGridView1.CurrentRow != null)
			{
				try
				{
					EditOPCServerForm editOPCServerForm = new EditOPCServerForm();
					SettingsOPCServer settingsOPCServer = Program.sL.tmp_listObject.OPCServers[this.dataGridView1.CurrentRow.Index];
					editOPCServerForm.tb_IPAddrServer.Text = settingsOPCServer.IpAddressServer;
					editOPCServerForm.tb_NameServer.Text = settingsOPCServer.NameServer;
					if (editOPCServerForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						settingsOPCServer.IpAddressServer = editOPCServerForm.tb_IPAddrServer.Text;
						settingsOPCServer.NameServer = editOPCServerForm.tb_NameServer.Text;
						Program.sL.Save();
						this.Bind();
					}
				}
				catch (System.Exception e2)
				{
					Program.showErrorMessage(e2, this);
				}
			}
		}

		private void btn_Remove_Click(object sender, System.EventArgs e)
		{
			if (this.dataGridView1.CurrentRow != null)
			{
				try
				{
					if (System.Windows.Forms.MessageBox.Show(string.Format("Удалить ссылку на OPC-сервер '{0}'?", this.dataGridView1.CurrentRow.Cells["IpAddressServer"].Value + "\\" + this.dataGridView1.CurrentRow.Cells["NameServer"].Value), "Внимание!", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation) != System.Windows.Forms.DialogResult.No)
					{
						Program.sL.tmp_listObject.OPCServers.RemoveAt(this.dataGridView1.CurrentRow.Index);
						Program.sL.Save();
						this.Bind();
					}
				}
				catch (System.Exception e2)
				{
					Program.showErrorMessage(e2, this);
				}
			}
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			if (this.dataGridView1.CurrentRow != null)
			{
				try
				{
					SettingsOPCServer settingsOPCServer = Program.sL.tmp_listObject.OPCServers[this.dataGridView1.CurrentRow.Index];
					Program.sL.tmp_listObject.DefaultOPCServer = new SettingsDefaultOPCServer();
					Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer = settingsOPCServer.IpAddressServer;
					Program.sL.tmp_listObject.DefaultOPCServer.NameServer = settingsOPCServer.NameServer;
					Program.sL.Save();
					this.Bind();
				}
				catch (System.Exception e2)
				{
					Program.showErrorMessage(e2, this);
				}
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChoiceOPCServersForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.IpAddressServer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameServer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.lbl_Default = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Remove = new System.Windows.Forms.Button();
            this.btn_Edit = new System.Windows.Forms.Button();
            this.btn_Add = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IpAddressServer,
            this.NameServer});
            this.dataGridView1.Location = new System.Drawing.Point(6, 42);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(515, 264);
            this.dataGridView1.TabIndex = 4;
            // 
            // IpAddressServer
            // 
            this.IpAddressServer.HeaderText = "IP-адрес сервера";
            this.IpAddressServer.Name = "IpAddressServer";
            this.IpAddressServer.ReadOnly = true;
            // 
            // NameServer
            // 
            this.NameServer.HeaderText = "Имя сервера";
            this.NameServer.Name = "NameServer";
            this.NameServer.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.lbl_Default);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_Remove);
            this.groupBox1.Controls.Add(this.btn_Edit);
            this.groupBox1.Controls.Add(this.btn_Add);
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(527, 331);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Список доступных OPC Серверов";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(300, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(221, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Установить сервером по-умолчанию";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // lbl_Default
            // 
            this.lbl_Default.AutoSize = true;
            this.lbl_Default.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_Default.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lbl_Default.Location = new System.Drawing.Point(164, 309);
            this.lbl_Default.Name = "lbl_Default";
            this.lbl_Default.Size = new System.Drawing.Size(75, 13);
            this.lbl_Default.TabIndex = 5;
            this.lbl_Default.Text = "Не выбран!";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 309);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "OPC-Сервер (по-умолчанию):";
            // 
            // btn_Remove
            // 
            this.btn_Remove.Image = global::OPCClient.Properties.Resources.deleteIcon;
            this.btn_Remove.Location = new System.Drawing.Point(80, 16);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(31, 25);
            this.btn_Remove.TabIndex = 2;
            this.btn_Remove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_Remove.UseVisualStyleBackColor = true;
            this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
            // 
            // btn_Edit
            // 
            this.btn_Edit.Image = global::OPCClient.Properties.Resources.editIcon;
            this.btn_Edit.Location = new System.Drawing.Point(43, 16);
            this.btn_Edit.Name = "btn_Edit";
            this.btn_Edit.Size = new System.Drawing.Size(31, 25);
            this.btn_Edit.TabIndex = 1;
            this.btn_Edit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_Edit.UseVisualStyleBackColor = true;
            this.btn_Edit.Click += new System.EventHandler(this.btn_Edit_Click);
            // 
            // btn_Add
            // 
            this.btn_Add.Image = global::OPCClient.Properties.Resources.addIcon;
            this.btn_Add.Location = new System.Drawing.Point(6, 16);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(31, 25);
            this.btn_Add.TabIndex = 0;
            this.btn_Add.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // ChoiceOPCServersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 333);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChoiceOPCServersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка OPC-сервера";
            this.Load += new System.EventHandler(this.ChoiceOPCServersForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
	}
}
