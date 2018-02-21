namespace cdcrush.forms
{
	partial class FormRestoreOLD
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btn_RESTORE = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.info_status = new System.Windows.Forms.RichTextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tableLayoutPanel4.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_RESTORE
			// 
			this.btn_RESTORE.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btn_RESTORE.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btn_RESTORE.ForeColor = System.Drawing.Color.ForestGreen;
			this.btn_RESTORE.Location = new System.Drawing.Point(3, 3);
			this.btn_RESTORE.Name = "btn_RESTORE";
			this.btn_RESTORE.Size = new System.Drawing.Size(105, 24);
			this.btn_RESTORE.TabIndex = 4;
			this.btn_RESTORE.Text = "ACTION";
			this.btn_RESTORE.UseVisualStyleBackColor = true;
			this.btn_RESTORE.Click += new System.EventHandler(this.btn_RESTORE_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(9, 143);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 10;
			this.button1.Text = "TESTS";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// progressBar1
			// 
			this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.progressBar1.Location = new System.Drawing.Point(114, 3);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(399, 24);
			this.progressBar1.TabIndex = 12;
			this.progressBar1.Value = 50;
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel4.ColumnCount = 2;
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 111F));
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.Controls.Add(this.btn_RESTORE, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this.progressBar1, 1, 0);
			this.tableLayoutPanel4.Location = new System.Drawing.Point(9, 357);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 1;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.Size = new System.Drawing.Size(516, 30);
			this.tableLayoutPanel4.TabIndex = 14;
			// 
			// info_status
			// 
			this.info_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.info_status.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.info_status.Location = new System.Drawing.Point(9, 393);
			this.info_status.Name = "info_status";
			this.info_status.ReadOnly = true;
			this.info_status.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.info_status.Size = new System.Drawing.Size(516, 15);
			this.info_status.TabIndex = 15;
			this.info_status.Text = "Ready.";
			this.info_status.WordWrap = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(9, 13);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(513, 124);
			this.panel1.TabIndex = 16;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "CDCRUSH";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label2.Location = new System.Drawing.Point(97, 5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "version 1.3";
			// 
			// FormRestore
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(537, 410);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.info_status);
			this.Controls.Add(this.tableLayoutPanel4);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "FormRestore";
			this.Text = "CDCRUSH Restore";
			this.Load += new System.EventHandler(this.FormRestore_Load);
			this.tableLayoutPanel4.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btn_RESTORE;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private System.Windows.Forms.RichTextBox info_status;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}