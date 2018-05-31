using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OPCClient
{
	public class EditOPCServerForm : System.Windows.Forms.Form
	{
		private OPCclient cl;

		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.Button btn_OK;

		private System.Windows.Forms.Label label1;

		private System.Windows.Forms.Label label2;

		private System.Windows.Forms.Button button1;

		public System.Windows.Forms.TextBox tb_IPAddrServer;

		public System.Windows.Forms.TextBox tb_NameServer;

		public EditOPCServerForm()
		{
			this.InitializeComponent();
		}

		private void btn_OK_Click(object sender, System.EventArgs e)
		{
			this.cl = new OPCclient();
			try
			{
				int num = this.cl.Connect(this.tb_IPAddrServer.Text, this.tb_NameServer.Text, 1000);
				if (num != 0)
				{
					throw new System.Exception(this.cl.ErrTxt);
				}
				this.cl.Disconnect();
				base.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			catch (System.Exception e2)
			{
				Program.showErrorMessage(e2, this);
				base.DialogResult = System.Windows.Forms.DialogResult.None;
			}
			finally
			{
				this.cl = null;
			}
		}

		private void ChoiceOPCServerForm_Load(object sender, System.EventArgs e)
		{
			try
			{
			}
			catch (System.Exception e2)
			{
				Program.showErrorMessage(e2, this);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditOPCServerForm));
            this.btn_OK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_IPAddrServer = new System.Windows.Forms.TextBox();
            this.tb_NameServer = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(200, 87);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 2;
            this.btn_OK.Text = "ОК";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP-адрес сервера";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Имя сервера";
            // 
            // tb_IPAddrServer
            // 
            this.tb_IPAddrServer.Location = new System.Drawing.Point(15, 22);
            this.tb_IPAddrServer.Name = "tb_IPAddrServer";
            this.tb_IPAddrServer.Size = new System.Drawing.Size(341, 20);
            this.tb_IPAddrServer.TabIndex = 0;
            // 
            // tb_NameServer
            // 
            this.tb_NameServer.Location = new System.Drawing.Point(15, 61);
            this.tb_NameServer.Name = "tb_NameServer";
            this.tb_NameServer.Size = new System.Drawing.Size(341, 20);
            this.tb_NameServer.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(282, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Отмена";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // EditOPCServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 116);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tb_NameServer);
            this.Controls.Add(this.tb_IPAddrServer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditOPCServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OPC-сервер";
            this.Load += new System.EventHandler(this.ChoiceOPCServerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
