using System;
using System.Windows.Forms;

namespace OPCClient
{
	internal class MyDataGridView : System.Windows.Forms.DataGridView
	{
		public MyDataGridView()
		{
			this.DoubleBuffered = true;
		}
	}
}
