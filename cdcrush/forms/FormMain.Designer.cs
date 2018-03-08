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
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label10;
			System.Windows.Forms.Label label11;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.info_status = new System.Windows.Forms.RichTextBox();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabRestore = new System.Windows.Forms.TabPage();
			this.panelRestore1 = new cdcrush.forms.PanelRestore();
			this.tabCrush = new System.Windows.Forms.TabPage();
			this.panelCompress1 = new cdcrush.forms.PanelCompress();
			this.tabSettings = new System.Windows.Forms.TabPage();
			this.group_debug = new System.Windows.Forms.GroupBox();
			this.chk_keepTemp = new System.Windows.Forms.CheckBox();
			this.info_ffmpeg_status = new System.Windows.Forms.Label();
			this.num_threads = new System.Windows.Forms.NumericUpDown();
			this.btn_temp_def = new System.Windows.Forms.Button();
			this.btn_ffmpeg_clear = new System.Windows.Forms.Button();
			this.btn_ffmpeg = new System.Windows.Forms.Button();
			this.btn_selectTemp = new System.Windows.Forms.Button();
			this.info_ffmpeg = new System.Windows.Forms.TextBox();
			this.info_tempFolder = new System.Windows.Forms.TextBox();
			this.tabInfo = new System.Windows.Forms.TabPage();
			this.link_donate = new System.Windows.Forms.LinkLabel();
			this.link_web = new System.Windows.Forms.LinkLabel();
			this.info_ver = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabRestore.SuspendLayout();
			this.tabCrush.SuspendLayout();
			this.tabSettings.SuspendLayout();
			this.group_debug.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.num_threads)).BeginInit();
			this.tabInfo.SuspendLayout();
			this.SuspendLayout();
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
			// label7
			// 
			label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			label7.Location = new System.Drawing.Point(147, 88);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(159, 1);
			label7.TabIndex = 3;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label6.Location = new System.Drawing.Point(3, 27);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(85, 13);
			label6.TabIndex = 1;
			label6.Text = "Temp Folder :";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label8.Location = new System.Drawing.Point(4, 140);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(142, 13);
			label8.TabIndex = 1;
			label8.Text = "Max Concurrent Tasks :";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label9.Location = new System.Drawing.Point(4, 85);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(109, 13);
			label9.TabIndex = 1;
			label9.Text = "FFmpeg.exe Path:";
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label10.Location = new System.Drawing.Point(144, 144);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(87, 13);
			label10.TabIndex = 1;
			label10.Text = "Source code :";
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label11.Location = new System.Drawing.Point(144, 190);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(59, 13);
			label11.TabIndex = 1;
			label11.Text = "Support :";
			// 
			// info_status
			// 
			this.info_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.info_status.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.info_status.Location = new System.Drawing.Point(12, 402);
			this.info_status.Name = "info_status";
			this.info_status.ReadOnly = true;
			this.info_status.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.info_status.Size = new System.Drawing.Size(467, 15);
			this.info_status.TabIndex = 19;
			this.info_status.Text = "Ready.";
			this.info_status.WordWrap = false;
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(12, 377);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(463, 24);
			this.progressBar1.TabIndex = 12;
			this.progressBar1.Value = 50;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.tabRestore);
			this.tabControl1.Controls.Add(this.tabCrush);
			this.tabControl1.Controls.Add(this.tabSettings);
			this.tabControl1.Controls.Add(this.tabInfo);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(467, 359);
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
			this.tabRestore.Size = new System.Drawing.Size(459, 325);
			this.tabRestore.TabIndex = 0;
			this.tabRestore.Text = "Restore a CD";
			this.tabRestore.UseVisualStyleBackColor = true;
			// 
			// panelRestore1
			// 
			this.panelRestore1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelRestore1.Location = new System.Drawing.Point(3, 3);
			this.panelRestore1.Name = "panelRestore1";
			this.panelRestore1.Size = new System.Drawing.Size(453, 319);
			this.panelRestore1.TabIndex = 0;
			// 
			// tabCrush
			// 
			this.tabCrush.Controls.Add(this.panelCompress1);
			this.tabCrush.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabCrush.Location = new System.Drawing.Point(4, 25);
			this.tabCrush.Name = "tabCrush";
			this.tabCrush.Padding = new System.Windows.Forms.Padding(3);
			this.tabCrush.Size = new System.Drawing.Size(459, 330);
			this.tabCrush.TabIndex = 1;
			this.tabCrush.Text = "Compress a CD";
			this.tabCrush.UseVisualStyleBackColor = true;
			// 
			// panelCompress1
			// 
			this.panelCompress1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelCompress1.Location = new System.Drawing.Point(3, 3);
			this.panelCompress1.Name = "panelCompress1";
			this.panelCompress1.Size = new System.Drawing.Size(453, 324);
			this.panelCompress1.TabIndex = 0;
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.group_debug);
			this.tabSettings.Controls.Add(this.info_ffmpeg_status);
			this.tabSettings.Controls.Add(this.num_threads);
			this.tabSettings.Controls.Add(this.btn_temp_def);
			this.tabSettings.Controls.Add(this.btn_ffmpeg_clear);
			this.tabSettings.Controls.Add(this.btn_ffmpeg);
			this.tabSettings.Controls.Add(this.btn_selectTemp);
			this.tabSettings.Controls.Add(label8);
			this.tabSettings.Controls.Add(label9);
			this.tabSettings.Controls.Add(label6);
			this.tabSettings.Controls.Add(this.info_ffmpeg);
			this.tabSettings.Controls.Add(this.info_tempFolder);
			this.tabSettings.Location = new System.Drawing.Point(4, 25);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.Size = new System.Drawing.Size(459, 358);
			this.tabSettings.TabIndex = 2;
			this.tabSettings.Text = "Settings";
			this.tabSettings.UseVisualStyleBackColor = true;
			// 
			// group_debug
			// 
			this.group_debug.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.group_debug.Controls.Add(this.chk_keepTemp);
			this.group_debug.Location = new System.Drawing.Point(3, 212);
			this.group_debug.Name = "group_debug";
			this.group_debug.Size = new System.Drawing.Size(448, 78);
			this.group_debug.TabIndex = 8;
			this.group_debug.TabStop = false;
			this.group_debug.Text = "Debugging";
			// 
			// chk_keepTemp
			// 
			this.chk_keepTemp.AutoSize = true;
			this.chk_keepTemp.Location = new System.Drawing.Point(6, 33);
			this.chk_keepTemp.Name = "chk_keepTemp";
			this.chk_keepTemp.Size = new System.Drawing.Size(105, 17);
			this.chk_keepTemp.TabIndex = 6;
			this.chk_keepTemp.Text = "Keep Temp Files";
			this.chk_keepTemp.UseVisualStyleBackColor = true;
			this.chk_keepTemp.CheckedChanged += new System.EventHandler(this.chk_keepTemp_CheckedChanged);
			// 
			// info_ffmpeg_status
			// 
			this.info_ffmpeg_status.AutoSize = true;
			this.info_ffmpeg_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.info_ffmpeg_status.ForeColor = System.Drawing.Color.Green;
			this.info_ffmpeg_status.Location = new System.Drawing.Point(347, 124);
			this.info_ffmpeg_status.Name = "info_ffmpeg_status";
			this.info_ffmpeg_status.Size = new System.Drawing.Size(103, 13);
			this.info_ffmpeg_status.TabIndex = 5;
			this.info_ffmpeg_status.Text = "FFmpeg is ready.";
			// 
			// num_threads
			// 
			this.num_threads.Location = new System.Drawing.Point(7, 156);
			this.num_threads.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.num_threads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.num_threads.Name = "num_threads";
			this.num_threads.Size = new System.Drawing.Size(124, 20);
			this.num_threads.TabIndex = 4;
			this.num_threads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.num_threads.ValueChanged += new System.EventHandler(this.num_threads_ValueChanged);
			// 
			// btn_temp_def
			// 
			this.btn_temp_def.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_temp_def.Location = new System.Drawing.Point(399, 43);
			this.btn_temp_def.Name = "btn_temp_def";
			this.btn_temp_def.Size = new System.Drawing.Size(50, 20);
			this.btn_temp_def.TabIndex = 3;
			this.btn_temp_def.Text = "Default";
			this.btn_temp_def.UseVisualStyleBackColor = true;
			this.btn_temp_def.Click += new System.EventHandler(this.btn_temp_def_Click);
			// 
			// btn_ffmpeg_clear
			// 
			this.btn_ffmpeg_clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_ffmpeg_clear.Location = new System.Drawing.Point(402, 101);
			this.btn_ffmpeg_clear.Name = "btn_ffmpeg_clear";
			this.btn_ffmpeg_clear.Size = new System.Drawing.Size(48, 20);
			this.btn_ffmpeg_clear.TabIndex = 3;
			this.btn_ffmpeg_clear.Text = "Clear";
			this.btn_ffmpeg_clear.UseVisualStyleBackColor = true;
			this.btn_ffmpeg_clear.Click += new System.EventHandler(this.btn_ffmpeg_clear_Click);
			// 
			// btn_ffmpeg
			// 
			this.btn_ffmpeg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_ffmpeg.Location = new System.Drawing.Point(378, 101);
			this.btn_ffmpeg.Name = "btn_ffmpeg";
			this.btn_ffmpeg.Size = new System.Drawing.Size(24, 20);
			this.btn_ffmpeg.TabIndex = 3;
			this.btn_ffmpeg.Text = "...";
			this.btn_ffmpeg.UseVisualStyleBackColor = true;
			this.btn_ffmpeg.Click += new System.EventHandler(this.btn_ffmpeg_Click);
			// 
			// btn_selectTemp
			// 
			this.btn_selectTemp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_selectTemp.Location = new System.Drawing.Point(375, 43);
			this.btn_selectTemp.Name = "btn_selectTemp";
			this.btn_selectTemp.Size = new System.Drawing.Size(24, 20);
			this.btn_selectTemp.TabIndex = 3;
			this.btn_selectTemp.Text = "...";
			this.btn_selectTemp.UseVisualStyleBackColor = true;
			this.btn_selectTemp.Click += new System.EventHandler(this.btn_selectTemp_Click);
			// 
			// info_ffmpeg
			// 
			this.info_ffmpeg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.info_ffmpeg.Location = new System.Drawing.Point(6, 101);
			this.info_ffmpeg.Name = "info_ffmpeg";
			this.info_ffmpeg.ReadOnly = true;
			this.info_ffmpeg.Size = new System.Drawing.Size(370, 20);
			this.info_ffmpeg.TabIndex = 0;
			// 
			// info_tempFolder
			// 
			this.info_tempFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.info_tempFolder.Location = new System.Drawing.Point(7, 43);
			this.info_tempFolder.Name = "info_tempFolder";
			this.info_tempFolder.ReadOnly = true;
			this.info_tempFolder.Size = new System.Drawing.Size(366, 20);
			this.info_tempFolder.TabIndex = 0;
			// 
			// tabInfo
			// 
			this.tabInfo.Controls.Add(label7);
			this.tabInfo.Controls.Add(this.link_donate);
			this.tabInfo.Controls.Add(this.link_web);
			this.tabInfo.Controls.Add(label4);
			this.tabInfo.Controls.Add(label11);
			this.tabInfo.Controls.Add(label10);
			this.tabInfo.Controls.Add(label3);
			this.tabInfo.Controls.Add(this.info_ver);
			this.tabInfo.Controls.Add(label5);
			this.tabInfo.Controls.Add(label2);
			this.tabInfo.Controls.Add(label1);
			this.tabInfo.Location = new System.Drawing.Point(4, 25);
			this.tabInfo.Name = "tabInfo";
			this.tabInfo.Size = new System.Drawing.Size(459, 358);
			this.tabInfo.TabIndex = 3;
			this.tabInfo.Text = "Info";
			this.tabInfo.UseVisualStyleBackColor = true;
			// 
			// link_donate
			// 
			this.link_donate.AutoSize = true;
			this.link_donate.Location = new System.Drawing.Point(144, 204);
			this.link_donate.Name = "link_donate";
			this.link_donate.Size = new System.Drawing.Size(98, 13);
			this.link_donate.TabIndex = 2;
			this.link_donate.TabStop = true;
			this.link_donate.Text = "Donate with paypal";
			// 
			// link_web
			// 
			this.link_web.AutoSize = true;
			this.link_web.Location = new System.Drawing.Point(144, 158);
			this.link_web.Name = "link_web";
			this.link_web.Size = new System.Drawing.Size(117, 13);
			this.link_web.TabIndex = 2;
			this.link_web.TabStop = true;
			this.link_web.Text = "Project Page on Github";
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
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(491, 419);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.info_status);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(502, 420);
			this.Name = "FormMain";
			this.Text = "CDCRUSH";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabRestore.ResumeLayout(false);
			this.tabCrush.ResumeLayout(false);
			this.tabSettings.ResumeLayout(false);
			this.tabSettings.PerformLayout();
			this.group_debug.ResumeLayout(false);
			this.group_debug.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.num_threads)).EndInit();
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
		private System.Windows.Forms.TextBox info_tempFolder;
		private System.Windows.Forms.NumericUpDown num_threads;
		private System.Windows.Forms.Button btn_ffmpeg;
		private System.Windows.Forms.TextBox info_ffmpeg;
		private System.Windows.Forms.Button btn_temp_def;
		private System.Windows.Forms.Button btn_ffmpeg_clear;
		private System.Windows.Forms.Label info_ffmpeg_status;
		private System.Windows.Forms.LinkLabel link_donate;
		private System.Windows.Forms.GroupBox group_debug;
		private System.Windows.Forms.CheckBox chk_keepTemp;
	}
}