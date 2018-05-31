using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

namespace OPCClient
{
	internal class WorkThread
	{
		private System.Threading.Timer timer;

		private System.Threading.TimerCallback tm_WorkThread_Callback;

		private System.TimeSpan tsStart;

		private System.TimeSpan tsPeriod;

		private System.Data.DataTable _tVals;

		private System.DateTime lastSuccessTransaction;

		private System.Data.SqlClient.SqlConnection conn;

		private System.Data.SqlClient.SqlConnectionStringBuilder sb;

		private System.Data.SqlClient.SqlCommand cmd;

		private System.Data.SqlClient.SqlTransaction tr;

		private System.Threading.WaitHandle wh;

		public System.TimeSpan StartTime
		{
			get
			{
				return this.tsStart;
			}
		}

		public System.TimeSpan PeriodTime
		{
			get
			{
				return this.tsPeriod;
			}
		}

		public System.DateTime LastSuccessTransaction
		{
			get
			{
				return this.lastSuccessTransaction;
			}
		}

		public WorkThread(System.Data.DataTable tVals)
		{
			this.lastSuccessTransaction = System.DateTime.MinValue;
			this._tVals = tVals;
			this.InitSql();
			this.tsPeriod = new System.TimeSpan(600000000L);
			this.tm_WorkThread_Callback = new System.Threading.TimerCallback(this.tm_workthread_proc);
			this.CalcStart();
			this.timer = new System.Threading.Timer(this.tm_WorkThread_Callback, this, this.tsStart, this.tsPeriod);
			System.Console.WriteLine("Запуск задачи произойдет через {0}", this.tsStart);
		}

		private void InitSql()
		{
			this.sb = new System.Data.SqlClient.SqlConnectionStringBuilder();
			this.sb.DataSource = Program.sL.tmp_listObject.DataBase.DataSource;
			this.sb.InitialCatalog = Program.sL.tmp_listObject.DataBase.InitialCatalog;
			this.sb.UserID = Program.sL.tmp_listObject.DataBase.UserID;
			this.sb.Password = Program.sL.tmp_listObject.DataBase.Password;
			this.sb.IntegratedSecurity = Program.sL.tmp_listObject.DataBase.IntegratedSecurity;
			this.cmd = new System.Data.SqlClient.SqlCommand("Insert into Vals ([time],[tag_id],[val]) \r\n            Values (@time,@tag_id,@val)");
			this.cmd.Parameters.Add("time", System.Data.SqlDbType.DateTime);
			this.cmd.Parameters.Add("tag_id", System.Data.SqlDbType.Int);
			this.cmd.Parameters.Add("val", System.Data.SqlDbType.Real);
		}

		private void CalcStart()
		{
			System.DateTime dateTime = System.DateTime.Now.AddMinutes(1.0);
			System.DateTime dateTime2 = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0);
			this.tsStart = dateTime2.Subtract(System.DateTime.Now);
		}

		protected virtual void tm_workthread_proc(object state)
		{
			System.DateTime now = System.DateTime.Now;
			string value = now.ToShortTimeString();
			System.Console.WriteLine("{0} Запись в БД -------------------------", now);
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
			try
			{
				this.conn.Open();
				if (this.cmd == null)
				{
					this.InitSql();
				}
				using (this.conn)
				{
					this.tr = this.conn.BeginTransaction();
					this.cmd.Connection = this.conn;
					this.cmd.Transaction = this.tr;
					foreach (System.Data.DataRow dataRow in this._tVals.Rows)
					{
						try
						{
							if (System.Convert.ToString(dataRow["Value"]).Contains("Ошибка:"))
							{
								throw new System.Exception(System.Convert.ToString(dataRow["Value"]).Replace("Ошибка:", ""));
							}
							this.cmd.Parameters["time"].Value = value;
							this.cmd.Parameters["tag_id"].Value = dataRow["TagId"];
							this.cmd.Parameters["val"].Value = dataRow["Value"];
							this.cmd.ExecuteNonQuery();
						}
						catch (System.Exception ex)
						{
							System.Diagnostics.Trace.WriteLine(string.Format("Ошибка записи в БД: {0}. Тег {1}:{2}", ex.Message, dataRow["TagId"], dataRow["TagName"]));
						}
					}
					this.tr.Commit();
					this.lastSuccessTransaction = now;
					System.Console.WriteLine("Завершение транзакции -------------------", System.DateTime.Now);
				}
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
			}
		}

		public void StopAndDisposeWork()
		{
			this.timer.Dispose();
			this.sb = null;
            if (cmd != null)
            {
                this.cmd.Dispose();
                this.cmd = null;
            }
			this.tm_WorkThread_Callback = null;
		}
	}
}
