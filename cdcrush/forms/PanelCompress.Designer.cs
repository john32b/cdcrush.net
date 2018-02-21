namespace cdcrush.forms
{
	partial class PanelCompress
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.Label label3;
			System.Windows.Forms.TableLayoutPanel table_IO;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.GroupBox groupBox1;
			this.btn_input_in = new System.Windows.Forms.Button();
			this.input_in = new System.Windows.Forms.TextBox();
			this.input_out = new System.Windows.Forms.TextBox();
			this.btn_input_out = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.combo_audioq = new System.Windows.Forms.ComboBox();
			this.info_tracks = new System.Windows.Forms.TextBox();
			this.info_size1 = new System.Windows.Forms.TextBox();
			this.info_size0 = new System.Windows.Forms.TextBox();
			this.btn_CRUSH = new System.Windows.Forms.Button();
			this.info_md5 = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.info_cdtitle = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			label3 = new System.Windows.Forms.Label();
			table_IO = new System.Windows.Forms.TableLayoutPanel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			table_IO.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			groupBox1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			label3.Dock = System.Windows.Forms.DockStyle.Top;
			label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label3.Location = new System.Drawing.Point(203, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(98, 13);
			label3.TabIndex = 5;
			label3.Text = "Audio";
			// 
			// table_IO
			// 
			table_IO.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			table_IO.ColumnCount = 2;
			table_IO.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			table_IO.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			table_IO.Controls.Add(this.btn_input_in, 1, 1);
			table_IO.Controls.Add(label1, 0, 0);
			table_IO.Controls.Add(this.input_in, 0, 1);
			table_IO.Controls.Add(this.input_out, 0, 3);
			table_IO.Controls.Add(this.btn_input_out, 1, 3);
			table_IO.Controls.Add(label2, 0, 2);
			table_IO.Location = new System.Drawing.Point(3, 3);
			table_IO.Name = "table_IO";
			table_IO.RowCount = 4;
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle());
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle());
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle());
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle());
			table_IO.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			table_IO.Size = new System.Drawing.Size(444, 82);
			table_IO.TabIndex = 17;
			// 
			// btn_input_in
			// 
			this.btn_input_in.Dock = System.Windows.Forms.DockStyle.Top;
			this.btn_input_in.Location = new System.Drawing.Point(402, 16);
			this.btn_input_in.Name = "btn_input_in";
			this.btn_input_in.Size = new System.Drawing.Size(39, 23);
			this.btn_input_in.TabIndex = 2;
			this.btn_input_in.Text = "...";
			this.btn_input_in.UseVisualStyleBackColor = true;
			this.btn_input_in.Click += new System.EventHandler(this.btn_input_in_Click);
			// 
			// label1
			// 
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label1.Location = new System.Drawing.Point(3, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(75, 13);
			label1.TabIndex = 0;
			label1.Text = "Input File :";
			// 
			// input_in
			// 
			this.input_in.Dock = System.Windows.Forms.DockStyle.Top;
			this.input_in.Location = new System.Drawing.Point(3, 16);
			this.input_in.Name = "input_in";
			this.input_in.Size = new System.Drawing.Size(393, 20);
			this.input_in.TabIndex = 1;
			// 
			// input_out
			// 
			this.input_out.Dock = System.Windows.Forms.DockStyle.Top;
			this.input_out.Location = new System.Drawing.Point(3, 58);
			this.input_out.Name = "input_out";
			this.input_out.Size = new System.Drawing.Size(393, 20);
			this.input_out.TabIndex = 1;
			// 
			// btn_input_out
			// 
			this.btn_input_out.Dock = System.Windows.Forms.DockStyle.Top;
			this.btn_input_out.Location = new System.Drawing.Point(402, 58);
			this.btn_input_out.Name = "btn_input_out";
			this.btn_input_out.Size = new System.Drawing.Size(39, 23);
			this.btn_input_out.TabIndex = 2;
			this.btn_input_out.Text = "...";
			this.btn_input_out.UseVisualStyleBackColor = true;
			this.btn_input_out.Click += new System.EventHandler(this.btn_input_out_Click);
			// 
			// label2
			// 
			label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label2.Location = new System.Drawing.Point(3, 42);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(118, 13);
			label2.TabIndex = 0;
			label2.Text = "Destination Folder :";
			// 
			// label4
			// 
			label4.ForeColor = System.Drawing.SystemColors.ControlDark;
			label4.Location = new System.Drawing.Point(3, 84);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(178, 13);
			label4.TabIndex = 19;
			label4.Text = "Leave blank for same folder as input";
			// 
			// label5
			// 
			label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label5.Location = new System.Drawing.Point(3, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(53, 13);
			label5.TabIndex = 0;
			label5.Text = "CD Title";
			// 
			// label6
			// 
			label6.Dock = System.Windows.Forms.DockStyle.Top;
			label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label6.Location = new System.Drawing.Point(151, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(46, 13);
			label6.TabIndex = 6;
			label6.Text = "Tracks";
			// 
			// label7
			// 
			label7.Dock = System.Windows.Forms.DockStyle.Top;
			label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label7.Location = new System.Drawing.Point(3, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(51, 13);
			label7.TabIndex = 6;
			label7.Text = "Size:";
			// 
			// label8
			// 
			label8.Dock = System.Windows.Forms.DockStyle.Top;
			label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label8.Location = new System.Drawing.Point(60, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(85, 13);
			label8.TabIndex = 20;
			label8.Text = "Crushed Size:";
			// 
			// label9
			// 
			label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label9.Location = new System.Drawing.Point(3, 0);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(81, 13);
			label9.TabIndex = 20;
			label9.Text = "Track 1 MD5";
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Image = global::cdcrush.Properties.Resources.dropimage;
			this.pictureBox1.Location = new System.Drawing.Point(3, 103);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(128, 128);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// combo_audioq
			// 
			this.combo_audioq.Dock = System.Windows.Forms.DockStyle.Top;
			this.combo_audioq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.combo_audioq.FormattingEnabled = true;
			this.combo_audioq.Location = new System.Drawing.Point(203, 16);
			this.combo_audioq.Name = "combo_audioq";
			this.combo_audioq.Size = new System.Drawing.Size(98, 21);
			this.combo_audioq.TabIndex = 4;
			// 
			// info_tracks
			// 
			this.info_tracks.BackColor = System.Drawing.SystemColors.Info;
			this.info_tracks.Dock = System.Windows.Forms.DockStyle.Top;
			this.info_tracks.Enabled = false;
			this.info_tracks.Location = new System.Drawing.Point(151, 16);
			this.info_tracks.Name = "info_tracks";
			this.info_tracks.ReadOnly = true;
			this.info_tracks.Size = new System.Drawing.Size(46, 20);
			this.info_tracks.TabIndex = 7;
			this.info_tracks.Text = "25";
			this.info_tracks.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// info_size1
			// 
			this.info_size1.BackColor = System.Drawing.SystemColors.Info;
			this.info_size1.Dock = System.Windows.Forms.DockStyle.Top;
			this.info_size1.Location = new System.Drawing.Point(3, 16);
			this.info_size1.Name = "info_size1";
			this.info_size1.ReadOnly = true;
			this.info_size1.Size = new System.Drawing.Size(51, 20);
			this.info_size1.TabIndex = 7;
			this.info_size1.Text = "650MB";
			this.info_size1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// info_size0
			// 
			this.info_size0.BackColor = System.Drawing.SystemColors.Info;
			this.info_size0.Dock = System.Windows.Forms.DockStyle.Top;
			this.info_size0.Location = new System.Drawing.Point(60, 16);
			this.info_size0.Name = "info_size0";
			this.info_size0.ReadOnly = true;
			this.info_size0.Size = new System.Drawing.Size(85, 20);
			this.info_size0.TabIndex = 21;
			this.info_size0.Text = "68MB";
			this.info_size0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btn_CRUSH
			// 
			this.btn_CRUSH.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_CRUSH.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btn_CRUSH.ForeColor = System.Drawing.Color.Green;
			this.btn_CRUSH.Location = new System.Drawing.Point(3, 237);
			this.btn_CRUSH.Name = "btn_CRUSH";
			this.btn_CRUSH.Size = new System.Drawing.Size(444, 39);
			this.btn_CRUSH.TabIndex = 22;
			this.btn_CRUSH.Text = "CRUSH";
			this.btn_CRUSH.UseVisualStyleBackColor = true;
			this.btn_CRUSH.Click += new System.EventHandler(this.btn_CRUSH_Click);
			// 
			// info_md5
			// 
			this.info_md5.BackColor = System.Drawing.SystemColors.Info;
			this.info_md5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.info_md5.Dock = System.Windows.Forms.DockStyle.Top;
			this.info_md5.Location = new System.Drawing.Point(3, 16);
			this.info_md5.Name = "info_md5";
			this.info_md5.ReadOnly = true;
			this.info_md5.Size = new System.Drawing.Size(298, 13);
			this.info_md5.TabIndex = 21;
			this.info_md5.Text = "c1a8d182a940e751f1c9a506ada22fe8";
			this.info_md5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			groupBox1.Controls.Add(this.tableLayoutPanel3);
			groupBox1.Controls.Add(this.tableLayoutPanel2);
			groupBox1.Controls.Add(this.tableLayoutPanel1);
			groupBox1.Location = new System.Drawing.Point(137, 103);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(310, 128);
			groupBox1.TabIndex = 23;
			groupBox1.TabStop = false;
			groupBox1.Text = "Cue Info";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(label5, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.info_cdtitle, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(304, 32);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// info_cdtitle
			// 
			this.info_cdtitle.BackColor = System.Drawing.SystemColors.Info;
			this.info_cdtitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.info_cdtitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.info_cdtitle.Location = new System.Drawing.Point(3, 16);
			this.info_cdtitle.Name = "info_cdtitle";
			this.info_cdtitle.Size = new System.Drawing.Size(298, 13);
			this.info_cdtitle.TabIndex = 1;
			this.info_cdtitle.Text = "Dummy Info, CD title with a really long name etc";
			this.info_cdtitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.info_cdtitle.WordWrap = false;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(label9, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.info_md5, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 48);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(304, 32);
			this.tableLayoutPanel2.TabIndex = 1;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 4;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(label7, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.info_size1, 0, 1);
			this.tableLayoutPanel3.Controls.Add(label8, 1, 0);
			this.tableLayoutPanel3.Controls.Add(label3, 3, 0);
			this.tableLayoutPanel3.Controls.Add(this.combo_audioq, 3, 1);
			this.tableLayoutPanel3.Controls.Add(this.info_tracks, 2, 1);
			this.tableLayoutPanel3.Controls.Add(this.info_size0, 1, 1);
			this.tableLayoutPanel3.Controls.Add(label6, 2, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 80);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(304, 40);
			this.tableLayoutPanel3.TabIndex = 24;
			// 
			// PanelCompress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(groupBox1);
			this.Controls.Add(this.btn_CRUSH);
			this.Controls.Add(label4);
			this.Controls.Add(table_IO);
			this.Controls.Add(this.pictureBox1);
			this.MaximumSize = new System.Drawing.Size(600, 280);
			this.MinimumSize = new System.Drawing.Size(450, 280);
			this.Name = "PanelCompress";
			this.Size = new System.Drawing.Size(450, 280);
			this.Load += new System.EventHandler(this.PanelCompress_Load);
			table_IO.ResumeLayout(false);
			table_IO.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ComboBox combo_audioq;
		private System.Windows.Forms.Button btn_input_in;
		private System.Windows.Forms.TextBox input_in;
		private System.Windows.Forms.TextBox input_out;
		private System.Windows.Forms.Button btn_input_out;
		private System.Windows.Forms.TextBox info_tracks;
		private System.Windows.Forms.TextBox info_size1;
		private System.Windows.Forms.TextBox info_size0;
		private System.Windows.Forms.Button btn_CRUSH;
		private System.Windows.Forms.TextBox info_md5;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TextBox info_cdtitle;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
	}
}
