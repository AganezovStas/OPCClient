using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.MessageBox;
using OPCClient.Properties;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace OPCClient
{
	public class JoinForm : System.Windows.Forms.Form
	{
		private System.Data.DataTable dtList;

		public System.Data.SqlClient.SqlConnectionStringBuilder sb;

		private string serversXmlFile = "ServerList.xml";

		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.ComboBox cbServerList;

		private System.Windows.Forms.PictureBox pictureBox1;

		private System.Windows.Forms.Label label1;

		private System.Windows.Forms.ComboBox cbServerType;

		private System.Windows.Forms.Label label2;

		private System.Windows.Forms.ComboBox cbChoiceAutentification;

		private System.Windows.Forms.Label label3;

		private System.Windows.Forms.Label label4;

		private System.Windows.Forms.Label label5;

		private System.Windows.Forms.TextBox tbUser;

		private System.Windows.Forms.TextBox tbPass;

		private System.Windows.Forms.Button btnConnect;

		private System.Windows.Forms.Button btnCancel;

		private System.ComponentModel.BackgroundWorker bw_loader;

		private System.Windows.Forms.StatusStrip statusStrip1;

		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;

		private System.Windows.Forms.Label label6;

		private System.Windows.Forms.ComboBox cbInitialCatalog;

		private System.Windows.Forms.Label label7;

		private System.Windows.Forms.ComboBox cb_ServerIdentifier;

		public JoinForm()
		{
			this.InitializeComponent();
			try
			{
				this.dtList = new System.Data.DataTable("Servers");
				this.cb_ServerIdentifier.Text = Program.sL.tmp_listObject.ServerIdentifier;
				this.cbServerList.Items.Add(Program.sL.tmp_listObject.DataBase.DataSource);
				this.tbUser.Text = Program.sL.tmp_listObject.DataBase.UserID;
				this.tbPass.Text = Program.sL.tmp_listObject.DataBase.Password;
				this.cbChoiceAutentification.SelectedIndex = (Program.sL.tmp_listObject.DataBase.IntegratedSecurity ? 0 : 1);
				this.cbInitialCatalog.Items.Add(Program.sL.tmp_listObject.DataBase.InitialCatalog);
				this.cbInitialCatalog.SelectedIndex = 0;
			}
			catch
			{
			}
			this.bw_loader.RunWorkerAsync();
		}

		private void btnRefreshServerList_Click(object sender, System.EventArgs e)
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			try
			{
				this.dtList = SmoApplication.EnumAvailableSqlServers(false);
				this.cbServerList.Items.Clear();
				foreach (System.Data.DataRow dataRow in this.dtList.Rows)
				{
					string text = dataRow["Server"].ToString();
					if (dataRow["Instance"] != null && dataRow["Instance"].ToString().Length > 0)
					{
						text = text + "\\" + dataRow["Instance"].ToString();
					}
					if (this.cbServerList.Items.IndexOf(text) < 0)
					{
						this.cbServerList.Items.Add(text);
					}
				}
			}
			finally
			{
				this.Cursor = System.Windows.Forms.Cursors.Default;
			}
		}

		private void JoinForm_Load(object sender, System.EventArgs e)
		{
			this.cbServerType.SelectedIndex = 0;
			if (this.cbServerList.Items.Count > 0)
			{
				this.cbServerList.SelectedIndex = 0;
			}
		}

		private void cbServerList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ((sender as System.Windows.Forms.ComboBox).SelectedIndex == this.cbServerList.Items.IndexOf("<Найти в локальной сети>"))
			{
				this.toolStripStatusLabel1.Text = "Поиск серверов...";
				System.Windows.Forms.Application.DoEvents();
				this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
				System.Windows.Forms.Application.UseWaitCursor = true;
				this.cbServerList.Items.Clear();
				try
				{
					this.dtList = SmoApplication.EnumAvailableSqlServers(false);
					System.Windows.Forms.Application.DoEvents();
					this.dtList.TableName = "Servers";
					this.cbServerList.Items.Clear();
					foreach (System.Data.DataRow dataRow in this.dtList.Rows)
					{
						string text = dataRow["Server"].ToString();
						if (dataRow["Instance"] != null && dataRow["Instance"].ToString().Length > 0)
						{
							text = text + "\\" + dataRow["Instance"].ToString();
						}
						if (System.Net.Dns.GetHostName() == text)
						{
							this.cbServerList.Items.Add("(Local)");
						}
						else if (this.cbServerList.Items.IndexOf(text) < 0)
						{
							this.cbServerList.Items.Add(text);
						}
					}
					this.cbServerList.Items.Add("<Найти в локальной сети>");
				}
				finally
				{
					this.Cursor = System.Windows.Forms.Cursors.Default;
					System.Windows.Forms.Application.UseWaitCursor = false;
					this.toolStripStatusLabel1.Text = "Поиск завершен";
				}
			}
		}

		private string GetConnectionString()
		{
			this.sb = new System.Data.SqlClient.SqlConnectionStringBuilder();
			ConnectionString.DataSource = this.cbServerList.Text;
			ConnectionString.Password = this.tbPass.Text;
			ConnectionString.UserID = this.tbUser.Text;
			ConnectionString.IntegratedSecurity = (this.cbChoiceAutentification.SelectedIndex == 0);
			this.sb.DataSource = this.cbServerList.Text;
			this.sb.Password = this.tbPass.Text;
			this.sb.UserID = this.tbUser.Text;
			if (this.cbChoiceAutentification.SelectedIndex == 0)
			{
				this.sb.IntegratedSecurity = true;
			}
			return this.sb.ConnectionString;
		}

		private void btnConnect_Click(object sender, System.EventArgs e)
		{
			using (System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(this.GetConnectionString()))
			{
				this.toolStripStatusLabel1.Text = "Установка подключения...";
				System.Windows.Forms.Application.DoEvents();
				try
				{
					this.dtList.WriteXml(this.serversXmlFile, System.Data.XmlWriteMode.WriteSchema, false);
				}
				catch
				{
				}
				try
				{
					using (sqlConnection)
					{
						sqlConnection.Open();
						string cmdText = "SELECT name FROM sys.databases; ";
						using (System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(cmdText, sqlConnection))
						{
							sqlCommand.ExecuteReader();
						}
					}
					this.toolStripStatusLabel1.Text = "Подключение установлено";
					System.Windows.Forms.Application.DoEvents();
					Program.sL.tmp_listObject.ServerIdentifier = this.cb_ServerIdentifier.Text;
					Program.sL.tmp_listObject.DataBase.IntegratedSecurity = (this.cbChoiceAutentification.SelectedIndex == 0);
					Program.sL.tmp_listObject.DataBase.InitialCatalog = this.cbInitialCatalog.Text;
					Program.sL.tmp_listObject.DataBase.DataSource = this.cbServerList.Text;
					Program.sL.tmp_listObject.DataBase.UserID = this.tbUser.Text;
					Program.sL.tmp_listObject.DataBase.Password = this.tbPass.Text;
					Program.sL.Save();
					base.DialogResult = System.Windows.Forms.DialogResult.OK;
				}
				catch (System.Exception ex)
				{
					this.toolStripStatusLabel1.Text = "";
					new ExceptionMessageBox(ex)
					{
						Caption = "Ошибка",
						InnerException = ex.InnerException,
						Buttons = ExceptionMessageBoxButtons.OK,
						Symbol = ExceptionMessageBoxSymbol.Error
					}.Show(this);
					base.DialogResult = System.Windows.Forms.DialogResult.None;
				}
			}
		}

		private void cbChoiceAutentification_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ((sender as System.Windows.Forms.ComboBox).SelectedIndex == 0)
			{
				this.tbUser.Text = System.Environment.UserName + "\\" + System.Environment.UserDomainName;
				this.tbUser.Enabled = false;
				this.tbPass.Enabled = false;
			}
			else
			{
				this.tbUser.Enabled = true;
				this.tbPass.Enabled = true;
			}
		}

		private void bw_loader_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			try
			{
				if (this.cbServerList.Items.IndexOf("(Local)") < 0)
				{
					System.Windows.Forms.Application.DoEvents();
					this.dtList = SmoApplication.EnumAvailableSqlServers(true);
					this.dtList.TableName = "Servers";
				}
			}
			finally
			{
			}
		}

		private void bw_loader_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			try
			{
				if (this.dtList != null && this.dtList.Rows.Count == 1)
				{
					System.Data.DataRow dataRow = this.dtList.Rows[0];
					string text = "(Local)";
					if (dataRow["Instance"] != null && dataRow["Instance"].ToString().Length > 0)
					{
						text = text + "\\" + dataRow["Instance"].ToString();
					}
					if (this.cbServerList.Items.IndexOf(text) < 0)
					{
						this.cbServerList.Items.Add(text);
					}
				}
				this.cbServerList.Items.Add("<Найти в локальной сети>");
			}
			finally
			{
			}
		}

		private void cbInitialCatalog_DropDown(object sender, System.EventArgs e)
		{
			using (System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(this.GetConnectionString()))
			{
				this.toolStripStatusLabel1.Text = "Установка подключения...";
				System.Windows.Forms.Application.DoEvents();
				try
				{
					using (sqlConnection)
					{
						sqlConnection.Open();
						string cmdText = "SELECT name FROM sys.databases; ";
						using (System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(cmdText, sqlConnection))
						{
							System.Data.SqlClient.SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
							this.cbInitialCatalog.Items.Clear();
							using (sqlDataReader)
							{
								while (sqlDataReader.Read())
								{
									this.cbInitialCatalog.Items.Add(sqlDataReader.GetString(0));
								}
							}
						}
					}
					this.toolStripStatusLabel1.Text = "Подключение установлено";
					System.Windows.Forms.Application.DoEvents();
				}
				catch (System.Exception ex)
				{
					this.toolStripStatusLabel1.Text = "";
					new ExceptionMessageBox(ex)
					{
						Caption = "Ошибка",
						InnerException = ex.InnerException,
						Buttons = ExceptionMessageBoxButtons.OK,
						Symbol = ExceptionMessageBoxSymbol.Error
					}.Show(this);
					base.DialogResult = System.Windows.Forms.DialogResult.None;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JoinForm));
            this.cbServerList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbServerType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbChoiceAutentification = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bw_loader = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.cbInitialCatalog = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cb_ServerIdentifier = new System.Windows.Forms.ComboBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbServerList
            // 
            this.cbServerList.FormattingEnabled = true;
            this.cbServerList.Location = new System.Drawing.Point(146, 130);
            this.cbServerList.Name = "cbServerList";
            this.cbServerList.Size = new System.Drawing.Size(243, 21);
            this.cbServerList.TabIndex = 0;
            this.cbServerList.SelectedIndexChanged += new System.EventHandler(this.cbServerList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Тип сервера:";
            // 
            // cbServerType
            // 
            this.cbServerType.Enabled = false;
            this.cbServerType.FormattingEnabled = true;
            this.cbServerType.Items.AddRange(new object[] {
            "Компонент Database Engine"});
            this.cbServerType.Location = new System.Drawing.Point(146, 103);
            this.cbServerType.Name = "cbServerType";
            this.cbServerType.Size = new System.Drawing.Size(243, 21);
            this.cbServerType.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Имя сервера:";
            // 
            // cbChoiceAutentification
            // 
            this.cbChoiceAutentification.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChoiceAutentification.FormattingEnabled = true;
            this.cbChoiceAutentification.Items.AddRange(new object[] {
            "Проверка подлинности Windows",
            "Проверка подлинности SQL Server"});
            this.cbChoiceAutentification.Location = new System.Drawing.Point(146, 157);
            this.cbChoiceAutentification.MaxDropDownItems = 2;
            this.cbChoiceAutentification.Name = "cbChoiceAutentification";
            this.cbChoiceAutentification.Size = new System.Drawing.Size(243, 21);
            this.cbChoiceAutentification.TabIndex = 6;
            this.cbChoiceAutentification.SelectedIndexChanged += new System.EventHandler(this.cbChoiceAutentification_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Проверка подлинности:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(54, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Имя пользователя:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 210);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Пароль:";
            // 
            // tbUser
            // 
            this.tbUser.Location = new System.Drawing.Point(165, 184);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(224, 20);
            this.tbUser.TabIndex = 10;
            // 
            // tbPass
            // 
            this.tbPass.Location = new System.Drawing.Point(165, 210);
            this.tbPass.Name = "tbPass";
            this.tbPass.PasswordChar = '*';
            this.tbPass.Size = new System.Drawing.Size(224, 20);
            this.tbPass.TabIndex = 11;
            // 
            // btnConnect
            // 
            this.btnConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConnect.Location = new System.Drawing.Point(176, 266);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(109, 23);
            this.btnConnect.TabIndex = 12;
            this.btnConnect.Text = "Сохранить";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(291, 266);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // bw_loader
            // 
            this.bw_loader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_loader_DoWork);
            this.bw_loader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_loader_RunWorkerCompleted);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 298);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(400, 22);
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(55, 17);
            this.toolStripStatusLabel1.Text = "                ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 242);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "База данных:";
            // 
            // cbInitialCatalog
            // 
            this.cbInitialCatalog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInitialCatalog.FormattingEnabled = true;
            this.cbInitialCatalog.Location = new System.Drawing.Point(146, 239);
            this.cbInitialCatalog.MaxDropDownItems = 5;
            this.cbInitialCatalog.Name = "cbInitialCatalog";
            this.cbInitialCatalog.Size = new System.Drawing.Size(243, 21);
            this.cbInitialCatalog.TabIndex = 15;
            this.cbInitialCatalog.DropDown += new System.EventHandler(this.cbInitialCatalog_DropDown);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::OPCClient.Properties.Resources.SQLServer;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(400, 72);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Blue;
            this.label7.Location = new System.Drawing.Point(12, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Идентификатор:";
            // 
            // cb_ServerIdentifier
            // 
            this.cb_ServerIdentifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ServerIdentifier.FormattingEnabled = true;
            this.cb_ServerIdentifier.Items.AddRange(new object[] {
            "KIASU",
            "WEBSERV"});
            this.cb_ServerIdentifier.Location = new System.Drawing.Point(146, 77);
            this.cb_ServerIdentifier.Name = "cb_ServerIdentifier";
            this.cb_ServerIdentifier.Size = new System.Drawing.Size(243, 21);
            this.cb_ServerIdentifier.TabIndex = 4;
            // 
            // JoinForm
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(400, 320);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbInitialCatalog);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.tbPass);
            this.Controls.Add(this.tbUser);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbChoiceAutentification);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cb_ServerIdentifier);
            this.Controls.Add(this.cbServerType);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.cbServerList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "JoinForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Соединение с сервером";
            this.Load += new System.EventHandler(this.JoinForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
