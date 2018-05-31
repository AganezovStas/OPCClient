using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OPCClient
{
	public class ChoiceTagsForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.Label label1;

		public ChoiceTagsForm()
		{
			this.InitializeComponent();
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(ChoiceTagsForm));
			this.label1 = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.label1.Location = new Point(138, 117);
			this.label1.Name = "label1";
			this.label1.Size = new Size(296, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Данная фича на данный момент в разработке...";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new Size(582, 283);
			base.Controls.Add(this.label1);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ChoiceTagsForm";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Tag explorer - обзор тегов сервера";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
