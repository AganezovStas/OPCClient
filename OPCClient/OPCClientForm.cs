using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCClient
{
	public class OPCClientForm : System.Windows.Forms.Form
	{
        bool cancelbtnclick;

		private OPCclient cl;

		private System.Data.SqlClient.SqlConnectionStringBuilder sb;

		private System.Data.SqlClient.SqlConnection conn;

		private System.Data.SqlClient.SqlDataAdapter da;

		private bool isSqlError = false;

		public System.Data.DataTable dt;

		public System.Data.DataTable tChanges;

		public string tablename;

		public string spid;

		public string Error = "";

		private System.Data.DataTable tVals;

		private System.Data.DataTable tTags;

		private System.Data.DataTable tError;

		private System.DateTime currentTime;

		private bool stop_ = true;

		private System.TimeSpan tsStart;

		private bool autostart = true;

		private WorkThread thrDB;

		private System.Windows.Forms.FormWindowState _OldFormState;

		private CancellationToken ctl = new CancellationToken(false);

		private int defaultCheckingInterval = 0;

		private Color previousBackColor;

		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.Button button1;

		private System.Windows.Forms.Button button2;

		private System.Windows.Forms.Button btn_Start;

		private System.Windows.Forms.ComboBox comboBox1;

		private MyDataGridView dataGridView1;

		private System.ComponentModel.BackgroundWorker backgroundWorker1;

		private System.Windows.Forms.Timer timer1;

		private System.Windows.Forms.NotifyIcon _notifyIcon;

		private System.Windows.Forms.ContextMenuStrip contextMenuNotify;

		private System.Windows.Forms.ToolStripMenuItem tsmItemExit;

		private System.Windows.Forms.MenuStrip menuStrip1;

		private System.Windows.Forms.ToolStripMenuItem настройкаToolStripMenuItem;

		private System.Windows.Forms.ToolStripMenuItem справкаToolStripMenuItem;

		private System.Windows.Forms.Timer timer2;

		private System.Windows.Forms.ToolStripMenuItem соединениеСБДNurbaToolStripMenuItem;

		private System.Windows.Forms.ToolStripMenuItem источникOPCсерверToolStripMenuItem;

		private System.Windows.Forms.StatusStrip statusStrip1;

		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;

		private System.Windows.Forms.ToolStripStatusLabel lbl_DataBase;

		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;

		private System.Windows.Forms.ToolStripStatusLabel lbl_OPCServer;

		private System.Windows.Forms.Button btn_Stop;

		private System.Windows.Forms.ToolStripMenuItem MenuItem_Autorun;

		private System.Windows.Forms.ListBox lboxError;

		private System.Windows.Forms.Button btnHide;

		private System.Windows.Forms.Button btnShowLog;

		private System.Windows.Forms.SplitContainer splitContainer1;

		private System.Windows.Forms.Label lblError;

		private System.Windows.Forms.Label lbl_DateTime;

		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;

		private System.Windows.Forms.ToolStripStatusLabel lblLastSuccessTransaction;

		private System.Windows.Forms.ToolStripMenuItem настройкаToolStripMenuItem1;

		private System.Windows.Forms.ToolStripMenuItem соединениеСБДКИАСУToolStripMenuItem;

		private System.Windows.Forms.ToolStripMenuItem источникOPCсерверToolStripMenuItem1;

		private System.Windows.Forms.ToolStripMenuItem автозапускToolStripMenuItem;

		private System.Windows.Forms.ToolStripMenuItem справкаToolStripMenuItem1;

		private System.Windows.Forms.ToolStripMenuItem таблицаТеговToolStripMenuItem;

		private System.Windows.Forms.ToolStripMenuItem добавитьToolStripMenuItem;

		private System.Windows.Forms.ToolStripMenuItem редактироватьToolStripMenuItem;
        private System.Windows.Forms.Timer timer_reconnect;

		private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem;

		public OPCClientForm()
		{
			this.InitializeComponent();
			this.splitContainer1.Panel2Collapsed = true;
		}

		private void OPCClientForm_Load(object sender, System.EventArgs e)
		{
			Program.onError += new Program.methodContainer(this.Program_onError);
			this.tError = new System.Data.DataTable("Errors");
			this.tError.Columns.Add(new System.Data.DataColumn("Text", typeof(string)));
			this.lboxError.DataSource = this.tError;
			this.lboxError.DisplayMember = "Text";
			this.SetCheckboxState();
			this.SetTimeSpanStart();
			this.Reconnect();
			this.lbl_OPCServer.Text = Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer + "\\" + Program.sL.tmp_listObject.DefaultOPCServer.NameServer;
			this.dt = new System.Data.DataTable();
			this.dt.Columns.Add("bool Item", typeof(string));
			this.dt.Columns.Add("bool Name", typeof(string));
			this.dt.Columns.Add("Type", typeof(string));
			this.dt.Columns.Add("TypeRW", typeof(string));
		}

		private void Reconnect()
		{
			this.sb = new System.Data.SqlClient.SqlConnectionStringBuilder();
			this.sb.DataSource = Program.sL.tmp_listObject.DataBase.DataSource;
			this.sb.InitialCatalog = Program.sL.tmp_listObject.DataBase.InitialCatalog;
			this.sb.UserID = Program.sL.tmp_listObject.DataBase.UserID;
			this.sb.Password = Program.sL.tmp_listObject.DataBase.Password;
			this.sb.IntegratedSecurity = Program.sL.tmp_listObject.DataBase.IntegratedSecurity;
			this.lbl_DataBase.Text = this.sb.DataSource + "\\" + this.sb.InitialCatalog;
		}

		private void Program_onError(string message)
		{
			base.Invoke(new System.EventHandler(delegate(object param0, System.EventArgs param1)
			{
				try
				{
					this.WriteError(message);
				}
				catch
				{
				}
			}));
		}

		private void WriteError(string message)
		{
			try
			{
				System.Diagnostics.Trace.WriteLine(message);
				this.Error = message;
				message = string.Format("{0:F}: {1}", System.DateTime.Now, message);
				if (this.tError.Rows.Count == 10)
				{
					this.tError.Rows.RemoveAt(0);
				}
				this.tError.Rows.Add(new object[]
				{
					message
				});
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Trace.WriteLine("Ошибка при записи лога, функция voi WriteError(message). Описание: " + ex.Message);
			}
		}

		private void cl_onError(object sender, ErrorEventArgs e)
		{
			if (e.ErrorCode > 0)
			{
				this.WriteError(string.Concat(new object[]
				{
					"Ошибка произошла при вызове метода ",
					e.ErrorMethodName,
					" : ",
					e.ErrorText,
					" (Код ошибки ",
					e.ErrorCode,
					")"
				}));
			}
		}

		private void SetTimeSpanStart()
		{
			this.tsStart = new System.TimeSpan(0, 0, 10);
			this.btn_Start.Enabled = true;
			this.btn_Start.Text = "Запуск";
		}

		private void _notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if (e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					if (base.WindowState == System.Windows.Forms.FormWindowState.Normal || base.WindowState == System.Windows.Forms.FormWindowState.Maximized)
					{
						this._OldFormState = base.WindowState;
						base.WindowState = System.Windows.Forms.FormWindowState.Minimized;
					}
					else
					{
						base.Show();
						base.WindowState = this._OldFormState;
						this.stop_ = true;
					}
				}
			}
			catch (System.Exception ex)
			{
				Program.LogWrite(ex.Message, "_notifyIcon_MouseClick", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 162);
				Program.showErrorMessage(ex, this);
			}
		}

		private void OPCClientForm_Resize(object sender, System.EventArgs e)
		{
			try
			{
				if (System.Windows.Forms.FormWindowState.Minimized == base.WindowState)
				{
					base.Hide();
				}
			}
			catch (System.Exception ex)
			{
				Program.LogWrite(ex.Message, "OPCClientForm_Resize", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 184);
				Program.showErrorMessage(ex, this);
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			int num = this.cl.Init("tableSignal2.xlsx", TypeCnfFile.xlsxTable2);
			num = this.cl.Connect(Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer, Program.sL.tmp_listObject.DefaultOPCServer.NameServer, 3000);
			this.cl.DataChange += new System.EventHandler<ReadDataEventArgs>(this.cl_DataChange);
		}

		private void cl_DataChange(object sender, ReadDataEventArgs e)
		{
			string name = e.Vars[0].Name;
			string val = e.Vars[0].Value.ToString();
			base.Invoke(new System.EventHandler(delegate(object param0, System.EventArgs param1)
			{
				this.lbl_DataBase.Text = name;
				this.lbl_DateTime.Text = val;
				try
				{
					if (this.tVals != null)
					{
						foreach (VarData current in e.Vars)
						{
							System.Data.DataRow[] array = this.tVals.Select("TagName='" + current.Name + "'");
							System.Data.DataRow[] array2 = array;
							for (int i = 0; i < array2.Length; i++)
							{
								System.Data.DataRow dataRow = array2[i];
								dataRow["Value"] = current.Value;
							}
						}
					}
				}
				catch
				{
				}
			}));
		}

		private void initlabels(string name, string val)
		{
			this.lbl_DataBase.Text = name;
			this.lbl_DateTime.Text = val;
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			bool flag2;
			bool flag = this.cl.ReadBool("PC100/HIS11113.RUN_STAT", out flag2);
			bool flag3 = this.cl.ReadBool("1DRG01AP001_RUN_STAT", out flag2);
			bool flag4 = this.cl.ReadBool("2DRG01AP001_RUN_STAT", out flag2);
			bool flag5 = this.cl.ReadBool("3DRG01AP001_RUN_STAT", out flag2);
			System.Windows.Forms.MessageBox.Show(string.Format("HIS = {0}\r\n1DRG = {1}\r\n2DRG = {2}\r\n3DRG = {3}", new object[]
			{
				flag,
				flag3,
				flag4,
				flag5
			}));
		}

		private void Form1_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
		{
			this.cl.Disconnect();
		}

		public void ListODBCsources()
		{
			int envHandle = 0;
			if (OdbcWrapper.SQLAllocEnv(ref envHandle) != -1)
			{
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(1024);
				System.Text.StringBuilder stringBuilder2 = new System.Text.StringBuilder(1024);
				int num = 0;
				int num2 = 0;
				this.comboBox1.Items.Clear();
				for (int num3 = OdbcWrapper.SQLDataSources(envHandle, 32, stringBuilder, stringBuilder.Capacity, ref num, stringBuilder2, stringBuilder2.Capacity, ref num2); num3 == 0; num3 = OdbcWrapper.SQLDataSources(envHandle, 1, stringBuilder, stringBuilder.Capacity, ref num, stringBuilder2, stringBuilder2.Capacity, ref num2))
				{
					this.comboBox1.Items.Add(stringBuilder + System.Environment.NewLine + stringBuilder2);
				}
			}
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			this.SetTimeSpanStart();
			this.autostart = false;
			int num = 0;
			while (this.backgroundWorker1.IsBusy)
			{
				System.Windows.Forms.Application.DoEvents();
				System.Threading.Thread.Sleep(1000);
				num++;
				if (num % 10 == 0)
				{
					System.Windows.Forms.MessageBox.Show("Прошло 10 итераций ожидания backgroundworker. Отрубаю его нахрен.");
                    cancelbtnclick = true;
					this.backgroundWorker1.CancelAsync();
				}
			}
            cancelbtnclick = false;
			this.backgroundWorker1.RunWorkerAsync();
			this.btn_Stop.Enabled = true;
			this.btn_Start.Enabled = false;
		}

		private void InitTagsTable(System.ComponentModel.DoWorkEventArgs e)
		{
			this.tTags = new System.Data.DataTable("Tags");
			if (this.conn == null)
			{
				this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
			}
			else if (this.conn.ConnectionString == null)
			{
				this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
			}
			else if (this.conn.ConnectionString == "")
			{
				this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
			}
			Task task = this.conn.OpenAsync(this.ctl);
			int num = 0;
			while (!task.IsCompleted)
			{
				if (num == 10)
				{
					e.Cancel = true;
					throw new System.Exception("Не удалось подключиться к SQL серверу");
				}
				System.Threading.Thread.Sleep(1000);
				num++;
			}
			using (this.conn)
			{
				this.da = new System.Data.SqlClient.SqlDataAdapter("select * from dbo.Tags", this.conn);
				this.da.Fill(this.tTags);
			}
			this.da.InsertCommand = new System.Data.SqlClient.SqlCommand("INSERT INTO [" + Program.sL.tmp_listObject.DataBase.InitialCatalog + "].[dbo].[Tags]\r\n([TagID],[TagName],[TagType],[Inv],[Description],[Note],[groupID],[code],[div],[min],[max]) Values (@TagID,@TagName,@TagType,0,@Description,@Note,0,@code,1,0,@max)", this.conn);
			this.da.UpdateCommand = new System.Data.SqlClient.SqlCommand("UPDATE [" + Program.sL.tmp_listObject.DataBase.InitialCatalog + "].[dbo].[Tags] \r\nSET [TagName] = @TagName, [TagType] = @TagType, [Description] = @Description, [Note] = @Note, [code] = @code, [max] = @max WHERE [TagID] = @TagID", this.conn);
			this.da.DeleteCommand = new System.Data.SqlClient.SqlCommand("DELETE FROM [" + Program.sL.tmp_listObject.DataBase.InitialCatalog + "].[dbo].[Tags] WHERE [TagID] = @TagID", this.conn);
		}

		private void InitSqlCommandParameters(System.Data.SqlClient.SqlCommand cmd, System.Data.DataRow row)
		{
			cmd.Parameters.Clear();
			cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("TagID", System.Data.SqlDbType.Int));
			cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("TagName", System.Data.SqlDbType.NVarChar));
			cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("TagType", System.Data.SqlDbType.Char));
			cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("Description", System.Data.SqlDbType.NVarChar));
			cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("Note", System.Data.SqlDbType.NVarChar));
			cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("Code", System.Data.SqlDbType.Int));
			cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("Max", System.Data.SqlDbType.Float));
			cmd.Parameters["TagID"].Value = row["TagID"];
			cmd.Parameters["TagName"].Value = row["TagName"];
			cmd.Parameters["TagType"].Value = row["TagType"];
			cmd.Parameters["Description"].Value = row["Description"];
			cmd.Parameters["Note"].Value = row["Note"];
			cmd.Parameters["Code"].Value = row["Code"];
			cmd.Parameters["Max"].Value = row["Max"];
		}

		private int OPCClient_Init()
		{
			foreach (System.Data.DataRow dataRow in this.tTags.Rows)
			{
				object obj = dataRow["TagName"];
				object obj2 = (System.Convert.ToString(dataRow["TagType"]) == "F") ? "double" : "bool";
				this.dt.Rows.Add(new object[]
				{
					obj,
					obj,
					obj2,
					"read"
				});
			}
			if (Program.sL.tmp_listObject.ServerIdentifier == "KIASU")
			{
				this.dt.Rows.Add(new object[]
				{
					"OPC_CLIENT.KIASU_CLIENT_INPUT",
					"OPC_CLIENT.KIASU_CLIENT_INPUT",
					"bool",
					"read/write"
				});
				this.dt.Rows.Add(new object[]
				{
					"OPC_CLIENT.KIASU_CLIENT_LAST_DATE",
					"OPC_CLIENT.KIASU_CLIENT_LAST_DATE",
					"string",
					"read/write"
				});
			}
			if (Program.sL.tmp_listObject.ServerIdentifier == "WEBSERV")
			{
				this.dt.Rows.Add(new object[]
				{
					"OPC_CLIENT.WEBSERV_CLIENT_INPUT",
					"OPC_CLIENT.WEBSERV_CLIENT_INPUT",
					"bool",
					"read/write"
				});
				this.dt.Rows.Add(new object[]
				{
					"OPC_CLIENT.WEBSERV_CLIENT_LAST_DATE",
					"OPC_CLIENT.WEBSERV_CLIENT_LAST_DATE",
					"string",
					"read/write"
				});
			}
			int result = this.cl.Init(this.dt, TypeCnfFile.dataTable);
			Items items = this.cl.GetItems();
			return result;
		}

        int tryed = 1;

		private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			try
			{
				this.InitTagsTable(e);
				this.isSqlError = false;
				int num = 0;
				this.Connect(Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer, Program.sL.tmp_listObject.DefaultOPCServer.NameServer, out num);
                
                while ((num > 0) && (!cancelbtnclick))
                {
                    //while (num > 0)
                    Trace.WriteLine("Попытка подключения № " + tryed);
                    Thread.Sleep(10 * 1000);
                    this.CheckAndConnect_to_OPC(out num);
                    tryed++;
                }
				if (num == 0)
				{
					this.tVals = this.tTags.Copy();
					System.Data.DataColumn column = new System.Data.DataColumn("Value", typeof(object));
					this.tVals.Columns.Add(column);
					e.Result = this.tVals;
				}
				else
				{
					e.Result = (this.Error = this.cl.ErrTxt);
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				this.isSqlError = true;
				this.таблицаТеговToolStripMenuItem.Enabled = false;
				e.Result = ex.Message;
				Program.LogWrite(ex.Message, "backgroundWorker1_DoWork", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 452);
			}
			catch (System.Exception ex2)
			{
				e.Result = ex2.Message;
				Program.LogWrite(ex2.Message, "backgroundWorker1_DoWork", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 453);
			}
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				this.btn_Stop.Enabled = false;
                cancelbtnclick = false;
				this.btn_Start.Enabled = true;
			}
			else if (e.Result is System.Data.DataTable)
			{
				this.таблицаТеговToolStripMenuItem.Enabled = true;
				this.dataGridView1.DataSource = this.tVals;
				int num = 0;
				this.CheckAndConnect_to_OPC(out num);
				this.lbl_OPCServer.Text = this.cl.URL;
				this.timer1.Enabled = true;
				this.thrDB = new WorkThread(this.tVals);
				this.btn_Stop.Enabled = true;
			}
			else
			{
                cancelbtnclick = false;
				Program.LogWrite(e.Result.ToString(), "backgroundWorker1_RunWorkerCompleted", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 484);
			}
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Application.DoEvents();
			this.UpdateTagValues();
			this.lbl_OPCServer.Text = this.cl.URL;
		}

		private void Connect(string IPAddr, string NameServ, out int err)
		{
			err = 0;
			this.cl = new OPCclient();
			this.cl.onError += new System.EventHandler<ErrorEventArgs>(this.cl_onError);
			this.OPCClient_Init();
            System.Diagnostics.Trace.WriteLine(string.Format("Попытка подключения к OPC-серверу: {0}\\{1}", IPAddr, NameServ));
			err = this.cl.Connect(IPAddr, NameServ, 10);
            if (err == 0)
                System.Diagnostics.Trace.WriteLine("Подключено к серверу " + this.cl.URL);
            else 
                System.Diagnostics.Trace.WriteLine("Не удалось подключиться к серверу " + this.cl.URL);
		}

		public bool CheckAndConnect_to_OPC(out int errNum)
		{
			errNum = 0;
			bool connectionState = this.cl.GetConnectionState();
			string errTxt = this.cl.ErrTxt;
			if (!connectionState)
			{
				this.Connect(Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer, Program.sL.tmp_listObject.DefaultOPCServer.NameServer, out errNum);
				connectionState = this.cl.GetConnectionState();
				if (errNum != 0 || !connectionState)
				{
					foreach (SettingsOPCServer current in Program.sL.tmp_listObject.OPCServers)
					{                        
						//System.Diagnostics.Trace.WriteLine("Попытка подключения к OPC-серверу : " + current.IpAddressServer + "\\" + current.NameServer);
						this.Connect(current.IpAddressServer, current.NameServer, out errNum);
						connectionState = this.cl.GetConnectionState();
						if (connectionState)
						{
							//System.Diagnostics.Trace.WriteLine("Подключено к серверу " + this.cl.URL);
							break;
						}
					}
				}
				if (!connectionState)
				{
					Program.LogWrite("Не удалось подключиться ни к одному из OPC-серверов. Проверьте наличие подключение сетевой карты", "CheckAndConnect_to_OPC", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 549);
				}
			}
			return connectionState;
		}

		public void UpdateTagValues()
		{
			try
			{
				bool flag = false;
				this.defaultCheckingInterval++;
				try
				{
					if (this.defaultCheckingInterval > 600)
					{
						this.defaultCheckingInterval = -1;
						string b = Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer + "\\" + Program.sL.tmp_listObject.DefaultOPCServer.NameServer;
						if (this.cl.URL != b)
						{
                            System.Diagnostics.Trace.WriteLine("Текущий OPC-сервер не является сервером по умолчанию...");
                            System.Diagnostics.Trace.WriteLine("Попытка подключения к OPC-серверу (default): " + Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer + "\\" + Program.sL.tmp_listObject.DefaultOPCServer.NameServer);
							OPCclient oPCclient = new OPCclient();
							oPCclient.Connect(Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer, Program.sL.tmp_listObject.DefaultOPCServer.NameServer, 1000);
							if (oPCclient.ErrNum == 0)
							{
                                System.Diagnostics.Trace.WriteLine("Подключено к серверу (default): " + this.cl.URL);
								int num = 0;
								this.Connect(Program.sL.tmp_listObject.DefaultOPCServer.IpAddressServer, Program.sL.tmp_listObject.DefaultOPCServer.NameServer, out num);
							}
                            else
                                System.Diagnostics.Trace.WriteLine("Не удалось подключиться к OPC-серверу (default)");
						}
					}
				}
				catch (System.Exception ex)
				{
				}
				int second = System.DateTime.Now.Second;
				if (second % 10 == 0)
				{
					if (!this.cl.GetConnectionState())
					{
						int num2;
						this.CheckAndConnect_to_OPC(out num2);
					}
				}
				foreach (System.Data.DataRow dataRow in this.tVals.Rows)
				{
					if (System.Convert.ToString(dataRow["TagType"]) == "F")
					{
						double num3 = this.cl.ReadReal(System.Convert.ToString(dataRow["TagName"]), out flag);
						if (!flag)
						{
							dataRow["Value"] = num3;
						}
						else
						{
							dataRow["Value"] = "Ошибка: " + this.cl.ErrTxt;
						}
					}
					else
					{
						bool flag2 = this.cl.ReadBool(System.Convert.ToString(dataRow["TagName"]), out flag);
						if (!flag)
						{
							if (flag2)
							{
								dataRow["Value"] = 1;
							}
							else
							{
								dataRow["Value"] = 0;
							}
						}
						else
						{
							dataRow["Value"] = "Ошибка: " + this.cl.ErrTxt;
						}
					}
				}
				if (Program.sL.tmp_listObject.ServerIdentifier == "KIASU")
				{
					this.cl.WriteBool("OPC_CLIENT.KIASU_CLIENT_INPUT", true, out flag);
					this.cl.WriteString("OPC_CLIENT.KIASU_CLIENT_LAST_DATE", this.thrDB.LastSuccessTransaction.ToString("F"), out flag);
				}
				if (Program.sL.tmp_listObject.ServerIdentifier == "WEBSERV")
				{
					this.cl.WriteBool("OPC_CLIENT.WEBSERV_CLIENT_INPUT", true, out flag);
					this.cl.WriteString("OPC_CLIENT.WEBSERV_CLIENT_LAST_DATE", this.thrDB.LastSuccessTransaction.ToString("F"), out flag);
				}
			}
			catch (System.Exception ex)
			{
				Program.LogWrite(ex.Message, "UpdateTagValues", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 641);
			}
		}

		private void timer2_Tick(object sender, System.EventArgs e)
		{
			try
			{
				this.currentTime = System.DateTime.Now;
				this.lbl_DateTime.Text = this.currentTime.ToLongDateString() + " " + this.currentTime.ToShortTimeString();
				this.previousBackColor = this.lblError.BackColor;
				if (this.thrDB != null)
				{
					this.lblLastSuccessTransaction.Text = this.thrDB.LastSuccessTransaction.ToString("F");
				}
				if (this.Error != string.Empty)
				{
					this.splitContainer1.Panel2Collapsed = false;
					this.lblError.Visible = true;
					this.lblError.BackColor = ((this.previousBackColor == SystemColors.Control) ? Color.Salmon : SystemColors.Control);
					this.previousBackColor = this.lblError.ForeColor;
				}
				else
				{
					this.splitContainer1.Panel2Collapsed = true;
					this.lblError.Visible = false;
				}
				if (this.autostart)
				{
					this.tsStart = this.tsStart.Subtract(new System.TimeSpan(0, 0, 1));
					this.btn_Start.Text = string.Format("Запуск ({0})", this.tsStart.Seconds);
					if (this.tsStart.Seconds == 0)
					{
						this.btn_Start.Enabled = false;
						this.backgroundWorker1.RunWorkerAsync();
						System.Threading.Thread.Sleep(1000);
						this.btn_Stop.Enabled = true;
						this.autostart = false;
					}
				}
				if (System.DateTime.Now.Second % 59 == 0)
				{
					if (this.isSqlError)
					{
						this.btn_Stop_Click(this.btn_Stop, new System.EventArgs());
						this.autostart = true;
					}
				}
			}
			catch (System.Exception ex)
			{
				Program.LogWrite(ex.Message, "timer2_Tick", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 693);
			}
		}

		private void соединениеСБДNurbaToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			new JoinForm().ShowDialog();
			try
			{
				this.Reconnect();
				if (this.tVals != null)
				{
					this.tVals.Clear();
				}
				this.button3_Click(this.btn_Start, new System.EventArgs());
			}
			catch (System.Exception e2)
			{
				Program.showErrorMessage(e2, this);
			}
		}

		private void источникOPCсерверToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			ChoiceOPCServersForm choiceOPCServersForm = new ChoiceOPCServersForm();
			choiceOPCServersForm.ShowDialog();
		}

		private void chbAutorun_CheckedChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (this.MenuItem_Autorun.Checked)
				{
					Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
					if (registryKey != null)
					{
						registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
						registryKey.SetValue(System.Windows.Forms.Application.ProductName, System.Windows.Forms.Application.ExecutablePath);
						registryKey.Close();
					}
				}
				else
				{
					Microsoft.Win32.RegistryKey registryKey2 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
					registryKey2.DeleteValue(System.Windows.Forms.Application.ProductName, false);
					registryKey2.Close();
				}
			}
			catch (System.Exception ex)
			{
				Program.LogWrite("Ошибка работы с реестром: " + ex.Message, "chbAutorun_CheckedChanged", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 750);
				System.Windows.Forms.MessageBox.Show("Ошибка работы с реестром: " + ex.Message, "Ошибка приложения", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Hand);
			}
		}

		private void SetCheckboxState()
		{
			try
			{
				Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
				if (registryKey != null)
				{
					this.MenuItem_Autorun.Checked = (registryKey.GetValue(System.Windows.Forms.Application.ProductName) != null);
				}
			}
			catch (System.Exception ex)
			{
				Program.LogWrite("Ошибка работы с реестром: " + ex.Message, "SetCheckboxState", "c:\\Users\\root\\Documents\\Visual Studio 2012\\Projects\\OPCClient\\OPCClient\\OPCClientForm.cs", 771);
				System.Windows.Forms.MessageBox.Show("Ошибка работы с реестром: " + ex.Message, "Ошибка приложения", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Hand);
			}
		}

		private void btn_Stop_Click(object sender, System.EventArgs e)
		{
			try
			{
                cancelbtnclick = true;
				this.timer1.Enabled = false;
				this.btn_Stop.Enabled = false;
				this.SetTimeSpanStart();
                if (tVals != null)
                {
                    tVals.Dispose();
                    tVals = null;
                    dataGridView1.DataSource = null;
                }
                if (thrDB != null)
				    this.thrDB.StopAndDisposeWork();
			}
			catch (System.Exception e2)
			{
				Program.showErrorMessage(e2, this);
			}
		}

		private void btnShowLog_Click(object sender, System.EventArgs e)
		{
		}

		private void btnHide_Click(object sender, System.EventArgs e)
		{
			this.Error = "";
		}

		private void OPCClientForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			if (e.CloseReason == System.Windows.Forms.CloseReason.UserClosing)
			{
				base.WindowState = System.Windows.Forms.FormWindowState.Minimized;
				e.Cancel = true;
			}
		}

		private void tsmItemExit_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Application.Exit();
			System.GC.Collect();
		}

		private void TagAdd()
		{
			if (this.tVals != null)
			{
				if (this.tVals.Columns.Count != 0)
				{
					System.Data.DataRow dataRow = this.tVals.NewRow();
					try
					{
						dataRow["TagID"] = this.tVals.Compute("max(TagID) + 1", "");
						dataRow["Code"] = 0;
						dataRow["max"] = 99999999;
						dataRow["TagType"] = "B";
						dataRow["GroupID"] = 0;
						dataRow["Inv"] = 0;
						dataRow["div"] = 1;
						dataRow["min"] = 0;
						this.tVals.Rows.Add(dataRow);
						dataRow.BeginEdit();
						if (new EditTagForm(dataRow, this.cl.server)
						{
							rowIndex = this.dataGridView1.Rows.Count - 1
						}.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							if (this.conn == null)
							{
								this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
							}
							else if (this.conn.ConnectionString == null)
							{
								this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
							}
							else if (this.conn.ConnectionString == "")
							{
								this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
							}
							this.conn.Open();
							using (this.conn)
							{
								System.Data.DataTable changes = dataRow.Table.GetChanges();
								this.da.InsertCommand.Connection = this.conn;
								this.InitSqlCommandParameters(this.da.InsertCommand, dataRow);
								this.da.InsertCommand.ExecuteNonQuery();
								dataRow.Table.AcceptChanges();
							}
							Items items = this.cl.GetItems();
							int num = 0;
							items.Add(System.Convert.ToString(dataRow["TagName"]), System.Convert.ToString(dataRow["TagName"]), TypeRW.Read, out num);
						}
						else
						{
							dataRow.Table.RejectChanges();
						}
					}
					catch (System.Exception e)
					{
						dataRow.Table.RejectChanges();
						Program.showErrorMessage(e, this);
					}
				}
			}
		}

		private void TagEdit()
		{
			if (this.tVals != null)
			{
				if (this.tVals.Columns.Count != 0)
				{
					if (this.dataGridView1.CurrentRow != null)
					{
						if (this.dataGridView1.CurrentRow.DataBoundItem is System.Data.DataRowView)
						{
							System.Data.DataRow row = (this.dataGridView1.CurrentRow.DataBoundItem as System.Data.DataRowView).Row;
							try
							{
								row.BeginEdit();
								EditTagForm editTagForm = new EditTagForm(row, this.cl.server);
								string itemName = System.Convert.ToString(row["TagName"]);
								editTagForm.rowIndex = this.dataGridView1.CurrentRow.Index;
								if (editTagForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
								{
									if (this.conn == null)
									{
										this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
									}
									else if (this.conn.ConnectionString == null)
									{
										this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
									}
									else if (this.conn.ConnectionString == "")
									{
										this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
									}
									this.conn.Open();
									using (this.conn)
									{
										this.InitSqlCommandParameters(this.da.UpdateCommand, row);
										this.da.UpdateCommand.Connection = this.conn;
										this.da.UpdateCommand.ExecuteNonQuery();
										row.Table.AcceptChanges();
									}
									Items items = this.cl.GetItems();
									int num = 0;
									items.RemoveByName(itemName, TypeRW.Read, out num);
									items.Add(System.Convert.ToString(row["TagName"]), System.Convert.ToString(row["TagName"]), TypeRW.Read, out num);
								}
								else
								{
									row.Table.RejectChanges();
								}
							}
							catch (System.Exception e)
							{
								row.Table.RejectChanges();
								Program.showErrorMessage(e, this);
							}
						}
					}
				}
			}
		}

		private void TagDelete()
		{
			if (this.tVals != null)
			{
				if (this.tVals.Columns.Count != 0)
				{
					if (this.dataGridView1.CurrentRow != null)
					{
						if (this.dataGridView1.CurrentRow.DataBoundItem is System.Data.DataRowView)
						{
							System.Data.DataRow row = (this.dataGridView1.CurrentRow.DataBoundItem as System.Data.DataRowView).Row;
							try
							{
								if (System.Windows.Forms.MessageBox.Show(string.Format("Удалить тэг '{0}'?", row["TagName"]), "Внимание!", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
								{
									if (this.conn == null)
									{
										this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
									}
									else if (this.conn.ConnectionString == null)
									{
										this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
									}
									else if (this.conn.ConnectionString == "")
									{
										this.conn = new System.Data.SqlClient.SqlConnection(this.sb.ToString());
									}
									this.conn.Open();
									using (this.conn)
									{
										this.da.DeleteCommand.Parameters.Clear();
										this.da.DeleteCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("TagID", row["TagID"]));
										this.da.DeleteCommand.Connection = this.conn;
										this.da.DeleteCommand.ExecuteNonQuery();
										row.Delete();
										row.Table.AcceptChanges();
									}
								}
							}
							catch (System.Exception e)
							{
								row.Table.RejectChanges();
								Program.showErrorMessage(e, this);
							}
						}
					}
				}
			}
		}

		private void dataGridView1_DoubleClick(object sender, System.EventArgs e)
		{
			this.TagEdit();
		}

		private void добавитьToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			this.TagAdd();
		}

		private void редактироватьToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			this.TagEdit();
		}

		private void удалитьToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			this.TagDelete();
		}

		private void dataGridView1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == System.Windows.Forms.Keys.Return)
			{
				this.TagEdit();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OPCClientForm));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_Start = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new OPCClient.MyDataGridView();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this._notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.настройкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.соединениеСБДNurbaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.источникOPCсерверToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Autorun = new System.Windows.Forms.ToolStripMenuItem();
            this.таблицаТеговToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.добавитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.редактироватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.справкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbl_DataBase = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbl_OPCServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblLastSuccessTransaction = new System.Windows.Forms.ToolStripStatusLabel();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.lboxError = new System.Windows.Forms.ListBox();
            this.btnHide = new System.Windows.Forms.Button();
            this.btnShowLog = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblError = new System.Windows.Forms.Label();
            this.lbl_DateTime = new System.Windows.Forms.Label();
            this.timer_reconnect = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuNotify.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(475, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(556, 30);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(9, 30);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(109, 23);
            this.btn_Start.TabIndex = 3;
            this.btn_Start.Text = "Запуск";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(678, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1183, 420);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.DoubleClick += new System.EventHandler(this.dataGridView1_DoubleClick);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // _notifyIcon
            // 
            this._notifyIcon.ContextMenuStrip = this.contextMenuNotify;
            this._notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("_notifyIcon.Icon")));
            this._notifyIcon.Text = "OPC-клиент";
            this._notifyIcon.Visible = true;
            this._notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this._notifyIcon_MouseClick);
            // 
            // contextMenuNotify
            // 
            this.contextMenuNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmItemExit});
            this.contextMenuNotify.Name = "contextMenuNotify";
            this.contextMenuNotify.Size = new System.Drawing.Size(109, 26);
            // 
            // tsmItemExit
            // 
            this.tsmItemExit.Name = "tsmItemExit";
            this.tsmItemExit.Size = new System.Drawing.Size(108, 22);
            this.tsmItemExit.Text = "Выход";
            this.tsmItemExit.Click += new System.EventHandler(this.tsmItemExit_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкаToolStripMenuItem,
            this.справкаToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1204, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // настройкаToolStripMenuItem
            // 
            this.настройкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.соединениеСБДNurbaToolStripMenuItem,
            this.источникOPCсерверToolStripMenuItem,
            this.MenuItem_Autorun,
            this.таблицаТеговToolStripMenuItem});
            this.настройкаToolStripMenuItem.Name = "настройкаToolStripMenuItem";
            this.настройкаToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.настройкаToolStripMenuItem.Text = "Настройка";
            // 
            // соединениеСБДNurbaToolStripMenuItem
            // 
            this.соединениеСБДNurbaToolStripMenuItem.Name = "соединениеСБДNurbaToolStripMenuItem";
            this.соединениеСБДNurbaToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.соединениеСБДNurbaToolStripMenuItem.Text = "Соединение с БД (КИАСУ)";
            this.соединениеСБДNurbaToolStripMenuItem.Click += new System.EventHandler(this.соединениеСБДNurbaToolStripMenuItem_Click);
            // 
            // источникOPCсерверToolStripMenuItem
            // 
            this.источникOPCсерверToolStripMenuItem.Name = "источникOPCсерверToolStripMenuItem";
            this.источникOPCсерверToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.источникOPCсерверToolStripMenuItem.Text = "Источник (OPC-сервер)";
            this.источникOPCсерверToolStripMenuItem.Click += new System.EventHandler(this.источникOPCсерверToolStripMenuItem_Click);
            // 
            // MenuItem_Autorun
            // 
            this.MenuItem_Autorun.CheckOnClick = true;
            this.MenuItem_Autorun.Name = "MenuItem_Autorun";
            this.MenuItem_Autorun.Size = new System.Drawing.Size(218, 22);
            this.MenuItem_Autorun.Text = "Автозапуск";
            this.MenuItem_Autorun.CheckedChanged += new System.EventHandler(this.chbAutorun_CheckedChanged);
            // 
            // таблицаТеговToolStripMenuItem
            // 
            this.таблицаТеговToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьToolStripMenuItem,
            this.редактироватьToolStripMenuItem,
            this.удалитьToolStripMenuItem});
            this.таблицаТеговToolStripMenuItem.Enabled = false;
            this.таблицаТеговToolStripMenuItem.Name = "таблицаТеговToolStripMenuItem";
            this.таблицаТеговToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.таблицаТеговToolStripMenuItem.Text = "Таблица тегов";
            // 
            // добавитьToolStripMenuItem
            // 
            this.добавитьToolStripMenuItem.Name = "добавитьToolStripMenuItem";
            this.добавитьToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Insert)));
            this.добавитьToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.добавитьToolStripMenuItem.Text = "Добавить";
            this.добавитьToolStripMenuItem.Click += new System.EventHandler(this.добавитьToolStripMenuItem_Click);
            // 
            // редактироватьToolStripMenuItem
            // 
            this.редактироватьToolStripMenuItem.Name = "редактироватьToolStripMenuItem";
            this.редактироватьToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.редактироватьToolStripMenuItem.Text = "Редактировать         Enter";
            this.редактироватьToolStripMenuItem.Click += new System.EventHandler(this.редактироватьToolStripMenuItem_Click);
            // 
            // удалитьToolStripMenuItem
            // 
            this.удалитьToolStripMenuItem.Name = "удалитьToolStripMenuItem";
            this.удалитьToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.удалитьToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.удалитьToolStripMenuItem.Text = "Удалить";
            this.удалитьToolStripMenuItem.Click += new System.EventHandler(this.удалитьToolStripMenuItem_Click);
            // 
            // справкаToolStripMenuItem
            // 
            this.справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            this.справкаToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.справкаToolStripMenuItem.Text = "Справка";
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lbl_DataBase,
            this.toolStripStatusLabel3,
            this.lbl_OPCServer,
            this.toolStripStatusLabel2,
            this.lblLastSuccessTransaction});
            this.statusStrip1.Location = new System.Drawing.Point(0, 579);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1204, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(77, 17);
            this.toolStripStatusLabel1.Text = "База данных:";
            // 
            // lbl_DataBase
            // 
            this.lbl_DataBase.ForeColor = System.Drawing.Color.Maroon;
            this.lbl_DataBase.Name = "lbl_DataBase";
            this.lbl_DataBase.Size = new System.Drawing.Size(74, 17);
            this.lbl_DataBase.Text = "База данных";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(83, 17);
            this.toolStripStatusLabel3.Text = "OPC - сервер:";
            // 
            // lbl_OPCServer
            // 
            this.lbl_OPCServer.ForeColor = System.Drawing.Color.Maroon;
            this.lbl_OPCServer.Name = "lbl_OPCServer";
            this.lbl_OPCServer.Size = new System.Drawing.Size(80, 17);
            this.lbl_OPCServer.Text = "OPC - сервер";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(195, 17);
            this.toolStripStatusLabel2.Text = "Предыдущая запись произведена:";
            // 
            // lblLastSuccessTransaction
            // 
            this.lblLastSuccessTransaction.ForeColor = System.Drawing.Color.Maroon;
            this.lblLastSuccessTransaction.Name = "lblLastSuccessTransaction";
            this.lblLastSuccessTransaction.Size = new System.Drawing.Size(71, 17);
            this.lblLastSuccessTransaction.Text = "Неизвестно";
            // 
            // btn_Stop
            // 
            this.btn_Stop.Enabled = false;
            this.btn_Stop.Location = new System.Drawing.Point(124, 30);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(109, 23);
            this.btn_Stop.TabIndex = 11;
            this.btn_Stop.Text = "Останов";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // lboxError
            // 
            this.lboxError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lboxError.BackColor = System.Drawing.SystemColors.Control;
            this.lboxError.FormattingEnabled = true;
            this.lboxError.HorizontalScrollbar = true;
            this.lboxError.Location = new System.Drawing.Point(3, 4);
            this.lboxError.Name = "lboxError";
            this.lboxError.ScrollAlwaysVisible = true;
            this.lboxError.Size = new System.Drawing.Size(1093, 82);
            this.lboxError.TabIndex = 12;
            // 
            // btnHide
            // 
            this.btnHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHide.Location = new System.Drawing.Point(1099, 4);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(84, 23);
            this.btnHide.TabIndex = 13;
            this.btnHide.Text = "Квитировать";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // btnShowLog
            // 
            this.btnShowLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowLog.Location = new System.Drawing.Point(1096, 52);
            this.btnShowLog.Name = "btnShowLog";
            this.btnShowLog.Size = new System.Drawing.Size(84, 34);
            this.btnShowLog.TabIndex = 14;
            this.btnShowLog.Text = "Показать весь журнал";
            this.btnShowLog.UseVisualStyleBackColor = true;
            this.btnShowLog.Click += new System.EventHandler(this.btnShowLog_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(9, 59);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lboxError);
            this.splitContainer1.Panel2.Controls.Add(this.btnShowLog);
            this.splitContainer1.Panel2.Controls.Add(this.btnHide);
            this.splitContainer1.Size = new System.Drawing.Size(1183, 517);
            this.splitContainer1.SplitterDistance = 420;
            this.splitContainer1.TabIndex = 15;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.BackColor = System.Drawing.Color.Salmon;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblError.Location = new System.Drawing.Point(1121, 33);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(71, 17);
            this.lblError.TabIndex = 16;
            this.lblError.Text = "Ошибка!";
            this.lblError.Visible = false;
            // 
            // lbl_DateTime
            // 
            this.lbl_DateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_DateTime.AutoSize = true;
            this.lbl_DateTime.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.lbl_DateTime.Location = new System.Drawing.Point(1049, 6);
            this.lbl_DateTime.Name = "lbl_DateTime";
            this.lbl_DateTime.Size = new System.Drawing.Size(90, 13);
            this.lbl_DateTime.TabIndex = 17;
            this.lbl_DateTime.Text = "Время в накыне";
            // 
            // timer_reconnect
            // 
            this.timer_reconnect.Interval = 1000;
            this.timer_reconnect.Tick += new System.EventHandler(this.timer_reconnect_Tick);
            // 
            // OPCClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 601);
            this.Controls.Add(this.lbl_DateTime);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "OPCClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "АК \"Алроса\". ОФ №16. OPC-клиент";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OPCClientForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.OPCClientForm_Load);
            this.Resize += new System.EventHandler(this.OPCClientForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuNotify.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        private void timer_reconnect_Tick(object sender, EventArgs e)
        {

        }
	}
}
