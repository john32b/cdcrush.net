namespace cdcrush.forms
{
	partial class FormMain
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label6;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.info_status = new System.Windows.Forms.RichTextBox();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabRestore = new System.Windows.Forms.TabPage();
			this.panelRestore1 = new cdcrush.forms.PanelRestore();
			this.tabCrush = new System.Windows.Forms.TabPage();
			this.panelCompress1 = new cdcrush.forms.PanelCompress();
			this.tabSettings = new System.Windows.Forms.TabPage();
			this.tabInfo = new System.Windows.Forms.TabPage();
			this.link_web = new System.Windows.Forms.LinkLabel();
			this.info_ver = new System.Windows.Forms.Label();
			this.info_tempFolder = new System.Windows.Forms.TextBox();
			this.check_tempDef = new System.Windows.Forms.CheckBox();
			this.btn_selectTemp = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabRestore.SuspendLayout();
			this.tabCrush.SuspendLayout();
			this.tabSettings.SuspendLayout();
			this.tabInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// info_status
			// 
			this.info_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.info_status.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.info_status.Location = new System.Drawing.Point(12, 361);
			this.info_status.Name = "info_status";
			this.info_status.ReadOnly = true;
			this.info_status.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.info_status.Size = new System.Drawing.Size(462, 15);
			this.info_status.TabIndex = 19;
			this.info_status.Text = "Ready.";
			this.info_status.WordWrap = false;
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(12, 334);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(458, 24);
			this.progressBar1.TabIndex = 12;
			this.progressBar1.Value = 50;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.tabRestore);
			this.tabControl1.Controls.Add(this.tabCrush);
			this.tabControl1.Controls.Add(this.tabSettings);
			this.tabControl1.Controls.Add(this.tabInfo);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(462, 322);
			this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabControl1.TabIndex = 20;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabRestore
			// 
			this.tabRestore.Controls.Add(this.panelRestore1);
			this.tabRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabRestore.Location = new System.Drawing.Point(4, 25);
			this.tabRestore.Name = "tabRestore";
			this.tabRestore.Padding = new System.Windows.Forms.Padding(3);
			this.tabRestore.Size = new System.Drawing.Size(454, 293);
			this.tabRestore.TabIndex = 0;
			this.tabRestore.Text = "Restore a CD";
			this.tabRestore.UseVisualStyleBackColor = true;
			// 
			// panelRestore1
			// 
			this.panelRestore1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelRestore1.Location = new System.Drawing.Point(3, 3);
			this.panelRestore1.MaximumSize = new System.Drawing.Size(600, 280);
			this.panelRestore1.MinimumSize = new System.Drawing.Size(450, 280);
			this.panelRestore1.Name = "panelRestore1";
			this.panelRestore1.Size = new System.Drawing.Size(450, 280);
			this.panelRestore1.TabIndex = 0;
			// 
			// tabCrush
			// 
			this.tabCrush.Controls.Add(this.panelCompress1);
			this.tabCrush.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabCrush.Location = new System.Drawing.Point(4, 25);
			this.tabCrush.Name = "tabCrush";
			this.tabCrush.Padding = new System.Windows.Forms.Padding(3);
			this.tabCrush.Size = new System.Drawing.Size(454, 293);
			this.tabCrush.TabIndex = 1;
			this.tabCrush.Text = "Compress a CD";
			this.tabCrush.UseVisualStyleBackColor = true;
			// 
			// panelCompress1
			// 
			this.panelCompress1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelCompress1.Location = new System.Drawing.Point(3, 3);
			this.panelCompress1.MaximumSize = new System.Drawing.Size(600, 280);
			this.panelCompress1.MinimumSize = new System.Drawing.Size(450, 280);
			this.panelCompress1.Name = "panelCompress1";
			this.panelCompress1.Size = new System.Drawing.Size(450, 280);
			this.panelCompress1.TabIndex = 0;
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.btn_selectTemp);
			this.tabSettings.Controls.Add(this.check_tempDef);
			this.tabSettings.Controls.Add(label6);
			this.tabSettings.Controls.Add(this.info_tempFolder);
			this.tabSettings.Location = new System.Drawing.Point(4, 25);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.Size = new System.Drawing.Size(454, 293);
			this.tabSettings.TabIndex = 2;
			this.tabSettings.Text = "Settings";
			this.tabSettings.UseVisualStyleBackColor = true;
			// 
			// tabInfo
			// 
			this.tabInfo.Controls.Add(label7);
			this.tabInfo.Controls.Add(this.link_web);
			this.tabInfo.Controls.Add(label4);
			this.tabInfo.Controls.Add(label3);
			this.tabInfo.Controls.Add(this.info_ver);
			this.tabInfo.Controls.Add(label5);
			this.tabInfo.Controls.Add(label2);
			this.tabInfo.Controls.Add(label1);
			this.tabInfo.Location = new System.Drawing.Point(4, 25);
			this.tabInfo.Name = "tabInfo";
			this.tabInfo.Size = new System.Drawing.Size(454, 293);
			this.tabInfo.TabIndex = 3;
			this.tabInfo.Text = "Info";
			this.tabInfo.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label1.Location = new System.Drawing.Point(142, 44);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(123, 25);
			label1.TabIndex = 0;
			label1.Text = "CDCRUSH";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label2.Location = new System.Drawing.Point(144, 98);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(52, 13);
			label2.TabIndex = 1;
			label2.Text = "Author :";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label3.Location = new System.Drawing.Point(144, 113);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(57, 13);
			label3.TabIndex = 1;
			label3.Text = "Version :";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.ForeColor = System.Drawing.SystemColors.GrayText;
			label4.Location = new System.Drawing.Point(144, 69);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(165, 13);
			label4.TabIndex = 1;
			label4.Text = "Highy compress cd-image games ";
			// 
			// link_web
			// 
			this.link_web.AutoSize = true;
			this.link_web.Location = new System.Drawing.Point(144, 139);
			this.link_web.Name = "link_web";
			this.link_web.Size = new System.Drawing.Size(117, 13);
			this.link_web.TabIndex = 2;
			this.link_web.TabStop = true;
			this.link_web.Text = "Project Page on Github";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label5.Location = new System.Drawing.Point(202, 98);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(53, 13);
			label5.TabIndex = 1;
			label5.Text = "John Dimi";
			// 
			// info_ver
			// 
			this.info_ver.AutoSize = true;
			this.info_ver.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.info_ver.Location = new System.Drawing.Point(202, 113);
			this.info_ver.Name = "info_ver";
			this.info_ver.Size = new System.Drawing.Size(22, 13);
			this.info_ver.TabIndex = 1;
			this.info_ver.Text = "0.0";
			// 
			// label7
			// 
			label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			label7.Location = new System.Drawing.Point(147, 88);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(159, 1);
			label7.TabIndex = 3;
			// 
			// info_tempFolder
			// 
			this.info_tempFolder.Location = new System.Drawing.Point(13, 39);
			this.info_tempFolder.Name = "info_tempFolder";
			this.info_tempFolder.Size = new System.Drawing.Size(373, 20);
			this.info_tempFolder.TabIndex = 0;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label6.Location = new System.Drawing.Point(10, 20);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(167, 13);
			label6.TabIndex = 1;
			label6.Text = "Temp Folder for operations :";
			// 
			// check_tempDef
			// 
			this.check_tempDef.AutoSize = true;
			this.check_tempDef.Checked = true;
			this.check_tempDef.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_tempDef.Location = new System.Drawing.Point(323, 19);
			this.check_tempDef.Name = "check_tempDef";
			this.check_tempDef.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.check_tempDef.Size = new System.Drawing.Size(63, 17);
			this.check_tempDef.TabIndex = 2;
			this.check_tempDef.Text = "Default ";
			this.check_tempDef.UseVisualStyleBackColor = true;
			this.check_tempDef.CheckedChanged += new System.EventHandler(this.check_tempDef_CheckedChanged);
			// 
			// btn_selectTemp
			// 
			this.btn_selectTemp.Location = new System.Drawing.Point(392, 37);
			this.btn_selectTemp.Name = "btn_selectTemp";
			this.btn_selectTemp.Size = new System.Drawing.Size(49, 23);
			this.btn_selectTemp.TabIndex = 3;
			this.btn_selectTemp.Text = "...";
			this.btn_selectTemp.UseVisualStyleBackColor = true;
			this.btn_selectTemp.Click += new System.EventHandler(this.btn_selectTemp_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(486, 381);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.info_status);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(502, 420);
			this.Name = "FormMain";
			this.Text = "cdcrush";
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabRestore.ResumeLayout(false);
			this.tabCrush.ResumeLayout(false);
			this.tabSettings.ResumeLayout(false);
			this.tabSettings.PerformLayout();
			this.tabInfo.ResumeLayout(false);
			this.tabInfo.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox info_status;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabRestore;
		private System.Windows.Forms.TabPage tabCrush;
		private System.Windows.Forms.TabPage tabSettings;
		private PanelRestore panelRestore1;
		private System.Windows.Forms.TabPage tabInfo;
		private PanelCompress panelCompress1;
		private System.Windows.Forms.LinkLabel link_web;
		private System.Windows.Forms.Label info_ver;
		private System.Windows.Forms.Button btn_selectTemp;
		private System.Windows.Forms.CheckBox check_tempDef;
		private System.Windows.Forms.TextBox info_tempFolder;
	}
}