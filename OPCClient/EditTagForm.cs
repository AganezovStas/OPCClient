using Opc.Da;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OPCClient
{
	public class EditTagForm : System.Windows.Forms.Form
	{
		private System.Data.DataRow row;

		public int rowIndex = 0;

		private Server m_server;

		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.Label label1;

		private System.Windows.Forms.Label label2;

		private System.Windows.Forms.Label label3;

		private System.Windows.Forms.Label label4;

		private System.Windows.Forms.Label label5;

		private System.Windows.Forms.Label label6;

		private System.Windows.Forms.Label label7;

		private System.Windows.Forms.NumericUpDown ntb_TagID;

		private System.Windows.Forms.TextBox tb_TagName;

		private System.Windows.Forms.ComboBox cb_TagType;

		private System.Windows.Forms.TextBox tb_Description;

		private System.Windows.Forms.TextBox tb_Note;

		private System.Windows.Forms.NumericUpDown ntb_Code;

		private System.Windows.Forms.NumericUpDown ntb_Max;

		private System.Windows.Forms.GroupBox groupBox1;

		private System.Windows.Forms.Button btn_OK;

		private System.Windows.Forms.Button btn_Cancel;

		private System.Windows.Forms.Button btn_Choice;

		private System.Windows.Forms.Label lbl_TagType;

		public EditTagForm(Server server)
		{
			this.InitializeComponent();
			this.m_server = server;
		}

		public EditTagForm(System.Data.DataRow row, Server server)
		{
			this.InitializeComponent();
			this.row = row;
			this.m_server = server;
		}

		private void EditTagForm_Load(object sender, System.EventArgs e)
		{
			try
			{
				this.ntb_TagID.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.row.Table, "TagID", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, 0));
				this.tb_TagName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.row.Table, "TagName", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, ""));
				this.cb_TagType.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.row.Table, "TagType", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, "B"));
				this.tb_Description.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.row.Table, "Description", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, ""));
				this.tb_Note.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.row.Table, "Note", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, ""));
				this.ntb_Code.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.row.Table, "Code", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, 0));
				this.ntb_Max.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.row.Table, "Max", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, 0));
				System.Windows.Forms.BindingManagerBase bindingManagerBase = this.ntb_TagID.BindingContext[this.row.Table];
				bindingManagerBase.Position = this.rowIndex;
			}
			catch (System.Exception e2)
			{
				Program.showErrorMessage(e2, this);
			}
		}

		private void cb_TagType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.cb_TagType.SelectedIndex == 0)
			{
				this.lbl_TagType.Text = "(Bit) - битовые значения";
			}
			else
			{
				this.lbl_TagType.Text = "(Float) - аналоговые значения";
			}
		}

		private void btn_Choice_Click(object sender, System.EventArgs e)
		{
			if (this.m_server == null)
			{
				Program.showErrorMessage("OPC-сервер не инициализирован");
			}
			else
			{
				BrowseItemsDlg browseItemsDlg = new BrowseItemsDlg();
				browseItemsDlg.tag = this.tb_TagName.Text;
				if (System.Windows.Forms.DialogResult.OK == browseItemsDlg.Initialize(this.m_server))
				{
					this.tb_TagName.Text = browseItemsDlg.Element.ItemName;
					this.cb_TagType.SelectedIndex = ((browseItemsDlg.Element.Properties[9].Value.ToString() == "Binary Tag") ? 0 : 1);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditTagForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ntb_TagID = new System.Windows.Forms.NumericUpDown();
            this.tb_TagName = new System.Windows.Forms.TextBox();
            this.cb_TagType = new System.Windows.Forms.ComboBox();
            this.tb_Description = new System.Windows.Forms.TextBox();
            this.tb_Note = new System.Windows.Forms.TextBox();
            this.ntb_Code = new System.Windows.Forms.NumericUpDown();
            this.ntb_Max = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_TagType = new System.Windows.Forms.Label();
            this.btn_Choice = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ntb_TagID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntb_Code)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntb_Max)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Идентификатор (TagID):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Имя тега (TagName):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Тип тега (TagType):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(165, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Краткое описание (Description):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Полное описание (Note):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Зависимый тег (Code):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 211);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Макс. значение (Max):";
            // 
            // ntb_TagID
            // 
            this.ntb_TagID.Increment = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ntb_TagID.Location = new System.Drawing.Point(179, 16);
            this.ntb_TagID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ntb_TagID.Name = "ntb_TagID";
            this.ntb_TagID.ReadOnly = true;
            this.ntb_TagID.Size = new System.Drawing.Size(120, 20);
            this.ntb_TagID.TabIndex = 2;
            // 
            // tb_TagName
            // 
            this.tb_TagName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_TagName.Location = new System.Drawing.Point(179, 42);
            this.tb_TagName.Name = "tb_TagName";
            this.tb_TagName.Size = new System.Drawing.Size(276, 20);
            this.tb_TagName.TabIndex = 3;
            // 
            // cb_TagType
            // 
            this.cb_TagType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_TagType.FormattingEnabled = true;
            this.cb_TagType.Items.AddRange(new object[] {
            "B",
            "F"});
            this.cb_TagType.Location = new System.Drawing.Point(179, 68);
            this.cb_TagType.Name = "cb_TagType";
            this.cb_TagType.Size = new System.Drawing.Size(120, 21);
            this.cb_TagType.TabIndex = 4;
            this.cb_TagType.SelectedIndexChanged += new System.EventHandler(this.cb_TagType_SelectedIndexChanged);
            // 
            // tb_Description
            // 
            this.tb_Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_Description.Location = new System.Drawing.Point(179, 95);
            this.tb_Description.Multiline = true;
            this.tb_Description.Name = "tb_Description";
            this.tb_Description.Size = new System.Drawing.Size(356, 38);
            this.tb_Description.TabIndex = 5;
            // 
            // tb_Note
            // 
            this.tb_Note.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_Note.Location = new System.Drawing.Point(179, 139);
            this.tb_Note.Multiline = true;
            this.tb_Note.Name = "tb_Note";
            this.tb_Note.Size = new System.Drawing.Size(356, 38);
            this.tb_Note.TabIndex = 6;
            // 
            // ntb_Code
            // 
            this.ntb_Code.Location = new System.Drawing.Point(179, 183);
            this.ntb_Code.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ntb_Code.Name = "ntb_Code";
            this.ntb_Code.Size = new System.Drawing.Size(120, 20);
            this.ntb_Code.TabIndex = 7;
            // 
            // ntb_Max
            // 
            this.ntb_Max.Location = new System.Drawing.Point(179, 209);
            this.ntb_Max.Maximum = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.ntb_Max.Name = "ntb_Max";
            this.ntb_Max.Size = new System.Drawing.Size(120, 20);
            this.ntb_Max.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_TagType);
            this.groupBox1.Controls.Add(this.btn_Choice);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(541, 239);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Основные параметры тега";
            // 
            // lbl_TagType
            // 
            this.lbl_TagType.AutoSize = true;
            this.lbl_TagType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_TagType.Location = new System.Drawing.Point(305, 71);
            this.lbl_TagType.Name = "lbl_TagType";
            this.lbl_TagType.Size = new System.Drawing.Size(30, 13);
            this.lbl_TagType.TabIndex = 1;
            this.lbl_TagType.Text = "(тип)";
            // 
            // btn_Choice
            // 
            this.btn_Choice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Choice.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btn_Choice.Location = new System.Drawing.Point(461, 40);
            this.btn_Choice.Name = "btn_Choice";
            this.btn_Choice.Size = new System.Drawing.Size(75, 23);
            this.btn_Choice.TabIndex = 0;
            this.btn_Choice.Text = "Выбрать";
            this.btn_Choice.UseVisualStyleBackColor = true;
            this.btn_Choice.Click += new System.EventHandler(this.btn_Choice_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(380, 242);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 10;
            this.btn_OK.Text = "ОК";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(461, 242);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 11;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // EditTagForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 269);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.ntb_Max);
            this.Controls.Add(this.ntb_Code);
            this.Controls.Add(this.tb_Note);
            this.Controls.Add(this.tb_Description);
            this.Controls.Add(this.cb_TagType);
            this.Controls.Add(this.tb_TagName);
            this.Controls.Add(this.ntb_TagID);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditTagForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Добавление/редактирование тега";
            this.Load += new System.EventHandler(this.EditTagForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ntb_TagID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntb_Code)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntb_Max)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
