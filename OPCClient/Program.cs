using Microsoft.SqlServer.MessageBox;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace OPCClient
{
	internal static class Program
	{
		public delegate void methodContainer(string message);

		public static SettingsLoader sL;

		private static string logDirectory;

		private static string logFileName;

		private static MyTextWriterTraceListener myTextListener;

		public static event Program.methodContainer onError;

		public static string LogDirectory
		{
			get
			{
				return Program.logDirectory;
			}
		}

		public static string LogFileName
		{
			get
			{
				return Program.logFileName;
			}
		}

		private static bool IsSingleInstance()
		{
			bool result;
			try
			{
				System.Threading.Mutex.OpenExisting("DBMonitor_mutex");
			}
			catch
			{
				System.Threading.Mutex mutex = new System.Threading.Mutex(true, "DBMonitor_mutex");
				result = true;
				return result;
			}
			result = false;
			return result;
		}

		[System.STAThread]
		private static void Main()
		{
			if (Program.IsSingleInstance())
			{
				System.Windows.Forms.Application.EnableVisualStyles();
				System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
				Program.logDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
				Program.logDirectory = string.Format("{0}{1}log", Program.logDirectory, System.IO.Path.DirectorySeparatorChar);
				if (!System.IO.Directory.Exists(Program.logDirectory))
				{
					System.IO.Directory.CreateDirectory(Program.logDirectory);
				}
				Program.logFileName = System.IO.Path.Combine(Program.logDirectory, string.Format("{0:yyyy.MM.dd}_{0:HH-mm}_OPCClient.log", System.DateTime.Now));
				string message = "";
				try
				{
					System.IO.FileStream stream = new System.IO.FileStream(Program.logFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
					Program.myTextListener = new MyTextWriterTraceListener(stream, "");
					System.Diagnostics.Trace.Listeners.Add(Program.myTextListener);
					message = "Создание лог-файла...";
				}
				catch (System.Exception ex)
				{
					Program.myTextListener = new MyTextWriterTraceListener();
					System.Diagnostics.Trace.Listeners.Add(Program.myTextListener);
					message = string.Format("Ошибка при создании лог-файла: " + ex.Message, new object[0]);
				}
				System.Diagnostics.TextWriterTraceListener listener = new System.Diagnostics.TextWriterTraceListener(System.Console.Out);
				System.Diagnostics.Trace.Listeners.Add(listener);
				System.Diagnostics.Trace.WriteLine(message);
				System.Diagnostics.Trace.AutoFlush = true;
				System.Diagnostics.Trace.UseGlobalLock = false;
				while (true)
				{
					try
					{
						Program.sL = new SettingsLoader();
						System.Windows.Forms.Application.Run(new OPCClientForm());
					}
					catch (System.Exception ex)
					{
						if (System.Windows.Forms.MessageBox.Show(string.Format("Ошибка: {0}. Приложение будет закрыто. Перезапустить приложение?", ex.Message), "Ошибка", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Hand) == System.Windows.Forms.DialogResult.Yes)
						{
							continue;
						}
					}
					break;
				}
			}
		}

		public static void showErrorMessage(System.Exception e, System.Windows.Forms.Form f)
		{
			new ExceptionMessageBox(e)
			{
				Caption = "Ошибка",
				InnerException = e.InnerException,
				Buttons = ExceptionMessageBoxButtons.OK,
				Symbol = ExceptionMessageBoxSymbol.Error
			}.Show(f);
		}

		public static void showErrorMessage(string text)
		{
			System.Windows.Forms.MessageBox.Show(text, "Ошибка", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Hand);
		}

		public static void LogWrite(string text, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		{
			System.Diagnostics.Trace.Flush();
			string message = string.Format("{0}. Ошибка возникла: {1} (строка {2})", text, filePath, lineNumber);
			Program.onError(message);
		}
	}
}
